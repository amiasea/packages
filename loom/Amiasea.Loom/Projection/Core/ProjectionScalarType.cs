using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionScalarType : ProjectionInputTypeBase, IProjectionScalarType
    {
        private readonly Func<object, object> _coercer;

        public ProjectionScalarType(
            string name,
            Func<object, object> coercer)
            : base(name)
        {
            if (coercer == null) throw new ArgumentNullException(nameof(coercer));
            _coercer = coercer;
        }

        public object Coerce(object raw)
        {
            return _coercer(raw);
        }
    }
}
