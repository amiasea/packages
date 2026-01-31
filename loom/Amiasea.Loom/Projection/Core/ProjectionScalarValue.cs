using System;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionScalarValue : ProjectionArgumentValue
    {
        public object Raw { get; private set; }

        public ProjectionScalarValue(object raw)
        {
            Raw = raw;
        }
    }
}
