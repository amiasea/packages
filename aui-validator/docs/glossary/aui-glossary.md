# AUI Glossary
Author: Alfredo  
Version: 1.0  
Status: Stable  

A canonical glossary of all terminology used across the AUI Language, Validator, Renderer, and AUI Studio pipeline.  
This glossary is cross‑referenced with RFC‑002 and Appendixes A–F.

---

## A

### **Action** {#action}
A symbolic, non-executable identifier representing a user-triggered event.  
Used in props such as `"action": "submitForm"`.

### **AUI (Abstract UI)** {#aui}
The canonical, renderer‑agnostic intermediate representation of UI semantics.

### **AUI Document** {#aui-document}
A JSON object representing a complete AUI program.  
Rooted at `"root"` and typed with `"type": "aui"`.

---

## B

### **Binding** {#binding}
A pure, path‑like reference to data (e.g., `"user.name"`).  
No expressions, operators, or evaluation.

### **Binding Path** {#binding-path}
A dotted identifier chain referencing a value in the data context.

---

## C

### **Canonical Artifact** {#canonical-artifact}
The normalized, authoritative AUI artifact (`artifact.aui`) produced after validation.

### **Children** {#children}
An ordered array of nodes representing structural composition.

### **Component** {#component}
A reusable semantic UI fragment with props and parts.

### **Component Contract** {#component-contract}
A semantic API describing required props, allowed props, required parts, and optional parts.

### **Constraint Layout** {#constraint-layout}
A layout mode using alignment and stretch semantics.

---

## D

### **Data Context** {#data-context}
The hierarchical environment used to resolve bindings.

### **Determinism** {#determinism}
The guarantee that identical input produces identical output across all pipeline stages.

---

## E

### **Event Handler** {#event-handler}
A symbolic reference to an action.  
Never executable code.

---

## F

### **Flow Layout** {#flow-layout}
A layout mode using direction and gap semantics.

---

## G

### **Grammar (AUI DSL)** {#grammar}
The formal JSON-compatible grammar defining the syntactic shape of AUI documents.

---

## I

### **Identifier** {#identifier}
A name matching `/^[a-zA-Z_][a-zA-Z0-9_]*$/`.  
Used for kinds, parts, bindings, and actions.

### **Intent Layer (Wisk)** {#intent-layer}
The natural-language-to-AUI compiler that produces raw AUI artifacts.

---

## K

### **Kind** {#kind}
The semantic type of a node (e.g., `"text"`, `"container"`, `"button"`).

---

## L

### **Layout** {#layout}
Declarative, renderer-agnostic spatial semantics applied to containers.

---

## M

### **Metadata** {#metadata}
Non-semantic information used by tools, validators, or generators.  
Ignored by renderers unless explicitly supported.

---

## N

### **Node** {#node}
A structural unit in the AUI tree with `kind`, `props`, `parts`, and/or `children`.

---

## P

### **Parts** {#parts}
Named semantic slots inside a component.  
Each part contains a node.

### **Props** {#props}
Renderer-agnostic semantic properties attached to a node.

---

## R

### **Raw Artifact** {#raw-artifact}
The unvalidated, AI-generated AUI document (`artifact.raw.aui`).

### **Renderer** {#renderer}
The subsystem that maps canonical AUI to platform UI primitives.

### **Renderer Adapter** {#renderer-adapter}
A mapping layer that translates semantic kinds and props to platform-specific equivalents.

---

## S

### **Semantic Family** {#semantic-family}
A classification of node kinds (structural, textual, media, layout, interactive, composite).

### **Semantic Invariant** {#semantic-invariant}
A rule that must always hold (e.g., text nodes cannot have children).

### **Slot** {#slot}
A named insertion point inside a component (synonym: part).

### **Structural Validation** {#structural-validation}
Validation ensuring the document is a well-formed tree.

---

## T

### **Template Constructs** {#template-constructs}
Loops, conditions, or expressions that MUST NOT appear in canonical AUI.

### **Tree** {#tree}
The hierarchical structure of nodes forming the AUI document.

---

## V

### **Validated Artifact** {#validated-artifact}
The semantically correct AUI document (`artifact.validated.aui`) produced by the validator.

### **Validator** {#validator}
The subsystem enforcing grammar, semantics, contracts, layout rules, and invariants.

---

## W

### **Wisk** {#wisk}
The intent engine that transforms natural language into raw AUI.

---

# End of Glossary