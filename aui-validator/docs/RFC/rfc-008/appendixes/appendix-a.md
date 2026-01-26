# Appendix A: Minimal Example {#a}

**Status:** Draft  
**Version:** 0.1.0  
**Last Updated:** 2026‑01‑22

---

## A.1 Overview {#a1}

This appendix provides a complete minimal example demonstrating the [AUI](../../glossary/aui-glossary.md#aui) Data Model and [Binding](../../glossary/aui-glossary.md#binding) Engine in action.

---

## A.2 Complete ADM Example {#a2}

```json
{
  "type": "AUI.DataModel",
  "fields": {
    "title": { "type": "string" },
    "items": { "type": "array", "items": { "type": "Item" } }
  },
  "sources": {
    "title": { "source": "prop", "path": "header.title" },
    "items": { "source": "context", "key": "inventory" }
  },
  "transforms": {
    "items": [
      { "op": "filter", "expr": "item.visible == true" }
    ]
  },
  "bind": {
    "Header.title": "title",
    "List.items": "items"
  }
}
```

---

## A.3 Explanation {#a3}

### Fields Definition
- `title`: A simple string field for the header
- `items`: An array of `Item` objects for the list

### Sources
- `title` comes from a parent [component](../../glossary/aui-glossary.md#component) [prop](../../glossary/aui-glossary.md#props) at path `header.title`
- `items` comes from a global context provider with key `inventory`

### Transforms
- The `items` array is filtered to only include items where `visible == true`

### Bindings
- The `title` field is bound to `Header.title` [slot](../../glossary/aui-glossary.md#slot)
- The transformed `items` array is bound to `List.items` [slot](../../glossary/aui-glossary.md#slot)