# Appendix B: Reserved Keywords {#b}

**Status:** Draft  
**Version:** 0.1.0  
**Last Updated:** 2026‑01‑22

---

## B.1 Overview {#b1}

This appendix defines the reserved keywords used in the [AUI](../../glossary/aui-glossary.md#aui) Data Model and [Binding](../../glossary/aui-glossary.md#binding) Engine. These keywords have special meaning and MUST NOT be used as [identifiers](../../glossary/aui-glossary.md#identifier) for user‑defined fields or operations.

---

## B.2 Reserved Keywords {#b2}

### Core Keywords
- `source` — Defines data origin
- `transform` — Defines data transformation operations
- `bind` — Defines [binding](../../glossary/aui-glossary.md#binding) mappings
- `compute` — Defines computed fields

### Source Type Keywords
- `context` — Global or scoped [data context](../../glossary/aui-glossary.md#data-context) provider
- `prop` — Parent‑provided [props](../../glossary/aui-glossary.md#props)
- `static` — Literal values

---

## B.3 Validation Rules {#b3}

The [AUI Validator](../../glossary/aui-glossary.md#validator) MUST:
- Reject any user‑defined [identifiers](../../glossary/aui-glossary.md#identifier) that match reserved keywords
- Enforce proper usage of reserved keywords in their designated contexts
- Emit `AUI_RESERVED_KEYWORD_VIOLATION` error when violations are detected