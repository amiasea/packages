RFC‑005: AUI Studio Toolchain Specification
Status: Draft
Author: Alfredo
Category: Toolchain / Pipeline Architecture
Version: 1.0

1. Overview
This RFC defines the AUI Studio Toolchain, a novel, multi‑stage pipeline that transforms:
- natural language intent
into
- platform‑specific UI primitives
through a sequence of strictly bounded, deterministic, and semantically layered subsystems.
The toolchain is composed of four primary categories:
- Intent Engine (e.g., Wisk)
- Semantic Validation Engine (RFC‑003)
- Semantic‑to‑Platform Mapping Engine (RFC‑004)
- Platform Runtime Adapter (Host Integrator)
Each subsystem is:
- independently versioned
- independently testable
- strictly bounded
- cross‑referenced to the RFCs that define its semantics
This RFC defines the responsibilities, boundaries, invariants, and interop rules for the entire toolchain.

2. Goals
The AUI Studio Toolchain MUST:
- enforce strict separation of concerns
- preserve semantics across all stages
- guarantee determinism end‑to‑end
- define clear boundaries between subsystems
- define legal and illegal transformations
- define artifact types and lifecycle
- define cross‑subsystem invariants
The toolchain MUST NOT:
- infer semantics across subsystem boundaries
- allow subsystems to perform each other’s responsibilities
- introduce nondeterminism
- embed executable code at any stage

3. Toolchain Architecture
The toolchain consists of four subsystems, each defined as a category with explicit boundaries.
Intent Engine → Semantic Validator → Renderer Adapter → Host Integrator


Each subsystem consumes and produces a well‑defined artifact.

4. Subsystem Definitions (with Cross‑References)
4.1 Intent Engine
Category: Natural Language → Semantic IR Generator
Cross‑References:
- RFC‑002 (AUI Language)
- Appendix F (Artifact Lifecycle)
Responsibilities:
- transform natural language → raw AUI
- infer semantic structure
- infer layout relationships
- generate deterministic raw IR
Boundaries:
- MUST NOT validate semantics (RFC‑003)
- MUST NOT map to platform primitives (RFC‑004)
- MUST NOT embed renderer‑specific constructs
- MUST NOT output canonical AUI
Output:
- artifact.raw.aui (Appendix F)

4.2 Semantic Validator
Category: Semantic Validation Engine
Cross‑References:
- RFC‑003 (Validator Specification)
- Appendix C (Validator Algorithm)
- Appendix D (AUI DSL Grammar)
Responsibilities:
- enforce grammar
- enforce semantics
- enforce component contracts
- enforce layout rules
- enforce cross‑node invariants
- normalize structure
Boundaries:
- MUST NOT infer missing semantics
- MUST NOT map to platform primitives
- MUST NOT execute bindings or actions
Output:
- artifact.validated.aui (Appendix F)

4.3 Renderer Adapter
Category: Semantic‑to‑Platform Mapping Engine
Cross‑References:
- RFC‑004 (Renderer Adapter Specification)
- Appendix E (Renderer/Validator Interop Contract)
Responsibilities:
- map semantic kinds → platform primitives
- map semantic props → platform props
- expand composite components
- resolve semantic parts
- prepare symbolic events
Boundaries:
- MUST NOT infer layout
- MUST NOT reinterpret semantics
- MUST NOT reorder children
- MUST NOT embed executable code
Output:
- artifact.render.json (Appendix F)

4.4 Host Integrator
Category: Platform Runtime Adapter
Cross‑References:
- Appendix F (Artifact Lifecycle)
Responsibilities:
- instantiate platform UI components
- attach event handlers
- bind data contexts
- apply platform constraints
Boundaries:
- MUST NOT reinterpret AUI semantics
- MUST NOT mutate semantic meaning
- MUST NOT introduce nondeterminism
Output:
- artifact.host.ui

5. Artifact Lifecycle (Cross‑Referenced)
The toolchain produces the following artifacts (Appendix F):
|  |  |  |  | 
|  | artifact.raw.aui |  |  | 
|  | artifact.validated.aui |  |  | 
|  | artifact.aui |  |  | 
|  | artifact.render.json |  |  | 
|  | artifact.host.ui |  |  | 


Each artifact is:
- deterministic
- JSON‑serializable
- semantically layered

6. Subsystem Boundaries (Normative)
6.1 Intent Engine Boundaries
- MUST NOT validate
- MUST NOT canonicalize
- MUST NOT map to platform primitives
6.2 Semantic Validator Boundaries
- MUST NOT infer semantics
- MUST NOT perform rendering
- MUST NOT apply platform defaults
6.3 Renderer Adapter Boundaries
- MUST NOT validate
- MUST NOT infer layout
- MUST NOT execute actions
6.4 Host Integrator Boundaries
- MUST NOT reinterpret semantics
- MUST NOT reorder children
- MUST NOT introduce nondeterminism

7. Cross‑Subsystem Invariants
These invariants MUST hold across the entire toolchain:
7.1 Structural Invariants
- tree shape preserved
- ordering preserved
- no cycles introduced
7.2 Semantic Invariants
- kind semantics preserved
- props semantics preserved
- layout semantics preserved
7.3 Binding Invariants
- binding paths preserved
- no new bindings introduced
7.4 Determinism Invariants
- identical input → identical output
- stable normalization
- stable mapping

8. Transformation Legality Matrix
|  |  |  | 
|  |  |  | 
|  |  |  | 
|  |  |  | 
|  |  |  | 
|  |  |  | 
|  |  |  | 
|  |  |  | 
|  |  |  | 



9. Versioning
Each subsystem MUST declare:
- AUI version
- contract version
- subsystem version
Version mismatches MUST cause rejection.

10. Compliance
A toolchain is compliant if:
- each subsystem respects its boundaries
- each artifact is produced correctly
- invariants are preserved end‑to‑end
- determinism is preserved
- cross‑references remain valid
Compliance MUST be testable via:
- golden artifacts
- structural diffs
- semantic diffs
- mapping conformance tests
