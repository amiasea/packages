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
	Model    string
}

func NewClient(endpoint, apiKey, model string) *Client {
	return &Client{
		Endpoint: endpoint,
		APIKey:   apiKey,
		Model:    model,
	}
}

func (c *Client) GenerateIR(ctx context.Context, prompt string) ([]byte, error) {
	reqBody := map[string]any{
		"model": c.Model,
		"input": prompt,
	}

	b, _ := json.Marshal(reqBody)
	req, err := http.NewRequestWithContext(ctx, "POST", c.Endpoint, bytes.NewReader(b))
	if err != nil {
		return nil, err
	}

	req.Header.Set("Content-Type", "application/json")
	req.Header.Set("api-key", c.APIKey)

	resp, err := http.DefaultClient.Do(req)
	if err != nil {
		return nil, err
	}
	defer resp.Body.Close()

	if resp.StatusCode >= 300 {
		body, _ := io.ReadAll(resp.Body)
		return nil, fmt.Errorf("AI error: %s", string(body))
	}

	return io.ReadAll(resp.Body)
}
