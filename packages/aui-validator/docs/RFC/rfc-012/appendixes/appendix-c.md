RFC‑012 — Appendix C
Illustrative Reconciliation Patterns (Explicitly Not Prescriptive)
Version: 0.1.0
Parent RFC: RFC‑012 — Rendering Semantics

1. Purpose and Scope
This appendix provides examples of how a renderer may reconcile updates to the validated AUI model with the currently rendered output.
To be explicit:
- These examples do not define a reconciliation algorithm.
- They do not prescribe diffing, VDOM, keyed updates, or mutation strategies.
- They do not constrain renderer performance optimizations.
- They exist solely to illustrate how RFC‑012’s determinism and fidelity requirements manifest during updates.
Everything here is illustrative only.

2. Example: Keyed Element Update
Given a list:
Items = [A, B, C]


Rendered as:
<li data-key="A"></li>
<li data-key="B"></li>
<li data-key="C"></li>


If the model updates to:
Items = [A, C, D]


A renderer might:
- preserve A
- preserve C
- remove B
- insert D
Explicit truth:
This demonstrates identity preservation, not a required keyed diff algorithm.

3. Example: Structural Branch Change
Given a conditional layout:
if user.isLoggedIn:
    show Dashboard
else:
    show LoginForm


If state changes from logged‑out → logged‑in, a renderer might:
- remove LoginForm subtree
- insert Dashboard subtree
Explicit truth:
This illustrates branch switching, not a required mechanism for subtree replacement.

4. Example: Minimal DOM Mutation
Given a component:
Title(text="Hello")


If state updates to:
Title(text="Hello, Alfredo")


A renderer might:
- update only the text node
- avoid re‑creating the entire element
Explicit truth:
This shows minimal mutation, not a required optimization.

5. Example: Native View Reuse
Given:
Button.primary(label="Continue")


If label updates to:
"Next"


A native renderer might:
- reuse the existing view
- update only the label property
Explicit truth:
This demonstrates view reuse, not a required lifecycle model.

6. Example: Focus Preservation
If a user is focused on:
Input.email


And a state update re‑renders the parent component, a renderer might:
- preserve focus on Input.email
- unless the model explicitly removes or replaces it
Explicit truth:
This illustrates expected user‑experience stability, not a required focus API.

7. Example: Server Snapshot Regeneration
A server renderer may:
- regenerate the entire HTML snapshot
- but maintain semantic equivalence with the validated model
Explicit truth:
This shows a coarse‑grained reconciliation strategy, not a required one.

8. Closing Notes
These patterns:
- illustrate how renderers may reconcile updates
- reinforce determinism and identity preservation
- avoid prescribing algorithms or data structures
- maintain renderer‑agnostic semantics
They are examples, not schemas.
