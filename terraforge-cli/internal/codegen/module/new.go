package module

import (
	"fmt"
	"path/filepath"

	"github.com/amiasea/packages/terraforge-cli/internal/codegen/modulegraph"
	generatoriface "github.com/amiasea/packages/terraforge-cli/internal/generatoriface"
)

type moduleGenerator struct{}

func New() generatoriface.Generator {
	return &moduleGenerator{}
}

func (g *moduleGenerator) Name() string {
	return "module"
}

func formatReference(ref *modulegraph.Reference) string {
	return fmt.Sprintf("${%s.%s}", ref.Resource, ref.Field)
}

func (g *moduleGenerator) Generate(gr *modulegraph.Graph, outDir string) error {
	cfg := Config{
		OutputDir: filepath.Join(outDir, "modules"),
	}

	mods := make([]Module, 0, len(gr.Nodes))

	for _, n := range gr.Nodes {
		attrs := make(map[string]any)

		for k, v := range n.Attributes {
			if v.Reference != nil {
				attrs[k] = formatReference(v.Reference)
			} else {
				attrs[k] = v.Literal
			}
		}

		mods = append(mods, Module{
			Name:       n.Name,
			Type:       n.Type,
			Attributes: attrs,
			DependsOn:  n.DependsOn,
		})
	}

	return Generate(mods, cfg)
}
