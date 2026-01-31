using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Projection
{
    public interface IProjectionInputFieldDefinition
    {
        string Name { get; }

        IProjectionInputType Type { get; }

        bool IsNonNull { get; }

        ProjectionArgumentValue DefaultValue { get; }
    }
}
