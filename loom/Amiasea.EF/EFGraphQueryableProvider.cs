using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public sealed class EFGraphQueryableProvider : IGraphQueryableProvider
{
    private readonly DbContext _db;

    public EFGraphQueryableProvider(DbContext db)
    {
        _db = db;
    }

    public Task<IQueryable> GetRootAsync(
        string rootName,
        CancellationToken cancellationToken
    )
    {
        var clrType = ResolveRootClrType(rootName);

        var method = typeof(DbContext)
            .GetMethod(nameof(DbContext.Set), Type.EmptyTypes);

        if (method == null)
            throw new InvalidOperationException("DbContext.Set not found");

        var generic = method.MakeGenericMethod(clrType);

        var result = (IQueryable)generic.Invoke(_db, null);

        return Task.FromResult(result);
    }

    public async Task<object> ExecuteQueryAsync(
        IQueryable queryable,
        CancellationToken cancellationToken
    )
    {
        // Materialize asynchronously using EF Core
        var toListAsync = typeof(EntityFrameworkQueryableExtensions)
            .GetMethod(nameof(EntityFrameworkQueryableExtensions.ToListAsync),
                new[] { typeof(IQueryable<>).MakeGenericType(queryable.ElementType), typeof(CancellationToken) });

        if (toListAsync != null)
        {
            var generic = toListAsync.MakeGenericMethod(queryable.ElementType);
            return await (dynamic)generic.Invoke(null, new object[] { queryable, cancellationToken });
        }

        // Fallback (should never happen)
        return queryable.Cast<object>().ToList();
    }

    public Task<object> GetValueAsync(
        object instance,
        string fieldName,
        CancellationToken cancellationToken
    )
    {
        var prop = instance.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);

        if (prop == null)
            throw new InvalidOperationException(
                "Scalar " + instance.GetType().Name + "." + fieldName + " not found");

        var value = prop.GetValue(instance);

        return Task.FromResult(value);
    }

    private static Type ResolveRootClrType(string rootName)
    {
        throw new NotImplementedException(
            "Root type resolution for '" + rootName + "' must be provided by generated metadata.");
    }
}