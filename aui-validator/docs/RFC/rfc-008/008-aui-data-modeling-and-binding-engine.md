# RFC‑008: AUI Data Model & Binding Engine

**Status:** Draft  
**Author:** Alfredo  
**Category:** Data / Binding  
**Version:** 0.2.0  
**Last Updated:** 2026‑01‑22

---

## 1. Overview
The [AUI](../glossary/aui-glossary.md#aui) Data Model & Binding Engine defines the canonical representation, lifecycle, and [binding](../glossary/aui-glossary.md#binding) semantics for data flowing into AUI [components](../glossary/aui-glossary.md#component). It establishes [deterministic](../glossary/aui-glossary.md#determinism) rules for how data is declared, validated, transformed, and bound to component [parts](../glossary/aui-glossary.md#parts), [slots](../glossary/aui-glossary.md#slot), and behaviors.
This subsystem provides the data‑level foundation required before any spatial or structural interpretation of the UI can occur. Component structure, layout semantics, and spatial resolution all depend on the availability of fully resolved, validated, and typed data. For this reason, the Data Model & Binding Engine is defined prior to any subsystem that interprets or arranges components, ensuring that all downstream processes operate on stable, canonical data.
This RFC specifies:
- **The AUI Data Model (ADM)**
- **The Binding Engine (BE)**
- **Binding resolution order**
- **Transformation pipeline**
- **Error surfaces and diagnostics**
- **Integration points with other AUI subsystems**

---

## 2. Goals

The AUI Data Model & Binding Engine MUST:
- **Provide a [deterministic](../glossary/aui-glossary.md#determinism), declarative data model for all AUI [components](../glossary/aui-glossary.md#component)**
- **Ensure [bindings](../glossary/aui-glossary.md#binding) are reproducible, statically analyzable, and [validator](../glossary/aui-glossary.md#validator)-enforceable**
- **Support both static and dynamic data sources without introducing runtime ambiguity**
- **Enable [renderer](../glossary/aui-glossary.md#renderer)-agnostic data flow**
- **Establish a canonical error model for binding failures**
- **Guarantee that all component‑level data is resolved before any subsystem interprets component structure or spatial relationships**

The AUI Data Model & Binding Engine MUST NOT:
- **Define [renderer](../glossary/aui-glossary.md#renderer)-specific data hydration**
- **Define network or persistence layers**
- **Define component logic or event semantics (covered in RFC‑007)**

---

## 3. Non‑Goals

- **Defining [renderer](../glossary/aui-glossary.md#renderer)‑specific data hydration**
- **Defining network or persistence layers**
- **Defining component logic or event semantics (covered in RFC‑007)**

---

## 4. Terminology

- **ADM (AUI Data Model):** Canonical representation of data for a [component](../glossary/aui-glossary.md#component) [tree](../glossary/aui-glossary.md#tree)
- **[Binding](../glossary/aui-glossary.md#binding):** Declarative reference connecting ADM fields to component [parts](../glossary/aui-glossary.md#parts)/[slots](../glossary/aui-glossary.md#slot)
- **Resolver:** A [deterministic](../glossary/aui-glossary.md#determinism) function that resolves a binding to a concrete value
- **Transform:** Pure function applied to resolved data before hydration
- **Hydration:** Injection of resolved data into a [renderer adapter](../glossary/aui-glossary.md#renderer-adapter)

---

## 5. ADM Specification

### 5.1 ADM Root

```json
{
  "type": "AUI.DataModel",
  "version": "1.0",
  "fields": {},
  "sources": {},
  "transforms": {}
}
```

### 5.2 Fields
Fields define named values available to the component tree.

```json
{
  "fields": {
    "userName": { "type": "string", "required": true },
    "items": { "type": "array", "items": { "type": "Item" } }
  }
}
```

**Rules:**
- **Must be statically typed**
- **Must be [validator](../glossary/aui-glossary.md#validator)‑checkable**
- **Must not depend on runtime‑only constructs**

### 5.3 Sources

Sources define where data originates.

**Supported source types:**
- **"static"** — literal values
- **"prop"** — parent‑provided values
- **"context"** — global or scoped context providers
- **"computed"** — pure functions referencing other ADM fields

**Example:**

```json
{
  "sources": {
    "userName": { "source": "prop", "path": "user.name" },
    "items": { "source": "context", "key": "inventory" }
  }
}
```

### 5.4 Transforms

Transforms apply pure, [deterministic](../glossary/aui-glossary.md#determinism) functions to resolved values.

```json
{
  "transforms": {
    "items": [
      { "op": "filter", "expr": "item.inStock == true" },
      { "op": "sort", "expr": "item.name" }
    ]
  }
}
```

**Rules:**
- **Must be pure**
- **Must be statically analyzable**
- **Must not introduce side effects**

---

## 6. Binding Engine Specification

### 6.1 Binding Declaration

[Bindings](../glossary/aui-glossary.md#binding) attach ADM fields to [component](../glossary/aui-glossary.md#component) [parts](../glossary/aui-glossary.md#parts) or [slots](../glossary/aui-glossary.md#slot).

```json
{
  "bind": {
    "Header.title": "userName",
    "List.items": "items"
  }
}
```

### 6.2 Binding Resolution Order

1. **Field existence check**
2. **Source resolution**
3. **Type validation**
4. **Transform pipeline**
5. **Final value hydration**

### 6.3 Binding Types

- **Direct binding:** `"Header.title": "userName"`
- **Path binding:** `"Card.price": "product.cost.amount"`
- **Computed binding:** `"Summary.total": "computeTotal(items)"`

### 6.4 Resolver Semantics

Resolvers must:
- **Be [deterministic](../glossary/aui-glossary.md#determinism)**
- **Produce a value or a typed error**
- **Never mutate ADM state**

---

## 7. Transformation Pipeline

Transforms run in declared order.

**Pipeline:**

1. **Resolve field value**
2. **Apply transforms sequentially**
3. **Validate final type**
4. **Emit hydration payload**

If any step fails, the pipeline aborts with a structured error.

---

## 8. Error Model

### 8.1 Error Shape

```json
{
  "type": "AUI.BindingError",
  "field": "items",
  "stage": "transform",
  "message": "Filter expression invalid",
  "path": "List.items"
}
```

### 8.2 Error Categories

- **`MissingField`**
- **`InvalidSource`**
- **`TypeMismatch`**
- **`TransformError`**
- **`ResolverError`**
- **`HydrationError`**

---

## 9. Integration Points

### 9.1 With AUI Schema (RFC‑002)

- ADM fields must map to schema‑defined [component](../glossary/aui-glossary.md#component) inputs
- [Bindings](../glossary/aui-glossary.md#binding) must reference schema‑defined [parts](../glossary/aui-glossary.md#parts)/[slots](../glossary/aui-glossary.md#slot)

### 9.2 With AUI Validator (RFC‑003)

[Validator](../glossary/aui-glossary.md#validator) enforces:
- **Field type correctness**
- **Transform purity**
- **Binding existence**
- **Resolver determinism**

### 9.3 With Renderer Adapters (RFC‑004)

- **Renderer receives fully resolved, validated hydration payloads**
- **Renderer must not perform binding or transformation**

---

## 10. Security & Safety Considerations

- **No dynamic code execution**
- **No side‑effectful transforms**
- **No access to external systems**
- **All expressions must be statically validated**

---

## 11. Versioning

AUI Data Model uses semantic versioning:
- **MAJOR:** breaking changes to ADM structure or binding semantics
- **MINOR:** new source types, transform operations
- **PATCH:** clarifications, non‑breaking fixes

---

## 12. Future Extensions

- **Declarative data validation rules**
- **Cross‑component data dependencies**
- **Reactive data propagation semantics**
- **Optimistic data updates**

---

## Appendixes

- [📄 Appendix A — Minimal Example](./appendixes/appendix-a.md)
- [📄 Appendix B — Reserved Keywords](./appendixes/appendix-b.md)

---

## 13. Changelog

- **0.2.0** — Added architectural rationale for subsystem ordering
- **0.1.0** — Initial draft
