RFC‑007: AUI Semantic Event Model Specification
Status: Draft
Author: Alfredo
Category: Semantics / Interaction
Version: 1.0

1. Overview
This RFC defines the AUI Semantic Event Model, a novel subsystem that formalizes how user interactions are represented in AUI.
AUI events are:
- symbolic
- declarative
- renderer‑agnostic
- platform‑agnostic
- validator‑enforceable
- contract‑driven
- deterministic
They are not executable code, callbacks, closures, or platform event handlers.
This RFC defines:
- event identifiers
- event payload semantics
- event binding rules
- event propagation semantics
- event validation rules
- renderer adapter responsibilities
- host integrator responsibilities
- cross‑subsystem boundaries
This subsystem is invented specifically for AUI Studio.

2. Goals
The AUI Semantic Event Model MUST:
- define a declarative representation of user interactions
- be fully renderer‑agnostic
- be fully platform‑agnostic
- be validator‑enforceable
- be deterministic
- be safe for AI generation
- be compatible with Component Contracts (RFC‑006)
The model MUST NOT:
- embed executable code
- embed platform event handlers
- embed expressions
- embed dynamic evaluation
- depend on runtime state

3. Relationship to Other RFCs
RFC‑007 defines the semantic interaction layer that other subsystems depend on or must preserve.

**Integration summary:**
- **[RFC‑002](../rfc-002/002-aui-language-specification.md) (AUI Schema):** Event names must be valid identifiers and appear on schema‑defined component surfaces.
- **[RFC‑003](../rfc-003/003-aui-validator-specification.md) (AUI Validator):** Validator enforces event name validity, payload constraints, and contract alignment.
- **[RFC‑004](../rcf-004/004-renderer-adapter-specification.md) (Renderer Adapters):** Renderer adapters map semantic events to platform hooks without altering meaning.
- **[RFC‑005](../rfc-005/005-aui-studio-toolchain-specification.md) (AUI Studio Toolchain):** Toolchain must preserve event semantics through generation and transforms.
- **[RFC‑006](../rfc-006/006-aui-component-contract-specification.md) (Component Contracts):** Contracts declare allowed/required events and invariants.
- **[RFC‑008](../rfc-008/008-aui-data-modeling-and-binding-engine.md) (Data Model & Binding):** Event payloads may reference or carry bound data, but binding resolution is performed by RFC‑008 prior to event hydration.

RFC‑007 defines the semantics that all other RFCs must preserve when representing user interactions.

4. Event Structure
An AUI event is defined as:
"events": {
  "<eventName>": "<actionIdentifier>"
}


Where:
- eventName is a semantic event (e.g., "press", "submit", "change")
- actionIdentifier is a symbolic reference to an action defined in the host environment
4.1 Event Names
Event names MUST:
- be semantic, not platform‑specific
- match identifier grammar (RFC‑002 Appendix D)
- be defined by the component contract (RFC‑006)
Examples:
- "press"
- "submit"
- "change"
- "focus"
- "blur"
4.2 Action Identifiers
Action identifiers MUST:
- match identifier grammar
- be symbolic
- be non‑executable
- be resolved by the Host Integrator
Examples:
- "submitForm"
- "toggleMenu"
- "incrementCounter"

5. Event Semantics
AUI events are semantic triggers, not executable functions.
5.1 Event Meaning
An event represents:
“When this semantic interaction occurs, emit this symbolic action.”

5.2 Event Payload
Events MAY include a payload, but payloads MUST be:
- pure data
- JSON‑serializable
- platform‑agnostic
Example:
"events": {
  "submit": {
    "action": "submitForm",
    "payload": { "source": "login" }
  }
}


5.3 Event Propagation
AUI defines semantic propagation, not DOM‑style bubbling.
Propagation rules:
- events propagate up the AUI tree
- propagation stops when a component contract declares stopPropagation: true
- propagation is semantic, not platform‑specific

6. Validation Rules (Cross‑Reference: RFC‑003)
The validator MUST enforce:
- event names MUST be valid identifiers
- event names MUST be allowed by the component contract
- action identifiers MUST be valid identifiers
- payload MUST be JSON‑serializable
- no executable code
- no expressions
- no platform‑specific constructs
The validator MUST NOT:
- resolve actions
- interpret event meaning
- infer missing events

7. Renderer Adapter Rules (Cross‑Reference: RFC‑004)
Renderer Adapters MUST:
- map semantic events → platform event hooks
- preserve event names
- preserve action identifiers
- preserve payloads
- attach symbolic references only
Renderer Adapters MUST NOT:
- execute actions
- embed code
- transform action identifiers
- infer event semantics

8. Host Integrator Rules (Cross‑Reference: RFC‑005)
The Host Integrator MUST:
- resolve action identifiers to platform logic
- attach platform event handlers
- pass payloads to the action handler
- preserve semantic meaning
The Host Integrator MUST NOT:
- reinterpret event semantics
- mutate event payloads
- introduce nondeterminism

9. Component Contract Integration (Cross‑Reference: RFC‑006)
Component Contracts MAY define:
- allowed events
- required events
- forbidden events
- event invariants
Example:
"events": {
  "press": { "required": true },
  "submit": { "required": false }
}


The validator MUST enforce these rules.

10. Determinism Requirements
The event model MUST guarantee:
- same input → same output
- stable event ordering
- stable payload structure
- stable propagation semantics
Events MUST NOT:
- depend on runtime state
- depend on platform behavior
- depend on renderer behavior

11. Extensibility
The event model MAY support:
- namespaced event names
- custom payload schemas
- custom event families
Extensions MUST NOT:
- override core semantics
- weaken invariants
- introduce platform semantics

12. Compliance
A system is compliant if:
- it implements the event model as defined
- it preserves semantics across all subsystems
- it enforces validation rules
- it maps events deterministically
- it resolves actions correctly
Compliance MUST be testable via:
- golden event snapshots
- propagation tests
- contract conformance tests
