using System;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionListInputType : IProjectionInputType
    {
        public IProjectionInputType ElementType { get; }

        public ProjectionListInputType(IProjectionInputType elementType)
        {
            ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
        }

        public string Name => $"[{ElementType.Name}]";
    }
}