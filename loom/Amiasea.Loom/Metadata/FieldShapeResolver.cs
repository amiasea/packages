using System;
using System.Collections.Generic;

namespace Amiasea.Loom.Metadata
{
    public sealed class FieldShapeResolver : IFieldShapeResolver
    {
        private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, FieldShape>> _shapes;

        public FieldShapeResolver(
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, FieldShape>> shapes)
        {
            _shapes = shapes;
        }

        public FieldShape GetShape(string parentType, string fieldName)
        {
            IReadOnlyDictionary<string, FieldShape> fields;

            if (!_shapes.TryGetValue(parentType, out fields))
                throw new InvalidOperationException("Unknown parent type: " + parentType);

            FieldShape shape;

            if (!fields.TryGetValue(fieldName, out shape))
                throw new InvalidOperationException("Unknown field: " + parentType + "." + fieldName);

            return shape;
        }
    }
}