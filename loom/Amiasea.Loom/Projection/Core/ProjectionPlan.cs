using System;
using System.Collections.Generic;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionPlan
    {
        public Type RootClrType { get; private set; }
        public ProjectionSelection Selection { get; private set; }
        public IReadOnlyList<string> Includes { get; private set; }
        public FilterNode Filter { get; private set; }

        public ProjectionPlan(
            Type rootClrType,
            ProjectionSelection selection,
            IReadOnlyList<string> includes,
            FilterNode filter)
        {
            RootClrType = rootClrType;
            Selection = selection;
            Includes = includes;
            Filter = filter;
        }
    }
}