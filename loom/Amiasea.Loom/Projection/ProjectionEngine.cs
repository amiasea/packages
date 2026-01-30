using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amiasea.Loom.AST;
using Amiasea.Loom.Execution;

namespace Amiasea.Loom.Projection
{
    /// <summary>
    /// Default implementation of IProjectionEngine.
    /// Orchestrates: AST → ProjectionRequest → ExecutionEngine.
    /// </summary>
    public sealed class ProjectionEngine : IProjectionEngine
    {
        private readonly IExecutionEngine _executionEngine;
        private readonly IProjectionRequestFactory _requestFactory;

        public ProjectionEngine(
            IExecutionEngine executionEngine,
            IProjectionRequestFactory requestFactory)
        {
            if (executionEngine == null) throw new ArgumentNullException(nameof(executionEngine));
            if (requestFactory == null) throw new ArgumentNullException(nameof(requestFactory));

            _executionEngine = executionEngine;
            _requestFactory = requestFactory;
        }

        public async Task<object> ExecuteAsync(
            DocumentNode document,
            OperationNode operation,
            IGraphQueryableProvider provider,
            CancellationToken cancellationToken)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (operation == null) throw new ArgumentNullException(nameof(operation));
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            // 1. Translate AST + provider into an internal ProjectionRequest
            ProjectionRequest request = _requestFactory.Create(document, operation, provider);

            // 2. Execute via the internal execution engine
            ProjectionResult result = await _executionEngine
                .ExecuteAsync(request, cancellationToken)
                .ConfigureAwait(false);

            // 3. Return the raw data payload as the public compiler result
            return result.Data;
        }
    }

}
