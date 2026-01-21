package cmd

import (
    "fmt"
    "github.com/spf13/cobra"
)

var versionCmd = &cobra.Command{
    Use:   "version",
    Short: "Print the version number of Apperium",
    Run: func(cmd *cobra.Command, args []string) {
        fmt.Println("Apperium CLI v0.1.0")
    },
}

func init() {
    rootCmd.AddCommand(versionCmd)
}