using System;
using System.Collections.Generic;
using System.Text;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Execution
{
    public sealed class OrderByInputType : IOrderByInputType
    {
        public string Name
        {
            get { return "order_by"; }
        }

        public IReadOnlyList<IProjectionInputFieldDefinition> Fields { get; private set; }

        public bool AllowExtraFields
        {
            get { return false; }
        }

        public OrderByInputType()
        {
            Fields = new IProjectionInputFieldDefinition[]
            {
                new OrderByField("field", new StringScalarType(), true),
                new OrderByField("direction", new StringScalarType(), true)
            };
        }

        public IProjectionInputType InferExtraFieldType(string name, ProjectionArgumentValue rawValue)
        {
            throw new InvalidOperationException("order_by does not allow extra fields");
        }

        private sealed class OrderByField : IProjectionInputFieldDefinition
        {
            public string Name { get; private set; }
            public IProjectionInputType Type { get; private set; }
            public bool IsNonNull { get; private set; }
            public ProjectionArgumentValue DefaultValue
            {
                get { return null; }
            }

            public OrderByField(string name, IProjectionInputType type, bool isNonNull)
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (type == null) throw new ArgumentNullException(nameof(type));

                Name = name;
                Type = type;
                IsNonNull = isNonNull;
            }
        }
    }
}
