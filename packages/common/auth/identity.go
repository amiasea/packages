package auth

import "context"

type IdentityProvider interface {
    Login(ctx context.Context) (*Credentials, error)
    Refresh(ctx context.Context, creds *Credentials) (*Credentials, error)
    Validate(ctx context.Context, creds *Credentials) error
}