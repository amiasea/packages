package generatoriface

import "github.com/amiasea/packages/terraforge-cli/internal/codegen/modulegraph"

type Generator interface {
	Name() string
	Generate(*modulegraph.Graph, string) error
}
