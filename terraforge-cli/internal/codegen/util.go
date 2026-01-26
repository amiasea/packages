package codegen

import (
	"fmt"
	"os"
	"path/filepath"
)

// EnsureDir makes sure a directory exists, creating it if needed.
func EnsureDir(path string) error {
	if path == "" {
		return fmt.Errorf("directory path is empty")
	}
	return os.MkdirAll(path, 0o755)
}

// WriteFile writes content to a file, ensuring the parent directory exists.
func WriteFile(path string, data []byte) error {
	if err := EnsureDir(filepath.Dir(path)); err != nil {
		return err
	}
	return os.WriteFile(path, data, 0o644)
}

// Join returns a clean, platform‑correct path.
func Join(parts ...string) string {
	return filepath.Join(parts...)
}

// Exists checks whether a file or directory exists.
func Exists(path string) bool {
	_, err := os.Stat(path)
	return err == nil
}

// Must panics on error — useful for internal invariants, never for user‑facing code.
func Must(err error) {
	if err != nil {
		panic(err)
	}
}
