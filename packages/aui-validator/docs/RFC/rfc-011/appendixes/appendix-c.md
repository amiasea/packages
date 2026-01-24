RFC‑011 — Appendix C
Illustrative Interaction Flow Examples (Explicitly Not Prescriptive)
Version: 0.6.0
Parent RFC: RFC‑011 — AUI Interaction Model

1. Purpose and Scope
This appendix provides examples of interaction flows composed from events, guards, actions, and effects.
These examples:
- do not define a canonical flow language
- do not define a required structure
- do not imply a state machine schema
- do not constrain implementations
- do not represent best practices or recommendations
Their purpose is to help readers understand how the primitives defined in RFC‑011 can be assembled into coherent, declarative flows.
Everything in this appendix is illustrative only.

2. Example: Conditional Navigation Flow
This example demonstrates a simple flow where a guard determines whether navigation is allowed.
{
  "type": "AUI.Interaction",
  "event": "onClick",
  "target": "Button.checkout",
  "guard": "cart.items.length > 0",
  "action": {
    "type": "navigate",
    "to": "/checkout"
  },
  "effects": [
    { "type": "analytics", "event": "CheckoutAttempt" }
  ]
}


Explicit truth:
This is one possible way to express conditional navigation.
It is not a template, schema, or required pattern.

3. Example: Multi‑Step Declarative Flow
This example shows how multiple deterministic actions can be expressed without implying imperative sequencing.
{
  "type": "AUI.Interaction",
  "event": "onSubmit",
  "target": "Form.profile",
  "guard": "form.isValid == true",
  "action": [
    { "type": "state.update", "path": "user.profile", "value": "form.values" },
    { "type": "navigate", "to": "/profile/success" }
  ]
}


Explicit truth:
The list of actions is declarative.
Downstream systems determine how to consume it.
This is an example of a multi‑step flow, not a required structure.

4. Example: Focus‑Driven Interaction Flow
This example illustrates how focus management can be expressed declaratively.
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


Explicit truth:
Focus flows vary across platforms.
This example demonstrates possibility, not prescription.

5. Example: Branching via Multiple Interactions
Branching is expressed through multiple interaction definitions, not imperative logic.
{
  "type": "AUI.Interaction",
  "event": "onClick",
  "target": "Button.continue",
  "guard": "profile.isComplete == true",
  "action": { "type": "navigate", "to": "/next-step" }
}


{
  "type": "AUI.Interaction",
  "event": "onClick",
  "target": "Button.continue",
  "guard": "profile.isComplete == false",
  "action": { "type": "focus.move", "to": "Form.profile" }
}


Explicit truth:
This demonstrates how declarative branching works in AUI.
It is not a required pattern or a canonical branching model.

6. Example: Interaction Flow With Effects Only
Effects annotate expected side effects without altering behavior.
{
  "type": "AUI.Interaction",
  "event": "onHover",
  "target": "Card.product",
  "effects": [
    { "type": "analytics", "event": "ProductPreview" }
  ]
}


Explicit truth:
Effects are optional and declarative.
This example shows how they can be used without actions.

7. Example: Composite Flow With Guard, State, Navigation, and Effects
A more complete example combining multiple primitives.
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


Explicit truth:
This example demonstrates how multiple primitives can be composed.
It does not define a canonical “checkout flow” or any required structure.

8. Closing Notes
This appendix intentionally mirrors the explicitness of Appendix A and Appendix B:
- It shows examples, not schemas.
- It demonstrates possibilities, not requirements.
- It illustrates patterns, not canonical flows.
- It prioritizes clarity for cold readers over brevity.
The Interaction Model’s strength lies in its declarative primitives.
How those primitives are composed is flexible, renderer‑agnostic, and implementation‑defined.

If you want, I can now generate Appendix D or Appendix E in the same style — or help you refactor RFC‑011 to link to these appendixes with perfect cross‑referential clarity.
