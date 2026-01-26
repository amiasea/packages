RFC‑006: AUI Component Contract Specification
Status: Draft
Author: Alfredo
Category: Semantics / Contracts
Version: 1.0

1. Overview
This RFC defines the AUI Component Contract, a formal, declarative specification that describes the semantic API of a component.
A Component Contract defines:
- required props
- optional props
- required parts
- optional parts
- semantic invariants
- layout constraints
- binding constraints
- versioning rules
Component Contracts are a novel category in the AUI ecosystem:
They are not templates, not schemas, not renderer components, and not code.
They are semantic declarations that define what a component means, not how it is rendered.

This RFC defines the structure, semantics, validation rules, and cross‑subsystem boundaries for Component Contracts.

2. Goals
Component Contracts MUST:
- define the semantic API of a component
- be renderer‑agnostic
- be validator‑enforceable
- be deterministic
- be versioned
- be declarative
Component Contracts MUST NOT:
- contain executable code
- contain renderer‑specific props
- contain layout inference
- contain styling
- contain platform‑specific constructs

3. Relationship to Other RFCs
|  |  | 
|  |  | 
|  |  | 
|  |  | 
|  |  | 


Component Contracts are referenced by all other subsystems but defined only here.

4. Contract Structure
A Component Contract is a JSON object with the following structure:
{
  "kind": "card",
  "version": "1.0",
  "props": {
    "title": { "type": "string", "required": false },
    "action": { "type": "action", "required": false }
  },
  "parts": {
    "header": { "required": true },
    "body":   { "required": true },
    "footer": { "required": false }
  },
  "invariants": [
    "header.kind == 'text'",
    "body.kind != 'image'"
  ],
  "layout": {
    "allowed": ["flow", "constraint"],
    "default": "flow"
  }
}



5. Contract Fields
5.1 kind
The semantic identifier of the component.
- MUST match AUI identifier grammar (RFC‑002, Appendix D)
- MUST be unique within the contract registry
5.2 version
Semantic version of the contract.
- MAJOR: breaking changes
- MINOR: new props or parts
- PATCH: clarifications
5.3 props
Defines the semantic properties of the component.
Each prop MUST define:
- type
- required (boolean)
Prop types MUST be:
- primitive (string, number, boolean)
- semantic (binding, action)
- structured (object, array)
5.4 parts
Defines named semantic slots.
Each part MUST define:
- required (boolean)
Parts MUST NOT:
- define renderer‑specific semantics
- define layout semantics
5.5 invariants
Declarative semantic rules.
Examples:
- "text nodes MUST NOT have children"
- "button MUST have label or icon"
Invariants MUST be:
- pure
- deterministic
- validator‑enforceable
5.6 layout
Defines layout constraints.
- allowed: list of allowed layout modes
- default: default layout mode
Layout constraints MUST NOT:
- encode styling
- encode platform behavior

6. Contract Registry
Component Contracts MUST be stored in a Contract Registry, a deterministic, versioned collection of contract definitions.
Registry responsibilities:
- store contract definitions
- enforce version uniqueness
- expose lookup by kind
- expose lookup by version
Registry MUST NOT:
- infer contracts
- rewrite contracts
- merge contracts

7. Validation Rules (Cross‑Reference: RFC‑003)
The validator MUST enforce:
- required props
- required parts
- forbidden parts
- prop type correctness
- invariant correctness
- layout constraints
The validator MUST NOT:
- infer missing props
- infer missing parts
- infer layout

8. Renderer Mapping Rules (Cross‑Reference: RFC‑004)
Renderer Adapters MUST:
- map props according to contract semantics
- map parts according to contract semantics
- preserve invariant meaning
- preserve layout constraints
Renderer Adapters MUST NOT:
- reinterpret contract semantics
- override contract defaults
- drop contract fields

9. Toolchain Boundaries (Cross‑Reference: RFC‑005)
9.1 Intent Engine
- MAY reference contracts
- MUST NOT enforce contracts
9.2 Semantic Validator
- MUST enforce contracts
- MUST NOT modify contracts
9.3 Renderer Adapter
- MUST respect contracts
- MUST NOT reinterpret contracts
9.4 Host Integrator
- MUST NOT reinterpret contract semantics

10. Determinism Requirements
Component Contracts MUST guarantee:
- stable meaning across versions
- stable validation behavior
- stable renderer mapping
Contracts MUST NOT:
- introduce nondeterminism
- depend on runtime state

11. Extensibility
Component Contracts MAY support:
- namespaced kinds
- custom invariant languages
- custom prop types
Extensions MUST NOT:
- weaken invariants
- override core semantics
- introduce platform semantics

12. Compliance
A Component Contract is compliant if:
- it follows the structure defined in this RFC
- it is versioned
- it is deterministic
- it is validator‑enforceable
- it is renderer‑safe
A Contract Registry is compliant if:
- it stores only compliant contracts
- it enforces version uniqueness
- it exposes deterministic lookup
