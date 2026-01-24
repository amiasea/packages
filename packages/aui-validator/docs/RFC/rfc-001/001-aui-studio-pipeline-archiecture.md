# RFC‑001: AUI Studio Pipeline Architecture

**Status:** Draft  
**Author:** Alfredo  
**Category:** Architecture / Pipeline  
**Version:** 1.0  

---

## 1. Overview
This RFC defines the AUI Studio Pipeline, a [deterministic](../../glossary/aui-glossary.md#determinism), multi‑stage system that transforms human UI intent into renderer‑agnostic, semantically validated UI programs consumable by multiple rendering engines.
The pipeline introduces:
- A canonical intermediate representation ([AUI](../../glossary/aui-glossary.md#aui) Language)
- A semantic validation layer ([AUI Validator](../../glossary/aui-glossary.md#validator))
- A [renderer adapter](../../glossary/aui-glossary.md#renderer-adapter) layer (Antigravity)
- A [template](../../glossary/aui-glossary.md#template-constructs) evaluation layer (json‑render)
- An intent compiler ([Wisk](../../glossary/aui-glossary.md#wisk))
This document specifies the responsibilities, inputs, outputs, and constraints of each stage.

---

## 2. Terminology

### 2.1 MUST / SHOULD / MAY
This RFC uses normative language:
- MUST: required for correctness
- SHOULD: recommended but not required
- MAY: optional

### 2.2 Artifact Types
- T‑JSON — Template JSON with loops, conditions, expressions
- [Raw AUI Program](../../glossary/aui-glossary.md#raw-artifact) — Fully expanded JSON, not yet validated
- [Validated AUI Program](../../glossary/aui-glossary.md#validated-artifact) — Semantically correct AUI program
- AUI Language — [Canonical](../../glossary/aui-glossary.md#canonical-artifact), schema‑typed IR

### 2.3 Tools
- Wisk — Intent compiler
- json‑render — Template evaluator
- AUI Validator — Semantic validator
- Antigravity — Renderer adapter

---

## 3. Pipeline Summary
The AUI Studio pipeline consists of five categorical layers:
- [Intent Layer](../../glossary/aui-glossary.md#intent-layer)
- Template Layer
- Validation Layer
- Canonical IR Layer
- Rendering Layer
Each layer consumes a well‑defined artifact and produces another.

---

## 4. Intent Layer

### 4.1 Purpose
Convert human natural language into a structured, template‑based representation of UI intent.

### 4.2 Input
- Free‑form natural language
- Task descriptions
- User stories

### 4.3 Output
- T‑JSON (Template JSON)

### 4.4 Requirements
- MUST NOT perform validation
- MUST NOT produce [renderer](../../glossary/aui-glossary.md#renderer)‑specific output
- MUST express intent using schema vocabulary

### 4.5 Example Implementation: Wisk
Wisk is an intent compiler that:
- Parses natural language
- Maps phrases to schema [components](../../glossary/aui-glossary.md#component)
- Emits T‑JSON with loops, conditions, and expressions

---

## 5. Template Layer

### 5.1 Purpose
Convert T‑JSON into a fully expanded, deterministic JSON object graph.

### 5.2 Input
- T‑JSON

### 5.3 Output
- Raw AUI Program

### 5.4 Requirements
- MUST evaluate loops
- MUST resolve conditions
- MUST compute expressions
- MUST produce [deterministic](../../glossary/aui-glossary.md#determinism) JSON
- MUST NOT validate semantics

### 5.5 Example Implementation: json‑render
json‑render:
- Expands template constructs
- Produces concrete JSON
- Guarantees [structural](../../glossary/aui-glossary.md#structural-validation) correctness

---

## 6. Validation Layer

### 6.1 Purpose
Ensure the Raw AUI Program conforms to the AUI schema and is semantically valid.

### 6.2 Input
- Raw AUI Program

### 6.3 Output

One of:
- Validated AUI Program (success)
- Validation Errors (failure)

### 6.4 Requirements

The validator MUST enforce:

#### 6.4.1 [Props](../../glossary/aui-glossary.md#props)
- Required props exist
- Types match schema
- Unknown props rejected

#### 6.4.2 [Slots](../../glossary/aui-glossary.md#slot)
- Required slots present
- Allowed [children](../../glossary/aui-glossary.md#children) enforced
- Forbidden children rejected

#### 6.4.3 [Parts](../../glossary/aui-glossary.md#parts)
- Required parts present
- Structural constraints satisfied

#### 6.4.4 [Bindings](../../glossary/aui-glossary.md#binding)
- Binding targets exist
- Binding types match
- Binding paths valid

#### 6.4.5 Events
- Event names valid
- [Handler](../../glossary/aui-glossary.md#event-handler) signatures match

### 6.5 Non‑Responsibilities

The validator MUST NOT:
- Transform the program
- Normalize structure
- Infer [layout](../../glossary/aui-glossary.md#layout)
- Generate code
- Render UI

### 6.6 Example Implementation: AUI Validator
A deterministic, schema‑driven rule engine that:
- Walks the object [graph](../../glossary/aui-glossary.md#tree)
- Applies schema rules
- Emits errors or a validated program

---

## 7. Canonical IR Layer

### 7.1 Purpose
Represent validated UI intent in a renderer‑agnostic, schema‑typed format.

### 7.2 Input
- Validated AUI Program

### 7.3 Output
- AUI Language ([canonical IR](../../glossary/aui-glossary.md#canonical-artifact))

### 7.4 Requirements
- MUST be stable
- MUST be deterministic
- MUST be schema‑typed
- MUST be renderer‑agnostic

### 7.5 Optional Normalization

A Normalizer MAY:
- Apply defaults
- Canonicalize ordering
- Remove redundant structure

---

## 8. Rendering Layer

### 8.1 Purpose
Convert the canonical AUI IR into a renderer‑specific view tree suitable for execution by a UI framework or native runtime.

### 8.2 Input
- artifact.aui — the canonical, normalized AUI Language artifact

### 8.3 Output
- Renderer‑specific intermediate structures (e.g., React VDOM, Angular View Tree, SwiftUI View Graph)
- Ultimately: Browser DOM or native UI

### 8.4 Requirements

The rendering layer:
- MUST NOT modify the semantics of the AUI program
- MUST map AUI components to renderer primitives
- MUST support pluggable backends
- MAY apply renderer‑specific optimizations
- MUST treat the AUI Language as authoritative

### 8.5 Example Implementation: Antigravity
Antigravity is a renderer adapter that:
- Consumes artifact.aui
- Emits renderer‑specific structures
- Supports multiple backends (React, Angular, Vue, SwiftUI, Svelte)
- Provides a stable contract for adding new renderers

---

## 9. End‑to‑End Data Flow

```
Human Intent
    ↓
Wisk (Intent Compiler)
    ↓
🟢 T‑JSON
    ↓
json-render (Template Evaluator)
    ↓
🟢 artifact.raw.aui
    ↓
AUI Validator (Semantic Validation)
    ↓
🟢 artifact.validated.aui
    ↓
Normalizer (Optional)
    ↓
🟢 artifact.aui
    ↓
Antigravity (Renderer Adapter)
    ↓
React / Angular / Vue / SwiftUI / Svelte
    ↓
Browser DOM / Native UI
```

### Artifact Summary
- **T‑JSON** → Template IR
- **artifact.raw.aui** → Fully expanded, unvalidated AUI program
  - Produced by json-render.
- - Not validated.
- - Fully expanded.
- - Deterministic JSON.
  - Semantically untrusted.
- **artifact.validated.aui** → Semantically correct AUI program
  - Produced by the AUI Validator.
- - Same structure, but now certified.
  - No semantic errors.
- **artifact.aui** → Canonical, normalized AUI Language
  - The canonical AUI Language IR.
- - Renderer‑agnostic.
- - Schema‑typed.
- - Stable.
  - This is the "real" AUI program.

---

## 10. Non‑Goals

The AUI Studio pipeline does not perform the following responsibilities.
These are explicitly out of scope for this RFC and MUST NOT be implemented by any stage of the pipeline:

### 10.1 No Layout Resolution
- No flexbox, grid, spacing, or geometry computation
- Layout is renderer/runtime responsibility

### 10.2 No Runtime State Management
- No component state
- No signals, stores, reducers, or reactive models
- No event handler execution

### 10.3 No Business Logic Execution
- No data fetching
- No domain logic
- No side effects

### 10.4 No Code Generation (outside renderer adapters)
- The pipeline does not emit TypeScript, Swift, Kotlin, HTML, or CSS
- Only renderer adapters may generate renderer‑specific structures

### 10.5 No Semantic Inference
- No guessing missing props
- No auto‑fixing invalid structures
- No AI inference at this stage

### 10.6 No Rendering
- The pipeline does not produce DOM [nodes](../../glossary/aui-glossary.md#node)
- Rendering is the responsibility of the chosen UI framework

---

## 11. Future Extensions

Potential future RFCs may define:
- Schema DSL
- [Component](../../glossary/aui-glossary.md#component-contract) capability [metadata](../../glossary/aui-glossary.md#metadata)
- Layout semantics
- State machine integration
- Multi‑modal intent inputs

---

## 12. Conclusion

This RFC establishes the categorical architecture of AUI Studio:
- Wisk → intent compiler
- json‑render → template evaluator
- AUI Validator → semantic gatekeeper
- AUI Language → canonical IR
- Antigravity → renderer adapter
Each stage has a single, crisp responsibility.
The pipeline is deterministic, extensible, and renderer‑agnostic.