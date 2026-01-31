using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Amiasea.Loom.Execution;
using Amiasea.Loom.Projection;
using Microsoft.EntityFrameworkCore;

namespace Amiasea.Loom.EF.Schema
{
    public sealed class EFProjectionExecutor : IProjectionExecutor
    {
        private readonly DbContext _db;

        public EFProjectionExecutor(DbContext db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            _db = db;
        }

        public Task<ProjectionResult> ExecuteAsync(
            ProjectionPlan plan,
            CancellationToken cancellationToken)
        {
            if (plan == null)
                throw new ArgumentNullException(nameof(plan));

            // 1. Get the root DbSet<T>
            var set = GetDbSet(plan.RootClrType);

            // 2. Apply filter
            if (plan.Filter != null)
            {
                var where = BuildWhere(plan.RootClrType, plan.Filter);
                set = set.Where(where);
            }

            // 3. Apply includes
            set = ApplyIncludes(set, plan.Includes);

            // 4. Apply projection shaping
            var shaped = ApplySelection(set, plan);

            // 5. Materialize
            return MaterializeAsync(shaped, cancellationToken);
        }

        private IQueryable GetDbSet(Type clr)
        {
            var method = typeof(DbContext)
                .GetMethod("Set", Type.EmptyTypes)
                .MakeGenericMethod(clr);

            return (IQueryable)method.Invoke(_db, null);
        }

        private IQueryable ApplyIncludes(IQueryable query, IReadOnlyList<string> includes)
        {
            if (includes == null || includes.Count == 0)
                return query;

            foreach (var path in includes)
            {
                query = query.Include(path);
            }

            return query;
        }

        private LambdaExpression BuildWhere(Type clr, FilterNode filter)
        {
            var builder = typeof(EFWhereBuilder)
                .GetMethod("Build")
                .MakeGenericMethod(clr);

            return (LambdaExpression)builder.Invoke(null, new object[] { filter });
        }

        private IQueryable ApplySelection(IQueryable source, ProjectionPlan plan)
        {
            // If no selection shaping is required, return as-is.
            if (plan.Selection == null)
                return source;

            // Build lambda: e => new { ... }
            var param = Expression.Parameter(plan.RootClrType, "e");
            var body = BuildSelectionObject(param, plan.Selection);

            var lambda = Expression.Lambda(body, param);

            var selectMethod = typeof(Queryable)
                .GetMethods()
                .Where(m => m.Name == "Select" && m.GetParameters().Length == 2)
                .Single()
                .MakeGenericMethod(plan.RootClrType, body.Type);

            return (IQueryable)selectMethod.Invoke(null, new object[] { source, lambda });
        }

        private Expression BuildSelectionObject(
            ParameterExpression param,
            ProjectionSelection selection)
        {
            var bindings = new List<MemberBinding>();

            foreach (var field in selection.Fields)
            {
                var member = BuildMemberAccess(param, field.Name);

                Expression value;

                if (field.Nested != null)
                {
                    // Nested projection
                    value = BuildSelectionObject(member, field.Nested);
                }
                else
                {
                    value = member;
                }

                var prop = typeof(ProjectionRow).GetProperty(field.Name);
                bindings.Add(Expression.Bind(prop, value));
            }

            return Expression.MemberInit(
                Expression.New(typeof(ProjectionRow)),
                bindings
            );
        }

        private Expression BuildMemberAccess(Expression root, string path)
        {
            var parts = path.Split('.');
            Expression current = root;

            for (int i = 0; i < parts.Length; i++)
            {
                current = Expression.PropertyOrField(current, parts[i]);
            }

            return current;
        }

        private async Task<ProjectionResult> MaterializeAsync(
            IQueryable shaped,
            CancellationToken cancellationToken)
        {
            var listType = typeof(List<>).MakeGenericType(shaped.ElementType);
            var toListAsync = typeof(EntityFrameworkQueryableExtensions)
                .GetMethod("ToListAsync", new[] { typeof(IQueryable<>).MakeGenericType(shaped.ElementType), typeof(CancellationToken) })
                ?? typeof(EntityFrameworkQueryableExtensions)
                    .GetMethods()
                    .Where(m => m.Name == "ToListAsync" && m.GetParameters().Length == 2)
                    .Single()
                    .MakeGenericMethod(shaped.ElementType);

            var task = (Task)toListAsync.Invoke(null, new object[] { shaped, cancellationToken });
            await task.ConfigureAwait(false);

            var resultProperty = task.GetType().GetProperty("Result");
            var list = resultProperty.GetValue(task);

            return new ProjectionResult(list);
        }
    }
}