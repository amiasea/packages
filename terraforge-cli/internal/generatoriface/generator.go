package generatoriface

import (
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/modulegraph"
	"github.com/amiasea/packages/terraforge-cli/internal/filesystem"
)

type Generator interface {
	Name() string
	Generate(fs *filesystem.FS, g *modulegraph.Graph, outDir string) error
}
