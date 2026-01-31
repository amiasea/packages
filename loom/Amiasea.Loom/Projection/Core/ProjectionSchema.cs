using System;
using System.Collections.Generic;
using System.Linq;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionSchema
    {
        private readonly Dictionary<Type, ProjectionObjectType> _objects;
        private readonly Dictionary<Type, IProjectionOutputType> _outputs;

        public IReadOnlyList<ProjectionObjectType> ObjectTypes => _objects.Values.ToList();
        public IReadOnlyList<IProjectionOutputType> OutputTypes => _outputs.Values.ToList();

        public ProjectionSchema(
            IEnumerable<ProjectionObjectType> objects,
            IEnumerable<IProjectionOutputType> outputs)
        {
            _objects = objects.ToDictionary(o => o.ClrType);
            _outputs = outputs.ToDictionary(o => o.ClrType);
        }

        public ProjectionObjectType GetObject(Type clr)
        {
            _objects.TryGetValue(clr, out var obj);
            return obj;
        }

        public IProjectionOutputType GetOutput(Type clr)
        {
            _outputs.TryGetValue(clr, out var output);
            return output;
        }
    }
}