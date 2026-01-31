using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Execution
{
    public interface IExecutionEngine
    {
        Task<ProjectionResult> ExecuteAsync(
            ProjectionRequest request,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}

