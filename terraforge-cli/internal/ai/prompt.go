package ai

import (
	"encoding/json"

	"github.com/amiasea/packages/terraforge-cli/internal/codegen/schema"
)

func BuildPrompt(s *schema.Schema) (string, error) {
	b, err := json.MarshalIndent(s, "", "  ")
	if err != nil {
		return "", err
	}

	return `
You are a compiler frontend. Convert this schema into a Terraform provider IR.

Return ONLY valid JSON matching this structure:

{
  "provider": { "name": "...", "description": "...", "config": {} },
  "resources": [
    {
      "name": "...",
      "type": "...",
      "attributes": [
        { "name": "...", "type": "...", "required": true, "computed": false, "description": "..." }
      ],
      "create": { "method": "POST", "path": "...", "body": {}, "response_map": {} },
      "read":   { "method": "GET",  "path": "...", "body": {}, "response_map": {} },
      "update": { "method": "PUT",  "path": "...", "body": {}, "response_map": {} },
      "delete": { "method": "DELETE","path": "...", "body": {}, "response_map": {} }
    }
  ],
  "models": []
}

Schema:
` + string(b), nil
}
