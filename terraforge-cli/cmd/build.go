package cmd

import (
	"fmt"

	"github.com/spf13/cobra"

	"github.com/amiasea/packages/terraforge-cli/internal/generator/irgraph"
	genmodel "github.com/amiasea/packages/terraforge-cli/internal/generator/model"
	schema "github.com/amiasea/packages/terraforge-cli/internal/generator/schema"
	model "github.com/amiasea/packages/terraforge-cli/internal/model"
)

func NewBuildCmd() *cobra.Command {
	var schemaPath string

	cmd := &cobra.Command{
		Use:   "build",
		Short: "Validate schema and build the Terraforge IR graph",
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
			irSchema, err := model.Build(gm)
			if err != nil {
				return err
			}

			// 4. Convert IR → IR graph
			g, err := irgraph.Build(irSchema)
			if err != nil {
				return err
			}

			fmt.Printf("Build successful. Resources: %d, Edges: %d\n", len(g.Nodes), len(g.Edges))
			return nil
		},
	}

	cmd.Flags().StringVarP(&schemaPath, "schema", "s", "", "Path to schema.json")
	cmd.MarkFlagRequired("schema")

	return cmd
}
