Appendix D — AUI DSL Grammar (Canonical)
D.1 Purpose {#d1}
Appendix D defines the formal [grammar](../../glossary/aui-glossary.md#grammar) of the [AUI](../../glossary/aui-glossary.md#aui) Language as a [deterministic](../../glossary/aui-glossary.md#determinism), JSON‑compatible domain‑specific language (DSL).
This grammar is:
- structural, not behavioral
- [renderer](../../glossary/aui-glossary.md#renderer)‑agnostic
- [validator](../../glossary/aui-glossary.md#validator)‑friendly
- deterministic and unambiguous
- compatible with standard JSON parsers
It defines shape, ordering, and syntactic constraints, not semantics (covered in Appendix A and B).

D.2 Grammar Notation {#d2}
The grammar uses an extended EBNF‑style notation adapted for JSON:
- object → { pair ( "," pair )* }
- array → [ value ( "," value )* ]
- pair → string ":" value
- value → string | number | boolean | null | object | array
- ? → optional
- * → zero or more
- + → one or more
This grammar defines syntactic shape only.

D.3 Top‑Level Document Grammar {#d3}
auiDocument = object {
  "version": string,
  "type": "aui",
  "root": node,
  "metadata": object?
}


Constraints:
- version MUST be a semantic version string
- type MUST equal "aui"
- root MUST be a valid [node](../../glossary/aui-glossary.md#node)
- metadata MUST be an object if present

D.4 [Node](../../glossary/aui-glossary.md#node) Grammar {#d4}
node = object {
  "[kind](../../glossary/aui-glossary.md#kind)": string,
  "[props](../../glossary/aui-glossary.md#props)": object?,
  "[parts](../../glossary/aui-glossary.md#parts)": partsObject?,
  "[children](../../glossary/aui-glossary.md#children)": childrenArray?,
  "[metadata](../../glossary/aui-glossary.md#metadata)": object?
}


D.4.1 Exclusivity Rule {#d41}
(partsObject and childrenArray) MUST NOT coexist
unless explicitly allowed by the [component contract](../../glossary/aui-glossary.md#component-contract).


D.4.2 Node Ordering {#d42}
The following key order is RECOMMENDED (not required):
- kind
- props
- parts
- children
- metadata

D.5 Props Grammar {#d5}
props = object {
  ( string : value )*
}


Constraints:
- values MUST be JSON‑serializable
- values MUST NOT contain renderer‑specific constructs
- values MUST NOT contain executable code

D.6 Parts Grammar {#d6}
partsObject = object {
  ( partName : node )*
}

partName = identifier
identifier = /^[a-zA-Z_][a-zA-Z0-9_]*$/


Constraints:
- part names MUST match the component contract
- part values MUST be valid nodes
- parts MUST be objects, not arrays

D.7 Children Grammar {#d7}
childrenArray = [
  node ( "," node )*
]


Constraints:
- children MUST be ordered
- children MUST be valid nodes
- children MUST NOT appear with parts unless contract allows

D.8 Metadata Grammar {#d8}
metadata = object {
  ( string : value )*
}


Metadata is:
- non‑semantic
- optional
- ignored by renderers unless explicitly supported

D.9 [Identifier](../../glossary/aui-glossary.md#identifier) Grammar {#d9}
Identifiers appear in:
- part names
- component names
- [binding](../../glossary/aui-glossary.md#binding) paths
- [action](../../glossary/aui-glossary.md#action) names
identifier = /^[a-zA-Z_][a-zA-Z0-9_]*$/
path       = identifier ( "." identifier )*


Constraints:
- identifiers MUST NOT contain whitespace
- paths MUST NOT be empty
- paths MUST NOT begin or end with .

D.10 [Binding](../../glossary/aui-glossary.md#binding) Grammar {#d10}
Bindings appear inside [props](../../glossary/aui-glossary.md#props) or semantic fields.
binding = string matching path


Examples:
- "user.name"
- "item.id"
- "form.email.value"
Constraints:
- bindings MUST NOT contain expressions
- bindings MUST NOT contain operators
- bindings MUST NOT contain template syntax

D.11 [Action](../../glossary/aui-glossary.md#action) Grammar {#d11}
Actions are symbolic references.
action = string matching identifier


Constraints:
- MUST NOT contain arguments
- MUST NOT contain code
- MUST NOT contain parentheses

D.12 Component Contract Grammar (Reference Only) {#d12}
Component contracts (defined outside the AUI document) follow:
componentContract = object {
  "kind": identifier,
  "props": object?,
  "parts": object?
}


This grammar is referenced by the validator but not embedded in AUI documents.

D.13 Complete Grammar Summary {#d13}
auiDocument =
  object {
    "version": string,
    "type": "aui",
    "root": node,
    "metadata": object?
  }

node =
  object {
    "kind": string,
    "props": object?,
    "parts": object?,
    "children": array?,
    "metadata": object?
  }

props =
  object { ( string : value )* }

parts =
  object { ( identifier : node )* }

children =
  array [ node* ]

identifier =
  /^[a-zA-Z_][a-zA-Z0-9_]*$/

path =
  identifier ( "." identifier )*

binding = string matching path
action  = string matching identifier



D.14 Compliance Requirements {#d14}
A parser is compliant if it:
- accepts all documents matching this grammar
- rejects documents that violate structural rules
- preserves ordering of arrays
- preserves string values exactly
- does not infer semantics
A compliant parser MUST NOT:
- rewrite the document
- infer missing fields
- evaluate bindings
- evaluate actions
