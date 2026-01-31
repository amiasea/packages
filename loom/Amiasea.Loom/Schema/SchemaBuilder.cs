using System;
using System.Collections.Generic;
using Amiasea.Loom.Schema;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Schema
{
    public sealed class SchemaBuilder
    {
        private readonly List<IProjectionType> _types = new List<IProjectionType>();
        private readonly List<IProjectionType> _rootTypes = new List<IProjectionType>();

        public TypeBuilder Type(string name)
        {
            return new TypeBuilder(this, name);
        }

        internal void AddType(IProjectionType type)
        {
            _types.Add(type);
        }

        public SchemaBuilder Root(string typeName)
        {
            IProjectionType t = _types.Find(x => x.Name == typeName);
            if (t == null)
                throw new InvalidOperationException("Unknown type '" + typeName + "'.");

            _rootTypes.Add(t);
            return this;
        }

        public IProjectionSchema Build()
        {
            return new ProjectionSchema(_types, _rootTypes);
        }
    }
}