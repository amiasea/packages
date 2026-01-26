package generate

import (
	"github.com/spf13/cobra"
)

func NewGenerateCmd() *cobra.Command {
	cmd := &cobra.Command{
		Use:   "generate",
		Short: "Generate Terraforge artifacts",
		Long:  "Generate Terraforge models, diagrams, provider source code, and other compiler artifacts.",
		Run: func(cmd *cobra.Command, args []string) {
			// If someone runs `terraforge generate` with no subcommand:
			_ = cmd.Help()
		},
	}

	// Subcommands
	cmd.AddCommand(NewModelCmd())
	cmd.AddCommand(NewDiagramCmd())
	cmd.AddCommand(NewProviderCmd())
	cmd.AddCommand(NewModuleCmd())

	// Future (not yet fully implemented):
	cmd.AddCommand(NewDocsCmd())

	return cmd
}
