package module

import (
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/modulegraph"
)

// GenerateFromGraph takes a modulegraph.Graph, converts it into []Module,
// and then emits the corresponding Terraform module files to disk.
func GenerateFromGraph(g *modulegraph.Graph, cfg Config) error {
	modules, err := TransformGraph(g)
	if err != nil {
		return err
	}

	return Generate(modules, cfg)
}
