RFC‑003: AUI Validator Specification
Status: Draft
Author: Alfredo
Category: Validation / Semantics
Version: 1.0

1. Overview
The AUI Validator is the authoritative subsystem responsible for ensuring that all AUI documents are:
- structurally valid
- semantically correct
- contract‑compliant
- layout‑sound
- deterministic
- renderer‑safe
The validator is the only component in the AUI pipeline allowed to reject an artifact.
It is the gatekeeper between:
- artifact.raw.aui (unvalidated)
and
- artifact.validated.aui (semantically correct)
This RFC defines the validator’s architecture, rule engine, invariants, error model, and extension points.

2. Goals
The validator MUST:
- enforce the AUI DSL grammar (Appendix D)
- enforce semantic rules (Appendix A)
- enforce component contracts
- enforce layout semantics
- enforce cross‑node invariants
- normalize structure without altering meaning
- produce deterministic output
The validator MUST NOT:
- infer missing semantics
- rewrite meaning
- introduce renderer‑specific constructs
- execute bindings or actions
- perform layout computation
- perform rendering

3. Architecture
The validator is composed of five major subsystems:
- Parser
- Normalizer
- Rule Engine
- Invariant Engine
- Error Reporter
3.1 Parser
Responsible for:
- parsing UTF‑8 JSON
- verifying top‑level structure
- rejecting malformed documents
3.2 Normalizer
Responsible for:
- inserting missing optional fields
- canonicalizing empty structures
- stabilizing key ordering (optional)
- preparing the document for rule evaluation
Normalization MUST NOT change semantics.
3.3 Rule Engine
Executes:
- structural rules
- semantic rules
- contract rules
- layout rules
Rules MUST be:
- pure
- deterministic
- stateless
- order‑independent
3.4 Invariant Engine
Executes cross‑node invariants requiring global knowledge:
- binding resolution
- action resolution
- cycle detection
- part/child exclusivity
- semantic family constraints
3.5 Error Reporter
Produces:
- structured errors
- stable error codes
- JSONPath pointers
- deterministic ordering

4. Validation Pipeline
The validator MUST execute the following phases in order:
- Parse & Normalize
- Structural Validation
- Semantic Validation
- Component Contract Validation
- Layout Validation
- Cross‑Node Invariant Validation
- Finalization & Normalization
This pipeline is normative and MUST NOT be reordered.

5. Structural Validation
Structural validation ensures the document is a well‑formed AUI tree.
5.1 Tree Shape
- MUST be a tree
- MUST NOT contain cycles
- MUST NOT contain self‑references
5.2 Node Shape
Each node MUST contain:
- kind (string)
- zero or one of: children, parts
- optional: props, metadata
5.3 Exclusivity Rules
- children and parts MUST NOT coexist unless contract allows
5.4 ID Rules
If IDs exist:
- MUST be unique
- MUST match identifier grammar

6. Semantic Validation
Semantic validation ensures each node’s meaning is well‑formed.
6.1 Kind Validation
- MUST be a known kind or namespaced kind
6.2 Props Validation
- MUST be pure data
- MUST match semantic expectations
- MUST NOT contain renderer‑specific fields
6.3 Parts Validation
- part names MUST match contract
- required parts MUST exist
- forbidden parts MUST NOT exist
6.4 Children Validation
- children MUST be valid nodes
- children MUST satisfy kind‑specific rules

7. Component Contract Validation
Contracts define the semantic API of a component.
7.1 Prop Rules
- required props MUST exist
- prop types MUST match contract
7.2 Part Rules
- required parts MUST exist
- extra parts MUST NOT exist
- part kinds MUST match contract
7.3 Semantic Invariants
Examples:
- button MUST have label or icon
- text MUST NOT have children
- image MUST have src

8. Layout Validation
Layout semantics MUST be validated independently of rendering.
8.1 Flow Layout
- direction MUST be valid
- gap MUST be valid
8.2 Constraint Layout
- align MUST be valid
- stretch MUST be boolean
8.3 Exclusivity
- MUST NOT mix flow and constraint layout

9. Cross‑Node Invariant Validation
These invariants require global knowledge.
9.1 Binding Resolution
- all bindings MUST resolve
- no circular bindings
9.2 Action Resolution
- all actions MUST be declared
9.3 Semantic Family Rules
- text nodes MUST NOT have children
- list nodes MUST have repeat semantics or item parts
9.4 Part/Child Exclusivity
- MUST NOT define both unless contract allows

10. Normalization
After validation:
- remove unused metadata
- canonicalize ordering
- canonicalize empty structures
Normalization MUST NOT change semantics.

11. Error Model
Errors MUST be:
- deterministic
- stable
- machine‑readable
- human‑readable
11.1 Error Format
{
  "code": "AUI_INVALID_PROP_TYPE",
  "message": "Expected string for prop 'label'",
  "path": "$.root.parts.header"
}


11.2 Error Categories
- structural
- semantic
- contract
- layout
- invariant

12. Determinism Requirements
The validator MUST guarantee:
- same input → same output
- same errors → same order
- same normalization → same structure

13. Extensibility
The validator MAY support:
- namespaced kinds
- custom rule packs
- custom contract registries
Extensions MUST NOT:
- override core semantics
- weaken invariants
- introduce nondeterminism

14. Versioning
The validator MUST enforce:
- AUI version compatibility
- contract version compatibility
Version mismatches MUST cause rejection.

15. Compliance
A validator is compliant if it:
- implements all phases
- enforces all invariants
- rejects invalid AUI
- preserves semantics
- produces deterministic output
Compliance MUST be testable via:
- golden tests
- structural diffs
- invariant suites
