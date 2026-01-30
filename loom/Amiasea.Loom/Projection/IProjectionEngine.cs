using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amiasea.Loom.AST;
using Amiasea.Loom.Execution;

namespace Amiasea.Loom.Projection
{
    /// <summary>
    /// Compiles a validated GraphQL operation into a projected result.
    /// This is the ONLY public entry point for Loom's compiler.
    /// </summary>
    public interface IProjectionEngine
    {
        Task<object> ExecuteAsync(
            DocumentNode document,
            OperationNode operation,
            IGraphQueryableProvider provider,
            CancellationToken cancellationToken
        );
    }
}