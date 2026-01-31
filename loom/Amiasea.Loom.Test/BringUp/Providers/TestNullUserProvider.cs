using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Test.BringUp;

public sealed class TestNullUserProvider : IGraphQueryableProvider
{
    private readonly List<User> _users = new()
    {
        new User { Id = 1, Name = null }   // <-- REQUIRED FOR THE TEST
    };

    public Task<IQueryable> GetRootAsync(string rootName, CancellationToken cancellationToken)
    {
        return Task.FromResult((IQueryable)new[] { new QueryRoot() }.AsQueryable());
    }

    public Task<object> ExecuteQueryAsync(IQueryable queryable, CancellationToken cancellationToken)
    {
        return Task.FromResult((object)queryable.Cast<object>().ToList());
    }

    public Task<object?> GetValueAsync(object instance, string fieldName, CancellationToken cancellationToken)
    {
        if (instance is QueryRoot)
        {
            return fieldName switch
            {
                "users" => Task.FromResult<object?>(_users),
                "user" => Task.FromResult<object?>(_users.First()),
                _ => throw new InvalidOperationException("Unknown field: " + fieldName)
            };
        }

        if (instance is User u)
        {
            return fieldName switch
            {
                "id" => Task.FromResult<object?>(u.Id),
                "name" => Task.FromResult<object?>(u.Name),   // <-- returns null
                _ => throw new InvalidOperationException("Unknown field: " + fieldName)
            };
        }

        throw new InvalidOperationException("Unknown instance type: " + instance.GetType().Name);
    }

    private sealed class QueryRoot { }

    private sealed class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }   // <-- nullable
    }
}