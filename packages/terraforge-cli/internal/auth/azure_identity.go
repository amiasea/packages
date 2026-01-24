package auth

import (
    "context"
    "encoding/json"
    "errors"
    "fmt"
    "net/http"
    "net/url"
    "time"
	"strings"

    common "github.com/amiasea/packages/common/auth"
)

const (
    azureDeviceCodeURL = "https://login.microsoftonline.com/common/oauth2/v2.0/devicecode"
    azureTokenURL      = "https://login.microsoftonline.com/common/oauth2/v2.0/token"
)

type AzureProvider struct {
    ClientID string
    Scopes   []string
}

type deviceCodeResponse struct {
    UserCode        string `json:"user_code"`
    DeviceCode      string `json:"device_code"`
    VerificationURI string `json:"verification_uri"`
    Interval        int    `json:"interval"`
    ExpiresIn       int    `json:"expires_in"`
    Message         string `json:"message"`
}

type tokenResponse struct {
    AccessToken  string `json:"access_token"`
    RefreshToken string `json:"refresh_token"`
    ExpiresIn    int    `json:"expires_in"`
    Error        string `json:"error"`
}

// Login implements Azure Device Code Flow
func (p *AzureProvider) Login(ctx context.Context) (*common.Credentials, error) {
    dc, err := p.startDeviceFlow(ctx)
    if err != nil {
        return nil, err
    }

    fmt.Println(dc.Message)

    token, err := p.pollForToken(ctx, dc)
    if err != nil {
        return nil, err
    }

    creds := &common.Credentials{
        AccessToken:  token.AccessToken,
        RefreshToken: token.RefreshToken,
        ExpiresAt:    time.Now().Add(time.Duration(token.ExpiresIn) * time.Second),
        Provider:     "azure",
    }

    return creds, nil
}

func (p *AzureProvider) Refresh(ctx context.Context, creds *common.Credentials) (*common.Credentials, error) {
    data := url.Values{}
    data.Set("client_id", p.ClientID)
    data.Set("grant_type", "refresh_token")
    data.Set("refresh_token", creds.RefreshToken)
    data.Set("scope", p.scopeString())

    req, _ := http.NewRequestWithContext(ctx, "POST", azureTokenURL, 
        strings.NewReader(data.Encode()))
    req.Header.Set("Content-Type", "application/x-www-form-urlencoded")

    resp, err := http.DefaultClient.Do(req)
    if err != nil {
        return nil, err
    }
    defer resp.Body.Close()

    var tr tokenResponse
    if err := json.NewDecoder(resp.Body).Decode(&tr); err != nil {
        return nil, err
    }

    if tr.Error != "" {
        return nil, common.ErrRefreshRequired
    }

    return &common.Credentials{
        AccessToken:  tr.AccessToken,
        RefreshToken: tr.RefreshToken,
        ExpiresAt:    time.Now().Add(time.Duration(tr.ExpiresIn) * time.Second),
        Provider:     "azure",
    }, nil
}

func (p *AzureProvider) Validate(ctx context.Context, creds *common.Credentials) error {
    req, _ := http.NewRequestWithContext(ctx, "GET",
        "https://management.azure.com/subscriptions?api-version=2020-01-01", nil)
    req.Header.Set("Authorization", "Bearer "+creds.AccessToken)

    resp, err := http.DefaultClient.Do(req)
    if err != nil {
        return err
    }
    defer resp.Body.Close()

    if resp.StatusCode == 401 {
        return common.ErrExpired
    }

    if resp.StatusCode >= 400 {
        return fmt.Errorf("azure validation failed: %s", resp.Status)
    }

    return nil
}

// --- internal helpers ---

func (p *AzureProvider) startDeviceFlow(ctx context.Context) (*deviceCodeResponse, error) {
    data := url.Values{}
    data.Set("client_id", p.ClientID)
    data.Set("scope", p.scopeString())

    req, _ := http.NewRequestWithContext(ctx, "POST", azureDeviceCodeURL,
        strings.NewReader(data.Encode()))
    req.Header.Set("Content-Type", "application/x-www-form-urlencoded")

    resp, err := http.DefaultClient.Do(req)
    if err != nil {
        return nil, err
    }
    defer resp.Body.Close()

    var dc deviceCodeResponse
    if err := json.NewDecoder(resp.Body).Decode(&dc); err != nil {
        return nil, err
    }

    return &dc, nil
}

func (p *AzureProvider) pollForToken(ctx context.Context, dc *deviceCodeResponse) (*tokenResponse, error) {
    interval := time.Duration(dc.Interval) * time.Second

    for {
        select {
        case <-ctx.Done():
            return nil, ctx.Err()
        case <-time.After(interval):
            token, err := p.requestToken(ctx, dc.DeviceCode)
            if err != nil {
                if errors.Is(err, errAuthorizationPending) {
                    continue
                }
                return nil, err
            }
            return token, nil
        }
    }
}

var errAuthorizationPending = errors.New("authorization_pending")

func (p *AzureProvider) requestToken(ctx context.Context, deviceCode string) (*tokenResponse, error) {
    data := url.Values{}
    data.Set("client_id", p.ClientID)
    data.Set("grant_type", "urn:ietf:params:oauth:grant-type:device_code")
    data.Set("device_code", deviceCode)

    req, _ := http.NewRequestWithContext(ctx, "POST", azureTokenURL,
        strings.NewReader(data.Encode()))
    req.Header.Set("Content-Type", "application/x-www-form-urlencoded")

    resp, err := http.DefaultClient.Do(req)
    if err != nil {
        return nil, err
    }
    defer resp.Body.Close()

    var tr tokenResponse
    if err := json.NewDecoder(resp.Body).Decode(&tr); err != nil {
        return nil, err
    }

    if tr.Error == "authorization_pending" {
        return nil, errAuthorizationPending
    }

    if tr.Error != "" {
        return nil, fmt.Errorf("azure token error: %s", tr.Error)
    }

    return &tr, nil
}

func (p *AzureProvider) scopeString() string {
    if len(p.Scopes) == 0 {
        return "https://management.azure.com/.default offline_access"
    }
    return strings.Join(p.Scopes, " ")
}