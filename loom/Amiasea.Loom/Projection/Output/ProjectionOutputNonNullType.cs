using System;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionOutputNonNullType : IProjectionOutputType, IProjectionType
    {
        public IProjectionOutputType InnerType { get; }

        public ProjectionOutputNonNullType(IProjectionOutputType innerType)
        {
            InnerType = innerType ?? throw new ArgumentNullException(nameof(innerType));
        }

        public string Name => InnerType.Name;

        public IProjectionFieldDefinition GetField(string name)
        {
            return null;
        }
    }
}