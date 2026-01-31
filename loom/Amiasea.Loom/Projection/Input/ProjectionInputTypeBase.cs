using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Projection
{
    public abstract class ProjectionInputTypeBase : IProjectionInputType
    {
        public string Name { get; private set; }

        protected ProjectionInputTypeBase(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name;
        }
    }
}
