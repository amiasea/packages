package model

// Graph represents the semantic resource graph derived from the schema.
// This is the compiler's IR: a stable structure consumed by all generators.
type Graph struct {
	Nodes map[string]*Node
	Edges []Edge
}

// Node represents a resource in the IR graph.
type Node struct {
	Name       string
	Properties map[string]Property
}

// Property represents a typed field on a resource in the IR.
type Property struct {
	Name string
	Type string
}

// Edge represents a dependency between two resources.
// Example: A → B means A depends on B.
type Edge struct {
	From string
	To   string
}
