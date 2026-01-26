package codegen

import (
	"path/filepath"

	"github.com/amiasea/packages/terraforge-cli/internal/codegen/diagram"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/irbuild"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/irgraph"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/model"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/module"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/modulegraph"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/providersrc"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/schema"
	"github.com/amiasea/packages/terraforge-cli/internal/ir"
)

// Result bundles the outputs of the compiler build pipeline.
type Result struct {
	Schema      *schema.Schema
	Model       *model.Model
	IR          *ir.IR
	IRGraph     *irgraph.Graph
	ModuleGraph *modulegraph.Graph
}

// Build performs the canonical Terraforge compiler pipeline.
func Build(schemaPath string, outDir string) (*Result, error) {
	// 1. Load schema.json
	s, err := schema.Load(schemaPath)
	if err != nil {
		return nil, err
	}

	// 2. Convert schema → generator model
	gm, err := model.FromSchema(s)
	if err != nil {
		return nil, err
	}

	// 3. Convert generator model → canonical IR
	irModel, err := irbuild.Build(gm)
	if err != nil {
		return nil, err
	}

	// 4. IR → IRGraph
	irg, err := irgraph.Build(irModel)
	if err != nil {
		return nil, err
	}

	// 5. IRGraph → ModuleGraph
	mg, err := modulegraph.Build(irg)
	if err != nil {
		return nil, err
	}

	// 6. Run artifact generators
	if err := module.GenerateFromGraph(mg, module.Config{
		OutputDir: filepath.Join(outDir, "modules"),
	}); err != nil {
		return nil, err
	}

	if err := providersrc.GenerateFromGraph(mg, providersrc.Config{
		OutputDir: filepath.Join(outDir, "provider"),
		Package:   "main",
	}); err != nil {
		return nil, err
	}

	if err := diagram.Generate(mg, diagram.Config{
		OutputDir: filepath.Join(outDir, "diagram"),
		Filename:  "diagram.dot",
	}); err != nil {
		return nil, err
	}

	return &Result{
		Schema:      s,
		Model:       gm,
		IR:          irModel,
		IRGraph:     irg,
		ModuleGraph: mg,
	}, nil
}
