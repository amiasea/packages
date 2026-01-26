package irgraph

import (
	"fmt"

	ir "github.com/amiasea/packages/terraforge-cli/internal/model"
)

// NodeID is a stable identifier for a node in the graph.
// In Terraforge, this is simply the resource name.
type NodeID string

// Node represents a resource in the dependency graph.
type Node struct {
	ID       NodeID
	Resource *ir.Resource
}

// Edge represents a directed dependency: From → To.
// Meaning: From depends on To.
type Edge struct {
	From NodeID
	To   NodeID
}

// Graph is the dependency graph over IR resources.
type Graph struct {
	Nodes map[NodeID]*Node
	Edges []Edge

	// Adjacency lists for fast traversal.
	Outgoing map[NodeID][]NodeID
	Incoming map[NodeID][]NodeID
}

// NewGraph constructs an empty graph.
func NewGraph() *Graph {
	return &Graph{
		Nodes:    make(map[NodeID]*Node),
		Edges:    make([]Edge, 0),
		Outgoing: make(map[NodeID][]NodeID),
		Incoming: make(map[NodeID][]NodeID),
	}
}

// AddNode inserts a resource into the graph.
func (g *Graph) AddNode(res *ir.Resource) NodeID {
	id := NodeID(res.Name)
	if _, exists := g.Nodes[id]; exists {
		return id
	}
	g.Nodes[id] = &Node{
		ID:       id,
		Resource: res,
	}
	return id
}

// AddEdge adds a directed edge From → To (From depends on To).
func (g *Graph) AddEdge(from, to NodeID) {
	e := Edge{From: from, To: to}
	g.Edges = append(g.Edges, e)
	g.Outgoing[from] = append(g.Outgoing[from], to)
	g.Incoming[to] = append(g.Incoming[to], from)
}

// MustNode returns the node or panics if missing.
// Useful internally during graph construction.
func (g *Graph) MustNode(id NodeID) *Node {
	n, ok := g.Nodes[id]
	if !ok {
		panic(fmt.Sprintf("graph: missing node %q", id))
	}
	return n
}
