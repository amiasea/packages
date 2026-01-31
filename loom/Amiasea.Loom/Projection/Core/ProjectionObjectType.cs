using System;
using System.Collections.Generic;
using System.Linq;
using Amiasea.Loom.Schema;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionObjectType : IProjectionOutputType
    {
        private readonly Dictionary<string, ProjectionFieldNode> _fields;

        public string Name { get; }
        public Type ClrType { get; }
        public IReadOnlyList<ProjectionFieldNode> Fields => _fields.Values.ToList();

        public ProjectionObjectType(string name, Type clrType, IEnumerable<ProjectionFieldNode> fields)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ClrType = clrType ?? throw new ArgumentNullException(nameof(clrType));

            _fields = fields.ToDictionary(f => f.Name);
        }

        public ProjectionFieldNode GetField(string name)
        {
            _fields.TryGetValue(name, out var field);
            return field;
        }
    }
}