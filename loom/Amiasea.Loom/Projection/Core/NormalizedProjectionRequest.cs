using System;
using System.Collections.Generic;

namespace Amiasea.Loom.Projection
{
    public sealed class NormalizedProjectionRequest
    {
        public IProjectionType RootType { get; private set; }
        public IReadOnlyList<NormalizedProjectionField> Fields { get; private set; }
        public ProjectionContext Context { get; private set; }

        public NormalizedProjectionRequest(
            IProjectionType rootType,
            IReadOnlyList<NormalizedProjectionField> fields,
            ProjectionContext context)
        {
            if (rootType == null) throw new ArgumentNullException(nameof(rootType));
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            if (context == null) throw new ArgumentNullException(nameof(context));

            RootType = rootType;
            Fields = fields;
            Context = context;
        }
    }
}
