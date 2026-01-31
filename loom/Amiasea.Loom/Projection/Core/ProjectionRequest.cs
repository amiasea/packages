using System;
using System.Collections.Generic;
using System.Text;
using Amiasea.Loom.Execution;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionRequest
    {
        public string RootTypeName { get; private set; }
        public IReadOnlyList<ProjectionFieldNode> Fields { get; private set; }
        public ProjectionContext Context { get; private set; }

        public ProjectionRequest(
            string rootTypeName,
            IReadOnlyList<ProjectionFieldNode> fields,
            ProjectionContext context)
        {
            if (rootTypeName == null) throw new ArgumentNullException(nameof(rootTypeName));
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            if (context == null) throw new ArgumentNullException(nameof(context));

            RootTypeName = rootTypeName;
            Fields = fields;
            Context = context;
        }
    }
}
