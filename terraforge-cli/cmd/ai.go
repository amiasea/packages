package cmd

import (
	"context"
	"fmt"
	"os"
	"path/filepath"
	"strings"

	"github.com/spf13/cobra"

	"github.com/amiasea/packages/terraforge-cli/internal/ai"
	"github.com/amiasea/packages/terraforge-cli/internal/filesystem"
)

func NewAICmd() *cobra.Command {
	var schemaPath string
	var promptPath string
	var outDir string
	var endpoint string
	var apiKey string
	var model string

	cmd := &cobra.Command{
		Use:   "ai",
		Short: "Generate Terraform provider using Azure AI",
		RunE: func(cmd *cobra.Command, args []string) error {
			// fs := filesystem.New()

			// 1. Read schema.json
			schemaBytes, err := os.ReadFile(schemaPath)
			if err != nil {
				return fmt.Errorf("failed to read schema: %w", err)
			}

			// 1b. Read prompt.txt using filesystem.go

			promptBytes, err := os.ReadFile(promptPath)
			if err != nil {
				return fmt.Errorf("failed to read prompt file: %w", err)
			}

			// 2. Infer module path + package name from folder name
			cwd, err := os.Getwd()
			if err != nil {
				return fmt.Errorf("failed to get working directory: %w", err)
			}

			folder := filepath.Base(cwd)

			// module path: github.com/amiasea/<folder>
			modulePath := "github.com/amiasea/" + folder

			// package name: replace '-' with '_'
			packageName := strings.ReplaceAll(folder, "-", "_")

			// 3. Build the AI prompt (now using prompt.txt)
			prompt, err := ai.BuildPrompt(
				string(promptBytes), // NEW: raw prompt template
				string(schemaBytes),
				modulePath,
				packageName,
				"registry.terraform.io/amiasea/"+folder,
			)
			if err != nil {
				return fmt.Errorf("failed to build prompt")
			}
			// if err != nil {
			// 	return fmt.Errorf("failed to build prompt: %w", err)
			// }

			// 4. Call Azure AI
			client := ai.NewClient(endpoint, apiKey, model)
			output, err := client.GenerateProvider(context.Background(), prompt)
			if err != nil {
				return fmt.Errorf("AI generation failed: %w", err)
			}

			// 5. Write provider.go + main.go
			if err := filesystem.WriteGeneratedFiles(outDir, output); err != nil {
				return fmt.Errorf("failed to write generated files: %w", err)
			}

			fmt.Println("Provider generated successfully.")
			return nil
		},
	}

	cmd.Flags().StringVar(&schemaPath, "schema", "schema.json", "Path to schema.json")
	cmd.Flags().StringVar(&promptPath, "prompt", "prompt.txt", "Path to prompt.txt") // NEW FLAG
	cmd.Flags().StringVar(&outDir, "out", ".", "Output directory")
	cmd.Flags().StringVar(&endpoint, "endpoint", "", "Azure AI endpoint")
	cmd.Flags().StringVar(&apiKey, "key", "", "Azure API key")
	cmd.Flags().StringVar(&model, "model", "gpt-4o-mini", "Model name")

	return cmd
}
