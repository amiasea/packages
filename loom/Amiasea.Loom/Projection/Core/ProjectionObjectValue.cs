using System;
using System.Collections.Generic;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionObjectValue : ProjectionArgumentValue
    {
        public IReadOnlyDictionary<string, ProjectionArgumentValue> Fields { get; private set; }

        public ProjectionObjectValue(IReadOnlyDictionary<string, ProjectionArgumentValue> fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            Fields = fields;
        }
    }
}
