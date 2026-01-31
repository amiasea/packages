using System;
using System.Collections.Generic;
using System.Text;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Execution
{
    public sealed class ComparisonInput
    {
        public ProjectionArgumentValue Eq { get; private set; }
        public ProjectionArgumentValue Ne { get; private set; }
        public ProjectionArgumentValue Gt { get; private set; }
        public ProjectionArgumentValue Gte { get; private set; }
        public ProjectionArgumentValue Lt { get; private set; }
        public ProjectionArgumentValue Lte { get; private set; }
        public ProjectionArgumentValue Like { get; private set; }
        public ProjectionArgumentValue Between { get; private set; }

        public ComparisonInput(
            ProjectionArgumentValue eq,
            ProjectionArgumentValue ne,
            ProjectionArgumentValue gt,
            ProjectionArgumentValue gte,
            ProjectionArgumentValue lt,
            ProjectionArgumentValue lte,
            ProjectionArgumentValue like,
            ProjectionArgumentValue between)
        {
            Eq = eq;
            Ne = ne;
            Gt = gt;
            Gte = gte;
            Lt = lt;
            Lte = lte;
            Like = like;
            Between = between;
        }
    }
}
