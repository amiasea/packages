package diagram

import (
	"path/filepath"

	"github.com/amiasea/packages/terraforge-cli/internal/codegen/modulegraph"
	"github.com/amiasea/packages/terraforge-cli/internal/filesystem"
	generatoriface "github.com/amiasea/packages/terraforge-cli/internal/generatoriface"
)

// diagramGenerator implements the Generator interface.
type diagramGenerator struct{}

// New returns a diagram generator that satisfies generatoriface.Generator.
func New() generatoriface.Generator {
	return &diagramGenerator{}
}

func (g *diagramGenerator) Name() string {
	return "diagram"
}

// Generate consumes the module graph and writes a DOT diagram.
func (g *diagramGenerator) Generate(fs *filesystem.FS, gr *modulegraph.Graph, outDir string) error {
	cfg := Config{
		OutputDir: filepath.Join(outDir, "diagram"),
		Filename:  "diagram.dot",
	}

	return Generate(fs, gr, cfg)
}
