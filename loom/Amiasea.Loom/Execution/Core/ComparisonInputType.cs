using System;
using System.Collections.Generic;
using System.Text;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Execution
{
    public sealed class ComparisonInputType : IComparisonInputType
    {
        public string Name
        {
            get { return "comparison"; }
        }

        public IReadOnlyList<IProjectionInputFieldDefinition> Fields { get; private set; }

        public bool AllowExtraFields
        {
            get { return false; }
        }

        public ComparisonInputType(IProjectionInputType scalarType)
        {
            if (scalarType == null) throw new ArgumentNullException(nameof(scalarType));

            Fields = new IProjectionInputFieldDefinition[]
            {
                new ComparisonField("eq", scalarType),
                new ComparisonField("ne", scalarType),
                new ComparisonField("gt", scalarType),
                new ComparisonField("gte", scalarType),
                new ComparisonField("lt", scalarType),
                new ComparisonField("lte", scalarType),
                new ComparisonField("like", scalarType),
                new ComparisonField("between", new ComparisonBetweenListType(scalarType))
            };
        }

        public IProjectionInputType InferExtraFieldType(string name, ProjectionArgumentValue rawValue)
        {
            throw new InvalidOperationException("comparison input does not allow extra fields");
        }

        private sealed class ComparisonField : IProjectionInputFieldDefinition
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

            public ComparisonField(string name, IProjectionInputType type)
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (type == null) throw new ArgumentNullException(nameof(type));

                Name = name;
                Type = type;
            }
        }

        private sealed class ComparisonBetweenListType : IProjectionListType
        {
            public string Name
            {
                get { return "[" + ElementType.Name + "]"; }
            }

            public IProjectionInputType ElementType { get; private set; }

            public ComparisonBetweenListType(IProjectionInputType elementType)
            {
                if (elementType == null) throw new ArgumentNullException(nameof(elementType));
                ElementType = elementType;
            }
        }
    }
}
