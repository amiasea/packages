package model

import genmodel "github.com/amiasea/packages/terraforge-cli/internal/generator/model"

// Build converts a generator model into the IR (SchemaIR).
func Build(m *genmodel.Model) (*SchemaIR, error) {
	ir := &SchemaIR{
		Resources: make(map[string]*Resource),
	}

	for name, r := range m.Resources {
		attrs := make(map[string]Value)

		for key, val := range r.Attributes {
			if val.Reference != nil {
				attrs[key] = Value{
					Reference: &Reference{
						Resource: val.Reference.Resource,
						Field:    val.Reference.Field,
					},
				}
			} else {
				attrs[key] = Value{
					Literal: val.Literal,
				}
			}
		}

		ir.Resources[name] = &Resource{
			Name:       r.Name,
			Type:       r.Type,
			Attributes: attrs,
			DependsOn:  r.DependsOn,
		}
	}

	return ir, nil
}
