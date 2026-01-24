package cmd

import (
    "fmt"
    "github.com/spf13/cobra"
)

var versionCmd = &cobra.Command{
    Use:   "version",
    Short: "Print the version number of Terraforge CLI",
    Run: func(cmd *cobra.Command, args []string) {
        fmt.Println("Terraforge CLI v0.1.0")
    },
}