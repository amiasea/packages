# Appendix B — AUI Renderer Contract (Canonical)

## B.1 Purpose {#b1}

The [Renderer](../../glossary/aui-glossary.md#renderer) Contract defines the obligations, behaviors, and interpretation rules that any AUI‑compliant renderer must follow.

It ensures that:
- [AUI documents](../../glossary/aui-glossary.md#aui-document) behave consistently across platforms
- semantic meaning is preserved
- [layout](../../glossary/aui-glossary.md#layout) and [component](../../glossary/aui-glossary.md#component) contracts are respected
- validators and generators can rely on stable invariants
- renderers remain interchangeable without breaking intent

This appendix is normative.

---

## B.2 Renderer Responsibilities {#b2}

**A renderer MUST:**
- accept a [validated](../../glossary/aui-glossary.md#validated-artifact) AUI document
- construct a UI [tree](../../glossary/aui-glossary.md#tree) that preserves AUI semantics
- apply layout rules deterministically
- resolve components and slots
- bind data expressions
- attach [event handlers](../../glossary/aui-glossary.md#event-handler) symbolically
- produce a stable, predictable UI output

**A renderer MUST NOT:**
- reinterpret semantics
- mutate the AUI document
- introduce side effects during evaluation
- execute event handler strings as code
- infer layout when layout is explicitly defined

---

## B.3 Rendering Pipeline {#b3}
A compliant renderer MUST implement the following pipeline:
- Parse Phase
- Accept JSON input.
- Validate structural shape (optional if validator already ran).
- Normalize missing optional fields.
- Component Resolution Phase
- Expand component definitions.
- Resolve slots.
- Inline component bodies with slot content.
- Condition Evaluation Phase
- Evaluate conditions.if and conditions.unless.
- Remove nodes that do not satisfy conditions.
- Conditions MUST NOT trigger side effects.
- Repeat Expansion Phase
- For each repeat, clone the node for each item.
- Bind repeat.as to the current item.
- Replace data bindings accordingly.
- Layout Computation Phase
- Apply layout rules to containers.
- Respect direction, alignment, justification, and gap.
- Do not infer layout when explicit layout is present.
- Style Application Phase
- Apply inline style overrides.
- Merge with renderer defaults.
- Never mutate the AUI document.
- Event Binding Phase
- Attach event handler references.
- Treat event strings as opaque identifiers.
- Never execute event code.
- Render Phase
- Produce the final UI tree.
- Map AUI primitives to platform primitives.
- Guarantee deterministic output.

## B.4 Component Contract {#b4}

### B.4.1 [Component](../../glossary/aui-glossary.md#component) Definition Rules {#b41}

A renderer MUST treat [component](../../glossary/aui-glossary.md#component-contract) nodes as:
- reusable templates
- parameterizable via `props`
- expandable into concrete UI trees

A component MAY contain:
- `slots`
- `children`
- `props`

A component MUST NOT:
- define `repeat`
- define `conditions`
- define `layout` unless it is a layout component

### B.4.2 [Slot](../../glossary/aui-glossary.md#slot) Resolution Rules {#b42}

When rendering a component instance:
- slot names MUST match the component definition
- missing slots MUST default to empty arrays
- extra slots MUST be ignored
- slot content MUST be inserted in the correct position
- slot content MUST be rendered in order

Slots MUST NOT:
- contain other slots
- override component structure
- introduce layout metadata

---

## B.5 Layout Contract {#b5}

### B.5.1 Layout Interpretation {#b51}

A renderer MUST interpret layout as a declarative constraint system.

**Required behaviors:**
- `direction` determines primary axis
- `align` determines cross‑axis alignment
- `justify` determines main‑axis distribution
- `gap` inserts spacing between children

**Forbidden behaviors:**
- inferring layout when layout is present
- reordering children
- collapsing empty containers unless explicitly allowed

### B.5.2 Layout Defaults {#b52}

If a container has no layout:
- renderer MAY apply platform defaults
- defaults MUST be documented
- defaults MUST be stable across versions

---

## B.6 Data Binding Contract {#b6}

### B.6.1 [Binding](../../glossary/aui-glossary.md#binding) Semantics {#b61}

A renderer MUST treat data values as:
- pure expressions
- path‑like references
- evaluated against a provided [data context](../../glossary/aui-glossary.md#data-context)

A renderer MUST NOT:
- execute arbitrary code
- mutate the data context
- allow side effects

### B.6.2 [Binding](../../glossary/aui-glossary.md#binding-path) Resolution Order {#b62}

Bindings MUST be resolved in this order:
1. repeat context (`repeat.as`)
2. local [node](../../glossary/aui-glossary.md#node) data
3. inherited [data context](../../glossary/aui-glossary.md#data-context)
4. global data context

Bindings MUST be resolved lazily.

---

## B.7 Event Contract {#b7}

### B.7.1 Event Handler Semantics {#b71}

Event handler values MUST be treated as:
- opaque identifiers
- symbolic references
- never executable code

A renderer MUST:
- attach event references
- expose them to the host environment
- never interpret or execute them

### B.7.2 Event Dispatch Rules {#b72}

When an event fires:
- renderer MUST emit the symbolic event name
- renderer MUST include contextual metadata
- renderer MUST NOT execute business logic

Event dispatch is the responsibility of the host environment.

---

## B.8 Error Handling Contract {#b8}

**A renderer MUST:**
- fail fast on malformed AUI
- provide structured error messages
- include node paths in errors
- never attempt to "fix" invalid AUI

**A renderer MAY:**
- include debug metadata
- expose developer tooling hooks

**A renderer MUST NOT:**
- silently drop nodes
- rewrite the document
- infer missing semantics

---

## B.9 Determinism Requirements {#b9}

**A renderer MUST produce:**
- identical output for identical input
- stable ordering of children
- stable layout computation
- stable component expansion

**A renderer MUST NOT:**
- introduce randomness
- depend on external state
- reorder nodes for optimization

---

## B.10 Extensibility Rules {#b10}

Renderers MAY extend AUI with:
- custom node types
- custom layout primitives
- custom style systems

Extensions MUST:
- be namespaced
- not break core semantics
- not override standard node types
- not change the meaning of existing fields

---

## B.11 Meta Contract {#b11}

Renderers MUST ignore `meta` fields unless:
- explicitly configured to use them
- they contain renderer‑specific hints

`meta` MUST NOT affect rendering by default.

---

## B.12 Compliance Checklist {#b12}

A renderer is compliant if it satisfies:
- structural interpretation rules
- component contract
- slot contract
- layout contract
- data binding contract
- event contract
- determinism requirements
- error handling rules
- extensibility rules

Compliance MUST be testable via:
- golden snapshots
- structural diffing
- deterministic layout tests
