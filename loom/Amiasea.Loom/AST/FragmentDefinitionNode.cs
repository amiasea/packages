using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.AST
{

    // Fragments – needed for real-world queries, but still purely structural.
    public sealed class FragmentDefinitionNode
    {
        public FragmentDefinitionNode(
            string name,
            string typeCondition,
            SelectionSetNode selectionSet
        )
        {
            Name = name;
            TypeCondition = typeCondition;
            SelectionSet = selectionSet;
        }

        public string Name { get; }
        public string TypeCondition { get; }
        public SelectionSetNode SelectionSet { get; }
    }
}