RFC‑010: AUI Component Model
Status: Draft
Author: Alfredo
Category: Component Semantics / Structural Contracts
Version: 0.2.0
Last Updated: 2026‑01‑22

1. Overview
The AUI Component Model defines the canonical structure, contracts, and semantic rules that govern how components are composed, instantiated, and validated within the AUI ecosystem. It establishes the structural grammar of the UI: what a component is, what it contains, what it exposes, and what it requires.
This subsystem must follow the Layout Engine because component‑level validation depends on a normalized spatial structure. The Layout Engine provides the resolved hierarchy, slot/part placements, and constraint‑resolved geometry that this subsystem uses to enforce component correctness. Without that normalized structure, component contracts cannot be deterministically validated.
The Component Model complements the Data Model & Binding Engine (RFC‑008) by defining the structural side of the same equation:
- RFC‑008 resolves data
- RFC‑009 resolves structure
- RFC‑010 resolves component semantics
Together, they form the compiler‑grade foundation of AUI.

2. Purpose
The Component Model provides:
- A canonical definition of what constitutes a component
- Structural contracts for parts, slots, children, and composition
- Rules for component instantiation and lifecycle
- Validation surfaces for component correctness
- A deterministic interface for downstream rendering semantics
This subsystem ensures that every component instance is structurally sound, semantically valid, and compatible with the surrounding UI.

3. Why This Subsystem Follows the Layout Engine
Component contracts cannot be validated in isolation.
They require:
- a resolved hierarchy
- concrete slot/part placements
- data‑driven branching already applied
- visibility and conditional structure already resolved
- normalized geometry and spatial relationships
All of these are produced by the Layout Engine.
The Component Model must therefore operate after spatial normalization, ensuring that:
- required parts are present
- optional parts are correctly placed
- slots contain valid children
- component boundaries are respected
- structural rules are enforced deterministically
This ordering guarantees that component validation is stable, analyzable, and free of runtime ambiguity.

4. Component Definition Specification
A component definition consists of:
- Metadata — name, version, category
- Parts — named structural regions
- Slots — extensible insertion points
- Props — typed inputs
- Constraints — structural and semantic rules
- Children Rules — allowed or forbidden child types
4.1 Example Component Definition
{
  "type": "AUI.Component",
  "name": "Card",
  "version": "1.0",
  "parts": {
    "header": { "required": false },
    "body":   { "required": true },
    "footer": { "required": false }
  },
  "slots": {
    "actions": { "allowedTypes": ["Button", "Link"] }
  },
  "props": {
    "title": { "type": "string", "required": false },
    "elevated": { "type": "boolean", "default": false }
  },
  "children": {
    "allowedTypes": ["CardSection", "CardMedia"]
  }
}



5. Component Instance Specification
A component instance is the concrete realization of a component definition within the Layout Tree.
It includes:
- resolved props (from RFC‑008)
- resolved parts and slots (from RFC‑009)
- validated children
- structural metadata
5.1 Instance Requirements
Each instance must satisfy:
- all required parts present
- all slot contents valid
- all props typed correctly
- all children allowed by the component definition
- all structural constraints satisfied

6. Structural Contracts
6.1 Parts
Parts are fixed structural regions.
Rules:
- Required parts must appear exactly once
- Optional parts may appear zero or one time
- Parts cannot be dynamically renamed or retyped
- Parts must appear in the positions defined by the Layout Tree
6.2 Slots
Slots are extensible insertion points.
Rules:
- Slot contents must be valid component types
- Slot ordering must follow layout semantics
- Slot contents must be structurally compatible with the parent component
- Slot contents must be validated after layout resolution
6.3 Children
Children rules define:
- allowed child types
- forbidden child types
- maximum or minimum child counts
- structural patterns (e.g., alternating types)

7. Validation Model
Component validation occurs after layout normalization and includes:
7.1 Part Validation
- required parts present
- no extraneous parts
- correct placement
7.2 Slot Validation
- allowed types only
- correct ordering
- correct cardinality
7.3 Prop Validation
- type correctness
- required props present
- default values applied
7.4 Structural Validation
- allowed children only
- no invalid nesting
- no structural cycles
All validation errors must be deterministic and schema‑encoded.

8. Error Model
8.1 Error Shape
{
  "type": "AUI.ComponentError",
  "component": "Card",
  "instanceId": "card_42",
  "stage": "slotValidation",
  "message": "Invalid child type in 'actions' slot",
  "path": "Card.actions[0]"
}


8.2 Error Categories
- MissingPart
- InvalidPart
- InvalidSlotContent
- InvalidChildType
- PropTypeMismatch
- MissingRequiredProp
- StructuralViolation

9. Integration Points
9.1 With RFC‑008 (Data Model & Binding Engine)
The Component Model consumes:
- resolved props
- validated data
- deterministic hydration payloads
Props must be fully resolved before component validation.
9.2 With RFC‑009 (Layout Engine)
The Component Model consumes:
- normalized layout hierarchy
- resolved slot/part placements
- structural metadata
- constraint‑resolved geometry
Component validation depends on this normalized structure.
9.3 With Downstream Rendering Semantics
The Component Model provides:
- validated component instances
- structural metadata
- part/slot mappings
- component‑level semantics
This enables renderer‑agnostic rendering.

10. Determinism Requirements
The Component Model must be:
- deterministic
- pure
- reproducible
- statically analyzable
- validator‑enforced
No dynamic mutation or runtime‑dependent behavior is allowed.

11. Versioning
Semantic versioning applies:
- MAJOR: breaking changes to component contract semantics
- MINOR: new part/slot types, new constraints
- PATCH: clarifications or non‑breaking fixes

12. Changelog
- 0.2.0 — Added explicit architectural rationale for ordering relative to RFC‑008 and RFC‑009
- 0.1.0 — Initial draft
