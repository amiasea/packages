using System;
using System.Collections.Generic;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Schema
{
    public sealed class TypeBuilder
    {
        private readonly SchemaBuilder _parent;
        private readonly string _name;
        private readonly List<IFieldSchema> _fields = new List<IFieldSchema>();

        public TypeBuilder(SchemaBuilder parent, string name)
        {
            _parent = parent;
            _name = name;
        }

        public FieldBuilder Field(string name)
        {
            return new FieldBuilder(this, name);
        }

        internal void AddField(IFieldSchema field)
        {
            _fields.Add(field);
        }

        public SchemaBuilder EndType()
        {
            _parent.AddType(new ProjectionObjectType(_name, _fields));
            return _parent;
        }
    }
}