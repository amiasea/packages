package ai

import (
	"encoding/json"

	ir "github.com/amiasea/packages/terraforge-cli/internal/ir"
)

func ParseIR(data []byte) (*ir.IR, error) {
	var out ir.IR
	if err := json.Unmarshal(data, &out); err != nil {
		return nil, err
	}
	return &out, nil
}
