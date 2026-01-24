# RFC Directory

Welcome to the place where intent becomes law and speculation becomes structure.  
Every document here exists to clarify, not to impress; to define, not to decorate.

# Terraforge CLI — Roadmap

This document is a disciplined execution plan for the Terraforge CLI. It is direct, complete, and ordered for delivery.

---

## Phase 1 — Finish the CLI Commands

### 1) cmd/build.go (currently empty)
**Mission:** Orchestrate the entire pipeline.

**Responsibilities**
- Parse flags:
  - --schema
  - --out
  - --provider-only, --module-only, etc.
- Call internal/generator in order:
  1. schema.Load()
  2. graph.Build()
  3. provider.Generate()
  4. module.Generate()
  5. docs.Generate()
  6. plan.Generate()

**Command structure (sketch)**
var buildCmd = &cobra.Command{ Use: "build", Short: "Generate all Terraforge artifacts", RunE: func(cmd *cobra.Command, args []string) error { schema, err := schema.Load(schemaPath) if err != nil { return err } g := graph.Build(schema) if !providerOnly { provider.Generate(g, outDir) } if !moduleOnly { module.Generate(g, outDir) } docs.Generate(g, outDir) plan.Generate(g, outDir) return nil },}

### 2) cmd/login.go
Already implemented. Remaining tasks:
- Ensure token caching is deterministic
- Ensure it integrates cleanly with azure_identity.go

### 3) cmd/root.go
Minimal work:
- Add build command
- Add global flags (e.g., --verbose, --config)

### 4) cmd/version.go
Already fine. Optional: embed version via -ldflags.

---

## Phase 2 — Solidify Internal Generator Architecture

Your folder structure already mirrors a compiler pipeline. Define responsibilities and outputs.

### 1) internal/generator/schema/
- Load schema from:
  - Local file
  - Remote URL
- Validate schema
- Produce canonical internal representation (IR)

**Output:** *schema.Schema (IR root)

### 2) internal/generator/graph/
- Convert schema IR into a dependency graph
- Detect cycles
- Provide topological ordering for generation

**Output:** *graph.Graph

### 3) internal/generator/provider/
- Generate provider code (Go or Terraform)
- Use graph ordering
- Emit deterministic folder structure

### 4) internal/generator/module/
- Generate modules from provider outputs
- Ensure stable naming and path resolution

### 5) internal/generator/docs/
- Generate Markdown docs
- Include:
  - Resource descriptions
  - Inputs/outputs
  - Examples

### 6) internal/generator/plan/
- Generate a “plan” artifact:
  - JSON
  - DAG
  - Terraform plan wrapper

---

## Phase 3 — Build Pipeline (End‑to‑End)

### 1) Define a Pipeline struct
type Pipeline struct { Schema *schema.Schema Graph *graph.Graph }

### 2) Define a deterministic sequence
func (p *Pipeline) Run(outDir string) error { provider.Generate(p.Graph, outDir) module.Generate(p.Graph, outDir) docs.Generate(p.Graph, outDir) plan.Generate(p.Graph, outDir) return nil }

### 3) cmd/build.go calls:
pipeline := generator.NewPipeline(schemaPath)
pipeline.Run(outDir)

---

## Phase 4 — Developer Experience Polish

1) --dry-run
Print what would be generated.

2) --graphviz
Emit a .dot file from internal/generator/graph.

3) --watch
Optional: watch schema file and regenerate.

4) Logging levels
- --verbose
- --debug

---

## Phase 5 — Testing & Validation

**Unit tests**
- schema loader
- graph cycle detection
- provider deterministic output

**Integration tests**
- Full pipeline with a sample schema

**Golden tests**
- Compare generated output to expected fixtures

---

## Phase 6 — Release & Packaging

- Add Makefile or magefile.go
- Add go:embed for templates
- Add version injection
- Produce binaries for:
  - Windows
  - Linux
  - macOS

---

## Summary — Combined Roadmap

1) Finish build.go
2) Formalize schema → graph → generation pipeline
3) Implement deterministic generators
4) Add DX polish
5) Add tests
6) Package & release

---

# RFC Directory

This directory is the record of architectural intent. It is the canon.

## Structure
- rfc-000-series — The foundations
- rfc-100-series — Core mechanics and operational rules
- rfc-200-series — Extensions, optional layers, ambitious ideas
- rfc-900-series — Meta-specs and interpretive frameworks

## Principles
- Clarity over cleverness
- Determinism over implication
- Isolation over entanglement
- Every RFC must justify its own existence

## Conventions
- Filenames follow: rfc-XYZ.md
- Each document begins with:
  - RFC number
  - Title
  - Status
  - Summary
- Cross-references are limited to the immediately preceding RFC unless explicitly allowed

## Purpose
This directory exists to preserve intent, not decoration.
