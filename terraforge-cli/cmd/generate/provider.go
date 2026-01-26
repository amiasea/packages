package generate

import (
	"fmt"
	"os"
	"path/filepath"

	"github.com/spf13/cobra"

	"github.com/amiasea/packages/terraforge-cli/internal/codegen/irbuild"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/irgraph"
	genmodel "github.com/amiasea/packages/terraforge-cli/internal/codegen/model"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/modulegraph"
	providersrc "github.com/amiasea/packages/terraforge-cli/internal/codegen/providersrc"
	schema "github.com/amiasea/packages/terraforge-cli/internal/codegen/schema"
	"github.com/amiasea/packages/terraforge-cli/internal/filesystem"
)

func NewProviderCmd() *cobra.Command {
	var schemaPath string
	var outDir string

	cmd := &cobra.Command{
		Use:   "provider",
		Short: "Generate Terraforge provider source code",
		RunE: func(cmd *cobra.Command, args []string) error {
			fs := filesystem.New()

			if outDir == "" {
				cwd, err := os.Getwd()
				if err != nil {
					return fmt.Errorf("failed to get working directory: %w", err)
				}
				outDir = filepath.Join(cwd, ".terraforge", "provider")
			}

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

			// 6. Generate provider source code
			cfg := providersrc.Config{
				OutputDir: outDir,
				Package:   "main",
			}

			if err := providersrc.GenerateFromGraph(fs, mg, cfg); err != nil {
				return err
			}

			fmt.Println("Provider source generated at:", outDir)
			return nil
		},
	}

	cmd.Flags().StringVarP(&schemaPath, "schema", "s", "", "Path to the schema.json file")
	cmd.MarkFlagRequired("schema")
	cmd.Flags().StringVar(&outDir, "out", "", "Output directory for provider source (default: .terraforge/provider)")

	return cmd
}
