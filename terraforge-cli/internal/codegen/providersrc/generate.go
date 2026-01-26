package providersrc

import (
	"fmt"
	"os"
	"path/filepath"

	"github.com/amiasea/packages/terraforge-cli/internal/codegen/modulegraph"
)

type Config struct {
	OutputDir string
	Package   string
}

func GenerateFromGraphInternal(gr *modulegraph.Graph, cfg Config) error {
	if cfg.OutputDir == "" {
		return fmt.Errorf("output directory is required")
	}
	if cfg.Package == "" {
		cfg.Package = "main"
	}

	if err := os.MkdirAll(cfg.OutputDir, 0o755); err != nil {
		return fmt.Errorf("failed to create provider output dir: %w", err)
	}

	code := fmt.Sprintf(`package %s

import "fmt"

func main() {
    fmt.Println("Terraforge provider stub - resources: %d")
}
`, cfg.Package, len(gr.Nodes))

	outPath := filepath.Join(cfg.OutputDir, "main.go")
	if err := os.WriteFile(outPath, []byte(code), 0o644); err != nil {
		return fmt.Errorf("failed to write provider main.go: %w", err)
	}

	return nil
}
