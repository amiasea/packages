using System;
using System.Collections.Generic;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionListValue : ProjectionArgumentValue
    {
        public IReadOnlyList<ProjectionArgumentValue> Elements { get; private set; }

        public ProjectionListValue(IReadOnlyList<ProjectionArgumentValue> elements)
        {
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            Elements = elements;
        }
    }
}
