Appendix C — AUI Validator Algorithm (Canonical)
C.1 Purpose {#c1}
The AUI [Validator](../../glossary/aui-glossary.md#validator) is the authoritative mechanism that ensures an [AUI document](../../glossary/aui-glossary.md#aui-document) is:
- structurally valid
- semantically correct
- contract‑compliant
- layout‑sound
- [renderer](../../glossary/aui-glossary.md#renderer)‑agnostic
It is the only component in the AUI pipeline allowed to reject an artifact.
It MUST produce [deterministic](../../glossary/aui-glossary.md#determinism) results for identical input.

C.2 Validation Pipeline Overview {#c2}
A compliant validator MUST execute the following phases in order:
- Parse & Normalize
- [Structural Validation](../../glossary/aui-glossary.md#structural-validation)
- Semantic Validation
- Component Contract Validation
- Layout Validation
- Cross‑Node Invariant Validation
- Finalization & Normalization
Each phase MUST be pure, deterministic, and side‑effect‑free.

C.3 Phase 1 — Parse & Normalize {#c3}
C.3.1 Input Requirements {#c31}
- UTF‑8 JSON
- No comments
- Deterministic key ordering (recommended but not required)
C.3.2 Normalization Steps {#c32}
- Insert missing optional fields with defaults
- Normalize empty children to []
- Normalize empty parts to {}
- Normalize empty props to {}
Normalization MUST NOT change semantic meaning.

C.4 Phase 2 — Structural Validation {#c4}
The validator MUST ensure:
C.4.1 Tree Shape {#c41}
- Document forms a [tree](../../glossary/aui-glossary.md#tree), not a graph
- No cycles
- No self‑references
C.4.2 [Node](../../glossary/aui-glossary.md#node) Requirements {#c42}
Each node MUST contain:
- kind (string)
- zero or one of: children, parts
- optional: props, metadata
C.4.3 Forbidden Structures {#c43}
- children and parts coexisting unless explicitly allowed
- non‑object props
- non‑object parts
- non‑array children
C.4.4 ID Uniqueness (if IDs exist) {#c44}
- IDs MUST be unique across the entire document
- IDs MUST match ^[a-zA-Z_][a-zA-Z0-9_]*$ ([identifier](../../glossary/aui-glossary.md#identifier) pattern)

C.5 Phase 3 — Semantic Validation {#c5}
Semantic validation ensures each node’s meaning is well‑formed.
C.5.1 Kind Validation {#c51}
- [kind](../../glossary/aui-glossary.md#kind) MUST be a known [semantic](../../glossary/aui-glossary.md#semantic-family) kind
- unknown kinds MUST be namespaced (e.g., x-foo)
C.5.2 Props Validation {#c52}
- props MUST be pure data
- props MUST match the semantic expectations of the kind
- props MUST NOT contain renderer‑specific keys
C.5.3 Parts Validation {#c53}
- part names MUST match the component’s declared contract
- missing required parts MUST be errors
- extra parts MUST be errors
C.5.4 Children Validation {#c54}
- children MUST be ordered
- children MUST be valid nodes
- children MUST satisfy kind‑specific rules (e.g., text cannot have children)

C.6 Phase 4 — Component Contract Validation {#c6}
Component contracts define the semantic API of a component.
C.6.1 Contract Lookup {#c61}
For each node with a declared contract:
- load contract definition
- validate props against contract types
- validate required parts
- validate forbidden parts
C.6.2 Contract Enforcement Rules {#c62}
- missing required props → error
- wrong prop type → error
- missing required parts → error
- extra parts → error
- invalid part kind → error
C.6.3 Contract Invariants {#c63}
Examples:
- button MUST have label or icon
- list MUST have item part
- card MUST have header and body
These invariants MUST be enforced here.

C.7 Phase 5 — Layout Validation {#c7}
Layout semantics MUST be validated independently of rendering.
C.7.1 [Flow Layout](../../glossary/aui-glossary.md#flow-layout) Rules {#c71}
- direction MUST be "horizontal" or "vertical"
- gap MUST be a number or CSS length string
- layout MUST NOT encode styling
C.7.2 [Constraint Layout](../../glossary/aui-glossary.md#constraint-layout) Rules {#c72}
- align MUST be one of: start, center, end
- stretch MUST be boolean
C.7.3 Layout Exclusivity {#c73}
A node MUST NOT declare:
- both flow and constraint layout
- layout props on non‑layout kinds

C.8 Phase 6 — Cross‑Node Invariant Validation {#c8}
These invariants require global knowledge of the document.
C.8.1 [Binding](../../glossary/aui-glossary.md#binding) Resolution {#c81}
- all bindings MUST reference valid paths
- no unresolved bindings
- no circular bindings
C.8.2 [Action](../../glossary/aui-glossary.md#action) Resolution {#c82}
- all actions MUST be declared
- no unknown actions
- no dynamic evaluation
C.8.3 Part/Child Exclusivity {#c83}
- nodes MUST NOT define both parts and children unless contract allows it
C.8.4 [Semantic Family](../../glossary/aui-glossary.md#semantic-family) Rules {#c84}
Examples:
- text nodes MUST NOT have [children](../../glossary/aui-glossary.md#children)
- image nodes MUST have src
- list nodes MUST have repeat semantics or item parts

C.9 Phase 7 — Finalization & Normalization {#c9}
After all validations pass:
C.9.1 Normalization {#c91}
- remove unused metadata
- canonicalize ordering
- canonicalize empty structures
C.9.2 Output {#c92}
The validator MUST output:
- artifact.validated.aui ([Validated Artifact](../../glossary/aui-glossary.md#validated-artifact))
- structured list of warnings (optional)
C.9.3 [Determinism](../../glossary/aui-glossary.md#determinism) {#c93}
The validator MUST guarantee:
- same input → same output
- same errors → same order
- same normalization → same structure

C.10 Error Model {#c10}
C.10.1 Error Format {#c101}
{
  "code": "AUI_INVALID_PROP_TYPE",
  "message": "Expected string for prop 'label'",
  "path": "$.root.parts.header"
}


C.10.2 Error Requirements {#c102}
Errors MUST be:
- deterministic
- stable across versions
- machine‑readable
- human‑readable
C.10.3 Error Categories {#c103}
- structural
- semantic
- contract
- layout
- invariant

C.11 Compliance Requirements {#c11}
A validator is compliant if it:
- implements all phases
- enforces all invariants
- rejects invalid AUI
- never mutates semantic meaning
- produces deterministic output
Compliance MUST be testable via:
- golden tests
- structural diffs
- invariant suites
