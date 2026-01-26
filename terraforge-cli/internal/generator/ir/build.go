package irbuild

import (
	genmodel "github.com/amiasea/packages/terraforge-cli/internal/generator/model"
	ir "github.com/amiasea/packages/terraforge-cli/internal/model"
)

// Build converts the generator model into the semantic IR (SchemaIR).
func Build(m *genmodel.Model) (*ir.SchemaIR, error) {
	out := &ir.SchemaIR{
		Resources: make(map[string]*ir.Resource),
	}

	for name, r := range m.Resources {
		res := &ir.Resource{
			Name:       r.Name,
			Type:       r.Type,
			Attributes: make(map[string]ir.Value),
			DependsOn:  nil,
		}

		for attrName, attrVal := range r.Attributes {
			var ref *ir.Reference
			if attrVal.Reference != nil {
				ref = &ir.Reference{
					Resource: attrVal.Reference.Resource,
					Field:    attrVal.Reference.Field,
				}
			}

			res.Attributes[attrName] = ir.Value{
				Literal:   attrVal.Literal,
				Reference: ref,
			}
		}

		out.Resources[name] = res
	}

	return out, nil
}
