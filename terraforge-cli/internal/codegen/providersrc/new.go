package providersrc

import (
	"path/filepath"

	"github.com/amiasea/packages/terraforge-cli/internal/codegen/modulegraph"
	generatoriface "github.com/amiasea/packages/terraforge-cli/internal/generatoriface"
)

type ProviderSrcGenerator struct{}

func New() generatoriface.Generator {
	return &ProviderSrcGenerator{}
}

func (g *ProviderSrcGenerator) Name() string {
	return "providersrc"
}

func (g *ProviderSrcGenerator) Generate(gr *modulegraph.Graph, outDir string) error {
	cfg := Config{
		OutputDir: filepath.Join(outDir, "providersrc"),
		Package:   "main",
	}
	return GenerateFromGraph(gr, cfg)
}
