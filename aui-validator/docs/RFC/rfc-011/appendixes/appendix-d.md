RFC‑011 — Appendix D
Illustrative Guard Expression Patterns (Explicitly Not a Grammar)
Version: 0.6.0
Parent RFC: RFC‑011 — AUI Interaction Model

1. Purpose and Scope
This appendix provides examples of guard expression patterns to help readers understand how guards are used declaratively within the Interaction Model.
To be explicit:
- This appendix does not define a guard language.
- It does not define a grammar.
- It does not define a syntax.
- It does not constrain how implementations parse or evaluate guards.
- It does not prescribe naming, operators, or expression structure.
These examples exist solely to illustrate the shape and intent of guard expressions as described in RFC‑011:
- pure
- deterministic
- statically analyzable
- referencing resolved data
- referencing event payloads
- never mutating state
Everything in this appendix is illustrative only.

2. Example Pattern: Simple Boolean Check
A guard that checks a single resolved data field.
"guard": "user.isLoggedIn == true"


Explicit truth:
This is one possible boolean check.
It does not imply a required operator set or expression syntax.

3. Example Pattern: Length or Count Checks
A common pattern for validating collections.
"guard": "cart.items.length > 0"


Explicit truth:
This demonstrates the intent of checking collection size, not a required property name or method.

4. Example Pattern: Event‑Driven Conditions
Guards may reference event payloads, as long as they remain pure and analyzable.
"guard": "event.key == 'Enter'"


Explicit truth:
This shows how event payloads can be referenced.
It does not define the structure of event.

5. Example Pattern: Composite Boolean Expressions
Guards may combine multiple conditions declaratively.
"guard": "form.isValid == true && user.isAuthenticated == true"


Explicit truth:
This illustrates composition, not a canonical operator set.

6. Example Pattern: Negation
A simple negation pattern.
"guard": "!profile.isComplete"


Explicit truth:
This demonstrates intent, not a required negation operator.

7. Example Pattern: Branching via Multiple Guards
Branching is expressed through multiple interaction definitions, not imperative logic.
"guard": "profile.isComplete == true"


"guard": "profile.isComplete == false"


Explicit truth:
This pattern shows how declarative branching works.
It does not define a branching language.

8. Example Pattern: Guarding Against Undefined or Missing Data
Guards may ensure required data is present.
"guard": "order.total != null"


Explicit truth:
This is an example of defensive checking, not a required null‑checking idiom.

9. Example Pattern: Guards Referencing Derived or Computed Values
Guards may reference values produced by upstream resolution (RFC‑008).
"guard": "pricing.finalTotal <= budget.limit"


Explicit truth:
This demonstrates referencing resolved data, not a required naming convention.

10. Example Pattern: Guards Without Actions
Guards may be used to prevent an event from firing at all.
{
  "type": "AUI.Interaction",
  "event": "onClick",
  "target": "Button.submit",
  "guard": "form.isValid == false"
}


Explicit truth:
This example shows that guards can block interactions.
It does not imply a required behavior for guard‑only definitions.

11. Example Pattern: Guards With Effects Only
Guards may allow effects to fire without actions.
{
  "type": "AUI.Interaction",
  "event": "onHover",
  "target": "Card.product",
  "guard": "product.isAvailable == true",
  "effects": [
    { "type": "analytics", "event": "ProductHovered" }
  ]
}


Explicit truth:
This illustrates a valid pattern, not a required structure.

12. Closing Notes
This appendix mirrors the explicitness of A, B, and C:
- It provides examples, not definitions.
- It illustrates patterns, not grammars.
- It shows possibilities, not constraints.
- It reinforces declarative purity, not syntax.
Guards in AUI are intentionally flexible.
Their only requirements come from RFC‑011’s semantics: purity, determinism, analyzability, and reference to validated data.
