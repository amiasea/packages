using System.Threading;
using Amiasea.Loom.Execution;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Test.BringUp
{
public sealed class TestProjectionPlanner : IProjectionPlanner
{
    public ProjectionPlan Plan(NormalizedProjectionRequest request, CancellationToken cancellationToken)
    {
        // Minimal valid plan: just use the request as the root execution node
        return new ProjectionPlan(request);
    }
}
}
