package generate

import (
	"fmt"

	"github.com/spf13/cobra"

	genmodel "github.com/amiasea/packages/terraforge-cli/internal/generator/model"
	schema "github.com/amiasea/packages/terraforge-cli/internal/generator/schema"
)

func NewModelCmd() *cobra.Command {
	var schemaPath string

	cmd := &cobra.Command{
		Use:   "model",
		Short: "Load and inspect the Terraforge generator model (typed schema)",
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

			// 3. Print generator model contents
			fmt.Printf("Resources: %d\n", len(gm.Resources))
			for name, r := range gm.Resources {
				fmt.Printf("- %s (type: %s)\n", name, r.Type)
				for attrName := range r.Attributes {
					fmt.Printf("    %s\n", attrName)
				}
			}

			return nil
		},
	}

	cmd.Flags().StringVarP(&schemaPath, "schema", "s", "", "Path to the schema.json file")
	cmd.MarkFlagRequired("schema")

	return cmd
}
