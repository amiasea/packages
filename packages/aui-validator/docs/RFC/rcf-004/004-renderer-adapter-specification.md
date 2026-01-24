RFC‑004: Renderer Adapter Specification
Status: Draft
Author: Alfredo
Category: Rendering / Platform Integration
Version: 1.0

1. Overview
This RFC defines a novel category of software subsystem called the:
Renderer Adapter — a deterministic, semantics‑preserving compiler that transforms canonical AUI into platform‑specific UI primitives.

This category does not exist in traditional UI toolchains.
It is invented specifically for the AUI ecosystem.
Renderer Adapters sit between:
- Semantic Layer
- produced by the Semantic Validator (Category: Validation Engine)
- Platform Layer
- consumed by the Host Integrator (Category: Platform Runtime Adapter)
Renderer Adapters are responsible for:
- mapping semantic kinds → platform primitives
- mapping semantic props → platform props
- expanding composite components
- resolving semantic parts
- preparing symbolic events
- preserving determinism
Renderer Adapters MUST NOT:
- infer layout
- reinterpret semantics
- execute actions
- evaluate bindings
- introduce nondeterminism
This RFC defines the architecture, invariants, and responsibilities of this newly invented category.

2. Novel Category Definition: Renderer Adapter
2.1 What Makes This Category New
Traditional UI systems have:
- renderers
- compilers
- templating engines
- component frameworks
None of these perform the role defined here.
A Renderer Adapter is a new category because:
- It consumes a semantic IR (AUI), not templates or code.
- It performs semantic‑to‑platform mapping, not rendering.
- It is deterministic, not reactive or stateful.
- It is pure, not tied to runtime state.
- It is contract‑driven, not heuristic.
- It is renderer‑agnostic, not bound to a specific UI library.
This subsystem is invented to support the AUI pipeline’s strict separation of:
- intent
- semantics
- validation
- rendering
- platform integration
No pre‑existing tool performs this exact role.

3. Relationship to Other Tools (Explicit Categories)
| System | Category | Relationship | Reference |
| --- | --- | --- | --- |
| Semantic Validator | Validation Engine | Produces the canonical input artifact the adapter consumes. | RFC‑003 |
| AUI Toolchain | Orchestration | Places the adapter in the pipeline and manages artifact flow. | RFC‑005 |
| Component Contracts | Contract Layer | Defines allowed kinds/props/parts the adapter must respect. | RFC‑006 |
| Semantic Event Model | Interaction Semantics | Provides event names/payload semantics that must be preserved. | RFC‑007 |
| Data Model & Binding | Data Semantics | Resolves bindings before adapter hydration; adapter must not evaluate bindings. | RFC‑008 |
| Rendering Semantics | Renderer Semantics | Defines downstream interpretation rules the adapter must satisfy. | RFC‑012 |

Renderer Adapters do not replace any of these tools.
They form a new, required stage in the AUI pipeline.

4. Input Requirements
Renderer Adapters MUST accept only:
- artifact.aui
- produced by the Semantic Validator
- canonical
- normalized
- semantically correct
Renderer Adapters MUST reject:
- raw AUI (Intent Engine output)
- unvalidated AUI
- malformed AUI
- version‑mismatched AUI
This ensures the adapter never performs validation or inference.

5. Output Requirements
Renderer Adapters MUST output:
artifact.render.json


This artifact is:
- platform‑specific
- deterministic
- structurally equivalent to the AUI tree
- semantically equivalent to the AUI tree
- ready for consumption by the Host Integrator
Renderer Adapters MUST NOT output:
- executable code
- platform state
- inferred layout
- inferred semantics

6. Architecture
Renderer Adapters consist of four subsystems:
- Kind Mapper
- Prop Mapper
- Component Expander
- Event Preparer
Each subsystem is defined below with explicit novelty and category context.

7. Kind Mapper (Subsystem)
Category: Semantic‑to‑Platform Mapping Engine
Novelty: This is the first mapping layer that treats UI semantics as a stable IR.
Responsibilities:
- map semantic kinds → platform primitives
- ensure mapping is deterministic
- ensure mapping is one‑to‑one
Rules:
- MUST NOT infer mapping from props
- MUST NOT drop kinds
- MUST NOT reorder nodes

8. Prop Mapper (Subsystem)
Category: Semantic‑to‑Platform Mapping Engine
Novelty: Unlike traditional frameworks, props are semantic, not renderer‑specific.
Responsibilities:
- map semantic props → platform props
- preserve semantic meaning
- apply platform defaults without overriding semantics
Rules:
- MUST NOT introduce new semantics
- MUST NOT reinterpret values
- MUST NOT embed executable code

9. Component Expander (Subsystem)
Category: Semantic‑to‑Platform Mapping Engine
Novelty: Expansion is deterministic and contract‑driven, not template‑driven.
Responsibilities:
- inline composite components
- resolve semantic parts
- preserve ordering
Rules:
- MUST NOT introduce new nodes
- MUST NOT drop nodes
- MUST NOT reorder children

10. Event Preparer (Subsystem)
Category: Semantic‑to‑Platform Mapping Engine
Novelty: Events remain symbolic, not executable.
Responsibilities:
- attach symbolic event references
- prepare event metadata for the Host Integrator
Rules:
- MUST NOT execute actions
- MUST NOT embed code
- MUST NOT transform event names

11. Determinism Requirements
Renderer Adapters MUST guarantee:
- same input → same output
- stable mapping
- stable expansion
- stable defaults
Renderer Adapters MUST NOT:
- depend on runtime state
- introduce randomness
- reorder children

12. Error Model
Errors MUST be:
- deterministic
- structured
- machine‑readable
- human‑readable
Error Categories:
- unknown kind
- unknown prop
- missing mapping
- invalid layout
- invalid contract
- version mismatch
Error Format:
{
  "code": "AUI_RENDERER_UNKNOWN_KIND",
  "message": "No mapping found for kind 'card'",
  "path": "$.root.children[0]"
}



13. Versioning
Renderer Adapters MUST enforce:
- AUI version compatibility
- contract version compatibility
- adapter version compatibility
Version mismatches MUST cause rejection.

14. Extensibility
Renderer Adapters MAY support:
- namespaced kinds
- custom mapping packs
- platform‑specific extensions
Extensions MUST NOT:
- override core semantics
- weaken invariants
- introduce nondeterminism

15. Compliance
A Renderer Adapter is compliant if it:
- implements all mapping rules
- preserves semantics
- preserves structure
- preserves determinism
- rejects invalid input
- produces valid renderer artifacts
Compliance MUST be testable via:
- golden snapshots
- structural diffs
- mapping conformance tests
