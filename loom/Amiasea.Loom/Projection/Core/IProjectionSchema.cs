using Amiasea.Loom.Schema;

namespace Amiasea.Loom.Projection
{
    public interface IProjectionSchema
    {
        IProjectionType GetRootType(string name);
        IProjectionType GetTypeByName(string name);

        /// <summary>
        /// Returns the field schema for a given type + field name.
        /// </summary>
        IFieldSchema GetField(string typeName, string fieldName);
    }
}