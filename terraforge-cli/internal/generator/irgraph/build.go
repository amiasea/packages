package irgraph

import ir "github.com/amiasea/packages/terraforge-cli/internal/model"

// Build constructs a dependency graph from the IR.
// It performs dependency inference based on attribute references.
func Build(schema *ir.SchemaIR) (*Graph, error) {
	g := NewGraph()

	// 1. Add all resources as nodes.
	for _, res := range schema.Resources {
		g.AddNode(res)
	}

	// 2. Infer edges from attribute references and DependsOn.
	for _, res := range schema.Resources {
		fromID := NodeID(res.Name)

		// Explicit depends_on
		for _, depName := range res.DependsOn {
			toID := NodeID(depName)
			g.AddEdge(fromID, toID)
		}

		// Attribute-based references
		for _, val := range res.Attributes {
			if val.Reference == nil {
				continue
			}
			toID := NodeID(val.Reference.Resource)
			g.AddEdge(fromID, toID)
		}
	}

	return g, nil
}
