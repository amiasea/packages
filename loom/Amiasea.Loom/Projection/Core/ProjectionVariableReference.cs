using System;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionVariableReference : ProjectionArgumentValue
    {
        public string Name { get; private set; }

        public ProjectionVariableReference(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name;
        }
    }
}
