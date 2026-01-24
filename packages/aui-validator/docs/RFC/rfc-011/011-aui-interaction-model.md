RFC‑011: AUI Interaction Model
Status: Draft
Author: Alfredo
Category: Interaction / Behavioral Semantics
Version: 0.6.0
Last Updated: 2026‑01‑22

1. Overview
In AUI, structure alone does not define a UI.
Behavior is identity.
A component without behavior is inert — a shell.
A system without a formal interaction model is unstable — nondeterministic, unvalidated, unpredictable.
The AUI Interaction Model establishes the canonical, declarative representation of user‑driven behavior.
It defines how events are described, how interactive affordances attach to components, how state transitions occur, and how interaction flows are validated.
Without this subsystem, behavior becomes:
- Ad‑hoc — scattered across imperative code
- Platform‑specific — tied to a particular runtime
- Hard to analyze — no stable schema for tools
- Non‑deterministic — behavior varies across environments
The Interaction Model prevents this drift.
It introduces a compiler‑grade, analyzable, deterministic layer for interaction semantics.
Like a TRON program verifying identity, the Interaction Model demands:
“Declare your event.
Declare your guard.
Declare your action.
Only then may you execute.”
This subsystem follows the Component Model because behavior cannot be interpreted until the UI’s structure, parts, slots, and contracts are validated.

2. Why This RFC Exists
This RFC answers a foundational architectural question:
How do we represent user interactions in a way that is deterministic, declarative, analyzable, and renderer‑agnostic?
Most UI frameworks eventually invent an interaction layer — informally, inconsistently, or buried inside imperative code.
AUI instead elevates interaction to a first‑class declarative artifact.
This RFC exists to:
- define a stable schema for interactions
- ensure interactions can be validated before runtime
- guarantee deterministic behavior across platforms
- separate behavior from rendering
- enable tooling, validators, and compilers to reason about behavior
Behavior is not an afterthought.
Behavior is semantics.
Behavior is identity.

3. Architectural Positioning
The Interaction Model sits at a precise point in the AUI pipeline:
- RFC‑008 — Data Model resolves all data.
- RFC‑009 — Layout Engine resolves spatial structure.
- RFC‑010 — Component Model validates component semantics.
- RFC‑011 — Interaction Model defines behavior on top of validated structure.
- RFC‑012 — Rendering Semantics consumes all upstream outputs.
Interaction must follow component validation because:
- events target components
- guards reference validated data
- actions depend on component identity and structure
- flows must be checked against component contracts
This ordering ensures behavior is grounded in a stable, deterministic UI.

4. Interaction Definition Specification
An interaction definition consists of:
- Events — user‑initiated triggers
- Targets — the component instance or part receiving the event
- Guards — declarative conditions controlling applicability
- Actions — deterministic state transitions or commands
- Effects — optional declarative annotations
This specification defines the shape of interaction definitions.
It does not prescribe a single canonical structure.
Multiple valid forms exist.
Appendixes A–E provide illustrative examples, not schemas.

5. Event Model
Events represent user‑initiated interactions — the moment a program declares its intent.
5.1 Example Interaction Definition (Illustrative Only)
This example is Non‑Normative.
It illustrates one possible shape.
It does not constrain the allowed structure.
{
  "type": "AUI.Interaction",
  "event": "onClick",
  "target": "Button.primary",
  "guard": "items.length > 0",
  "action": {
    "type": "navigate",
    "to": "/details"
  }
}


Appendix A contains additional examples.
5.2 Example Event Type Set (Illustrative Only)
This list is not a registry.
It exists solely to give intuition.
- onClick
- onPress
- onHover
- onFocus
- onBlur
- onSubmit
- onChange
- onKeyDown
- onKeyUp
Appendix B expands on event examples.
5.3 Event Requirements
Events must:
- reference a valid component instance
- reference a valid part or slot (if applicable)
- be statically analyzable
- be validated against component contracts

6. Guard Semantics
Guards are declarative boolean expressions determining whether an event may fire.
Rules:
- Purity — must be pure
- Data source — must reference resolved data (RFC‑008)
- No mutation — must not mutate state
- Static validation — must be statically analyzable
Example:
"guard": "user.isLoggedIn == true"


Appendix D provides illustrative guard patterns.

7. Action Semantics
Actions define what happens when an event fires — the deterministic response of the system.
7.1 Action Types
- State transitions
- Navigation
- Command dispatch
- Focus management
7.2 Action Requirements
Actions must:
- be deterministic
- be statically analyzable
- reference valid component instances
- not introduce side effects outside the declarative model
Appendix E provides illustrative action composition patterns.

8. Effects
Effects are optional declarative annotations describing expected side effects.
Examples:
- analytics events
- logging
- accessibility announcements
Effects are not executed by the Interaction Model.
They are consumed by downstream systems.
Appendix C includes examples of flows that incorporate effects.

9. Interaction Validation
Validation ensures:
- event targets exist
- guards reference valid data
- actions reference valid components
- actions do not violate component contracts
- no circular interaction chains
- no unreachable interactions
Validation occurs after component validation and before rendering semantics.

10. Error Model
10.1 Error Shape
{
  "type": "AUI.InteractionError",
  "event": "onClick",
  "target": "Button.primary",
  "stage": "guardValidation",
  "message": "Guard references undefined field 'inventoryCount'",
  "path": "Interactions[3]"
}


10.2 Error Categories
- InvalidEventTarget
- InvalidGuardExpression
- InvalidActionReference
- StructuralViolation
- CircularInteraction
- UnreachableInteraction

11. Integration Points
11.1 With RFC‑008 (Data Model)
Consumes:
- resolved data fields
- validated bindings
- deterministic values for guard evaluation
11.2 With RFC‑009 (Layout Engine)
Consumes:
- resolved spatial structure
- part/slot placements
- component visibility and branching
11.3 With RFC‑010 (Component Model)
Consumes:
- validated component instances
- structural metadata
- component contracts
11.4 With RFC‑012 (Rendering Semantics)
Provides:
- validated interaction definitions
- event‑to‑component mappings
- state transition semantics
- declarative effects

12. Determinism Requirements
The Interaction Model must be:
- deterministic
- pure
- reproducible
- statically analyzable
- validator‑enforced
Like a TRON program verifying identity, the model rejects ambiguity.

13. Versioning
Semantic versioning applies:
- MAJOR — breaking changes
- MINOR — new event types, action types, guard capabilities
- PATCH — clarifications or non‑breaking fixes

14. Changelog
- 0.1.0 — Initial draft