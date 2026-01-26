RFC‑012: AUI Rendering Semantics
Status: Draft
Author: Alfredo
Category: Rendering / Execution Semantics
Version: 0.1.0
Last Updated: 2026‑01‑22

1. Overview
The AUI Rendering Semantics define how a fully validated AUI application is rendered into a concrete output (DOM, native views, canvas, etc.) in a deterministic, declarative way.
Upstream subsystems provide:
- RFC‑008 — Data Model: resolved data and bindings
- RFC‑009 — Layout Engine: spatial structure
- RFC‑010 — Component Model: validated components and contracts
- RFC‑011 — Interaction Model: validated interaction definitions
RFC‑012 consumes these and defines:
- how components become renderable instances
- how layout is realized in a concrete rendering environment
- how interaction hooks are attached to rendered elements
- how state changes propagate to rendered output
Rendering is the final step where the system must answer, with no ambiguity:
“Given this validated model, what exactly appears, and how does it behave, in this environment?”


2. Why This RFC Exists
Without a formal rendering semantics, AUI would risk:
- Renderer drift: different platforms interpreting the same model differently
- Hidden behavior: rendering engines smuggling in extra semantics
- Non‑determinism: identical inputs producing divergent outputs
- Tooling gaps: no stable contract for renderers, simulators, or visual diff tools
This RFC exists to:
- define a renderer‑agnostic semantic contract
- ensure deterministic rendering given the same validated inputs
- separate what is rendered from how a platform implements it
- enable testing, simulation, and visual diffing against a stable semantic model
RFC‑012 does not define a specific renderer implementation.
It defines the rules any conforming renderer must follow.

3. Architectural Positioning
Rendering Semantics sit at the end of the AUI pipeline:
- Data Model (RFC‑008) — resolves data and bindings
- Layout Engine (RFC‑009) — resolves spatial structure
- Component Model (RFC‑010) — validates components and contracts
- Interaction Model (RFC‑011) — defines behavior
- Rendering Semantics (RFC‑012) — realizes the UI in a concrete environment
Rendering must:
- not reinterpret upstream semantics
- not introduce new behavior
- not bypass validation
- not mutate upstream models
It is a pure consumer of validated models, responsible for producing a concrete, observable UI.

4. Rendering inputs
A conforming renderer receives a fully validated application model, including:
- Component tree: instances, parts, slots, and structural metadata
- Layout tree: resolved positions, sizes, and visibility
- Data bindings: resolved values and binding metadata
- Interaction definitions: events, guards, actions, effects
- Theme / styling model (if defined elsewhere): resolved visual tokens
The renderer must treat these inputs as authoritative.
If they are invalid, the error lies upstream, not in rendering.

5. Rendering outputs
Rendering outputs are environment‑specific, but semantically equivalent across platforms.
Examples:
- DOM nodes in a browser
- Native views in a mobile runtime
- Canvas or WebGL primitives
- Server‑side HTML snapshots
RFC‑012 does not prescribe a specific output format.
It requires that:
- the observable structure matches the validated component and layout models
- the observable behavior matches the Interaction Model
- the visual semantics (where defined) are consistent with upstream tokens

6. Component rendering semantics
6.1 Component instance realization
For each validated component instance, the renderer must:
- create a corresponding rendered entity (e.g., DOM subtree, native view hierarchy)
- respect parts and slots as defined in RFC‑010
- ensure identity is preserved for interaction and state mapping
6.2 Structural fidelity
The rendered structure must:
- reflect the validated component hierarchy
- respect conditional branches and visibility rules from layout
- avoid introducing extra semantic nodes that change behavior
Renderers may introduce internal nodes for performance or platform reasons, but these must not alter the semantic structure as observed by interactions and accessibility.

7. Layout rendering semantics
7.1 Layout realization
The renderer must:
- realize the resolved layout tree from RFC‑009
- respect constraints such as size, position, and flow
- honor visibility, branching, and conditional inclusion
7.2 Non‑interference
Renderers may apply platform‑specific layout optimizations, but:
- must not change interaction hit‑targets
- must not change visibility semantics (e.g., hidden vs. removed)
- must not reorder elements in a way that breaks interaction or accessibility expectations

8. Interaction attachment semantics
Rendering is where interaction definitions become concrete event hooks.
8.1 Event binding
For each validated interaction definition (RFC‑011), the renderer must:
- bind the specified event to the correct target (component instance or part)
- ensure the event is routed to the Interaction Model for guard evaluation and action execution
- avoid attaching extra, implicit behavior not described in the Interaction Model
8.2 Guard and action execution
When an event fires:
- The renderer forwards the event payload to the Interaction Model.
- The Interaction Model evaluates guards and actions.
- The renderer applies any resulting state changes and view updates.
The renderer must not:
- bypass guard evaluation
- execute actions out of band
- introduce additional side effects beyond those described by the model

9. State and re‑render semantics
9.1 State changes
When state changes occur (e.g., via actions):
- the data model is updated according to RFC‑008
- the component and layout models may be recomputed if necessary
- the renderer receives updated models and must reconcile them with the current output
9.2 Reconciliation
Renderers may use any reconciliation strategy (e.g., virtual DOM, diffing, keyed updates), but must ensure:
- semantic equivalence between pre‑ and post‑update models
- deterministic results given the same inputs
- no lost interactions (e.g., focus, selection) unless explicitly dictated by the model

10. Error model
Rendering errors occur when:
- the renderer cannot realize a valid model in the target environment
- platform constraints prevent a faithful representation
- required capabilities are missing
A rendering error must:
- be reported as a RenderingError (shape is implementation‑defined)
- identify the component or interaction that could not be rendered
- avoid silently degrading semantics without explicit configuration
Rendering errors are distinct from:
- InteractionError (RFC‑011)
- Component validation errors (RFC‑010)
- Data resolution errors (RFC‑008)

11. Integration points
11.1 With RFC‑008 (Data Model)
Rendering consumes:
- resolved values
- binding metadata
- state updates produced by actions
11.2 With RFC‑009 (Layout Engine)
Rendering consumes:
- resolved layout tree
- visibility and branching decisions
- spatial relationships
11.3 With RFC‑010 (Component Model)
Rendering consumes:
- validated component instances
- parts and slots
- structural metadata
11.4 With RFC‑011 (Interaction Model)
Rendering consumes:
- validated interaction definitions
- event‑to‑component mappings
- action and effect semantics

12. Determinism requirements
Given the same:
- data model
- layout model
- component model
- interaction model
- rendering configuration
a conforming renderer must produce semantically equivalent output across runs and environments.
Determinism includes:
- consistent structure
- consistent behavior
- consistent state transitions

13. Versioning
Semantic versioning applies:
- MAJOR: breaking changes to rendering semantics
- MINOR: new capabilities that do not break existing semantics
- PATCH: clarifications and non‑breaking fixes

14. Potential appendixes (for your approval)
Not creating them yet—just identifying and justifying:
- Appendix A — Illustrative Rendering Scenarios
- Example end‑to‑end renderings (DOM, native, server‑side)
- Justification: helps cold readers see how the same model appears in different environments.
- Appendix B — Accessibility Mapping Examples
- How components and interactions map to accessibility primitives (roles, labels, focus order).
- Justification: critical for understanding that rendering semantics include a11y semantics.
- Appendix C — Reconciliation Patterns (Illustrative)
- Examples of how updates are applied without prescribing an algorithm.
- Justification: clarifies expectations around stability, keys, and identity.
