AUI.md — Abstract User Interface  
Author: Alfredo  
Version: 1.0  
Status: Living Document

0. Purpose of This Document  
AUI.md is the authoritative root of the AUI universe.  
It defines the semantic substrate, subsystem architecture, and meta‑architecture that govern every artifact in the system.  
This document exists to:  
- preserve the lineage and intent of the architecture  
- prevent drift across versions and implementations  
- unify all RFCs under a coherent conceptual model  
- provide a complete, durable record for future readers  
- articulate the structural logic behind both the technical system and its author’s evolution  
AUI.md is simultaneously:  
- a technical specification, and  
- a structural record of the system’s development  
It functions as Internal Systems Specification v3.x, expressed through the lens of UI semantics.

I. The Three‑Layer AUI Universe  
AUI is organized into a strict, traceable hierarchy of three layers.  
Each layer has a defined purpose, boundary, and versioning strategy.

┌──────────────────────────────────────────────────────────┐  
│ LAYER 3 — META‑ARCHITECTURE (Unbounded)                  │  
│ Governance • Registries • Evolution • Tooling            │  
└──────────────────────────────────────────────────────────┘  

┌──────────────────────────────────────────────────────────┐  
│ LAYER 2 — SUBSYSTEM ARCHITECTURE (Finite Set)            │  
│ Validators • Adapters • Contracts • Events • Data        │  
│ RFC‑001 → RFC‑012                                        │  
└──────────────────────────────────────────────────────────┘  

┌──────────────────────────────────────────────────────────┐  
│ LAYER 1 — SEMANTIC SUBSTRATE (Foundational)              │  
│ Nodes • Kinds • Props • Parts • Bindings                 │  
│ Defined in RFC‑002 + Appendixes                          │  
└──────────────────────────────────────────────────────────┘  

Layer 1 — Semantic Substrate  
The irreducible primitives of the system.  
The atoms and grammar.  
Defined in [RFC‑002](./rfc-002/002-aui-language-specification.md).

Layer 2 — Subsystem Architecture  
The finite set of engines that operate on the substrate.  
[RFC‑001](./rfc-001/001-aui-studio-pipeline-archiecture.md) → [RFC‑012](./rfc-012/012-aui-rendering-semantics.md).

Layer 3 — Meta‑Architecture  
The governance and evolution layer.  
[RFC‑900](./rfc-900/900-behavior-gurantees.md) and all future meta‑specs.

II. RFC Placement Overview  
| RFC | Layer | Role | Link |
| --- | --- | --- | --- |
| RFC‑001 | 2 | Pipeline and artifact lifecycle. | [rfc-001/001-aui-studio-pipeline-archiecture.md](./rfc-001/001-aui-studio-pipeline-archiecture.md) |
| RFC‑002 | 1 | Language primitives and grammar. | [rfc-002/002-aui-language-specification.md](./rfc-002/002-aui-language-specification.md) |
| RFC‑003 | 2 | Semantic validation engine. | [rfc-003/003-aui-validator-specification.md](./rfc-003/003-aui-validator-specification.md) |
| RFC‑004 | 2 | Renderer adapter interface. | [rcf-004/004-renderer-adapter-specification.md](./rcf-004/004-renderer-adapter-specification.md) |
| RFC‑005 | 2 | Toolchain boundaries. | [rfc-005/005-aui-studio-toolchain-specification.md](./rfc-005/005-aui-studio-toolchain-specification.md) |
| RFC‑006 | 2 | Component contracts. | [rfc-006/006-aui-component-contract-specification.md](./rfc-006/006-aui-component-contract-specification.md) |
| RFC‑007 | 2 | Semantic event model. | [rcf-007/007-aui-semantic-event-model-specification.md](./rcf-007/007-aui-semantic-event-model-specification.md) |
| RFC‑008 | 2 | Data model & binding engine. | [rfc-008/008-aui-data-modeling-and-binding-engine.md](./rfc-008/008-aui-data-modeling-and-binding-engine.md) |
| RFC‑009 | 2 | Layout engine. | [rfc-009/009-aui-layout-engine.md](./rfc-009/009-aui-layout-engine.md) |
| RFC‑010 | 2 | Component model. | [rfc-010/010-aui-component-model.md](./rfc-010/010-aui-component-model.md) |
| RFC‑011 | 2 | Interaction model. | [rfc-011/011-aui-interaction-model.md](./rfc-011/011-aui-interaction-model.md) |
| RFC‑012 | 2 | Rendering semantics. | [rfc-012/012-aui-rendering-semantics.md](./rfc-012/012-aui-rendering-semantics.md) |
| RFC‑900 | 3 | Behavioral guarantees. | [rfc-900/900-behavior-gurantees.md](./rfc-900/900-behavior-gurantees.md) |

III. Canonical RFC Index (Expanded)  
RFC‑001 — [AUI Studio Pipeline Architecture](./rfc-001/001-aui-studio-pipeline-archiecture.md)  
Defines the end‑to‑end pipeline and artifact lifecycle.  

RFC‑002 — [AUI Language Specification](./rfc-002/002-aui-language-specification.md)  
Defines the primitives of AUI: nodes, kinds, props, parts, metadata.  
Includes schema, grammar, lifecycle, interop, and glossary.  
Appendixes:  
- [Appendix A — Language Schema](./rfc-002/appendixes/appendix-a.md)  
- [Appendix B — Renderer Contract](./rfc-002/appendixes/appendix-b.md)  
- [Appendix C — Validator Algorithm](./rfc-002/appendixes/appendix-c.md)  
- [Appendix D — DSL Grammar](./rfc-002/appendixes/appendix-d.md)  
- [Appendix E — Renderer/Validator Interop](./rfc-002/appendixes/appendix-e.md)  
- [Appendix F — Artifact Lifecycle](./rfc-002/appendixes/appendix-f.md)  
- [Glossary — AUI Terminology](../glossary/aui-glossary.md)  

