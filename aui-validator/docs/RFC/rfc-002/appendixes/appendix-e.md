Appendix E — Renderer/Validator Interop Contract (Canonical)
E.1 Purpose {#e1}
Appendix E defines the formal interface between:
- the AUI [Validator](../../glossary/aui-glossary.md#validator) (Appendix C)
- the AUI [Renderer](../../glossary/aui-glossary.md#renderer) (Appendix B)
This contract ensures that:
- [validated](../../glossary/aui-glossary.md#validated-artifact) AUI artifacts are safe for rendering
- renderers can rely on validator guarantees
- validators do not assume renderer behavior
- both systems remain independently evolvable
- the pipeline is [deterministic](../../glossary/aui-glossary.md#determinism) end‑to‑end
This appendix is normative.

E.2 Interop Overview {#e2}
The validator and renderer communicate through a single artifact:
artifact.validated.aui ([Validated Artifact](../../glossary/aui-glossary.md#validated-artifact))


This artifact is:
- fully normalized
- semantically correct
- contract‑compliant
- layout‑sound
- [renderer](../../glossary/aui-glossary.md#renderer-adapter)‑agnostic
- free of [template constructs](../../glossary/aui-glossary.md#template-constructs)
- free of unresolved bindings
- free of renderer‑specific fields
The renderer MUST accept this artifact without additional validation beyond structural sanity checks.

E.3 Validator → Renderer Guarantees {#e3}
The validator MUST guarantee the following properties to the renderer.
E.3.1 Structural Guarantees {#e31}
- The document is a valid [tree](../../glossary/aui-glossary.md#tree).
- No cycles exist.
- No invalid [node](../../glossary/aui-glossary.md#node) shapes exist.
- [kind](../../glossary/aui-glossary.md#kind), [props](../../glossary/aui-glossary.md#props), [parts](../../glossary/aui-glossary.md#parts), [children](../../glossary/aui-glossary.md#children) follow [grammar](../../glossary/aui-glossary.md#grammar) rules.
- parts and children exclusivity is enforced.
E.3.2 Semantic Guarantees {#e32}
- All [kind](../../glossary/aui-glossary.md#kind) values are valid or namespaced ([semantic family](../../glossary/aui-glossary.md#semantic-family)).
- All props match semantic expectations.
- All required parts exist.
- No forbidden parts exist.
- All children satisfy kind‑specific rules.
E.3.3 Contract Guarantees {#e33}
- [Component contracts](../../glossary/aui-glossary.md#component-contract) are fully enforced.
- All required props are present.
- All prop types are correct.
- All part kinds match contract definitions.
E.3.4 Layout Guarantees {#e34}
- Layout props are valid.
- No mixed layout modes.
- No renderer‑specific layout constructs.
E.3.5 [Binding](../../glossary/aui-glossary.md#binding) Guarantees {#e35}
- All bindings resolve to valid paths.
- No circular bindings exist.
- No expressions or operators appear in bindings.
E.3.6 [Action](../../glossary/aui-glossary.md#action) Guarantees {#e36}
- All actions are declared.
- No dynamic code is embedded.
E.3.7 Determinism Guarantees {#e37}
- Node ordering is stable.
- Normalization is stable.
- The artifact is reproducible.

E.4 Renderer Responsibilities {#e4}
Given a validated artifact, the renderer MUST:
E.4.1 Trust Validator Output {#e41}
The renderer MUST assume:
- the artifact is semantically correct
- no invalid constructs remain
- no contract violations exist
- no layout errors exist
The renderer MUST NOT:
- re‑validate semantics
- reinterpret contracts
- infer missing semantics
E.4.2 Interpret Semantics Exactly {#e42}
The renderer MUST:
- map kind → platform primitives
- map props → platform props
- resolve parts and children deterministically
- apply layout rules exactly as declared
- attach event references symbolically
E.4.3 Preserve Meaning {#e43}
The renderer MUST NOT:
- reorder children
- rewrite props
- drop nodes
- infer layout
- execute event handler strings
E.4.4 Fail Fast on Structural Corruption {#e44}
If the artifact is corrupted (e.g., truncated JSON), the renderer MUST:
- fail immediately
- produce a structured error
- never attempt recovery

E.5 Shared Invariants {#e5}
Both validator and renderer MUST agree on the following invariants.
E.5.1 Node Identity {#e51}
Nodes are identified by:
- their position in the tree
- optional id fields (if present)
IDs MUST be stable and unique if used.
E.5.2 [Semantic Families](../../glossary/aui-glossary.md#semantic-family) {#e52}
Both systems MUST share the same [semantic families](../../glossary/aui-glossary.md#semantic-invariant):
- structural
- textual
- media
- interactive
- layout
- composite
E.5.3 Contract Definitions {#e53}
Contracts MUST be:
- versioned
- immutable once published
- shared across validator and renderer
E.5.4 Layout Semantics {#e54}
Layout semantics MUST be:
- declarative
- deterministic
- renderer‑agnostic

E.6 Error Interop Model {#e6}
E.6.1 Validator Errors {#e61}
Validator errors MUST:
- prevent rendering
- be machine‑readable
- include a JSONPath pointer
- include a stable error code
Renderer MUST NOT attempt to render invalid artifacts.
E.6.2 Renderer Errors {#e62}
Renderer errors MUST:
- indicate platform‑level issues
- never indicate semantic issues
- never indicate contract issues
- never indicate layout correctness issues
Renderer errors MUST NOT blame the validator for renderer‑specific failures.

E.7 Versioning Interop {#e7}
Validator and renderer MUST share:
- the same AUI version
- the same contract version
- the same semantic family definitions
If versions mismatch:
- validator MUST reject
- renderer MUST refuse to load

E.8 Extensibility Interop {#e8}
Extensions MUST follow:
E.8.1 Namespacing Rules {#e81}
Custom kinds MUST be namespaced:
"x-foo"
"x-card"
"x-layout-grid"


Validator MUST:
- allow namespaced kinds
- enforce structural rules
- not enforce semantics for unknown kinds
Renderer MUST:
- ignore unknown kinds unless it supports them
- never reinterpret unknown kinds as standard kinds
E.8.2 Forward Compatibility {#e82}
Validator MUST NOT:
- reject unknown namespaced kinds
- assume renderer support
Renderer MUST NOT:
- assume validator semantics for unknown kinds

E.9 Compliance Requirements
A validator/renderer pair is compliant if:
- validator enforces all guarantees
- renderer trusts validator output
- both share semantic definitions
- both follow versioning rules
- both follow namespacing rules
- both preserve determinism
Compliance MUST be testable via:
- golden artifacts
- cross‑system diffing
- contract conformance tests
- layout determinism tests
