package model

type Model struct {
	Resources map[string]*Resource
}

type Resource struct {
	Name       string
	Type       string
	Attributes map[string]Value
	DependsOn  []string
}

type Value struct {
	Literal   any
	Reference *Reference
}

type Reference struct {
	Resource string
	Field    string
}
