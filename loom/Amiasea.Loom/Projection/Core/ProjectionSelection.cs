using System.Collections.Generic;

namespace Amiasea.Loom.Projection
{
    /// <summary>
    /// Represents a set of selected fields in the IR.
    /// </summary>
    public sealed class ProjectionSelection
    {
        public IReadOnlyList<ProjectionSelectionField> Fields { get; private set; }

        public ProjectionSelection(IReadOnlyList<ProjectionSelectionField> fields)
        {
            Fields = fields;
        }
    }
}