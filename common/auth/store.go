package auth

import (
    "encoding/json"
    "os"
    "path/filepath"
)

func CredentialsPath(provider string) (string, error) {
    dir, err := os.UserConfigDir()
    if err != nil {
        return "", err
    }
    return filepath.Join(dir, "amiasea", provider, "credentials.json"), nil
}

func Load(provider string) (*Credentials, error) {
    path, err := CredentialsPath(provider)
    if err != nil {
        return nil, err
    }

    data, err := os.ReadFile(path)
    if err != nil {
        return nil, err
    }

    var creds Credentials
    if err := json.Unmarshal(data, &creds); err != nil {
        return nil, err
    }

    return &creds, nil
}

func Save(provider string, creds *Credentials) error {
    path, err := CredentialsPath(provider)
    if err != nil {
        return err
    }

    if err := os.MkdirAll(filepath.Dir(path), 0o755); err != nil {
        return err
    }

    data, err := json.MarshalIndent(creds, "", "  ")
    if err != nil {
        return err
    }

    return os.WriteFile(path, data, 0o600)
}