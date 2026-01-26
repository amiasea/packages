package codegen

import (
	"path/filepath"

	"github.com/amiasea/packages/terraforge-cli/internal/codegen/diagram"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/irgraph"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/module"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/modulegraph"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/providersrc"
	"github.com/amiasea/packages/terraforge-cli/internal/ir"
)

// Generate runs the full backend pipeline:
//
//	IR → IRGraph → ModuleGraph → module files, provider source, diagram
func Generate(ir *ir.IR, outDir string) error {
	// 1. IR → IRGraph
	irg, err := irgraph.Build(ir)
	if err != nil {
		return err
	}

	// 2. IRGraph → ModuleGraph
	mg, err := modulegraph.Build(irg)
	if err != nil {
		return err
	}

	// 3. Generate Terraform modules
	if err := module.GenerateFromGraph(mg, module.Config{
		OutputDir: filepath.Join(outDir, "modules"),
	}); err != nil {
		return err
	}

	// 4. Generate provider source code
	if err := providersrc.GenerateFromGraph(mg, providersrc.Config{
		OutputDir: filepath.Join(outDir, "provider"),
		Package:   "main",
	}); err != nil {
		return err
	}

	// 5. Generate dependency diagram
	if err := diagram.Generate(mg, diagram.Config{
		OutputDir: filepath.Join(outDir, "diagram"),
		Filename:  "diagram.dot",
	}); err != nil {
		return err
	}

	return nil
}
