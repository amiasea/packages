module github.com/amiasea/packages/terraforge-cli

go 1.25.6

replace github.com/amiasea/packages/common/auth => ../common/auth

require (
	github.com/amiasea/packages/common/auth v0.0.0-00010101000000-000000000000
	github.com/spf13/cobra v1.10.2
)

require (
	github.com/inconshreveable/mousetrap v1.1.0 // indirect
	github.com/spf13/pflag v1.0.9 // indirect
)
