using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amiasea.Loom.Execution;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionFieldDefinition : IProjectionFieldDefinition
    {
        public string Name { get; private set; }
        public IProjectionOutputType ReturnType { get; private set; }
        public IReadOnlyList<IProjectionArgumentDefinition> Arguments { get; private set; }
        public bool AllowExtraArguments { get; private set; }

        private readonly Func<string, ProjectionArgumentValue, IProjectionInputType> _extraArgumentTypeResolver;

        public ProjectionFieldDefinition(
            string name,
            IProjectionOutputType returnType,
            IEnumerable<IProjectionArgumentDefinition> arguments,
            bool allowExtraArguments,
            Func<string, ProjectionArgumentValue, IProjectionInputType> extraArgumentTypeResolver)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (returnType == null) throw new ArgumentNullException(nameof(returnType));
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));

            Name = name;
            ReturnType = returnType;
            Arguments = arguments.ToArray();
            AllowExtraArguments = allowExtraArguments;
            _extraArgumentTypeResolver = extraArgumentTypeResolver;
        }

        public IProjectionInputType InferExtraArgumentType(
            string name,
            ProjectionArgumentValue rawValue)
        {
            if (!AllowExtraArguments)
            {
                throw new InvalidOperationException(
                    "Field '" + Name + "' does not allow extra arguments.");
            }

            if (_extraArgumentTypeResolver == null)
            {
                throw new InvalidOperationException(
                    "No extra argument type resolver configured for field '" + Name + "'.");
            }

            return _extraArgumentTypeResolver(name, rawValue);
        }
    }
}
