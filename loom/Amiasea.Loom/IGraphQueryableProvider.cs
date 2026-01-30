using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public interface IGraphQueryableProvider
{
    Task<IQueryable> GetRootAsync(
        string rootName,
        CancellationToken cancellationToken
    );

    Task<object> ExecuteQueryAsync(
        IQueryable queryable,
        CancellationToken cancellationToken
    );

    Task<object> GetValueAsync(
        object instance,
        string fieldName,
        CancellationToken cancellationToken
    );
}