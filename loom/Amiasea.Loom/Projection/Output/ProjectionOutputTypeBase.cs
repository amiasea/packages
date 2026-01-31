using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Projection
{
    public abstract class ProjectionOutputTypeBase : ProjectionTypeBase, IProjectionOutputType
    {
        protected ProjectionOutputTypeBase(string name)
            : base(name)
        {
        }
    }
}
