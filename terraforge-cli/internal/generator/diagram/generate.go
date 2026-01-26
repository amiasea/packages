package diagram

import (
	"fmt"
	"os"
	"path/filepath"

	"github.com/amiasea/packages/terraforge-cli/internal/generator/modulegraph"
)

// Config controls where the diagram is written.
type Config struct {
	OutputDir string
	Filename  string
}

// Generate writes a DOT graph representing the module dependency graph.
func Generate(g *modulegraph.Graph, cfg Config) error {
	if cfg.OutputDir == "" {
		return fmt.Errorf("output directory is required")
	}
	if cfg.Filename == "" {
		cfg.Filename = "diagram.dot"
	}

	// Ensure output directory exists.
	if err := os.MkdirAll(cfg.OutputDir, 0o755); err != nil {
		return fmt.Errorf("failed to create diagram output dir: %w", err)
	}

	outPath := filepath.Join(cfg.OutputDir, cfg.Filename)
	f, err := os.Create(outPath)
	if err != nil {
		return fmt.Errorf("failed to create diagram file: %w", err)
	}
	defer f.Close()

	// Begin DOT graph.
	fmt.Fprintln(f, "digraph terraforge {")
	fmt.Fprintln(f, `  rankdir="LR";`)
	fmt.Fprintln(f, `  node [shape=box, style="rounded,filled", fillcolor="#eef"];`)

	// Emit nodes.
	for id, n := range g.Nodes {
		label := fmt.Sprintf("%s\n(%s)", n.Name, n.Type)
		fmt.Fprintf(f, "  \"%s\" [label=\"%s\"];\n", id, label)
	}

	// Emit edges by reconstructing them from DependsOn.
	for id, n := range g.Nodes {
		for _, dep := range n.DependsOn {
			fmt.Fprintf(f, "  \"%s\" -> \"%s\";\n", id, dep)
		}
	}

	// End DOT graph.
	fmt.Fprintln(f, "}")

	return nil
}
