using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amiasea.Loom;

namespace Amiasea.Loom.Test.BringUp;

public sealed class TestSingleUserProvider : IGraphQueryableProvider
{
    private readonly List<User> _users = new()
    {
        new User { Id = 42, Name = "Solo" }
    };

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
            return Task.FromResult<object?>(_users);

        if (instance is User u)
        {
            return Task.FromResult<object?>(fieldName switch
            {
                "id" => u.Id,
                "name" => u.Name,
                _ => throw new InvalidOperationException("Unknown field: " + fieldName)
            });
        }

        throw new InvalidOperationException("Unknown instance type: " + instance.GetType().Name);
    }

    private sealed class QueryRoot { }

    private sealed class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}