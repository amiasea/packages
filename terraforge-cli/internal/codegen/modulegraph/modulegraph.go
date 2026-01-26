package modulegraph

// Graph is the simplified, generation-ready graph used by the module generator.
// It contains only the information needed to produce Terraform modules.
type Graph struct {
	Nodes map[string]*Node
}

// Node represents a single module-ready resource.
type Node struct {
	Name       string
	Type       string
	Attributes map[string]Value
	DependsOn  []string
}

// Value represents either a literal or a reference to another resource.
type Value struct {
	Literal   any
	Reference *Reference
}

// Reference points to another resource's attribute.
type Reference struct {
	Resource string
	Field    string
}

// New creates an empty module graph.
func New() *Graph {
	return &Graph{
		Nodes: make(map[string]*Node),
	}
}

// AddNode inserts a new module node into the graph.
func (g *Graph) AddNode(name, typ string) *Node {
	n := &Node{
		Name:       name,
		Type:       typ,
		Attributes: make(map[string]Value),
		DependsOn:  []string{},
	}
	g.Nodes[name] = n
	return n
}
