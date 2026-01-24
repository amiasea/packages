package cmd

import (
    "context"
    "fmt"

    "github.com/spf13/cobra"

    common "github.com/amiasea/packages/common/auth"
    tfauth "github.com/amiasea/packages/terraforge-cli/internal/auth"
)

func NewLoginCmd() *cobra.Command {
    return &cobra.Command{
        Use:   "login",
        Short: "Authenticate with Azure to use Terraforge",
        RunE: func(cmd *cobra.Command, args []string) error {
            ctx := context.Background()

            provider := &tfauth.AzureProvider{
                ClientID: "d85cce37-e145-44c1-bec7-d66c33d2e406",
                Scopes:   []string{"https://management.azure.com/.default offline_access"},
            }

            // Try loading existing credentials
            creds, err := common.Load("azure")
            if err == nil {
                // Validate existing credentials
                if err := provider.Validate(ctx, creds); err == nil {
                    fmt.Println("Already logged in to Azure.")
                    return nil
                }

                // Try refreshing
                refreshed, err := provider.Refresh(ctx, creds)
                if err == nil {
                    if err := common.Save("azure", refreshed); err != nil {
                        return fmt.Errorf("failed to save refreshed credentials: %w", err)
                    }
                    fmt.Println("Token refreshed successfully.")
                    return nil
                }

                fmt.Println("Existing credentials expired. Re-authenticating…")
            }

            // Perform full login
            newCreds, err := provider.Login(ctx)
            if err != nil {
                return fmt.Errorf("login failed: %w", err)
            }

            if err := common.Save("azure", newCreds); err != nil {
                return fmt.Errorf("failed to save credentials: %w", err)
            }

            fmt.Println("Successfully logged in to Azure.")
            return nil
        },
    }
}