package cmd

import (
	"time"
    "encoding/json"
    "fmt"
    "io"
    "net/http"
    "net/url"

    "github.com/spf13/cobra"

	"github.com/amiasea/apperium-cli/internal/auth"
)

var loginCmd = &cobra.Command{
    Use:   "login",
    Short: "Authenticate with GitHub using device code flow",
    RunE: func(cmd *cobra.Command, args []string) error {
        fmt.Println("Starting device code login...")
        
		clientID := "2675978"

		// Step 2: Ask GitHub for a device code
		resp, err := http.PostForm(
			"https://github.com/login/device/code",
			url.Values{
				"client_id": {clientID},
				"scope":     {"repo read:org"},
			},
		)
		if err != nil {
			return fmt.Errorf("failed to request device code: %w", err)
		}
		defer resp.Body.Close()

		// Read the JSON response
		body, err := io.ReadAll(resp.Body)
		if err != nil {
			return fmt.Errorf("failed to read response: %w", err)
		}

		// Parse the JSON into a struct
		var deviceResp struct {
			DeviceCode      string `json:"device_code"`
			UserCode        string `json:"user_code"`
			VerificationURI string `json:"verification_uri"`
			ExpiresIn       int    `json:"expires_in"`
			Interval        int    `json:"interval"`
		}

		if err := json.Unmarshal(body, &deviceResp); err != nil {
			return fmt.Errorf("failed to parse JSON: %w", err)
		}

		// Print instructions for the user
		fmt.Println("To authenticate, visit:")
		fmt.Println("  ", deviceResp.VerificationURI)
		fmt.Println("and enter the code:")
		fmt.Println("  ", deviceResp.UserCode)

		for {
			resp, _ := http.PostForm("https://github.com/login/oauth/access_token", url.Values{
				"client_id":   {clientID},
				"device_code": {deviceResp.DeviceCode},
				"grant_type":  {"urn:ietf:params:oauth:grant-type:device_code"},
			})

			var tokenResp struct {
				AccessToken string `json:"access_token"`
				Error       string `json:"error"`
			}
			json.NewDecoder(resp.Body).Decode(&tokenResp)

			switch tokenResp.Error {
				case "authorization_pending":
					time.Sleep(time.Duration(deviceResp.Interval) * time.Second)
					continue
				case "slow_down":
					time.Sleep(time.Duration(deviceResp.Interval+5) * time.Second)
					continue
				case "expired_token":
					return fmt.Errorf("device code expired, please run login again")
			}

			// If we reach here, we have an access token
			fmt.Println("User completed authentication")
			fmt.Println("Access token:", tokenResp.AccessToken)
			break;
		}

		// Step 3: Discover the installation of the Apperium GitHub App

		installationID, err := auth.FetchInstallationID(tokenResp.AccessToken)
		if err != nil {
			return fmt.Errorf("failed to fetch installation: %w", err)
		}

		// 4.1: Start callback server (Step 5.2)
		fmt.Println("Starting local callback server...")
		codeCh := make(chan string)
		errCh := make(chan error)

		go func() {
			code, err := auth.StartCallbackServer()
			if err != nil {
				errCh <- err
				return
			}
			codeCh <- code
		}()

		// 4.2: Build OAuth URL
		oauthURL := auth.BuildOAuthURL()
		fmt.Println("Opening browser for authentication...")

		// 4.3: Open browser
		if err := openBrowser(oauthURL); err != nil {
			return fmt.Errorf("failed to open browser: %w", err)
		}

		// 4.4: Wait for callback
		var code string
		select {
		case code = <-codeCh:
			fmt.Println("Received OAuth code.")
		case err := <-errCh:
			return fmt.Errorf("callback error: %w", err)
		}

		// 5. Exchange code for user token
		userToken, err := auth.ExchangeCodeForToken(code)
		if err != nil {
			return fmt.Errorf("token exchange failed: %w", err)
		}

		// 6. Fetch installations
		installationID, err := auth.FetchInstallationID(userToken)
		if err != nil {
			return fmt.Errorf("failed to fetch installation: %w", err)
		}

		// 7. Exchange installation token
		installToken, err := auth.ExchangeInstallationToken(installationID)
		if err != nil {
			return fmt.Errorf("failed to get installation token: %w", err)
		}

		// 8. Store token
		if err := auth.StoreToken(installToken); err != nil {
			return fmt.Errorf("failed to store token: %w", err)
		}

		// 9. Success
		fmt.Println("You are now logged in.")
		return nil
    },
}

func init() {
    rootCmd.AddCommand(loginCmd)
}