using System;
using System.Collections.Generic;
using System.Text;
using Amiasea.Loom.Execution;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionDirective
    {
        public string Name { get; private set; }
        public IReadOnlyDictionary<string, ProjectionArgumentValue> Arguments { get; private set; }

        public ProjectionDirective(
            string name,
            IReadOnlyDictionary<string, ProjectionArgumentValue> arguments)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));

            Name = name;
            Arguments = arguments;
        }
    }
}
