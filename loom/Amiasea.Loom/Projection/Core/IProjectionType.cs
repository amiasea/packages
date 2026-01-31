using Amiasea.Loom.Schema;

namespace Amiasea.Loom.Projection
{
    public interface IProjectionType
    {
        string Name { get; }

        /// <summary>
        /// Returns the field schema for this type.
        /// </summary>
        IFieldSchema GetField(string fieldName);

        /// <summary>
        /// Returns all fields for introspection.
        /// </summary>
        System.Collections.Generic.IReadOnlyList<IFieldSchema> GetFields();
    }
}