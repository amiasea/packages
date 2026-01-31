using System;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionOutputScalarType : IProjectionOutputType, IProjectionType
    {
        public string Name { get; }
        public Type ClrType { get; }

        public ProjectionOutputScalarType(string name, Type clrType)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ClrType = clrType ?? throw new ArgumentNullException(nameof(clrType));
        }

        // Scalars have no fields
        public IProjectionFieldDefinition GetField(string name)
        {
            return null;
        }
    }
}