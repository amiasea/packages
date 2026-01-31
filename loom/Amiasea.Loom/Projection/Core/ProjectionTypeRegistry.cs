using System;
using System.Collections.Concurrent;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionTypeRegistry : IProjectionTypeRegistry
    {
        private readonly ConcurrentDictionary<Type, IProjectionOutputType> _outputTypes =
            new ConcurrentDictionary<Type, IProjectionOutputType>();

        private readonly ConcurrentDictionary<Type, ProjectionObjectType> _objectTypes =
            new ConcurrentDictionary<Type, ProjectionObjectType>();

        public ProjectionTypeRegistry(ProjectionSchema schema)
        {
            if (schema == null)
                throw new ArgumentNullException(nameof(schema));

            foreach (var obj in schema.ObjectTypes)
            {
                _objectTypes[obj.ClrType] = obj;
                _outputTypes[obj.ClrType] = obj;
            }

            foreach (var output in schema.OutputTypes)
            {
                _outputTypes[output.ClrType] = output;
            }
        }

        public ProjectionObjectType GetObjectType(Type clrType)
        {
            if (clrType == null)
                return null;

            _objectTypes.TryGetValue(clrType, out var result);
            return result;
        }

        public IProjectionOutputType GetOutputType(Type clrType)
        {
            if (clrType == null)
                return null;

            _outputTypes.TryGetValue(clrType, out var result);
            return result;
        }
    }
}