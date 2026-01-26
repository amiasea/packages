RFC‑012 — Appendix A
Illustrative Rendering Scenarios (Explicitly Not Prescriptive)
Version: 0.1.0
Parent RFC: RFC‑012 — Rendering Semantics

1. Purpose and Scope
This appendix provides examples of how a validated AUI application may be rendered in different environments.
To be explicit:
- These examples do not define a rendering API.
- They do not prescribe a renderer architecture.
- They do not constrain DOM, native, or server implementations.
- They exist solely to help readers understand how RFC‑012’s semantics manifest in practice.
Everything here is illustrative only.

2. Example: Browser DOM Rendering
Given a validated component tree:
App
 └─ Header
 └─ Button.primary


A DOM renderer might produce:
<div data-aui="App">
  <header data-aui="Header"></header>
  <button data-aui="Button.primary">Continue</button>
</div>


Explicit truth:
This demonstrates structural fidelity, not required markup.

3. Example: Native Mobile Rendering
The same validated model might produce:
VStack {
    HeaderView()
    PrimaryButton(label: "Continue")
}


Explicit truth:
This shows semantic equivalence, not a required native API.

4. Example: Server‑Side Snapshot Rendering
A server renderer may output:
<div data-aui="App">
  <header data-aui="Header"></header>
  <button data-aui="Button.primary" disabled="false">Continue</button>
</div>


Explicit truth:
This illustrates static rendering, not a hydration model.

5. Example: Interaction Attachment
Given an interaction:
{
  "event": "onClick",
  "target": "Button.primary",
  "action": { "type": "navigate", "to": "/next" }
}


A renderer might attach:
- a DOM event listener
- a native gesture recognizer
- a server‑side event binding
Explicit truth:
This shows attachment semantics, not a required mechanism.

6. Example: State Update → Re‑Render
If an action updates:
state.user.name = "Alfredo"


A renderer might:
- update a DOM text node
- trigger a SwiftUI state refresh
- regenerate a server snapshot
Explicit truth:
This demonstrates reactivity, not a required update algorithm.

7. Closing Notes
These scenarios:
- illustrate rendering semantics
- demonstrate cross‑platform equivalence
- reinforce determinism
- avoid prescribing implementation details
They are examples, not schemas.
