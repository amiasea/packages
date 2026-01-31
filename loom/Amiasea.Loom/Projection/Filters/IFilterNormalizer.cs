using Amiasea.Loom.AST;

namespace Amiasea.Loom.Projection
{
    public interface IFilterNormalizer
    {
        /// <summary>
        /// Normalizes a filter AST into a FilterNode IR.
        /// </summary>
        /// <param name="ast">The parsed filter AST.</param>
        /// <param name="rootType">The name of the root projection type.</param>
        /// <returns>A normalized FilterNode tree.</returns>
        FilterNode Normalize(ObjectValueNode ast, string rootType);
    }
}