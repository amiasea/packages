package ai

import (
	"fmt"
)

func BuildPrompt(
	promptTemplate string,
	schemaJSON string,
	modulePath string,
	packageName string,
	providerAddress string,
) (string, error) {

	// Interpolate the %s placeholders inside prompt.txt
	final := fmt.Sprintf(
		promptTemplate,
		packageName,     // %s #1
		modulePath,      // %s #2
		modulePath,      // %s #3
		packageName,     // %s #4
		modulePath,      // %s #5
		packageName,     // %s #6
		providerAddress, // %s #7
		packageName,     // %s #8
		providerAddress, // %s #9
		schemaJSON,      // %s #10
	)

	return final, nil
}
