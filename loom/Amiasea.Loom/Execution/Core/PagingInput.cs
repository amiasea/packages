using System;
using System.Collections.Generic;
using System.Text;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Execution
{
    public sealed class PagingInput
    {
        public ProjectionArgumentValue Limit { get; private set; }
        public ProjectionArgumentValue Offset { get; private set; }
        public IReadOnlyList<OrderByInput> OrderBy { get; private set; }

        public PagingInput(
            ProjectionArgumentValue limit,
            ProjectionArgumentValue offset,
            IReadOnlyList<OrderByInput> orderBy)
        {
            Limit = limit;
            Offset = offset;
            OrderBy = orderBy;
        }
    }
}
