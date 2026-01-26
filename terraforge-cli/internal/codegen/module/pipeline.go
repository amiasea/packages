package module

import (
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/modulegraph"
	"github.com/amiasea/packages/terraforge-cli/internal/filesystem"
)

// GenerateFromGraph takes a modulegraph.Graph, converts it into []Module,
// and then emits the corresponding Terraform module files to disk.
func GenerateFromGraph(fs *filesystem.FS, g *modulegraph.Graph, cfg Config) error {
	modules, err := TransformGraph(g)
	if err != nil {
		return err
	}

	return Generate(fs, modules, cfg)
}
