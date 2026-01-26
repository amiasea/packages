package ir

// IR is the canonical intermediate representation produced by the AI frontend.
// It contains a flat list of resources with attributes and dependency metadata.
type IR struct {
	Resources []*Resource `json:"resources"`
}

// Resource represents a single resource in the IR.
type Resource struct {
	Name       string                `json:"name"`
	Type       string                `json:"type"`
	Attributes map[string]*Attribute `json:"attributes"`
	DependsOn  []string              `json:"depends_on"`
}

// Attribute represents a resource attribute, which may be a literal or a reference.
type Attribute struct {
	Literal   any        `json:"literal,omitempty"`
	Reference *Reference `json:"reference,omitempty"`
}

// Reference points to another resource's attribute.
type Reference struct {
	Resource string `json:"resource"`
	Field    string `json:"field"`
}
