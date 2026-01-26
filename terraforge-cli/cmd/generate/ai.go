package generate

import (
	"context"
	"fmt"
	"os"
	"path/filepath"

	"github.com/spf13/cobra"

	"github.com/amiasea/packages/terraforge-cli/internal/ai"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen"
	"github.com/amiasea/packages/terraforge-cli/internal/codegen/schema"
)

func NewAICmd() *cobra.Command {
	var schemaPath string
	var outDir string
	var endpoint string
	var apiKey string
	var model string

	cmd := &cobra.Command{
		Use:   "ai",
		Short: "Generate provider IR using the Azure AI compiler frontend",
		RunE: func(cmd *cobra.Command, args []string) error {
			if outDir == "" {
				cwd, err := os.Getwd()
				if err != nil {
					return fmt.Errorf("failed to get working directory: %w", err)
				}
				outDir = filepath.Join(cwd, ".terraforge", "ai")
			}

			// 1. Load schema.json
			s, err := schema.Load(schemaPath)
			if err != nil {
				return err
			}

			// 2. Build prompt
			prompt, err := ai.BuildPrompt(s)
			if err != nil {
				return err
			}

			// 3. Call Azure AI
			client := ai.NewClient(endpoint, apiKey, model)
			raw, err := client.GenerateIR(context.Background(), prompt)
			if err != nil {
				return err
			}

			// 4. Parse AI → IR
			ir, err := ai.ParseIR(raw)
			if err != nil {
				return err
			}

			// 5. Run backend codegen
			if err := codegen.Generate(ir, outDir); err != nil {
				return err
			}

			fmt.Println("AI-generated provider IR written to:", outDir)
			return nil
		},
	}

	cmd.Flags().StringVar(&schemaPath, "schema", "schema.json", "Path to schema.json")
	cmd.Flags().StringVar(&outDir, "out", "", "Output directory for generated IR")
	cmd.Flags().StringVar(&endpoint, "endpoint", "", "Azure AI endpoint URL")
	cmd.Flags().StringVar(&apiKey, "key", "", "Azure AI API key")
	cmd.Flags().StringVar(&model, "model", "gpt-4o-mini", "Model name")

	cmd.MarkFlagRequired("schema")
	cmd.MarkFlagRequired("endpoint")
	cmd.MarkFlagRequired("key")

	return cmd
}
