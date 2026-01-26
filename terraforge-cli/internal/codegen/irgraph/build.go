package irgraph

import "github.com/amiasea/packages/terraforge-cli/internal/ir"

func Build(ir *ir.IR) (*Graph, error) {
	g := NewGraph()

	for _, res := range ir.Resources {
		g.AddNode(res)
	}

	for _, res := range ir.Resources {
		fromID := NodeID(res.Name)

		for _, dep := range res.DependsOn {
			g.AddEdge(fromID, NodeID(dep))
		}

		for _, attr := range res.Attributes {
			if attr.Reference != nil {
				g.AddEdge(fromID, NodeID(attr.Reference.Resource))
			}
		}
	}

	return g, nil
}
