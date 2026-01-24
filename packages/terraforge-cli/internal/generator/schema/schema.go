package schema

import (
    "encoding/json"
    "fmt"
    "os"
)

type Schema struct {
    Provider  ProviderDefinition  `json:"provider"`
    Resources []ResourceDefinition `json:"resources"`
}

type ProviderDefinition struct {
    Name    string `json:"name"`
    Version string `json:"version"`
}

type ResourceDefinition struct {
    Name   string            `json:"name"`
    Type   string            `json:"type"`
    Fields []FieldDefinition `json:"fields"`
}

type FieldDefinition struct {
    Name     string `json:"name"`
    Type     string `json:"type"`
    Required bool   `json:"required"`
}

func Load(path string) (*Schema, error) {
    data, err := os.ReadFile(path)
    if err != nil {
        return nil, fmt.Errorf("failed to read schema file: %w", err)
    }

    var s Schema
    if err := json.Unmarshal(data, &s); err != nil {
        return nil, fmt.Errorf("failed to parse schema: %w", err)
    }

    return &s, nil
}