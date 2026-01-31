using System;

namespace Amiasea.Loom.Projection
{
    internal sealed class ProjectionListTypeProxy : IProjectionListType
    {
        public string Name
        {
            get { return "[" + ElementType.Name + "]"; }
        }

        public IProjectionInputType ElementType { get; private set; }

        public ProjectionListTypeProxy(IProjectionInputType elementType)
        {
            if (elementType == null) throw new ArgumentNullException(nameof(elementType));
            ElementType = elementType;
        }
    }
}
