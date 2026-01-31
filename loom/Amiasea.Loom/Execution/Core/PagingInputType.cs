using System;
using System.Collections.Generic;
using System.Text;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Execution
{
    public sealed class PagingInputType : IPagingInputType
    {
        public string Name
        {
            get { return "paging"; }
        }

        public IReadOnlyList<IProjectionInputFieldDefinition> Fields { get; private set; }

        public bool AllowExtraFields
        {
            get { return false; }
        }

        public PagingInputType(
            IProjectionInputType intType,
            IOrderByInputType orderByType)
        {
            if (intType == null) throw new ArgumentNullException(nameof(intType));
            if (orderByType == null) throw new ArgumentNullException(nameof(orderByType));

            Fields = new IProjectionInputFieldDefinition[]
            {
                new PagingField("limit", intType),
                new PagingField("offset", intType),
                new PagingField("order_by", new OrderByListType(orderByType))
            };
        }

        public IProjectionInputType InferExtraFieldType(string name, ProjectionArgumentValue rawValue)
        {
            throw new InvalidOperationException("paging does not allow extra fields");
        }

        private sealed class PagingField : IProjectionInputFieldDefinition
        {
            public string Name { get; private set; }
            public IProjectionInputType Type { get; private set; }
            public bool IsNonNull
            {
                get { return false; }
            }

            public ProjectionArgumentValue DefaultValue
            {
                get { return null; }
            }

            public PagingField(string name, IProjectionInputType type)
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (type == null) throw new ArgumentNullException(nameof(type));

                Name = name;
                Type = type;
            }
        }

        private sealed class OrderByListType : IProjectionListType
        {
            public string Name
            {
                get { return "[" + ElementType.Name + "]"; }
            }

            public IProjectionInputType ElementType { get; private set; }

            public OrderByListType(IProjectionInputType elementType)
            {
                if (elementType == null) throw new ArgumentNullException(nameof(elementType));
                ElementType = elementType;
            }
        }
    }
}
