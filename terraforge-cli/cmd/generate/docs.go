package generate

import (
	"github.com/spf13/cobra"
)

func NewDocsCmd() *cobra.Command {
	cmd := &cobra.Command{
		Use:   "docs",
		Short: "Generate documentation from the Terraforge model",
		RunE: func(cmd *cobra.Command, args []string) error {
			// 1. Load schema
			// 2. Convert to generator model
			// 3. Convert to IR
			// 4. Generate docs (Markdown, HTML, etc.)
			return nil
		},
	}

	return cmd
}
