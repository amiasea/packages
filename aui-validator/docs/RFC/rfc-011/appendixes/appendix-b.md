RFC‑011 — Appendix B
Illustrative Event Examples (Explicitly Not a Schema)
Version: 0.6.0
Parent RFC: RFC‑011 — AUI Interaction Model

1. Purpose and Scope
This appendix provides examples of event types that an AUI implementation might support.
These examples exist solely to help readers understand the range of interactions the model can represent.
To be explicit:
- This appendix does not define a required event vocabulary.
- It does not define a schema.
- It does not define a registry.
- It does not constrain implementations.
- It does not imply that these names are preferred, recommended, or canonical.
Every event name in this appendix is illustrative only.
Implementations MAY define any event types they need, as long as they conform to the semantics in RFC‑011.

2. Activation Event Examples
These examples illustrate common activation patterns across UI systems.
They are included because they are familiar, not because they are required.
|  |  |  | 
|  | onClickonPressonTap |  | 
|  | onSubmit |  | 
|  | onToggleonSwitch |  | 


Explicit truth:
These names are placeholders.
They demonstrate the shape of activation events, not the set of activation events.

3. Pointer & Hover Event Examples
These examples show how pointer‑driven interactions might be expressed declaratively.
|  |  |  | 
|  | onHoveronHoverStartonHoverEnd |  | 
|  | onPointerMoveonPointerEnteronPointerLeave |  | 
|  | onPointerDownonPointerUp |  | 


Explicit truth:
Pointer semantics vary dramatically across platforms.
These examples are not normative in any way.

4. Focus & Blur Event Examples
These examples illustrate how focus‑related flows can be represented.
|  |  |  | 
|  | onFocus |  | 
|  | onBlur |  | 
|  | onFocusNextonFocusPrevious |  | 


Explicit truth:
Focus systems differ across platforms; these names are merely illustrative.

5. Keyboard Event Examples
These examples show how keyboard interactions might be expressed.
|  |  |  | 
|  | onKeyDown |  | 
|  | onKeyUp |  | 
|  | onKeyPress |  | 


Explicit truth:
Keyboard models differ widely; these names are not prescriptive.

6. Form & Input Event Examples
These examples illustrate input‑driven flows.
|  |  |  | 
|  | onChange |  | 
|  | onInput |  | 
|  | onValidateonInvalid |  | 


Explicit truth:
These examples exist because they are common patterns, not because they are required patterns.

7. Gesture Event Examples
These examples illustrate how gesture semantics could be represented.
|  |  |  | 
|  | onSwipeLeftonSwipeRight |  | 
|  | onPinchonPinchStartonPinchEnd |  | 
|  | onLongPress |  | 


Explicit truth:
Gesture vocabularies differ across ecosystems; these names are not authoritative.

8. Composite & High‑Level Event Examples
These examples illustrate how higher‑level semantics might be expressed.
|  |  |  | 
|  | onLoadonReady |  | 
|  | onVisibleonHidden |  | 
|  | onCustomEvent |  | 


Explicit truth:
High‑level events are entirely implementation‑defined; these examples are conceptual only.

9. Closing Notes
This appendix is intentionally explicit in its purpose:
- It provides examples, not definitions.
- It illustrates possibilities, not requirements.
- It shows patterns, not schemas.
- It demonstrates breadth, not boundaries.
The Interaction Model’s power comes from its declarative semantics, not from any fixed event vocabulary.
