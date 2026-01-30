
# Terraforge CLI

Terraforge CLI is a thin, AI‑powered compiler frontend for generating Terraform
providers directly from a `schema.json` file. The CLI does not contain a
traditional compiler pipeline, IR model, or code generation backend. Instead, it
delegates the entire provider‑generation process to an Azure AI model.

The architecture is intentionally minimal:


schema.json ↓ BuildPrompt(schemaJSON) ↓ Azure AI → Go provider source code ↓ Write provider.go

The CLI orchestrates this flow and nothing more.

---

## Features

- **AI‑driven provider generation**  
  Provide a `schema.json`, and Terraforge asks Azure AI to produce a complete
  Terraform provider implementation in Go.

- **Zero internal IR**  
  No typed IR, no IR graph, no transforms, no backend codegen. The AI *is* the
  compiler.

- **Minimal surface area**  
  Only the commands required for authentication, versioning, and AI‑based
  provider generation remain.

- **Deterministic filesystem output**  
  The generated provider is written to `.terraforge/ai/provider.go` unless
  otherwise specified.

---

## Installation

```bash
go install github.com/amiasea/packages/terraforge-cli@latest



Usage
Generate a provider using Azure AI
terraforge ai \
  --schema schema.json \
  --endpoint https://your-azure-endpoint \
  --key $AZURE_API_KEY \
  --model gpt-4o-mini


This produces:
.terraforge/ai/provider.go


containing the full Terraform provider implementation.
Check CLI version
terraforge version


Authenticate (if required for your environment)
terraforge login



Schema Format
schema.json is the only input to the compiler. It is passed verbatim to the AI model inside the prompt. No Go struct representation or intermediate transformation is performed.
Example:
{
  "provider": {
    "name": "example",
    "description": "Example provider"
  },
  "resources": [
    {
      "name": "example_resource",
      "type": "example_resource",
      "attributes": {
        "id": { "type": "string", "computed": true },
        "name": { "type": "string", "required": true }
      }
    }
  ]
}



Architecture
The CLI is intentionally small:
cmd/
    ai.go          # schema.json → AI → provider.go
    login.go
    version.go
    root.go

internal/
    ai/
        client.go  # Azure AI HTTP client
        prompt.go  # Builds prompt from raw schema.json
    auth/
        azure_identity.go
    filesystem/
        filesystem.go

The AI returns final Go code, and the CLI writes it to disk.

Philosophy
Terraforge CLI embraces a radically simplified compiler model:
- The schema is the source of truth.
- The AI is the compiler backend.
- The CLI is a thin orchestrator.
- No intermediate representation is maintained.
- No codegen logic exists in the CLI.
This keeps the system small, explicit, and easy to evolve.
