package module

import (
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/irgraph"
)

func FromIR(g *irgraph.Graph) ([]Module, error) {
	mods := make([]Module, 0, len(g.Nodes))
	for name := range g.Nodes {
		mods = append(mods, Module{Name: string(name)})
	}
	return mods, nil
}
