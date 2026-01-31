using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Projection
{
    public interface IProjectionInputObjectType : IProjectionInputType
    {
        IReadOnlyList<IProjectionInputFieldDefinition> Fields { get; }

        bool AllowExtraFields { get; }

        IProjectionInputType InferExtraFieldType(
            string name,
            ProjectionArgumentValue rawValue);
    }
}
