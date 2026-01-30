using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.AST
{

    // A selection set is just a list of selections.
    // For Loom v1, we only support FieldNode as a selection.
    public sealed class SelectionSetNode
    {
        public SelectionSetNode(IReadOnlyList<ISelectionNode> selections)
        {
            Selections = selections;
        }

        public IReadOnlyList<ISelectionNode> Selections { get; }
    }
}