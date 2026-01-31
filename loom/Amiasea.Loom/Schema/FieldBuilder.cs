using System;
using System.Collections.Generic;

namespace Amiasea.Loom.Schema
{
    public sealed class FieldBuilder
    {
        private readonly TypeBuilder _parent;
        private readonly string _name;

        private Type _type;
        private FieldKind _kind;
        private bool _nullable = true;

        private bool _isEnum;
        private List<string> _enumValues;

        private bool _isList;
        private Type _elementType;

        private List<string> _operators = new List<string>();

        private bool _isRangeBoundary;
        private string _rangeGroup;
        private string _rangeRole;

        private string _temporalGroup;
        private string _temporalRole;

        private string _geoGroup;
        private string _geoRole;

        private List<FieldDependency> _dependencies = new List<FieldDependency>();

        public FieldBuilder(TypeBuilder parent, string name)
        {
            _parent = parent;
            _name = name;
        }

        public FieldBuilder Type(Type t, FieldKind kind)
        {
            _type = t;
            _kind = kind;
            return this;
        }

        public FieldBuilder Nullable(bool value)
        {
            _nullable = value;
            return this;
        }

        public FieldBuilder Enum(params string[] values)
        {
            _isEnum = true;
            _enumValues = new List<string>(values);
            return this;
        }

        public FieldBuilder List(Type elementType)
        {
            _isList = true;
            _elementType = elementType;
            return this;
        }

        public FieldBuilder Operators(params string[] ops)
        {
            _operators.AddRange(ops);
            return this;
        }

        public FieldBuilder Range(string group, string role)
        {
            _isRangeBoundary = true;
            _rangeGroup = group;
            _rangeRole = role;
            return this;
        }

        public FieldBuilder Temporal(string group, string role)
        {
            _temporalGroup = group;
            _temporalRole = role;
            return this;
        }

        public FieldBuilder Geo(string group, string role)
        {
            _geoGroup = group;
            _geoRole = role;
            return this;
        }

        public FieldBuilder DependsOn(string whenField, string whenEquals, string targetField)
        {
            _dependencies.Add(new FieldDependency(targetField, whenField, whenEquals));
            return this;
        }

        public TypeBuilder EndField()
        {
            var schema = new FieldSchema(
                _name,
                _type,
                _kind,
                _nullable,
                _operators,
                _isEnum,
                _enumValues,
                _isList,
                _elementType,
                _isRangeBoundary,
                _rangeGroup,
                _rangeRole,
                _temporalGroup,
                _temporalRole,
                _geoGroup,
                _geoRole,
                _dependencies
            );

            _parent.AddField(schema);
            return _parent;
        }
    }
}