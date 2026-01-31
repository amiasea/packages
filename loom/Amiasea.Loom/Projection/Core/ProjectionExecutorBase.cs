using System.Threading.Tasks;

namespace Amiasea.Loom.Projection
{
    public abstract class ProjectionExecutorBase : IProjectionExecutor
    {
        public abstract Task<object> ExecuteAsync(ProjectionPlan plan);
    }
}