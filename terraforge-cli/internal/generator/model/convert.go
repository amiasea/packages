package model

import schema "github.com/amiasea/packages/terraforge-cli/internal/generator/schema"

// FromSchema converts schema.Schema → generator model.Model
func FromSchema(s *schema.Schema) (*Model, error) {
	m := &Model{
		Resources: map[string]*Resource{},
	}

	for _, r := range s.Resources {
		attrs := map[string]Value{}

		for _, f := range r.Fields {
			attrs[f.Name] = Value{
				Literal:   nil,
				Reference: nil,
			}
		}

		m.Resources[r.Name] = &Resource{
			Name:       r.Name,
			Type:       r.Type,
			Attributes: attrs,
			DependsOn:  nil,
		}
	}

	return m, nil
}
