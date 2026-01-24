package model

// Resource represents a resource definition parsed from schema.json
// before it is transformed into the semantic IR graph.
type Resource struct {
    Name       string
    Properties map[string]Property
}

// Property represents a field on a resource as defined in schema.json.
type Property struct {
    Name string
    Type string
}

// Model represents the entire parsed schema.json file.
// This is NOT the IR graph — it is the raw, structured schema model.
type Model struct {
    Resources map[string]*Resource
}