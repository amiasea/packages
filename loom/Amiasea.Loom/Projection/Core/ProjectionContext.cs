using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionContext
    {
        public IReadOnlyDictionary<string, object> Items { get; private set; }

        public ProjectionContext(IReadOnlyDictionary<string, object> items)
        {
            Items = items ?? new Dictionary<string, object>();
        }
    }
}
