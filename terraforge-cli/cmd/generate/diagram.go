package generate

import (
	"path/filepath"

	"github.com/spf13/cobra"

	"github.com/amiasea/packages/terraforge-cli/internal/codegen/diagram"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/irbuild"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/irgraph"
	genmodel "github.com/amiasea/packages/terraforge-cli/internal/codegen/model"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/modulegraph"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/schema"
)

func NewDiagramCmd() *cobra.Command {
	var outDir string
	var schemaPath string

	cmd := &cobra.Command{
		Use:   "diagram",
		Short: "Generate a DOT diagram of the resource dependency graph",
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

			// 6. Generate diagram
			cfg := diagram.Config{
				OutputDir: filepath.Join(outDir, "diagram"),
				Filename:  "diagram.dot",
			}

			return diagram.Generate(mg, cfg)
		},
	}

	cmd.Flags().StringVarP(&outDir, "out", "o", "out", "Output directory")
	cmd.Flags().StringVarP(&schemaPath, "schema", "s", "", "Path to schema.json")
	cmd.MarkFlagRequired("schema")

	return cmd
}
