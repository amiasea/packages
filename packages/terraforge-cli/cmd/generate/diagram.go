package generate

import (
    "fmt"
    "os"
    "path/filepath"

    "github.com/spf13/cobra"

    "github.com/amiasea/packages/terraforge-cli/internal/generator/model"
    "github.com/amiasea/packages/terraforge-cli/internal/generator/diagram"
    "github.com/amiasea/packages/terraforge-cli/internal/generator/schema"
)

func NewDiagramCmd() *cobra.Command {
    var schemaPath string
    var outDir string

    cmd := &cobra.Command{
        Use:   "diagram",
        Short: "Generate a Terraforge diagram (dependency graph visualization)",
        RunE: func(cmd *cobra.Command, args []string) error {
            if outDir == "" {
                cwd, err := os.Getwd()
                if err != nil {
                    return fmt.Errorf("failed to get working directory: %w", err)
                }
                outDir = filepath.Join(cwd, ".terraforge", "diagram")
            }

            s, err := schema.Load(schemaPath)
            if err != nil {
                return err
            }

            g, err := model.Build(s)
            if err != nil {
                return err
            }

            cfg := diagram.Config{
                OutputDir: outDir,
                Filename:  "diagram.dot",
            }

            if err := diagram.Generate(g, cfg); err != nil {
                return err
            }

            fmt.Println("Diagram generated at:", filepath.Join(outDir, "diagram.dot"))
            fmt.Printf("Nodes: %d, Edges: %d\n", len(g.Nodes), len(g.Edges))

            return nil
        },
    }

    cmd.Flags().StringVarP(&schemaPath, "schema", "s", "", "Path to schema.json")
    cmd.MarkFlagRequired("schema")
    cmd.Flags().StringVar(&outDir, "out", "", "Output directory for diagram artifacts")
    return cmd
}