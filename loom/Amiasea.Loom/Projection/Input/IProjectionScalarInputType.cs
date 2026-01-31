using System;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionScalarInputType : IProjectionInputType
    {
        public string Name { get; }
        public Type ClrType { get; }

        public ProjectionScalarInputType(string name, Type clrType)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ClrType = clrType ?? throw new ArgumentNullException(nameof(clrType));
        }
    }
}