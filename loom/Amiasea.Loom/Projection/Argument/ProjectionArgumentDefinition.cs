using System;
using Amiasea.Loom.Execution;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionArgumentDefinition : IProjectionArgumentDefinition
    {
        public string Name { get; }
        public IProjectionInputType Type { get; }
        public bool IsNonNull { get; }
        public ProjectionArgumentValue DefaultValue { get; }

        public ProjectionArgumentDefinition(
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