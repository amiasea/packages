package auth

import "time"

type Credentials struct {
    AccessToken  string    `json:"access_token"`
    RefreshToken string    `json:"refresh_token,omitempty"`
    ExpiresAt    time.Time `json:"expires_at"`
    Provider     string    `json:"provider"` // "github", "azure", etc.
}