package generate

import (
    "fmt"

    "github.com/spf13/cobra"

    "github.com/amiasea/packages/terraforge-cli/internal/generator/model"
    "github.com/amiasea/packages/terraforge-cli/internal/generator/schema"
)

func NewModelCmd() *cobra.Command {
    var schemaPath string

    cmd := &cobra.Command{
        Use:   "model",
        Short: "Build and inspect the Terraforge resource model",
        RunE: func(cmd *cobra.Command, args []string) error {
            s, err := schema.Load(schemaPath)
            if err != nil {
                return err
            }

            g, err := model.Build(s)
            if err != nil {
                return err
            }

            fmt.Printf("Nodes: %d\n", len(g.Nodes))
            fmt.Printf("Edges: %d\n", len(g.Edges))
            for _, e := range g.Edges {
                fmt.Printf("%s -> %s\n", e.From, e.To)
            }

            return nil
        },
    }

    cmd.Flags().StringVarP(&schemaPath, "schema", "s", "", "Path to the schema.json file")
    cmd.MarkFlagRequired("schema")

    return cmd
}