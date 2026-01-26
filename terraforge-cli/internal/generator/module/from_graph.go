package module

import (
	"github.com/amiasea/packages/terraforge-cli/internal/generator/modulegraph"
)

// FromGraph converts a modulegraph.Graph into []Module definitions.
func FromGraph(g *modulegraph.Graph) ([]Module, error) {
	modules := []Module{}

	for name, node := range g.Nodes {
		m := Module{
			Name:       name,
			Type:       node.Type,
			Attributes: map[string]any{},
			DependsOn:  node.DependsOn,
		}

		// Copy attributes
		for attrName, attrVal := range node.Attributes {
			if attrVal.Reference != nil {
				m.Attributes[attrName] = map[string]string{
					"resource": attrVal.Reference.Resource,
					"field":    attrVal.Reference.Field,
				}
			} else {
				m.Attributes[attrName] = attrVal.Literal
			}
		}

		modules = append(modules, m)
	}

	return modules, nil
}
