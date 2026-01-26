package diagram

import (
	"bytes"
	"fmt"
	"path/filepath"

	"github.com/amiasea/packages/terraforge-cli/internal/codegen/modulegraph"
	"github.com/amiasea/packages/terraforge-cli/internal/filesystem"
)

// Config controls where the diagram is written.
type Config struct {
	OutputDir string
	Filename  string
}

// Generate writes a DOT graph representing the module dependency graph.
func Generate(fs *filesystem.FS, g *modulegraph.Graph, cfg Config) error {
	if cfg.OutputDir == "" {
		return fmt.Errorf("output directory is required")
	}
	if cfg.Filename == "" {
		cfg.Filename = "diagram.dot"
	}

	// Ensure output directory exists.
	if err := fs.WriteFile(filepath.Join(cfg.OutputDir, ".keep"), []byte{}); err != nil {
		return fmt.Errorf("failed to initialize diagram output directory: %w", err)
	}

	outPath := filepath.Join(cfg.OutputDir, cfg.Filename)

	// Build DOT content in memory.
	var buf bytes.Buffer

	fmt.Fprintln(&buf, "digraph terraforge {")
	fmt.Fprintln(&buf, `  rankdir="LR";`)
	fmt.Fprintln(&buf, `  node [shape=box, style="rounded,filled", fillcolor="#eef"];`)

	// Emit nodes.
	for id, n := range g.Nodes {
		label := fmt.Sprintf("%s\n(%s)", n.Name, n.Type)
		fmt.Fprintf(&buf, "  \"%s\" [label=\"%s\"];\n", id, label)
	}

	// Emit edges from DependsOn.
	for id, n := range g.Nodes {
		for _, dep := range n.DependsOn {
			fmt.Fprintf(&buf, "  \"%s\" -> \"%s\";\n", id, dep)
		}
	}

	fmt.Fprintln(&buf, "}")

	// Write the DOT file.
	if err := fs.WriteFile(outPath, buf.Bytes()); err != nil {
		return fmt.Errorf("failed to write diagram file %s: %w", outPath, err)
	}

	return nil
}
