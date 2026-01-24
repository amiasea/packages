package provider

import (
    "fmt"
    "os"
    "path/filepath"

    "github.com/amiasea/packages/terraforge-cli/internal/generator/model"
)

type Config struct {
    OutputDir string
    Package   string
}

// Generate creates the provider source code from the model.
func Generate(g *model.Graph, cfg Config) error {
    if cfg.OutputDir == "" {
        return fmt.Errorf("output directory is required")
    }
    if cfg.Package == "" {
        cfg.Package = "main"
    }

    if err := os.MkdirAll(cfg.OutputDir, 0o755); err != nil {
        return fmt.Errorf("failed to create provider output dir: %w", err)
    }

    // Minimal placeholder for now.
    // Later: generate resources, schema, CRUD, etc.
    code := fmt.Sprintf(`package %s

import "fmt"

func main() {
    fmt.Println("Terraforge provider stub - resources: %d")
}
`, cfg.Package, len(g.Nodes))

    outPath := filepath.Join(cfg.OutputDir, "main.go")
    if err := os.WriteFile(outPath, []byte(code), 0o644); err != nil {
        return fmt.Errorf("failed to write provider main.go: %w", err)
    }

    return nil
}