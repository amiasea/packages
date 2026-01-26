package irbuild

import (
	genmodel "github.com/amiasea/packages/terraforge-cli/internal/codegen/model"
	ir "github.com/amiasea/packages/terraforge-cli/internal/ir"
)

// Build converts the generator model into the canonical IR.
func Build(m *genmodel.Model) (*ir.IR, error) {
	out := &ir.IR{
		Resources: make([]*ir.Resource, 0, len(m.Resources)),
	}

	for name, r := range m.Resources {
		res := &ir.Resource{
			Name:       name,
			Type:       r.Type,
			Attributes: make(map[string]*ir.Attribute),
			DependsOn:  r.DependsOn,
		}

		// Convert attributes
		for attrName, attrVal := range r.Attributes {
			var ref *ir.Reference
			if attrVal.Reference != nil {
				ref = &ir.Reference{
					Resource: attrVal.Reference.Resource,
					Field:    attrVal.Reference.Field,
				}
			}

			res.Attributes[attrName] = &ir.Attribute{
				Literal:   attrVal.Literal,
				Reference: ref,
			}
		}

		out.Resources = append(out.Resources, res)
	}

	return out, nil
}
