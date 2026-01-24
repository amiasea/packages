package cmd

import "github.com/amiasea/packages/terraforge-cli/internal/generate"

func NewBuildCmd() *cobra.Command {
    var schemaPath string

    cmd := &cobra.Command{
        Use: "build",
        RunE: func(cmd *cobra.Command, args []string) error {
            s, err := schema.Load(schemaPath)
            if err != nil {
                return err
            }
            return generate.Run(s)
        },
    }

    cmd.Flags().StringVarP(&schemaPath, "schema", "s", "", "Path to schema.json")
    cmd.MarkFlagRequired("schema")

    return cmd
}