using System;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionEnumValue : ProjectionArgumentValue
    {
        public string Raw { get; private set; }

        public ProjectionEnumValue(string raw)
        {
            if (raw == null) throw new ArgumentNullException(nameof(raw));
            Raw = raw;
        }
    }
}
