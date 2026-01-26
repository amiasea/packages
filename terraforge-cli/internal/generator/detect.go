package generator

import (
	genmodel "github.com/amiasea/packages/terraforge-cli/internal/generator/model"
	ir "github.com/amiasea/packages/terraforge-cli/internal/model"
)

// Kind represents the type of generator run requested.
type Kind string

const (
	KindUnknown  Kind = "unknown"
	KindModel    Kind = "model"
	KindDiagram  Kind = "diagram"
	KindModule   Kind = "module"
	KindProvider Kind = "provider"
)

// DetectFromSchema inspects the raw schema model and determines
// what kind of generator run is appropriate.
func DetectFromSchema(m *genmodel.Model) Kind {
	if len(m.Resources) == 0 {
		return KindUnknown
	}
	return KindModel
}

// DetectFromIR inspects the IR graph and determines what generator
// should run based on the structure of the graph.
func DetectFromIR(g *ir.Graph) Kind {
	if len(g.Nodes) == 0 {
		return KindUnknown
	}

	// If the IR contains edges, it’s likely a diagram or module run.
	if len(g.Edges) > 0 {
		return KindDiagram
	}

	// If the IR contains nodes but no edges, it’s likely a module or provider run.
	return KindModule
}
