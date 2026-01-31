using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.EF;

public sealed class EFProvider : IGraphQueryableProvider
{
    private readonly DbContext _db;

    public EFProvider(DbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public Task<IQueryable> GetRootAsync(string rootName, CancellationToken cancellationToken)
    {
        if (rootName == "Query")
            return Task.FromResult((IQueryable)new[] { new QueryRoot() }.AsQueryable());

        throw new InvalidOperationException($"Unknown root '{rootName}'.");
    }

    public Task<object> ExecuteQueryAsync(IQueryable queryable, CancellationToken cancellationToken)
    {
        return Task.FromResult((object)queryable.Cast<object>().ToList());
    }

    public Task<object?> GetValueAsync(object instance, string fieldName, CancellationToken cancellationToken)
    {
        if (instance is QueryRoot)
            return ResolveRootFieldAsync(fieldName, cancellationToken);

        return ResolveEntityFieldAsync(instance, fieldName, cancellationToken);
    }

    private Task<object?> ResolveRootFieldAsync(string fieldName, CancellationToken token)
    {
        var dbType = _db.GetType();
        var prop = dbType.GetProperty(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (prop == null)
            throw new InvalidOperationException($"Unknown field '{fieldName}' on Query.");

        var value = prop.GetValue(_db);

        if (value is IQueryable q)
            return Task.FromResult<object?>(q);

        throw new InvalidOperationException($"Field '{fieldName}' on Query is not a DbSet.");
    }

    private Task<object?> ResolveEntityFieldAsync(object instance, string fieldName, CancellationToken token)
    {
        var type = instance.GetType();
        var prop = type.GetProperty(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (prop == null)
            throw new InvalidOperationException($"Unknown field '{fieldName}' on type '{type.Name}'.");

        var value = prop.GetValue(instance);

        if (value == null)
            return Task.FromResult<object?>(null);

        if (value is IEnumerable && value is not string)
        {
            var entry = _db.Entry(instance);
            var nav = entry.Collections.FirstOrDefault(c => c.Metadata.Name == fieldName);

            if (nav != null)
                return Task.FromResult<object?>(nav.Query());

            return Task.FromResult<object?>(value);
        }

        return Task.FromResult<object?>(value);
    }

    private sealed class QueryRoot { }
}