package filesystem

import (
	"fmt"
	"os"
	"path/filepath"
)

type FS struct{}

// New returns a new filesystem abstraction.
func New() *FS {
	return &FS{}
}

// WriteFile writes data to a file, ensuring the directory exists.
func (fs *FS) WriteFile(path string, data []byte) error {
	dir := filepath.Dir(path)

	if err := os.MkdirAll(dir, 0o755); err != nil {
		return fmt.Errorf("failed to create directory %s: %w", dir, err)
	}

	if err := os.WriteFile(path, data, 0o644); err != nil {
		return fmt.Errorf("failed to write file %s: %w", path, err)
	}

	return nil
}

// ReadFile reads a file from disk.
func (fs *FS) ReadFile(path string) ([]byte, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file %s: %w", path, err)
	}
	return data, nil
}
