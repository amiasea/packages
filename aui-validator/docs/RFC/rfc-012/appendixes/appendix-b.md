RFC‑012 — Appendix B
Illustrative Accessibility Mapping Examples (Explicitly Not Prescriptive)
Version: 0.1.0
Parent RFC: RFC‑012 — Rendering Semantics

1. Purpose and Scope
This appendix provides examples of how AUI components and interactions may map to accessibility primitives in different rendering environments.
To be explicit:
- These examples do not define an accessibility API.
- They do not prescribe ARIA roles, native roles, or platform semantics.
- They do not constrain how renderers implement accessibility.
- They exist solely to illustrate how RFC‑012’s rendering semantics interact with accessibility expectations.
Everything here is illustrative only.

2. Example: Button Component → DOM Accessibility
Given a validated component:
Button.primary


A DOM renderer might produce:
<button role="button" aria-disabled="false">Continue</button>


Explicit truth:
This demonstrates a common mapping, not a required one.

3. Example: Input Component → Native Accessibility
Given:
Input.email


A native renderer might produce:
TextField("Email", text: $email)
    .accessibilityLabel("Email")
    .accessibilityHint("Enter your email address")


Explicit truth:
This shows semantic intent, not a required API.

4. Example: Focus Management → Accessibility Focus
Given an action:
{
  "type": "focus.move",
  "to": "Input.password"
}


A renderer might:
- move DOM focus
- move native accessibility focus
- announce the new focus target
Explicit truth:
This illustrates equivalence, not a required mechanism.

5. Example: Visibility → Accessibility Tree Inclusion
If layout resolves:
Button.secondary → hidden


A renderer might:
- remove it from the accessibility tree
- mark it as aria-hidden="true"
- mark it as accessibilityHidden = true
Explicit truth:
This shows expected semantics, not required attributes.

6. Example: Interaction Effects → Accessibility Announcements
Given an effect:
{ "type": "a11y.announce", "message": "Item deleted" }


A renderer might:
- trigger a screen reader announcement
- enqueue a native accessibility notification
Explicit truth:
This demonstrates intent, not a required API.

7. Example: Structural Fidelity → Accessibility Hierarchy
Given a component tree:
Card
 └─ Title
 └─ Description


A renderer might produce:
<div role="group">
  <h2>Title</h2>
  <p>Description</p>
</div>


Explicit truth:
This shows structural mapping, not required roles.

8. Closing Notes
These examples:
- illustrate how accessibility semantics may emerge from rendering
- reinforce that accessibility is part of rendering fidelity
- avoid prescribing platform‑specific APIs
- maintain renderer‑agnostic semantics
They are examples, not schemas.
