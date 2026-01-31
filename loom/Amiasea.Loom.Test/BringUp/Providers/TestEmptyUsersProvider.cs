using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amiasea.Loom;

namespace Amiasea.Loom.Test.BringUp;

public sealed class TestEmptyUsersProvider : IGraphQueryableProvider
{
    public Task<IQueryable> GetRootAsync(string rootName, CancellationToken cancellationToken)
    {
        if (rootName == "Query")
            return Task.FromResult((IQueryable)new[] { new QueryRoot() }.AsQueryable());

        throw new InvalidOperationException("Unknown root: " + rootName);
    }

    public Task<object> ExecuteQueryAsync(IQueryable queryable, CancellationToken cancellationToken)
    {
        return Task.FromResult((object)queryable.Cast<object>().ToList());
    }

    public Task<object?> GetValueAsync(object instance, string fieldName, CancellationToken cancellationToken)
    {
        if (instance is QueryRoot && fieldName == "users")
            return Task.FromResult<object?>(new List<object>());

        throw new InvalidOperationException("Unknown instance type: " + instance.GetType().Name);
    }

    private sealed class QueryRoot { }
}