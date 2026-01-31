using System.Threading;
using System.Threading.Tasks;

namespace Amiasea.Loom.Projection
{
    public interface IProjectionExecutor
    {
        Task<ProjectionResult> ExecuteAsync(
            ProjectionPlan plan,
            CancellationToken cancellationToken);
    }
}
