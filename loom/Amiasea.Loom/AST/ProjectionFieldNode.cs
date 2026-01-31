using System;
using System.Collections.Generic;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.AST
{

    public sealed class ProjectionFieldNode
    {
        public string Name { get; private set; }
        public IReadOnlyDictionary<string, ProjectionArgumentValue> Arguments { get; private set; }
        public IReadOnlyList<ProjectionFieldNode> Children { get; private set; }
        public string Alias { get; private set; }
        public IReadOnlyList<ProjectionDirective> Directives { get; private set; }

        public ProjectionFieldNode(
            string name,
            IReadOnlyDictionary<string, ProjectionArgumentValue> arguments,
            IReadOnlyList<ProjectionFieldNode> children,
            string alias,
            IReadOnlyList<ProjectionDirective> directives)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            if (children == null) throw new ArgumentNullException(nameof(children));

            Name = name;
            Arguments = arguments;
            Children = children;
            Alias = alias;
            Directives = directives ?? new List<ProjectionDirective>();
        }
    }
}