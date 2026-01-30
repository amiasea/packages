using System;
using System.Collections.Generic;

namespace Amiasea.Loom.AST
{
    // A field is the atomic unit Loom compiles:
    // - name
    // - alias (optional)
    // - arguments (optional)
    // - nested selection set (optional)
    public sealed class FieldNode : ISelectionNode
    {
        public FieldNode(
            string name,
            string alias,
            IReadOnlyList<ArgumentNode> arguments,
            SelectionSetNode selectionSet
        )
        {
            Name = name;
            Alias = alias;                 // may be null
            Arguments = arguments;
            SelectionSet = selectionSet;   // may be null
        }

        public string Name { get; }
        public string Alias { get; }               // null allowed
        public IReadOnlyList<ArgumentNode> Arguments { get; }
        public SelectionSetNode SelectionSet { get; }   // null allowed
    }
}