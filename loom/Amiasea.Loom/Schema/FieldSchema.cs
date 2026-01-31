using System;
using System.Collections.Generic;

namespace Amiasea.Loom.Schema
{
    public sealed class FieldSchema : IFieldSchema
    {
        private readonly string _name;
        private readonly Type _type;
        private readonly Type _elementType;
        private readonly FieldKind _kind;
        private readonly bool _isNullable;
        private readonly bool _isEnum;
        private readonly IReadOnlyList<string> _enumValues;
        private readonly bool _isList;
        private readonly IReadOnlyList<string> _allowedOperators;

        private readonly bool _isRangeBoundary;
        private readonly string _rangeGroup;
        private readonly string _rangeRole;

        private readonly string _temporalGroup;
        private readonly string _temporalRole;

        private readonly string _geoGroup;
        private readonly string _geoRole;

        private readonly IReadOnlyList<FieldDependency> _dependencies;

        public FieldSchema(
            string name,
            Type type,
            FieldKind kind,
            bool isNullable,
            IReadOnlyList<string> allowedOperators,
            bool isEnum,
            IReadOnlyList<string> enumValues,
            bool isList,
            Type elementType,
            bool isRangeBoundary,
            string rangeGroup,
            string rangeRole,
            string temporalGroup,
            string temporalRole,
            string geoGroup,
            string geoRole,
            IReadOnlyList<FieldDependency> dependencies)
        {
            _name = name;
            _type = type;
            _kind = kind;
            _isNullable = isNullable;
            _allowedOperators = allowedOperators;

            _isEnum = isEnum;
            _enumValues = enumValues;

            _isList = isList;
            _elementType = elementType;

            _isRangeBoundary = isRangeBoundary;
            _rangeGroup = rangeGroup;
            _rangeRole = rangeRole;

            _temporalGroup = temporalGroup;
            _temporalRole = temporalRole;

            _geoGroup = geoGroup;
            _geoRole = geoRole;

            _dependencies = dependencies;
        }

        public string Name { get { return _name; } }
        public Type Type { get { return _type; } }
        public Type ElementType { get { return _elementType; } }

        public FieldKind Kind { get { return _kind; } }
        public bool IsNullable { get { return _isNullable; } }

        public bool IsEnum { get { return _isEnum; } }
        public IReadOnlyList<string> EnumValues { get { return _enumValues; } }

        public bool IsList { get { return _isList; } }
        public IReadOnlyList<string> AllowedOperators { get { return _allowedOperators; } }

        public bool IsRangeBoundary { get { return _isRangeBoundary; } }
        public string RangeGroup { get { return _rangeGroup; } }
        public string RangeRole { get { return _rangeRole; } }

        public string TemporalGroup { get { return _temporalGroup; } }
        public string TemporalRole { get { return _temporalRole; } }

        public string GeoGroup { get { return _geoGroup; } }
        public string GeoRole { get { return _geoRole; } }

        public IReadOnlyList<FieldDependency> Dependencies { get { return _dependencies; } }
    }
}