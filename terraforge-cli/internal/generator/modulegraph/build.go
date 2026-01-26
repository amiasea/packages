package modulegraph

import (
	"github.com/amiasea/packages/terraforge-cli/internal/generator/irgraph"
)

// Build converts an IR graph (irgraph.Graph) into a module-ready graph (modulegraph.Graph).
func Build(irG *irgraph.Graph) (*Graph, error) {
	mg := New()

	for _, irNode := range irG.Nodes {
		res := irNode.Resource

		// Create modulegraph node
		mNode := &Node{
			Name:       res.Name,
			Type:       res.Type,
			Attributes: make(map[string]Value),
			DependsOn:  append([]string{}, res.DependsOn...),
		}

		// Convert attributes
		for key, val := range res.Attributes {
			if val.Reference != nil {
				mNode.Attributes[key] = Value{
					Reference: &Reference{
						Resource: val.Reference.Resource,
						Field:    val.Reference.Field,
					},
				}
			} else {
				mNode.Attributes[key] = Value{
					Literal: val.Literal,
				}
			}
		}

		mg.Nodes[mNode.Name] = mNode
	}

	return mg, nil
}
