package irgraph

import (
	"fmt"

	"github.com/amiasea/packages/terraforge-cli/internal/ir"
)

type NodeID string

type Node struct {
	ID       NodeID
	Resource *ir.Resource
}

type Edge struct {
	From NodeID
	To   NodeID
}

type Graph struct {
	Nodes    map[NodeID]*Node
	Edges    []Edge
	Outgoing map[NodeID][]NodeID
	Incoming map[NodeID][]NodeID
}

func NewGraph() *Graph {
	return &Graph{
		Nodes:    make(map[NodeID]*Node),
		Edges:    make([]Edge, 0),
		Outgoing: make(map[NodeID][]NodeID),
		Incoming: make(map[NodeID][]NodeID),
	}
}

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

func (g *Graph) AddEdge(from, to NodeID) {
	g.Edges = append(g.Edges, Edge{From: from, To: to})
	g.Outgoing[from] = append(g.Outgoing[from], to)
	g.Incoming[to] = append(g.Incoming[to], from)
}

func (g *Graph) MustNode(id NodeID) *Node {
	n, ok := g.Nodes[id]
	if !ok {
		panic(fmt.Sprintf("graph: missing node %q", id))
	}
	return n
}
