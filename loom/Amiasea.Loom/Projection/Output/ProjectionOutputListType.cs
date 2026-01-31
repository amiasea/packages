using System;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionOutputListType : IProjectionOutputType, IProjectionType
    {
        public IProjectionOutputType ElementType { get; }

        public ProjectionOutputListType(IProjectionOutputType elementType)
        {
            ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
        }

        public string Name => ElementType.Name;

        public IProjectionFieldDefinition GetField(string name)
        {
            return null;
        }
    }
}