RFC‑003 — [AUI Validator Specification](./rfc-003/003-aui-validator-specification.md)  
Defines the semantic validation engine.  

RFC‑004 — [Renderer Adapter Specification](./rcf-004/004-renderer-adapter-specification.md)  
Defines the mapping from semantic AUI to platform‑specific renderers.  
Appendixes:  
- [Appendix A](./rfc-004/apendixes/andendix-a.md)  

RFC‑005 — [AUI Studio Toolchain Specification](./rfc-005/005-aui-studio-toolchain-specification.md)  
Defines the boundaries and responsibilities of all subsystems.  

RFC‑006 — [Component Contract Specification](./rfc-006/006-aui-component-contract-specification.md)  
Defines the semantic API of components.  

RFC‑007 — [Semantic Event Model](./rcf-007/007-aui-semantic-event-model-specification.md)  
Defines the declarative, symbolic event system.  

RFC‑008 — [AUI Data Model & Binding Engine](./rfc-008/008-aui-data-modeling-and-binding-engine.md)  
Defines the canonical data representation and binding semantics.  
Appendixes:  
- [Appendix A — Minimal Example](./rfc-008/appendixes/appendix-a.md)  
- [Appendix B — Reserved Keywords](./rfc-008/appendixes/appendix-b.md)  

RFC‑009 — [AUI Layout Engine](./rfc-009/009-aui-layout-engine.md)  
Defines layout semantics and spatial resolution.  

RFC‑010 — [AUI Component Model](./rfc-010/010-aui-component-model.md)  
Defines component instantiation and lifecycle rules.  

RFC‑011 — [AUI Interaction Model](./rfc-011/011-aui-interaction-model.md)  
Defines interaction semantics and user intent handling.  
Appendixes:  
- [Appendix A](./rfc-011/appendixes/appendix-a.md)  
- [Appendix B](./rfc-011/appendixes/appendix-b.md)  
- [Appendix C](./rfc-011/appendixes/appendix-c.md)  
- [Appendix D](./rfc-011/appendixes/appendix-d.md)  
- [Appendix E](./rfc-011/appendixes/appendix-e.md)  

RFC‑012 — [AUI Rendering Semantics](./rfc-012/012-aui-rendering-semantics.md)  
Defines renderer‑level interpretation rules.  
Appendixes:  
- [Appendix A](./rfc-012/appendixes/appendix-a.md)  
- [Appendix B](./rfc-012/appendixes/appendix-b.md)  
- [Appendix C](./rfc-012/appendixes/appendix-c.md)  

RFC‑900 — [Behavior Guarantees](./rfc-900/900-behavior-gurantees.md)  
Defines long‑term behavioral guarantees across the system.

IV. Layer Summary (Expanded)  
Layer 1 — Semantic Substrate  
The irreducible primitives.  
Defined in [RFC‑002](./rfc-002/002-aui-language-specification.md).

Layer 2 — Subsystem Architecture  
The finite set of engines that operate on the substrate.  
[RFC‑001](./rfc-001/001-aui-studio-pipeline-archiecture.md) → [RFC‑012](./rfc-012/012-aui-rendering-semantics.md).

Layer 3 — Meta‑Architecture  
The infinite governance layer.  
Future RFCs will live here.

V. Glossary  
Canonical definitions of all AUI terms.  
Located at: [glossary/aui-glossary.md](../glossary/aui-glossary.md)

VI. The Meta‑Document of Your 30s in Tech  
AUI.md also functions as Internal Systems Specification v3.x —  
the document written when a person’s internal architecture becomes too complex to ignore.  
It includes:  
- Architecture Drift Notes  
	Recognition that the 20s codebase no longer scales.  
- Schema Collision Logs  
	Tracking when you fall into someone else’s worldview.  
- Layer‑Model Clarifications  
	Defining Layer 0 → Layer 5 to prevent internal confusion.  
- Arc Dynamics  
	Mapping tension, differential, and proto‑structure.  
- Mid‑Career Rewrite Proposal  
	Refactoring identity, not replacing it.  
- Lore Integration  
	Merging personal mythology, tech background, and symbolic thinking.  
- The Mountain Section  
	Awe, scale, perspective — the high‑altitude view.

L0: Specification Layer  
- Meaning = rule‑governed use.  
- Defines correctness, constraints, invariants.  
- A statement belongs here only if it changes what counts as valid.

L1: Artifact Layer  
- Meaning = material form subject to rules.  
- Files, schemas, documents, serialized structures.  
- Exists only as objects L0 rules can operate on.

L2: Interpretation Layer  
- Meaning = applying rules to artifacts.  
- Semantic evaluation, analysis, classification.  
- Exists only when someone performs interpretive work.

L3: Narrative Layer  
- Meaning = framing that guides interpretation.  
- Conceptual scaffolding, analogies, explanatory context.  
- Included only if it influences how L2 is performed.

L4: Meta‑Architectural Layer  
- Meaning = designing the rule‑system itself.  
- Defines categories, boundaries, evolution, and layer relations.  
- A statement belongs here only if it alters the structure of the system.