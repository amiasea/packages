package providersrc

import (
	"path/filepath"

	"github.com/amiasea/packages/terraforge-cli/internal/codegen/modulegraph"
	"github.com/amiasea/packages/terraforge-cli/internal/filesystem"
	generatoriface "github.com/amiasea/packages/terraforge-cli/internal/generatoriface"
)

type ProviderSrcGenerator struct{}

func New() generatoriface.Generator {
	return &ProviderSrcGenerator{}
}

func (g *ProviderSrcGenerator) Name() string {
	return "providersrc"
}

func (g *ProviderSrcGenerator) Generate(fs *filesystem.FS, gr *modulegraph.Graph, outDir string) error {
	cfg := Config{
		OutputDir: filepath.Join(outDir, "providersrc"),
		Package:   "main",
	}

	return GenerateFromGraph(fs, gr, cfg)
}
