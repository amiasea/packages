# Terraforge CLI

Terraforge CLI is a Terraform provider compiler and graph‑aware module generator.
It turns a high‑level schema into:

- a typed internal model
- a dependency graph
- a generated provider
- a generated module
- a generated diagram

All from a single command.

Terraform is declarative.
Its ecosystem is not.
Terraforge exists to close that gap.

## Why This Package Exists

Terraform requires humans to manually:

- wire dependencies
- propagate attributes
- group resources into modules
- scope providers
- reason about ordering

Terraforge automates these structural concerns by treating Terraform as a compilable graph, not a text format.

The CLI implements five architectural pillars:

1. **Dependency Inference**
  Resources reference each other through attributes.
  Terraforge reads the schema and infers a directed graph of dependencies.
2. **Attribute Propagation**
  Attributes flow through the graph:
  resource → module → provider → outputs.
  Terraforge threads these values automatically.
3. **Module Grouping**
  Resources are clustered into modules based on dependency structure.
  Modules become composable, reusable, and deterministic.
4. **Provider Scoping**
  Only the resources actually used are included in the generated provider.
  This produces minimal, purpose‑built providers.
5. **Topological Sorting**
  The dependency graph is sorted into a stable, acyclic order.
  This ordering drives module generation, provider generation, and diagrams.

Together, these pillars form a semantic compiler pipeline for Terraform.

## What the CLI Does Today

The current implementation includes:

- **terraforge generate model**
  Builds the internal IR from schema.json.
- **terraforge generate module**
  Emits Terraform modules based on dependency clusters.
- **terraforge generate provider**
  Generates a Terraform provider source tree.
- **terraforge generate diagram**
  Produces a dependency graph visualization.
- **terraforge build**
  (In progress) Orchestrates the full pipeline end‑to‑end.

The folder structure mirrors a compiler:

schema → IR → graph → provider/module/docs/diagram

Each stage is isolated, deterministic, and testable.

## How to Use It

1. **Place a schema.json in the project root**
  This defines resource types, attributes, and relationships.
2. **Run any generator**

  ```bash
  terraforge generate model
  terraforge generate module
  terraforge generate provider
  terraforge generate diagram
  ```

3. **Or run the full pipeline** (once build.go is complete)

  ```bash
  terraforge build --schema schema.json --out .terraforge
  ```

4. **Terraforge emits:**

  - a typed IR
  - a dependency graph
  - a provider implementation
  - a module tree
  - a diagram

This output can be used directly in Terraform projects.

## Roadmap

The roadmap below defines the remaining work to complete the compiler.

### Phase 1 — Finish the CLI Commands

1) **cmd/build.go**

- Mission: Orchestrate the entire pipeline.
- Responsibilities:
  - Parse flags:
    - --schema
    - --out
    - --provider-only, --module-only, etc.
  - Call internal/generator in order:
    - schema.Load()
    - graph.Build()
    - provider.Generate()
    - module.Generate()
    - docs.Generate()
    - diagram.Generate()

Command structure (sketch):

```go
var buildCmd = &cobra.Command{
    Use: "build",
    Short: "Generate all Terraforge artifacts",
    RunE: func(cmd *cobra.Command, args []string) error {
        schema, err := schema.Load(schemaPath)
        if err != nil { return err }

        g := graph.Build(schema)

        if !providerOnly { provider.Generate(g, outDir) }
        if !moduleOnly   { module.Generate(g, outDir) }

        docs.Generate(g, outDir)
        diagram.Generate(g, outDir)

        return nil
    },
}
```

2) **cmd/login.go**

- Already implemented.
- Remaining tasks:
  - deterministic token caching
  - clean integration with azure_identity.go

3) **cmd/root.go**

- Minimal work:
  - add build command
  - add global flags (--verbose, --config)

4) **cmd/version.go**

- Optional: embed version via -ldflags.

### Phase 2 — Solidify Internal Generator Architecture

