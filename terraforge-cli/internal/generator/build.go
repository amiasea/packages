package generator

import (
	irgraph "github.com/amiasea/packages/terraforge-cli/internal/generator/irgraph"
	genmodel "github.com/amiasea/packages/terraforge-cli/internal/generator/model"
	modulegraph "github.com/amiasea/packages/terraforge-cli/internal/generator/modulegraph"
	schema "github.com/amiasea/packages/terraforge-cli/internal/generator/schema"

	"github.com/amiasea/packages/terraforge-cli/internal/generator/diagram"
	"github.com/amiasea/packages/terraforge-cli/internal/generator/module"
	"github.com/amiasea/packages/terraforge-cli/internal/generator/providersrc"

	generatoriface "github.com/amiasea/packages/terraforge-cli/internal/generatorinterface"
	model "github.com/amiasea/packages/terraforge-cli/internal/model"
)

// Result bundles the outputs of the compiler build pipeline.
type Result struct {
	Schema      *schema.Schema
	Model       *genmodel.Model
	IR          *model.SchemaIR
	IRGraph     *irgraph.Graph
	ModuleGraph *modulegraph.Graph
}

// Build performs the canonical Terraforge compiler pipeline:
//
//  1. Load schema.json
//  2. Convert schema → generator model
//  3. Convert generator model → IR (SchemaIR)
//  4. Convert IR → IR dependency graph
//  5. Convert IR graph → module graph
//  6. Run all artifact generators (module, providersrc, diagram)
func Build(schemaPath string, outDir string) (*Result, error) {
	// 1. Load schema.json
	s, err := schema.Load(schemaPath)
	if err != nil {
		return nil, err
	}

	// 2. Convert schema → generator model
	gm, err := genmodel.FromSchema(s)
	if err != nil {
		return nil, err
	}

	// 3. Convert generator model → IR
	irModel, err := model.Build(gm)
	if err != nil {
		return nil, err
	}

	// 4. Convert IR → IR graph
	irg, err := irgraph.Build(irModel)
	if err != nil {
		return nil, err
	}

	// 5. Convert IR graph → module graph
	mg, err := modulegraph.Build(irg)
	if err != nil {
		return nil, err
	}

	// 6. Run all artifact generators
	generators := []generatoriface.Generator{
		module.New(),
		providersrc.New(),
		diagram.New(),
	}

	for _, gen := range generators {
		if err := gen.Generate(mg, outDir); err != nil {
			return nil, err
		}
	}

	return &Result{
		Schema:      s,
		Model:       gm,
		IR:          irModel,
		IRGraph:     irg,
		ModuleGraph: mg,
	}, nil
}
