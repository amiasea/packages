using System;
using System.Collections.Generic;
using System.Text;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Execution
{
    public sealed class NormalizedArgumentValue
    {
        public NormalizedArgumentKind Kind { get; private set; }
        public object Value { get; private set; }
        public IProjectionInputType Type { get; private set; }

        private NormalizedArgumentValue(
            NormalizedArgumentKind kind,
            object value,
            IProjectionInputType type)
        {
            Kind = kind;
            Value = value;
            Type = type;
        }

        public static NormalizedArgumentValue Scalar(object value, IProjectionScalarType type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return new NormalizedArgumentValue(NormalizedArgumentKind.Scalar, value, type);
        }

        public static NormalizedArgumentValue Enum(object value, IProjectionInputEnumType type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return new NormalizedArgumentValue(NormalizedArgumentKind.Enum, value, type);
        }

        public static NormalizedArgumentValue List(
            IReadOnlyList<NormalizedArgumentValue> elements,
            IProjectionInputType elementType)
        {
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            if (elementType == null) throw new ArgumentNullException(nameof(elementType));

            return new NormalizedArgumentValue(
                NormalizedArgumentKind.List,
                elements,
                new ProjectionListTypeProxy(elementType));
        }

        public static NormalizedArgumentValue Object(
            IReadOnlyDictionary<string, NormalizedArgumentValue> fields,
            IProjectionInputObjectType type)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            if (type == null) throw new ArgumentNullException(nameof(type));

            return new NormalizedArgumentValue(
                NormalizedArgumentKind.Object,
                fields,
                type);
        }

        public static NormalizedArgumentValue Variable(
            string name,
            IProjectionInputType type)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (type == null) throw new ArgumentNullException(nameof(type));

            return new NormalizedArgumentValue(
                NormalizedArgumentKind.Variable,
                name,
                type);
        }
    }
}
