RFC‑009: AUI Layout Engine
Status: Draft
Author: Alfredo
Category: Layout / Spatial Semantics
Version: 0.2.0
Last Updated: 2026‑01‑22

1. Overview
The AUI Layout Engine defines the subsystem responsible for transforming resolved semantic data into deterministic spatial structure. It interprets component intent, applies layout semantics, enforces structural constraints, and produces a normalized layout tree suitable for downstream rendering and component‑level contract enforcement.
This subsystem must follow the Data Model & Binding Engine ([RFC‑008](../rfc-008/008-aui-data-modeling-and-binding-engine.md)) because layout decisions depend on fully resolved, validated, and typed data. Without the canonical data produced by [RFC‑008](../rfc-008/008-aui-data-modeling-and-binding-engine.md), no spatial interpretation can be deterministic, analyzable, or validator‑safe.
The Layout Engine also produces structural outputs that are required by the subsystem responsible for enforcing component‑level contracts. That subsystem relies on the Layout Engine’s normalized spatial structure to validate component composition, slot/part satisfaction, and structural integrity.

2. Purpose
The Layout Engine provides:
- A deterministic interpretation of component intent
- A canonical spatial structure (Layout Tree)
- Constraint resolution and flow semantics
- Slot/part placement resolution
- Structural validation surfaces
- A normalized output consumed by downstream component‑contract logic
It acts as the bridge between:
- Data‑level semantics ([RFC‑008](../rfc-008/008-aui-data-modeling-and-binding-engine.md))
- Component‑level semantics ([RFC‑010](../rfc-010/010-aui-component-model.md))

3. Why This Subsystem Follows [RFC‑008](../rfc-008/008-aui-data-modeling-and-binding-engine.md)
The Layout Engine cannot operate on raw or unresolved data.
It requires:
- Fully typed ADM fields
- Resolved bindings
- Completed transform pipelines
- Deterministic hydration payloads
These are all guarantees provided exclusively by [RFC‑008](../rfc-008/008-aui-data-modeling-and-binding-engine.md).
Because layout decisions often depend on:
- conditional visibility
- dynamic lists
- computed values
- resolved slot/part bindings
- data‑driven structural branching
…the Layout Engine must receive stable, validated, final data before it can construct a spatial hierarchy.
[RFC‑008](../rfc-008/008-aui-data-modeling-and-binding-engine.md) ensures that all data entering this subsystem is:
- canonical
- validator‑checked
- deterministic
- renderer‑agnostic
- free of runtime ambiguity
This ordering is essential for compiler‑grade determinism.

4. Inputs
The Layout Engine consumes:
- Resolved ADM payloads from [RFC‑008](../rfc-008/008-aui-data-modeling-and-binding-engine.md)
- Component definitions (schema‑level structure, slot/part definitions, allowed children)
- Layout rule packs (domain‑specific layout semantics)
- Platform constraints from renderer adapters
All data entering this subsystem must already be validated and transformed.

5. Outputs
The Layout Engine produces:
- Layout Tree — canonical spatial hierarchy
- Resolved slot/part placements
- Constraint‑resolved geometry
- Normalized structural metadata
This output is required by the subsystem responsible for enforcing component‑level contracts. That subsystem cannot validate component composition or structural correctness without the Layout Engine’s normalized spatial representation.

6. Layout Tree Specification
The Layout Tree is a deterministic, hierarchical structure composed of:
- Nodes — components, containers, primitives
- Edges — parent/child relationships
- Constraints — size, alignment, spacing, flow rules
- Geometry — resolved spatial metrics
6.1 Node Requirements
Each node must satisfy:
- schema‑defined structural rules
- slot/part placement rules
- layout rule pack constraints
- data‑driven visibility and branching semantics (from [RFC‑008](../rfc-008/008-aui-data-modeling-and-binding-engine.md))
6.2 Determinism
Given identical inputs, the Layout Engine must produce identical Layout Trees.
No randomness, heuristics, or platform‑specific behavior is permitted.

7. Layout Semantics
The Layout Engine supports three semantic categories:
7.1 Flow Layout
- vertical
- horizontal
- grid
- wrapping
- responsive breakpoints
7.2 Constraint Layout
- pinning
- anchoring
- proportional sizing
- min/max rules
7.3 Region Layout
- header/body/footer
- sidebar/main
- overlays
- modal regions

8. Rule Packs
Rule Packs define domain‑specific layout behavior.
Examples:
- Standard Web Layout Rules
- Mobile‑First Responsive Rules
- Desktop Application Layout Rules
Rule Packs are:
- versioned
- composable
- validator‑checked
- platform‑agnostic

9. Error Model
The Layout Engine emits structured, deterministic errors:
9.1 Structural Errors
- invalid hierarchy
- missing required slots
- incompatible nesting
9.2 Constraint Errors
- unsatisfiable constraints
- circular dependencies
9.3 Semantic Errors
- data‑driven branching incompatible with component structure
- rule pack violations
Errors must be schema‑encoded and reproducible.

10. Integration Points
10.1 With [RFC‑008](../rfc-008/008-aui-data-modeling-and-binding-engine.md) (Data Model & Binding Engine)
The Layout Engine depends on [RFC‑008](../rfc-008/008-aui-data-modeling-and-binding-engine.md) for:
- resolved ADM fields
- validated bindings
- completed transform pipelines
- deterministic hydration payloads
Without these guarantees, layout cannot be stable or analyzable.
10.2 With [RFC‑010](../rfc-010/010-aui-component-model.md) (Component‑Level Semantics)
The Layout Engine provides:
- normalized spatial structure
- resolved slot/part placements
- structural metadata
- constraint‑resolved geometry
These outputs are required for the subsystem that enforces component‑level contracts ([RFC‑010](../rfc-010/010-aui-component-model.md)). That subsystem cannot validate component composition or structural correctness without the Layout Engine’s output.

11. Determinism Requirements
The Layout Engine must be:
- deterministic
- pure
- reproducible
- statically analyzable
- validator‑enforced
No side effects or runtime‑dependent behavior is allowed.

12. Versioning
Semantic versioning applies:
- MAJOR: breaking changes to layout semantics or tree structure
- MINOR: new layout modes, rule packs, or constraint types
- PATCH: clarifications or non‑breaking fixes

13. Changelog
- 0.2.0 — Added explicit architectural rationale for ordering relative to [RFC‑008](../rfc-008/008-aui-data-modeling-and-binding-engine.md) and downstream subsystems
- 0.1.0 — Initial draft
