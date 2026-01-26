RFC‑011 — Appendix E
Illustrative Action Composition Patterns (Explicitly Not a Schema)
Version: 0.6.0
Parent RFC: RFC‑011 — AUI Interaction Model

1. Purpose and Scope
This appendix provides examples of how actions may be composed within the Interaction Model.
To be explicit:
- This appendix does not define a canonical action vocabulary.
- It does not define a schema for actions.
- It does not define a required structure for multi‑action flows.
- It does not prescribe naming, fields, or action types.
- It does not imply a state machine or imperative execution model.
These examples exist solely to illustrate the shape and intent of action composition as described in RFC‑011:
- deterministic
- declarative
- statically analyzable
- referencing validated components
- never mutating state imperatively
Everything in this appendix is illustrative only.

2. Example Pattern: Simple State Update
A single declarative state update.
"action": {
  "type": "state.update",
  "path": "user.name",
  "value": "form.values.name"
}


Explicit truth:
This demonstrates the idea of updating state declaratively.
It does not define a required action type or field structure.

3. Example Pattern: Navigation Action
A declarative navigation action.
"action": {
  "type": "navigate",
  "to": "/dashboard"
}


Explicit truth:
This shows how navigation might be expressed.
It does not define a routing model or required fields.

4. Example Pattern: Command Dispatch
An action that dispatches a declarative command to a downstream system.
"action": {
  "type": "command.dispatch",
  "command": "CreateOrder",
  "args": { "id": "cart.id" }
}


Explicit truth:
This illustrates the concept of command dispatch.
It does not define a command schema or command bus.

5. Example Pattern: Focus Management
A declarative focus transition.
"action": {
  "type": "focus.move",
  "to": "Input.password"
}


Explicit truth:
This demonstrates focus management as a first‑class action.
It does not define a focus graph or navigation rules.

6. Example Pattern: Multi‑Action Bundle
Actions may be expressed as a list.
This list is declarative, not imperative.
"action": [
  { "type": "state.update", "path": "session.user", "value": "form.values" },
  { "type": "navigate", "to": "/welcome" }
]


Explicit truth:
This shows how multiple actions can be grouped.
It does not define ordering semantics or execution guarantees beyond determinism.

7. Example Pattern: Conditional Multi‑Action Flow
A guard determines whether the action bundle is applied.
"action": [
  { "type": "state.update", "path": "order", "value": "form.values" },
  { "type": "navigate", "to": "/order/confirm" }
]


Explicit truth:
This demonstrates composition, not a required flow structure.

8. Example Pattern: Action Without Navigation
Actions may update state without changing views.
"action": {
  "type": "state.update",
  "path": "filters.query",
  "value": "event.value"
}


Explicit truth:
This illustrates a common pattern, not a required one.

9. Example Pattern: Action With Effects Only
Actions may be omitted entirely when only effects are needed.
{
  "effects": [
    { "type": "analytics", "event": "Hovered" }
  ]
}


Explicit truth:
This shows that actions are optional.
It does not define an effect schema.

10. Example Pattern: No‑Op Action
An interaction may intentionally perform no action.
"action": null


Explicit truth:
This demonstrates that “no action” is a valid declarative choice.
It does not define a canonical representation for no‑ops.

11. Example Pattern: Declarative Error Handling
An action may express a declarative error response.
"action": {
  "type": "state.update",
  "path": "form.error",
  "value": "Invalid credentials"
}


Explicit truth:
This illustrates how error states might be represented.
It does not define an error model beyond RFC‑011’s semantics.

12. Example Pattern: Cross‑Component Interaction
Actions may reference validated component instances.
"action": {
  "type": "focus.move",
  "to": "Modal.login.Input.password"
}


Explicit truth:
This demonstrates referencing component identity.
It does not define a naming convention or component path syntax.

13. Closing Notes
This appendix completes the pedagogical suite for RFC‑011:
- Appendix A → example interaction definitions
- Appendix B → example event types
- Appendix C → example interaction flows
- Appendix D → example guard patterns
- Appendix E → example action patterns
Together, they give a cold reader a complete, intuitive understanding of the Interaction Model without defining schemas, registries, or canonical structures.
The Interaction Model’s power lies in its declarative primitives.
How those primitives are composed is flexible, renderer‑agnostic, and implementation‑defined.

Whenever you're ready, I can take your review and apply full write authority to integrate A–E into RFC‑011 with perfect structural alignment and cross‑referential clarity.
