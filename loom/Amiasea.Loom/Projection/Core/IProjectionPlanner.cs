using System.Threading;

namespace Amiasea.Loom.Projection
{
    public interface IProjectionPlanner
    {
        ProjectionPlan Plan(
            NormalizedProjectionRequest request,
            CancellationToken cancellationToken);
    }
}
