using System;
using System.Collections.Generic;
using System.Linq;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionInputEnumType : ProjectionInputTypeBase, IProjectionInputEnumType
    {
        private readonly Dictionary<string, object> _values;

        public ProjectionInputEnumType(
            string name,
            IEnumerable<string> values)
            : base(name)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            _values = values.ToDictionary(v => v, v => (object)v);
        }

        public object Coerce(string raw)
        {
            if (raw == null) throw new ArgumentNullException(nameof(raw));

            if (!_values.TryGetValue(raw, out var value))
            {
                throw new InvalidOperationException(
                    $"Invalid value '{raw}' for enum '{Name}'.");
            }

            return value;
        }
    }
}