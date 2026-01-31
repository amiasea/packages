using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionListType : ProjectionInputTypeBase, IProjectionListType
    {
        public IProjectionInputType ElementType { get; private set; }

        public ProjectionListType(IProjectionInputType elementType)
            : base("[" + (elementType == null ? "?" : elementType.Name) + "]")
        {
            if (elementType == null) throw new ArgumentNullException(nameof(elementType));
            ElementType = elementType;
        }
    }
}
