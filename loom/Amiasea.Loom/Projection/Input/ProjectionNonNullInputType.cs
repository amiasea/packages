using System;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionNonNullInputType : IProjectionInputType
    {
        public IProjectionInputType InnerType { get; }

        public ProjectionNonNullInputType(IProjectionInputType innerType)
        {
            InnerType = innerType ?? throw new ArgumentNullException(nameof(innerType));
        }

        public string Name => $"{InnerType.Name}!";
    }
}