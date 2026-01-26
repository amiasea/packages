RFC‑011 — Appendix A
Non‑Normative: Example Interaction Definitions
Status: Non‑Normative**
Version: 0.6.0
Parent RFC: RFC‑011 — AUI Interaction Model
Purpose: Provide illustrative examples of valid interaction definitions.
Scope: These examples do not define required structure or semantics.
They exist solely to help readers understand the Interaction Model.

1. Introduction
The Interaction Model supports multiple valid shapes for expressing events, guards, actions, and effects.
This appendix provides illustrative examples of interaction definitions that conform to the rules in RFC‑011.
These examples:
- are not prescriptive
- are not exhaustive
- are not a canonical schema
- are intended to build intuition for implementers and tool authors
All examples assume that component validation (RFC‑010) and data resolution (RFC‑008) have already occurred.

2. Minimal Interaction Definition
A minimal, valid interaction definition includes:
- an event
- a target
- an action
{
  "type": "AUI.Interaction",
  "event": "onClick",
  "target": "Button.primary",
  "action": {
    "type": "navigate",
    "to": "/home"
  }
}


This example omits guards and effects, both of which are optional.

3. Interaction With Guard
Guards are declarative boolean expressions referencing resolved data.
{
  "type": "AUI.Interaction",
  "event": "onSubmit",
  "target": "Form.login",
  "guard": "user.isAuthenticated == false",
  "action": {
    "type": "state.update",
    "path": "session.user",
    "value": "form.values"
  }
}


Notes:
- Guard must be pure.
- Guard must reference resolved data fields.
- Guard must be statically analyzable.

4. Interaction With Multiple Actions
Actions may be expressed as a list.
This does not imply imperative sequencing; it is a declarative bundle consumed by downstream systems.
{
  "type": "AUI.Interaction",
  "event": "onSubmit",
  "target": "Form.settings",
  "guard": "form.isValid == true",
  "action": [
    { "type": "state.update", "path": "settings", "value": "form.values" },
    { "type": "navigate", "to": "/settings/success" }
  ]
}



5. Interaction With Effects
Effects annotate expected side effects but are not executed by the Interaction Model.
{
  "type": "AUI.Interaction",
  "event": "onClick",
  "target": "Button.delete",
  "guard": "item.canDelete == true",
  "action": {
    "type": "command.dispatch",
    "command": "DeleteItem",
    "args": { "id": "item.id" }
  },
  "effects": [
    { "type": "analytics", "event": "ItemDeleted" },
    { "type": "a11y.announce", "message": "Item deleted" }
  ]
}



6. Interaction Targeting Component Parts
Targets may reference component parts or slots validated by RFC‑010.
{
  "type": "AUI.Interaction",
  "event": "onChange",
  "target": "Input.search.value",
  "action": {
    "type": "state.update",
    "path": "filters.query",
    "value": "event.value"
  }
}



7. Interaction With Focus Management
Focus actions are first‑class and declarative.
{
  "type": "AUI.Interaction",
  "event": "onKeyDown",
  "target": "Input.email",
  "guard": "event.key == 'Enter'",
  "action": {
    "type": "focus.move",
    "to": "Input.password"
  }
}



8. Interaction With Conditional Branching (Declarative)
Declarative branching is expressed through guards, not imperative logic.
{
  "type": "AUI.Interaction",
  "event": "onClick",
  "target": "Button.continue",
  "guard": "profile.isComplete == true",
  "action": {
    "type": "navigate",
    "to": "/next-step"
  }
}


A separate interaction definition would handle the incomplete case.

9. Composite Interaction Example
A more complete example combining multiple features:
{
  "type": "AUI.Interaction",
  "event": "onSubmit",
  "target": "Form.checkout",
  "guard": "cart.items.length > 0 && form.isValid == true",
  "action": [
    { "type": "state.update", "path": "order", "value": "form.values" },
    { "type": "navigate", "to": "/order/confirm" }
  ],
  "effects": [
    { "type": "analytics", "event": "CheckoutSubmitted" }
  ]
}



10. Closing Notes
These examples demonstrate:
- the flexibility of the Interaction Model
- the declarative nature of events, guards, actions, and effects
- the separation between behavior semantics and rendering semantics
- the validator‑friendly structure of interaction definitions
This appendix is intentionally illustrative and does not constrain implementations.
