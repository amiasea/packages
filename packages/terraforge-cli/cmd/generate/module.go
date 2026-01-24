package generate

import (
    "github.com/spf13/cobra"

    genmodel "github.com/amiasea/packages/terraforge-cli/internal/generator/model"
    schema "github.com/amiasea/packages/terraforge-cli/internal/generator/schema"
    ir "github.com/amiasea/packages/terraforge-cli/internal/model"
    module "github.com/amiasea/packages/terraforge-cli/internal/generator/module"
)

func NewModuleCmd() *cobra.Command {
    var schemaPath string
    var outDir string

    cmd := &cobra.Command{
        Use:   "module",
        Short: "Generate Terraform modules from the resource model",
        RunE: func(cmd *cobra.Command, args []string) error {
            // 1. Load schema.json → generator model
            gm, err := schema.Load(schemaPath)
            if err != nil {
                return err
            }

            // 2. Convert generator model → IR
            irModel, err := ir.Build(gm)
            if err != nil {
                return err
            }

            // 3. Convert IR → module definitions
            mods, err := module.Build(irModel)
            if err != nil {
                return err
            }

            // 4. Write module files to disk
            cfg := module.Config{OutputDir: outDir}
            return module.Generate(mods, cfg)
        },
    }

    cmd.Flags().StringVar(&schemaPath, "schema", "schema.json", "Path to schema.json")
    cmd.Flags().StringVar(&outDir, "out", ".terraforge/modules", "Output directory for modules")

    return cmd
}