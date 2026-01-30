using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.AST
{

    // A full GraphQL document: operations + fragments.
    // Loom will compile exactly one Operation at a time.
    public sealed class DocumentNode
    {
        public DocumentNode(
            IReadOnlyList<OperationNode> operations,
            IReadOnlyList<FragmentDefinitionNode> fragments
        )
        {
            Operations = operations;
            Fragments = fragments;
        }

        public IReadOnlyList<OperationNode> Operations { get; }
        public IReadOnlyList<FragmentDefinitionNode> Fragments { get; }
    }
}