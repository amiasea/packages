using System.Collections.Generic;

namespace Amiasea.Loom.Projection
{
    public sealed class FilterNode
    {
        // Comparison
        public string Field { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }

        // Logical
        public List<FilterNode> And { get; set; }
        public List<FilterNode> Or { get; set; }
        public FilterNode Not { get; set; }

        public FilterNode()
        {
            // C# 7.3 cannot use nullable annotations or init-only setters.
            // Everything is optional, so we leave them null by default.
        }
    }
}