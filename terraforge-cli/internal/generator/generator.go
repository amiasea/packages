package generator

import (
	"github.com/amiasea/packages/terraforge-cli/internal/generator/modulegraph"
)

// Generator is the interface implemented by all Terraforge artifact generators.
// Examples: module generator, provider generator, diagram generator.
type Generator interface {
	// Name returns a human-readable identifier for logging or CLI output.
	Name() string

	// Generate consumes the module graph and writes output files
	// into the specified directory.
	Generate(g *modulegraph.Graph, outDir string) error
}
