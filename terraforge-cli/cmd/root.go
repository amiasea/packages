package cmd

import (
	"fmt"
	"os"

	"github.com/spf13/cobra"
)

var rootCmd = &cobra.Command{
	Use:   "terraforge",
	Short: "Terraforge CLI",
}

func init() {
	rootCmd.AddCommand(NewLoginCmd())
	rootCmd.AddCommand(NewVersionCmd())
	rootCmd.AddCommand(NewAICmd()) // the only generator now
}

func Execute() {
	if err := rootCmd.Execute(); err != nil {
		fmt.Println(err)
		os.Exit(1)
	}
}
