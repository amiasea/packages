package auth

import "errors"

var (
    ErrNotLoggedIn     = errors.New("not logged in")
    ErrExpired         = errors.New("credentials expired")
    ErrInvalidToken    = errors.New("invalid token")
    ErrRefreshRequired = errors.New("refresh required")
)