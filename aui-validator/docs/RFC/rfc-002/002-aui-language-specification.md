# RFC‑002: AUI Language Specification
**Status:** Draft  
**Author:** Alfredo  
**Category:** Language / IR  
**Version:** 1.0  

---

## 1. Overview
This RFC defines the [AUI](../glossary/aui-glossary.md#aui) Language, the [canonical](../glossary/aui-glossary.md#canonical-artifact), schema‑typed, [renderer](../glossary/aui-glossary.md#renderer)‑agnostic intermediate representation (IR) used by AUI Studio. The AUI Language is the authoritative representation of UI semantics after validation and before rendering.

It is:
- declarative
- deterministic
- schema‑typed
- renderer‑agnostic
- stable across toolchains

The [canonical artifact](../glossary/aui-glossary.md#canonical-artifact) is:
- **artifact.aui** — normalized AUI program

Upstream artifacts include:
- **[artifact.raw.aui](../glossary/aui-glossary.md#raw-artifact)** — unvalidated  
- **[artifact.validated.aui](../glossary/aui-glossary.md#validated-artifact)** — semantically correct  

This RFC defines the structure, semantics, and constraints of the canonical form.

---

## 2. Goals
The AUI Language MUST:
- Represent UI intent in a stable, declarative format
- Encode [component](../glossary/aui-glossary.md#component) structure, [props](../glossary/aui-glossary.md#props), [slots](../glossary/aui-glossary.md#slot), [parts](../glossary/aui-glossary.md#parts), [bindings](../glossary/aui-glossary.md#binding), and events
- Be fully typed by the AUI Schema
- Be independent of any rendering engine
- Serve as the input to [renderer adapters](../glossary/aui-glossary.md#renderer-adapter) (e.g., Antigravity)
- Be [deterministic](../glossary/aui-glossary.md#determinism) and reproducible

The AUI Language MUST NOT:
- Contain renderer‑specific constructs
- Contain [template constructs](../glossary/aui-glossary.md#template-constructs) (loops, conditions, expressions)
- Contain natural language
- Contain unresolved bindings or invalid references

---

## 3. Artifact Definition

### 3.1 File Format
The canonical AUI Language artifact MUST be stored as: artifact.aui

### 3.2 Encoding
- UTF‑8 JSON  
- No comments  
- Deterministic key ordering (recommended)

### 3.3 Top‑Level Structure
```json
{
  "version": "1.0",
  "root": {},
  "metadata": {}  // See [Metadata](../glossary/aui-glossary.md#metadata)
}
```

---

## 4. [Grammar](../glossary/aui-glossary.md#grammar)
The AUI Language uses a [deterministic](../glossary/aui-glossary.md#determinism), JSON‑compatible structural [grammar](../glossary/aui-glossary.md#grammar) that defines how UI intent, component composition, and renderer‑agnostic semantics are expressed. The grammar is intentionally minimal: it defines shape, contracts, and semantic boundaries, not rendering behavior.

### 4.1 Core Document Structure
```bnf
document        = object ;
object          = "{" pair ( "," pair )* "}" ;
pair            = string ":" value ;
value           = string | number | object | array | boolean | null ;

auiDocument     = {  // See [AUI Document](../glossary/aui-glossary.md#aui-document)
  "version": string,
  "type": "aui",
  "root": node
} ;
```


### 4.2 [Node](../glossary/aui-glossary.md#node) Grammar
```typescript
node = {  // See [Node](../glossary/aui-glossary.md#node)
  "kind": string,          // semantic type ([Kind](../glossary/aui-glossary.md#kind))
  "props": object?,        // renderer-agnostic semantic properties ([Props](../glossary/aui-glossary.md#props))
  "parts": object?,        // named child slots ([Parts](../glossary/aui-glossary.md#parts))
  "children": [ node ]?    // ordered children ([Children](../glossary/aui-glossary.md#children))
} ;
```

**Notes:**
- `parts` and `children` MUST NOT coexist unless explicitly allowed by the [component contract](../glossary/aui-glossary.md#component-contract).
- `kind` is the canonical semantic [identifier](../glossary/aui-glossary.md#identifier), not a renderer component name.
- `props` MUST be pure data, never renderer-specific configuration.

---

## 5. Semantic Model
The semantic model defines what a [node](../glossary/aui-glossary.md#node) means, not how it is rendered. This is the [intent layer](../glossary/aui-glossary.md#intent-layer) [Wisk](../glossary/aui-glossary.md#wisk) compiles, json‑render evaluates, and Antigravity adapts.

### 5.1 Node Kinds
Each [kind](../glossary/aui-glossary.md#kind) belongs to one of the following [semantic families](../glossary/aui-glossary.md#semantic-family):

| Family | Examples | Description |
|--------|----------|-------------|
| Containers | `container`, `stack`, `grid` | Layout and composition |
| Content | `text`, `image`, `icon` | Displayable content |
| Interactive | `button`, `input`, `toggle` | User interaction |
| Composite | `card`, `list-item`, `form` | Multi-part components |

### 5.2 Props Semantics
Props must satisfy:
- Renderer‑agnostic
- Deterministic
- Validatable

**Example:**
```json
{
  "kind": "button",
  "props": {
    "label": "Submit",
    "action": "submitForm"  // See [Action](../glossary/aui-glossary.md#action)
  }
}
```

### 5.3 Parts Semantics
Parts define named semantic slots:

```json
{
  "kind": "card",
  "parts": {
    "header": { "kind": "text", "props": { "value": "Title" } },
    "body":   { "kind": "text", "props": { "value": "Content" } }
  }
}
```

**Rules:**
- Parts must match the component’s declared contract.
- Missing required parts are validator errors.
- Extra parts are forbidden.

---

## 6. [Component Contracts](../glossary/aui-glossary.md#component-contract)

### 6.1 Contract Structure
```json
{
  "kind": "card",
  "props": {
    "title": { "type": "string", "required": false }
  },
  "parts": {
    "header": { "required": true },
    "body":   { "required": true },
    "footer": { "required": false }
  }
}
```

### 6.2 Contract Enforcement
The [AUI Validator](../glossary/aui-glossary.md#validator) enforces:
- Prop type correctness
- Required part presence
- Forbidden part absence
- [Layout](../glossary/aui-glossary.md#layout) rules (if declared)
- [Semantic invariants](../glossary/aui-glossary.md#semantic-invariant)

---

## 7. [Layout](../glossary/aui-glossary.md#layout) Semantics
AUI [layout](../glossary/aui-glossary.md#layout) is semantic, not stylistic. It describes relationships, not pixels.

### 7.1 [Flow Layout](../glossary/aui-glossary.md#flow-layout)
```json
{
  "kind": "container",
  "props": {
    "direction": "vertical",
    "gap": 12
  }
}
```

### 7.2 [Constraint Layout](../glossary/aui-glossary.md#constraint-layout)
```json
{
  "kind": "constraint",
  "props": {
    "align": "start",
    "stretch": true
  }
}
```

### 7.3 Layout Rules
- Layout props must not encode styling.
- Layout must be deterministic.
- Layout must be renderer‑agnostic.

---

## 8. [Intent Layer](../glossary/aui-glossary.md#intent-layer) ([Wisk](../glossary/aui-glossary.md#wisk))

### 8.1 Intent → AUI Mapping
- Extract semantic structure
- Infer component kinds
- Infer layout relationships
- Normalize props
- Generate deterministic AUI

### 8.2 [Determinism](../glossary/aui-glossary.md#determinism) Guarantees
[Wisk](../glossary/aui-glossary.md#wisk) must produce:
- Same input → same AUI
- No renderer-specific output
- No styling

---

## 9. Template Evaluation (json‑render)

### 9.1 Responsibilities
- Expand composite components
- Resolve parts
- Inline defaults
- Produce a fully concrete tree

### 9.2 Non‑Responsibilities
- No styling
- No renderer-specific logic
- No layout inference

---

## 10. [Renderer Adapters](../glossary/aui-glossary.md#renderer-adapter) (Antigravity)

### 10.1 Responsibilities
- Map semantic kinds → renderer components
- Map semantic props → renderer props
- Apply renderer-specific defaults
- Enforce platform constraints

### 10.2 Adapter Contract
```json
{
  "mapKind": { "button": "AGButton", "text": "AGText" },
  "mapProps": {
    "button": {
      "label": "title",
      "action": "onPress"
    }
  }
}
```

---

## 11. Validation Model

### 11.1 Validation Phases
- [Structural validation](../glossary/aui-glossary.md#structural-validation)
- Semantic validation
- Contract validation
- [Layout](../glossary/aui-glossary.md#layout) validation
- Cross‑node invariants

### 11.2 Error Format
```json
{
  "code": "AUI_INVALID_PROP_TYPE",
  "message": "Expected string for prop 'label'",
  "path": "$.root.parts.header"
}
```

---

## 12. Versioning
AUI uses semantic versioning:
- **MAJOR:** breaking semantic changes
- **MINOR:** new kinds, new props
- **PATCH:** clarifications, non-breaking fixes

---

## 13. Security Model
- No executable code in props
- No renderer-specific escape hatches
- No dynamic evaluation
- All actions must be declared, not embedded

---

## 14. Future Extensions
- Declarative animation semantics
- Accessibility-first semantic props
- Cross-platform gesture semantics
- Declarative data-binding layer

---

## Appendixes

- [📄 Appendix A — AUI Language Schema](./appendixes/appendix-a.md)
- [📄 Appendix B — Renderer Contract](./appendixes/appendix-b.md)
- [📄 Appendix C — Validator Algorithm](./appendixes/appendix-c.md)
- [📄 Appendix D — AUI DSL Grammar](./appendixes/appendix-d.md)
- [📄 Appendix E — Validator ↔ Renderer Interop](./appendixes/appendix-e.md)
- [📄 Appendix F — Artifact Lifecycle](./appendixes/appendix-f.md)

# Cross‑Reference Index — Appendixes A through F

## 1. Core Language Concepts

| Concept | Definition | Reference |
|---------|------------|-----------|
| AUI Node | Structural unit of UI semantics | [A.3](./appendixes/appendix-a.md#a3), [A.4](./appendixes/appendix-a.md#a4) |
| Node Types | page, component, container, text, image, list, etc. | [A.3](./appendixes/appendix-a.md#a3) |
| Props | Renderer‑agnostic semantic properties | [A.5.2](./appendixes/appendix-a.md#a52) |
| Parts / Slots | Named semantic insertion points | [A.3.5](./appendixes/appendix-a.md#a35), [A.5.3](./appendixes/appendix-a.md#a53) |
| Children | Ordered child nodes | [A.3](./appendixes/appendix-a.md#a3), [A.4](./appendixes/appendix-a.md#a4) |
| Metadata | Non‑semantic toolchain hints | [A.8](./appendixes/appendix-a.md#a8) |
| [Identifiers](../glossary/aui-glossary.md#identifier) | Valid names for parts, bindings, actions | [D.9](./appendixes/appendix-d.md#d9) |
| [Bindings](../glossary/aui-glossary.md#binding) | Pure path‑like references ([Binding Path](../glossary/aui-glossary.md#binding-path)) | [A.5.3](./appendixes/appendix-a.md#a53), [D.10](./appendixes/appendix-d.md#d10) |
| [Actions](../glossary/aui-glossary.md#action) | Symbolic [event handler](../glossary/aui-glossary.md#event-handler) references | [B.7](./appendixes/appendix-b.md#b7), [D.11](./appendixes/appendix-d.md#d11) |

## 2. Structural Rules & Grammar

| Concept | Definition | Reference |
|---------|------------|-----------|
| JSON Grammar | Deterministic structural grammar | [D.2](./appendixes/appendix-d.md#d2)–[D.13](./appendixes/appendix-d.md#d13) |
| Node Grammar | Required fields, exclusivity rules | [D.4](./appendixes/appendix-d.md#d4) |
| Parts Grammar | Object of named nodes | [D.6](./appendixes/appendix-d.md#d6) |
| Children Grammar | Ordered array of nodes | [D.7](./appendixes/appendix-d.md#d7) |
| Binding Grammar | Path‑like string syntax | [D.10](./appendixes/appendix-d.md#d10) |
| Action Grammar | Identifier syntax | [D.11](./appendixes/appendix-d.md#d11) |
| Document Grammar | Top‑level structure | [D.3](./appendixes/appendix-d.md#d3) |

## 3. Semantic Model

| Concept | Definition | Reference |
|---------|------------|-----------|
| Semantic Families | Structural, textual, media, layout, etc. | [A.5.1](./appendixes/appendix-a.md#a51) |
| Props Semantics | Deterministic, renderer‑agnostic | [A.5.2](./appendixes/appendix-a.md#a52) |
| Parts Semantics | Contract‑driven slot semantics | [A.5.3](./appendixes/appendix-a.md#a53) |
| Layout Semantics | Flow, constraint, deterministic layout | A.7, [B.5](./appendixes/appendix-b.md#b5) |
| Component Semantics | Reusable semantic templates | [A.3.2](./appendixes/appendix-a.md#a32), [B.4](./appendixes/appendix-b.md#b4) |

## 4. Component Contracts

| Concept | Definition | Reference |
|---------|------------|-----------|
| Contract Structure | Props + parts API | [A.6.1](./appendixes/appendix-a.md#a61) |
| Contract Enforcement | Validator responsibilities | [A.6.2](./appendixes/appendix-a.md#a62), [C.6](./appendixes/appendix-c.md#c6) |
| Required Parts | Must exist | [A.6.2](./appendixes/appendix-a.md#a62) |
| Forbidden Parts | Must not exist | [A.6.2](./appendixes/appendix-a.md#a62) |
| Prop Type Rules | Type correctness | [C.6.2](./appendixes/appendix-c.md#c62) |
| Semantic Invariants | e.g., button must have label or icon | [C.6.3](./appendixes/appendix-c.md#c63) |

## 5. Layout System

| Concept | Definition | Reference |
|---------|------------|-----------|
| Flow Layout | direction, gap | A.7.1 |
| Constraint Layout | align, stretch | A.7.2 |
| Layout Rules | deterministic, semantic | A.7.3 |
| Renderer Layout Contract | how layout is interpreted | [B.5](./appendixes/appendix-b.md#b5) |
| Layout Validation | validator responsibilities | [C.7](./appendixes/appendix-c.md#c7) |

## 6. Validator Responsibilities

| Concept | Definition | Reference |
|---------|------------|-----------|  
| Validation Pipeline | 7‑phase algorithm | [C.2](./appendixes/appendix-c.md#c2) |
| [Structural Validation](../glossary/aui-glossary.md#structural-validation) | [tree](../glossary/aui-glossary.md#tree) shape, exclusivity | [C.4](./appendixes/appendix-c.md#c4) |
| Semantic Validation | kind, props, parts | [C.5](./appendixes/appendix-c.md#c5) |
| Contract Validation | props, parts, invariants | [C.6](./appendixes/appendix-c.md#c6) |
| Layout Validation | flow/constraint correctness | [C.7](./appendixes/appendix-c.md#c7) |
| Cross‑Node Invariants | bindings, actions, cycles | [C.8](./appendixes/appendix-c.md#c8) |
| Error Model | structured, deterministic | [C.10](./appendixes/appendix-c.md#c10) |
| Compliance | validator correctness | [C.11](./appendixes/appendix-c.md#c11) |

## 7. Renderer Responsibilities

| Concept | Definition | Reference |
|---------|------------|-----------|
| Renderer Pipeline | parse → resolve → layout → render | [B.3](./appendixes/appendix-b.md#b3) |
| Component Resolution | slot insertion, expansion | [B.4](./appendixes/appendix-b.md#b4) |
| Layout Interpretation | deterministic layout behavior | [B.5](./appendixes/appendix-b.md#b5) |
| Data Binding | lazy, pure, path‑based ([Data Context](../glossary/aui-glossary.md#data-context)) | [B.6](./appendixes/appendix-b.md#b6) |
| Event Binding | symbolic, non‑executable | [B.7](./appendixes/appendix-b.md#b7) |
| Error Handling | fail fast, structured | [B.8](./appendixes/appendix-b.md#b8) |
| Determinism | stable output | [B.9](./appendixes/appendix-b.md#b9) |
| Extensibility | namespaced kinds | [B.10](./appendixes/appendix-b.md#b10) |

## 8. Validator ↔ Renderer Interop

| Concept | Definition | Reference |
|---------|------------|-----------|
| Interop Contract | shared guarantees | [E.1](./appendixes/appendix-e.md#e1)–[E.2](./appendixes/appendix-e.md#e2) |
| Validator Guarantees | structural, semantic, layout | [E.3](./appendixes/appendix-e.md#e3) |
| Renderer Responsibilities | trust validator, preserve meaning | [E.4](./appendixes/appendix-e.md#e4) |
| Shared Invariants | identity, families, contracts | [E.5](./appendixes/appendix-e.md#e5) |
| Error Interop | validator vs renderer errors | [E.6](./appendixes/appendix-e.md#e6) |
| Versioning Interop | shared version requirements | [E.7](./appendixes/appendix-e.md#e7) |
| Extensibility Interop | namespaced kinds | [E.8](./appendixes/appendix-e.md#e8) |
| Compliance | pair correctness | [E.9](./appendixes/appendix-e.md#e9) |

## 9. Artifact Lifecycle

| Concept | Definition | Reference |
|---------|------------|-----------|
| Raw Artifact | unvalidated, AI‑generated | [F.3](./appendixes/appendix-f.md#f3) |
| Validated Artifact | semantically correct | [F.4](./appendixes/appendix-f.md#f4) |
| Canonical Artifact | normalized, authoritative | [F.5](./appendixes/appendix-f.md#f5) |
| Renderer Artifact | platform‑ready JSON | [F.6](./appendixes/appendix-f.md#f6) |
| Host Artifact | platform UI representation | [F.7](./appendixes/appendix-f.md#f7) |
| Invariant Preservation | structural, semantic, deterministic | [F.8](./appendixes/appendix-f.md#f8) |
| Transformation Matrix | legal/illegal transitions | [F.9](./appendixes/appendix-f.md#f9) |
| Error Propagation | stage‑specific errors | [F.10](./appendixes/appendix-f.md#f10) |
| Versioning Rules | shared version constraints | [F.11](./appendixes/appendix-f.md#f11) |
| Lifecycle Compliance | end‑to‑end correctness | [F.12](./appendixes/appendix-f.md#f12) |

## 10. Cross‑Appendix Concept Map

### Language Definition

| Appendix | Focus |
|----------|-------|
| A | Schema & semantics |
| D | Grammar |
| C | Validation |
| B | Rendering |
| E | Interop |
| F | Lifecycle |

### Contracts

| Appendix | Focus |
|----------|-------|
| A | Component contracts |
| C | Contract enforcement |
| B | Renderer contract |
| E | Shared contract invariants |

### Layout

| Appendix | Focus |
|----------|-------|
| A | Layout semantics |
| C | Layout validation |
| B | Layout interpretation |
| E | Shared layout invariants |

### Bindings & Actions

| Appendix | Focus |
|----------|-------|
| A | Semantics |
| D | Grammar |
| C | Validation |
| B | Renderer behavior |
| E | Interop guarantees |

### Determinism

| Appendix | Focus |
|----------|-------|
| A | Structural invariants |
| C | Deterministic validation |
| B | Deterministic rendering |
| E | Deterministic interop |
| F | Deterministic lifecycle |  