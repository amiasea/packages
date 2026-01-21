package auth

import (
	"bytes"
	"encoding/json"
	"fmt"
	"net/http"
	"net/url"
	"os"
	"time"
)

const (
	clientID     = "YOUR_CLIENT_ID"
	clientSecret = "YOUR_CLIENT_SECRET" // optional for GitHub Apps
	redirectURI  = "http://127.0.0.1:49152/callback"
)

// BuildOAuthURL constructs the GitHub OAuth URL for the user to authenticate.
func BuildOAuthURL() string {
	u, _ := url.Parse("https://github.com/login/oauth/authorize")
	q := u.Query()
	q.Set("client_id", clientID)
	q.Set("redirect_uri", redirectURI)
	q.Set("scope", "read:user read:org")
	u.RawQuery = q.Encode()
	return u.String()
}

// ExchangeCodeForToken exchanges the OAuth "code" for a user access token.
func ExchangeCodeForToken(code string) (string, error) {
	data := url.Values{}
	data.Set("client_id", clientID)
	data.Set("client_secret", clientSecret)
	data.Set("code", code)
	data.Set("redirect_uri", redirectURI)

	req, err := http.NewRequest("POST",
		"https://github.com/login/oauth/access_token",
		bytes.NewBufferString(data.Encode()),
	)
	if err != nil {
		return "", err
	}

	req.Header.Set("Accept", "application/json")
	req.Header.Set("Content-Type", "application/x-www-form-urlencoded")

	client := &http.Client{Timeout: 10 * time.Second}
	resp, err := client.Do(req)
	if err != nil {
		return "", fmt.Errorf("token exchange failed: %w", err)
	}
	defer resp.Body.Close()

	var tokenResp struct {
		AccessToken string `json:"access_token"`
		TokenType   string `json:"token_type"`
		Scope       string `json:"scope"`
	}

	if err := json.NewDecoder(resp.Body).Decode(&tokenResp); err != nil {
		return "", fmt.Errorf("failed to decode token response: %w", err)
	}

	if tokenResp.AccessToken == "" {
		return "", fmt.Errorf("empty access token in response")
	}

	return tokenResp.AccessToken, nil
}

// FetchInstallationID retrieves the GitHub App installation ID for the authenticated user.
func FetchInstallationID(userToken string) (int64, error) {
	req, err := http.NewRequest(
		"GET",
		"https://api.github.com/user/installations",
		nil,
	)
	if err != nil {
		return 0, fmt.Errorf("failed to create installations request: %w", err)
	}

	req.Header.Set("Authorization", "Bearer "+userToken)
	req.Header.Set("Accept", "application/vnd.github+json")

	client := &http.Client{}
	resp, err := client.Do(req)
	if err != nil {
		return 0, fmt.Errorf("failed to fetch installations: %w", err)
	}
	defer resp.Body.Close()

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return 0, fmt.Errorf("failed to read installations response: %w", err)
	}

	var instResp struct {
		TotalCount   int `json:"total_count"`
		Installations []struct {
			ID      int64 `json:"id"`
			Account struct {
				Login string `json:"login"`
				Type  string `json:"type"`
			} `json:"account"`
		} `json:"installations"`
	}

	if err := json.Unmarshal(body, &instResp); err != nil {
		return 0, fmt.Errorf("failed to parse installations JSON: %w", err)
	}

	if instResp.TotalCount == 0 {
		return 0, fmt.Errorf("no installations of the Apperium GitHub App found for this user")
	}

	// If there's only one installation, use it automatically
	var installationID int64
	if instResp.TotalCount == 1 {
		installationID = instResp.Installations[0].ID
		fmt.Println("Found installation in org:", instResp.Installations[0].Account.Login)
	} else {
		// Multiple installations — prompt user to choose
		fmt.Println("Multiple installations found. Select an organization:")
		for i, inst := range instResp.Installations {
			fmt.Printf("[%d] %s (%s)\n", i+1, inst.Account.Login, inst.Account.Type)
		}

		var choice int
		fmt.Print("Enter number: ")
		fmt.Scan(&choice)

		if choice < 1 || choice > len(instResp.Installations) {
			return 0, fmt.Errorf("invalid selection")
		}

		installationID = instResp.Installations[choice-1].ID
	}

	fmt.Println("Selected installation ID:", installationID)
	return installationID, nil
}

// ExchangeInstallationToken exchanges an installation ID for an installation access token.
func ExchangeInstallationToken(installationID int64) (string, error) {
	url := fmt.Sprintf("https://api.github.com/app/installations/%d/access_tokens", installationID)

	req, _ := http.NewRequest("POST", url, nil)
	req.Header.Set("Accept", "application/vnd.github+json")

	// GitHub App authentication requires a JWT here.
	jwt, err := GenerateAppJWT()
	if err != nil {
		return "", fmt.Errorf("failed to generate app JWT: %w", err)
	}

	req.Header.Set("Authorization", "Bearer "+jwt)

	client := &http.Client{Timeout: 10 * time.Second}
	resp, err := client.Do(req)
	if err != nil {
		return "", fmt.Errorf("failed to exchange installation token: %w", err)
	}
	defer resp.Body.Close()

	var tokenResp struct {
		Token string `json:"token"`
	}

	if err := json.NewDecoder(resp.Body).Decode(&tokenResp); err != nil {
		return "", fmt.Errorf("failed to decode installation token: %w", err)
	}

	if tokenResp.Token == "" {
		return "", fmt.Errorf("empty installation token in response")
	}

	return tokenResp.Token, nil
}

// StoreToken saves the installation token locally.
// You can replace this with DPAPI, Keychain, etc.
func StoreToken(token string) error {
	return os.WriteFile("token.txt", []byte(token), 0600)
}

// StartCallbackServer starts a temporary HTTP server that waits for the OAuth callback.
// It returns the "code" parameter from GitHub's redirect.
func StartCallbackServer() (string, error) {
	codeCh := make(chan string)
	errCh := make(chan error)

	mux := http.NewServeMux()

	mux.HandleFunc("/callback", func(w http.ResponseWriter, r *http.Request) {
		query := r.URL.Query()
		code := query.Get("code")

		if code == "" {
			http.Error(w, "Missing code parameter", http.StatusBadRequest)
			errCh <- fmt.Errorf("missing code parameter in callback")
			return
		}

		// Respond to the browser
		fmt.Fprintf(w, `
			<html>
				<body>
					<h2>Authentication complete</h2>
					<p>You may now close this window.</p>
				</body>
			</html>
		`)

		// Send the code back to the login flow
		codeCh <- code
	})

	server := &http.Server{
		Addr:    "127.0.0.1:49152",
		Handler: mux,
	}

	// Start server in background
	go func() {
		if err := server.ListenAndServe(); err != nil && err != http.ErrServerClosed {
			errCh <- err
		}
	}()

	// Wait for either the code or an error
	select {
	case code := <-codeCh:
		// Shutdown server gracefully
		ctx, cancel := context.WithTimeout(context.Background(), 2*time.Second)
		defer cancel()
		_ = server.Shutdown(ctx)
		return code, nil

	case err := <-errCh:
		// Shutdown server on error
		ctx, cancel := context.WithTimeout(context.Background(), 2*time.Second)
		defer cancel()
		_ = server.Shutdown(ctx)
		return "", err
	}
}