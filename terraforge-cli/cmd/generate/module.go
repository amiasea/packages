package generate

import (
	"github.com/spf13/cobra"

	"github.com/amiasea/packages/terraforge-cli/internal/generator/irgraph"
	genmodel "github.com/amiasea/packages/terraforge-cli/internal/generator/model"
	module "github.com/amiasea/packages/terraforge-cli/internal/generator/module"
	"github.com/amiasea/packages/terraforge-cli/internal/generator/modulegraph"
	schema "github.com/amiasea/packages/terraforge-cli/internal/generator/schema"
	model "github.com/amiasea/packages/terraforge-cli/internal/model"
)

func NewModuleCmd() *cobra.Command {
	var schemaPath string
	var outDir string

	cmd := &cobra.Command{
		Use:   "module",
		Short: "Generate Terraform modules from the resource model",
		RunE: func(cmd *cobra.Command, args []string) error {

			// 1. Load schema.json
			s, err := schema.Load(schemaPath)
			if err != nil {
				return err
			}

			// 2. Convert schema → generator model
			gm, err := genmodel.FromSchema(s)
			if err != nil {
				return err
			}

			// 3. Convert generator model → IR
			irModel, err := model.Build(gm)
			if err != nil {
				return err
			}

			// 4. Convert IR → IR graph
			irg, err := irgraph.Build(irModel)
			if err != nil {
				return err
			}

			// 5. Convert IR graph → module graph
			mg, err := modulegraph.Build(irg)
			if err != nil {
				return err
			}

			// 6. Convert module graph → module definitions
			mods, err := module.FromGraph(mg)
			if err != nil {
				return err
			}

			// 7. Write module files to disk
			cfg := module.Config{OutputDir: outDir}
			return module.Generate(mods, cfg)
		},
	}

	cmd.Flags().StringVar(&schemaPath, "schema", "schema.json", "Path to schema.json")
	cmd.Flags().StringVar(&outDir, "out", ".terraforge/modules", "Output directory for modules")

	return cmd
}
