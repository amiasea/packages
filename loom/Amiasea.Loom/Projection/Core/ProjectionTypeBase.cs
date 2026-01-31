using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Projection
{
    public abstract class ProjectionTypeBase : IProjectionType
    {
        public string Name { get; private set; }

        protected ProjectionTypeBase(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name;
        }

        public abstract IProjectionFieldDefinition GetField(string name);
    }
}
