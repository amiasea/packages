using System;
using System.Collections.Generic;
using Amiasea.Loom.Execution;

namespace Amiasea.Loom.Projection
{
    public sealed class NormalizedProjectionField
    {
        public IProjectionFieldDefinition Definition { get; private set; }
        public IReadOnlyDictionary<string, NormalizedArgumentValue> Arguments { get; private set; }
        public IReadOnlyList<NormalizedProjectionField> Children { get; private set; }
        public string Alias { get; private set; }
        public IReadOnlyList<ProjectionDirective> Directives { get; private set; }

        public NormalizedProjectionField(
            IProjectionFieldDefinition definition,
            IReadOnlyDictionary<string, NormalizedArgumentValue> arguments,
            IReadOnlyList<NormalizedProjectionField> children,
            string alias,
            IReadOnlyList<ProjectionDirective> directives)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            if (children == null) throw new ArgumentNullException(nameof(children));

            Definition = definition;
            Arguments = arguments;
            Children = children;
            Alias = alias;
            Directives = directives ?? new List<ProjectionDirective>();
        }
    }
}

