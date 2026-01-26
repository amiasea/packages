package module

// Module represents a Terraform module definition.
type Module struct {
	Name       string
	Type       string
	Attributes map[string]any
	DependsOn  []string
}
