using System.Collections.Generic;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionResult
    {
        public object Data { get; private set; }
        public IReadOnlyList<ProjectionError> Errors { get; private set; }

        public ProjectionResult(object data, IReadOnlyList<ProjectionError> errors)
        {
            Data = data;
            Errors = errors ?? new List<ProjectionError>();
        }
    }
}
