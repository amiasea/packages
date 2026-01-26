package generate

import (
	"github.com/spf13/cobra"

	"github.com/amiasea/packages/terraforge-cli/internal/codegen/irbuild"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/irgraph"
	genmodel "github.com/amiasea/packages/terraforge-cli/internal/codegen/model"
	module "github.com/amiasea/packages/terraforge-cli/internal/codegen/module"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/modulegraph"
	schema "github.com/amiasea/packages/terraforge-cli/internal/codegen/schema"
	"github.com/amiasea/packages/terraforge-cli/internal/filesystem"
)

func NewModuleCmd() *cobra.Command {
	var schemaPath string
	var outDir string

	cmd := &cobra.Command{
		Use:   "module",
		Short: "Generate Terraform modules from the resource model",
		RunE: func(cmd *cobra.Command, args []string) error {
			fs := filesystem.New()

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

			// 3. Convert generator model → canonical IR
			irModel, err := irbuild.Build(gm)
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

			// 6. Generate Terraform modules directly from the module graph
			cfg := module.Config{OutputDir: outDir}
			return module.GenerateFromGraph(fs, mg, cfg)
		},
	}

	cmd.Flags().StringVar(&schemaPath, "schema", "schema.json", "Path to schema.json")
	cmd.Flags().StringVar(&outDir, "out", ".terraforge/modules", "Output directory for modules")

	return cmd
}
