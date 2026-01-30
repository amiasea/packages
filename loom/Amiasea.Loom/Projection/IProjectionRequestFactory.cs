using System;
using System.Collections.Generic;
using System.Text;
using Amiasea.Loom.AST;
using Amiasea.Loom.Execution;

namespace Amiasea.Loom.Projection
{
    /// <summary>
    /// Translates a validated GraphQL operation into a ProjectionRequest
    /// that the execution engine can understand.
    /// </summary>
    public interface IProjectionRequestFactory
    {
        ProjectionRequest Create(
            DocumentNode document,
            OperationNode operation,
            IGraphQueryableProvider provider);
    }
}