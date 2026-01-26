package providersrc

import (
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/modulegraph"
)

// GenerateFromGraph emits provider source code directly from a modulegraph.Graph.
func GenerateFromGraph(g *modulegraph.Graph, cfg Config) error {
	return GenerateFromGraphInternal(g, cfg)
}
