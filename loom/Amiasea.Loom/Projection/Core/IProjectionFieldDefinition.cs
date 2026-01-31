using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Projection
{
    public interface IProjectionFieldDefinition
    {
        string Name { get; }

        IProjectionOutputType ReturnType { get; }

        IReadOnlyList<IProjectionArgumentDefinition> Arguments { get; }

        bool AllowExtraArguments { get; }

        IProjectionInputType InferExtraArgumentType(
            string name,
            ProjectionArgumentValue rawValue);
    }
}