Your folder structure already mirrors a compiler pipeline.
Define responsibilities and outputs.

1) **internal/generator/schema/**

- Load schema (local or remote)
- Validate
- Produce canonical IR
- Output: *schema.Schema

2) **internal/generator/graph/**

- Convert IR → dependency graph
- Detect cycles
- Provide topological ordering
- Output: *graph.Graph

3) **internal/generator/provider/**

- Generate provider code
- Use graph ordering
- Emit deterministic folder structure

4) **internal/generator/module/**

- Generate Terraform modules
- Ensure stable naming and path resolution

5) **internal/generator/docs/**

- Generate Markdown docs
- Include:
  - resource descriptions
  - inputs/outputs
  - examples

6) **internal/generator/diagram/**

- Generate a dependency graph visualization
- Emit DOT, SVG, or PNG depending on configuration

### Phase 3 — Build Pipeline (End‑to‑End)

1) **Define a Pipeline struct**

```go
type Pipeline struct {
    Schema *schema.Schema
    Graph  *graph.Graph
}
```

2) **Define a deterministic sequence**

```go
func (p *Pipeline) Run(outDir string) error {
    provider.Generate(p.Graph, outDir)
    module.Generate(p.Graph, outDir)
    docs.Generate(p.Graph, outDir)
    diagram.Generate(p.Graph, outDir)
    return nil
}
```

3) **cmd/build.go calls:**

```go
pipeline := generator.NewPipeline(schemaPath)
pipeline.Run(outDir)
```

### Phase 4 — Developer Experience Polish

- --dry-run
- --graphviz
- --watch
- logging levels (--verbose, --debug)

### Phase 5 — Testing & Validation

- Unit tests
  - schema loader
  - graph cycle detection
  - provider deterministic output
- Integration tests
  - full pipeline with a sample schema
- Golden tests
  - compare generated output to expected fixtures

### Phase 6 — Release & Packaging

- Makefile or magefile.go
- go:embed for templates
- version injection
- binaries for:
  - Windows
  - Linux
  - macOS

### Summary — Combined Roadmap

- Finish build.go
- Formalize schema → graph → generation pipeline
- Implement deterministic generators
- Add DX polish
- Add tests
- Package & release

## RFC Directory

This directory is the record of architectural intent.
It is the canon.

**Structure**

- rfc‑000‑series — foundations
- rfc‑100‑series — core mechanics
- rfc‑200‑series — extensions
- rfc‑900‑series — meta‑specs

**Principles**

- Clarity over cleverness
- Determinism over implication
- Isolation over entanglement
- Every RFC must justify its existence

**Conventions**

- Filenames: rfc‑XYZ.md
- Each document begins with:
  - RFC number
  - Title
  - Status
  - Summary
- Cross‑references limited unless explicitly justified

**Purpose**

This directory exists to preserve intent, not decoration.

## Terraforge Compiler Pipeline (Diagram)

```text
                   ┌──────────────────────────┐
                   │        schema.json       │
                   │  (user‑authored schema)  │
                   └─────────────┬────────────┘
                                 │
                                 ▼
                   ┌──────────────────────────┐
                   │   Schema Loader (IR0)    │
                   │ internal/generator/schema│
                   └─────────────┬────────────┘
                                 │
                                 ▼
                   ┌──────────────────────────┐
                   │  Canonical IR (IR1)      │
                   │  Types, attributes,      │
                   │  relationships           │
                   └─────────────┬────────────┘
                                 │
                                 ▼
                   ┌──────────────────────────┐
                   │ Dependency Graph Builder │
                   │ internal/generator/graph │
                   └─────────────┬────────────┘
                                 │
                                 ▼
                   ┌──────────────────────────┐
                   │  Dependency Graph (DAG)  │
                   │  Nodes, edges, cycles,   │
                   │  topological order       │
                   └─────────────┬────────────┘
                                 │
         ┌───────────────────────┼────────────────────────┐
         │                       │                        │
         ▼                       ▼                        ▼
┌───────────────────┐   ┌──────────────────┐    ┌───────────────────┐
│ Provider Generator│   │ Module Generator │    │ Diagram Generator │
│ internal/.../prov │   │ internal/.../mod │    │ internal/.../diag │
└──────────┬────────┘   └──────────┬───────┘    └──────────┬────────┘
           │                       │                        │
           ▼                       ▼                        ▼
┌──────────────────┐   ┌──────────────────┐    ┌──────────────────┐
│ Provider Source  │   │ Terraform Modules│    │ Graph Diagram    │
│ (Go code)        │   │ (HCL)            │    │ (DOT/SVG/PNG)    │
└──────────────────┘   └──────────────────┘    └──────────────────┘
```

### Narrative Summary

1. **Schema → IR**
   The schema loader reads schema.json and produces a canonical internal representation (IR1).
   This is the “typed AST” of Terraforge.
2. **IR → Graph**
   The graph builder converts IR1 into a dependency graph:
   - nodes = resources
   - edges = attribute‑based dependencies
   - graph is validated for cycles
   - graph is topologically sorted
   This is the semantic backbone of the compiler.
3. **Graph → Generators**
   Three generators consume the graph:
   - Provider Generator → produces Go provider code
   - Module Generator → produces Terraform modules
   - Diagram Generator → produces a visual DAG
   Each generator is deterministic and isolated.

### Horizontal Version

```text
schema.json
     │
     ▼
[ Schema Loader ]
     │
     ▼
[ Canonical IR ]
     │
     ▼
[ Dependency Graph Builder ]
     │
     ▼
[ DAG (topologically sorted) ]
  │           │            │
  ▼           ▼            ▼
Provider   Modules      Diagram
Generator  Generator    Generator
  │           │            │
  ▼           ▼            ▼
Provider   Terraform     Graph
Source     Modules       Diagram
```

## Terraforge Layered Architecture Diagram

This diagram shows the system as layers, not steps — the way a compiler engineer would present the architecture to a team.

```text
┌──────────────────────────────────────────────────────────────┐
│                        LAYER 4: OUTPUTS                      │
│  - Provider Source (Go)                                      │
│  - Terraform Modules (HCL)                                   │
│  - Dependency Diagram (DOT/SVG/PNG)                          │
└──────────────────────────────────────────────────────────────┘
                           ▲
                           │
┌──────────────────────────────────────────────────────────────┐
│                   LAYER 3: GENERATORS                        │
│  - provider/  → emits provider code                          │
│  - module/    → emits Terraform modules                      │
│  - diagram/   → emits graph visualization                    │
└──────────────────────────────────────────────────────────────┘
                           ▲
                           │
┌──────────────────────────────────────────────────────────────┐
│                 LAYER 2: SEMANTIC GRAPH                      │
│  - Dependency graph (DAG)                                    │
│  - Cycle detection                                           │
│  - Topological sorting                                       │
│  - Attribute propagation                                     │
│  - Module grouping logic                                     │
└──────────────────────────────────────────────────────────────┘
                           ▲
                           │
┌──────────────────────────────────────────────────────────────┐
│                LAYER 1: INTERNAL REPRESENTATION              │
│  - Canonical IR (resources, attributes, relationships)       │
│  - Derived types and normalized structures                   │
└──────────────────────────────────────────────────────────────┘
                           ▲
                           │
┌──────────────────────────────────────────────────────────────┐
│                LAYER 0: SCHEMA INGESTION                     │
│  - Load schema.json                                          │
│  - Validate                                                  │
│  - Convert to IR                                             │
└──────────────────────────────────────────────────────────────┘
```

### How to read this diagram

- Layer 0 ingests user intent (schema.json).
- Layer 1 normalizes it into a typed IR.
- Layer 2 builds the semantic graph — the heart of Terraforge.
- Layer 3 contains independent generators that consume the graph.
- Layer 4 is the final emitted artifact set.
