using System;
using System.Collections.Generic;

namespace Amiasea.Loom.AST
{
    // query, mutation, etc. Loom only cares about:
    // - operation name (optional)
    // - operation type ("query" / "mutation")
    // - root selection set
    public sealed class OperationNode
    {
        public OperationNode(
            string name,                 // may be null
            OperationKind kind,
            SelectionSetNode selectionSet
        )
        {
            Name = name;
            Kind = kind;
            SelectionSet = selectionSet;
        }

        public string Name { get; }              // null allowed
        public OperationKind Kind { get; }
        public SelectionSetNode SelectionSet { get; }
    }
}