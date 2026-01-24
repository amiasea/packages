# Appendix A — AUI Language Specification (Canonical)

## A.1 Overview {#a1}

The [AUI](../../glossary/aui-glossary.md#aui) Language defines a declarative, JSON‑serializable representation of UI intent.

It is designed to be:
- deterministic
- composable
- validator‑friendly
- [renderer](../../glossary/aui-glossary.md#renderer)‑agnostic
- safe for AI generation

Appendix A formalizes the syntax, semantics, and structural invariants that all [AUI documents](../../glossary/aui-glossary.md#aui-document) must satisfy.

---

## A.2 Core Schema {#a2}

```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "title": "AUI Language Schema",
  "type": "object",
  "required": ["type"],
  "properties": {
    "type": {
      "type": "string",
      "enum": ["page", "component", "layout", "slot", "text", "image", "container", "list", "form", "input", "button"]
    },
    "id": { "type": "string" },
    "props": { "type": "object" },
    "children": {
      "type": "array",
      "items": { "$ref": "#" }
    },
    "slots": {
      "type": "object",
      "patternProperties": {
        "^[a-zA-Z_][a-zA-Z0-9_]*$": {
          "type": "array",
          "items": { "$ref": "#" }
        }
      }
    },
    "layout": {
      "type": "object",
      "properties": {
        "direction": { "type": "string", "enum": ["horizontal", "vertical"] },
        "align": { "type": "string", "enum": ["start", "center", "end", "stretch"] },
        "justify": { "type": "string", "enum": ["start", "center", "end", "space-between", "space-around"] },
        "gap": { "type": "string" }
      }
    },
    "style": { "type": "object" },
    "events": {
      "type": "object",
      "patternProperties": {
        "^[a-zA-Z_][a-zA-Z0-9_]*$": { "type": "string" }
      }
    },
    "data": {
      "type": "object",
      "patternProperties": {
        "^[a-zA-Z_][a-zA-Z0-9_]*$": {
          "type": ["string", "number", "boolean", "object", "array", "null"]
        }
      }
    },
    "conditions": {
      "type": "object",
      "properties": {
        "if": { "type": "string" },
        "unless": { "type": "string" }
      }
    },
    "repeat": {
      "type": "object",
      "required": ["each", "as"],
      "properties": {
        "each": { "type": "string" },
        "as": { "type": "string" }
      }
    },
    "meta": { "type": "object" }
  },
  "additionalProperties": false
}
```

---

## A.3 Node Types {#a3}

### A.3.1 page {#a31}

Top‑level document root.

**May contain:**
- [`children`](../../glossary/aui-glossary.md#children)
- [`layout`](../../glossary/aui-glossary.md#layout)
- `meta`

**Must not** appear as a child of any other node.

### A.3.2 component {#a32}

Reusable UI fragment.

**May define:**
- [`slots`](../../glossary/aui-glossary.md#slot)
- [`props`](../../glossary/aui-glossary.md#props)
- [`children`](../../glossary/aui-glossary.md#children)

[Renderer](../../glossary/aui-glossary.md#renderer) decides how components map to actual UI primitives.

### A.3.3 container {#a33}

Generic grouping element.

**Semantics:**
- participates in layout
- may contain `children`
- may define `layout`

### A.3.4 layout {#a34}

Explicit layout intent.

Used when layout must be forced rather than inferred.

### A.3.5 slot {#a35}

Named insertion point inside a [component](../../glossary/aui-glossary.md#component).

**Rules:**
- [slot](../../glossary/aui-glossary.md#slot) names must be valid [identifiers](../../glossary/aui-glossary.md#identifier)
- slots may contain arrays of nodes
- slots cannot contain other slots directly

### A.3.6 text {#a36}

Leaf node.

`props.value` contains the string.

### A.3.7 image {#a37}

Leaf node.

`props.src` contains the URL or asset reference.

### A.3.8 list {#a38}

Repeating structure.

**May use:**
- `repeat.each`
- `repeat.as`

### A.3.9 form, input, button {#a39}

Semantic form primitives.

---

## A.4 Structural Invariants {#a4}

These are enforced by the [validator](../../glossary/aui-glossary.md#validator).

### A.4.1 Tree Invariants {#a41}

- The document must form a [tree](../../glossary/aui-glossary.md#tree), not a graph.
- No cycles.
- No duplicate `id` values.
- `page` must be the root.

### A.4.2 Layout Invariants {#a42}

- Only containers may define `layout`.
- `layout.direction` must be present if any other layout fields are present.
- `gap` must be a valid CSS length string.

### A.4.3 Slot Invariants {#a43}

- Slot names must match `^[a-zA-Z_][a-zA-Z0-9_]*$`.
- Slots must contain arrays, never single nodes.
- Slots cannot appear on non‑component nodes.

### A.4.4 Conditional Rendering Invariants {#a44}

- `conditions.if` and `conditions.unless` cannot both be present.
- Condition expressions must be valid identifiers or dotted paths.

### A.4.5 Repeat Invariants {#a45}

- `repeat.each` must reference a list‑typed binding.
- `repeat.as` must be a valid identifier.
- Repeat cannot coexist with `children` on the same node.

---

## A.5 Semantic Rules {#a5}

### A.5.1 Props Are Renderer‑Opaque {#a51}

The validator does not interpret `props`.

**Only checks:**
- object shape
- no forbidden keys
- no non‑serializable values

### A.5.2 Events Are Strings {#a52}

Events are not executable code.

They are symbolic references.

### A.5.3 Data Bindings Are Pure {#a53}

Bindings must be:
- side‑effect‑free
- path‑like
- JSON‑serializable

---

## A.6 Examples {#a6}

### A.6.1 Simple Page {#a61}

```json
{
  "type": "page",
  "children": [
    {
      "type": "text",
      "props": { "value": "Hello world" }
    }
  ]
}
```

### A.6.2 Component With Slots {#a62}

```json
{
  "type": "component",
  "id": "Card",
  "slots": {
    "header": [],
    "body": [],
    "footer": []
  }
}
```

### A.6.3 List With Repeat {#a63}

```json
{
  "type": "list",
  "repeat": {
    "each": "items",
    "as": "item"
  },
  "children": [
    {
      "type": "text",
      "data": { "value": "item.name" }
    }
  ]
}
```

---

## A.7 Validator Notes {#a7}

**The validator enforces:**
- structural correctness
- semantic invariants
- slot/component contracts
- layout rules
- repeat/condition exclusivity
- `id` uniqueness
- JSON‑serializability

**The validator does not enforce:**
- visual correctness
- renderer‑specific constraints
- business logic
- styling semantics

---

## A.8 Meta Section {#a8}

`meta` is reserved for:
- toolchain hints
- generator provenance
- validator annotations
- debugging information

It must never affect rendering.