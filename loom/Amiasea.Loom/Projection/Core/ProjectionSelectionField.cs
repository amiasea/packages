namespace Amiasea.Loom.Projection
{
    /// <summary>
    /// Represents a single selected field in the projection IR.
    /// </summary>
    public sealed class ProjectionSelectionField
    {
        /// <summary>
        /// The name of the field being selected.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Nested selection for object fields, or null for scalars.
        /// </summary>
        public ProjectionSelection Nested { get; private set; }

        public ProjectionSelectionField(string name, ProjectionSelection nested)
        {
            Name = name;
            Nested = nested;
        }
    }
}