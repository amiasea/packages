package ai

import (
	"bytes"
	"context"
	"encoding/json"
	"fmt"
	"io"
	"net/http"
)

type Client struct {
	Endpoint string
	APIKey   string
	Model    string // deployment name
}

func NewClient(endpoint, apiKey, model string) *Client {
	return &Client{
		Endpoint: endpoint,
		APIKey:   apiKey,
		Model:    model,
	}
}

// GenerateProvider sends the prompt to Azure AI and returns Go provider source code.
func (c *Client) GenerateProvider(ctx context.Context, prompt string) (string, error) {
	url := fmt.Sprintf(
		"%s/openai/deployments/%s/chat/completions?api-version=2024-10-01-preview",
		c.Endpoint,
		c.Model,
	)

	reqBody := map[string]any{
		"messages": []map[string]string{
			{"role": "user", "content": prompt},
		},
	}

	b, _ := json.Marshal(reqBody)
	req, err := http.NewRequestWithContext(ctx, "POST", url, bytes.NewReader(b))
	if err != nil {
		return "", err
	}

	req.Header.Set("Content-Type", "application/json")
	req.Header.Set("api-key", c.APIKey)

	resp, err := http.DefaultClient.Do(req)
	if err != nil {
		return "", err
	}
	defer resp.Body.Close()

	raw, _ := io.ReadAll(resp.Body)

	fmt.Println("=== RAW AI OUTPUT BEGIN ===")
	fmt.Println(string(raw))
	fmt.Println("=== RAW AI OUTPUT END ===")

	if resp.StatusCode >= 300 {
		return "", fmt.Errorf("AI error: %s", string(raw))
	}

	var parsed struct {
		Choices []struct {
			Message struct {
				Content string `json:"content"`
			} `json:"message"`
		} `json:"choices"`
	}

	if err := json.Unmarshal(raw, &parsed); err != nil {
		return "", err
	}

	if len(parsed.Choices) == 0 {
		return "", fmt.Errorf("no choices returned")
	}

	return parsed.Choices[0].Message.Content, nil
}
