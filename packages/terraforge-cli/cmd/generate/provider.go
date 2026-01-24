package generate

import (
    "fmt"
    "os"
    "path/filepath"

    "github.com/spf13/cobra"

    "github.com/amiasea/packages/terraforge-cli/internal/generator/model"
    "github.com/amiasea/packages/terraforge-cli/internal/generator/provider"
    "github.com/amiasea/packages/terraforge-cli/internal/generator/schema"
)

func NewProviderCmd() *cobra.Command {
    var schemaPath string
    var outDir string

    cmd := &cobra.Command{
        Use:   "provider",
        Short: "Generate Terraforge provider source code",
        RunE: func(cmd *cobra.Command, args []string) error {
            if outDir == "" {
                cwd, err := os.Getwd()
                if err != nil {
                    return fmt.Errorf("failed to get working directory: %w", err)
                }
                outDir = filepath.Join(cwd, ".terraforge", "provider")
            }

            s, err := schema.Load(schemaPath)
            if err != nil {
                return err
            }

            g, err := graph.Build(s)
            if err != nil {
                return err
            }

            cfg := provider.Config{
                OutputDir: outDir,
                Package:   "main",
            }

            if err := provider.Generate(g, cfg); err != nil {
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