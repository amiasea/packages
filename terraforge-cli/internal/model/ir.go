package model

// SchemaIR is the root IR structure produced from schema.json.
// It contains all resources in canonical, normalized form.
type SchemaIR struct {
	Resources map[string]*Resource `json:"resources"`
}

// Resource represents a single Terraform resource in the IR.
// It contains typed attributes and any explicit dependency hints.
type Resource struct {
	Name       string           `json:"name"`
	Type       string           `json:"type"`
	Attributes map[string]Value `json:"attributes"`
	DependsOn  []string         `json:"depends_on,omitempty"`
}

// Value represents an attribute value in the IR.
// It may be a literal, a reference, or a composite containing both.
type Value struct {
	Literal   any        `json:"literal,omitempty"`
	Reference *Reference `json:"reference,omitempty"`
}

// Reference represents a dependency on another resource's attribute.
// This is what the graph builder consumes to infer edges.
type Reference struct {
	Resource string `json:"resource"`
	Field    string `json:"field"`
}
