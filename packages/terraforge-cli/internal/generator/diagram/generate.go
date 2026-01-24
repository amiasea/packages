package plan

import (
    "fmt"
    "os"
    "path/filepath"

    "github.com/amiasea/packages/terraforge-cli/internal/generator/model"
)

type Config struct {
    OutputDir string
    Filename  string
}

// Generate writes a DOT graph representing the resource dependency model.
func Generate(g *model.Graph, cfg Config) error {
    if cfg.OutputDir == "" {
        return fmt.Errorf("output directory is required")
    }
    if cfg.Filename == "" {
        cfg.Filename = "plan.dot"
    }

    if err := os.MkdirAll(cfg.OutputDir, 0o755); err != nil {
        return fmt.Errorf("failed to create plan output dir: %w", err)
    }

    outPath := filepath.Join(cfg.OutputDir, cfg.Filename)

    f, err := os.Create(outPath)
    if err != nil {
        return fmt.Errorf("failed to create plan file: %w", err)
    }
    defer f.Close()

    // DOT header
    fmt.Fprintln(f, "digraph terraforge_plan {")
    fmt.Fprintln(f, `  rankdir="LR";`)
    fmt.Fprintln(f, `  node [shape=box, style="rounded,filled", fillcolor="#eef"];`)

    // Nodes
    for name := range g.Nodes {
        fmt.Fprintf(f, "  %q;\n", name)
    }

    // Edges
    for _, e := range g.Edges {
        fmt.Fprintf(f, "  %q -> %q;\n", e.From, e.To)
    }

    fmt.Fprintln(f, "}")

    return nil
}