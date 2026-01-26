package module

import ir "github.com/amiasea/packages/terraforge-cli/internal/model"

func FromIR(g *ir.Graph) ([]Module, error) {
	mods := make([]Module, 0, len(g.Nodes))
	for name := range g.Nodes {
		mods = append(mods, Module{Name: name})
	}
	return mods, nil
}
