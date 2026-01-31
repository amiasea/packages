using System;
using Amiasea.Loom.Execution;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionInputFieldDefinition : IProjectionInputFieldDefinition
    {
        public string Name { get; private set; }
        public IProjectionInputType Type { get; private set; }
        public bool IsNonNull { get; private set; }
        public ProjectionArgumentValue DefaultValue { get; private set; }

        public ProjectionInputFieldDefinition(
            string name,
            IProjectionInputType type,
            bool isNonNull,
            ProjectionArgumentValue defaultValue)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (type == null) throw new ArgumentNullException(nameof(type));

            Name = name;
            Type = type;
            IsNonNull = isNonNull;
            DefaultValue = defaultValue;
        }
    }
}