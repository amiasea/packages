RFC‑004 — Appendix A
Normative Definitions and Canonical Primitives
🧩 A.1 Purpose
Appendix A establishes the canonical primitives, baseline semantics, and non‑negotiable invariants required for any implementation of RFC‑004.
This appendix functions as the root schema for the entire document: if RFC‑004 is the organism, Appendix A is its genetic code.

A.2 Core Terminology
A.2.1 “Context Frame”
A Context Frame is the smallest addressable semantic unit.
It contains:
- A declared scope
- A state vector
- A resolution rule
- A boundary condition
A Context Frame MUST be:
- Deterministic within its declared scope
- Immutable once resolved
- Addressable by a stable identifier
A.2.2 “Actor Node”
An Actor Node is any entity capable of:
- Emitting signals
- Interpreting frames
- Mutating local state
Actor Nodes MAY be human, machine, composite, or abstract.
Actor Nodes MUST NOT assume global authority unless explicitly granted.
A.2.3 “Semantic Channel”
A Semantic Channel is a unidirectional or bidirectional conduit for meaning.
Channels define:
- Encoding
- Latency tolerance
- Error‑handling strategy
- Allowed transformations
A Channel MUST preserve intent even when content is transformed.

A.3 Primitive Operations
A.3.1 BIND
BIND(source, target) → link


Creates a stable association between two entities.
BIND MUST NOT introduce ambiguity; if ambiguity is detected, the operation fails.
A.3.2 RESOLVE
RESOLVE(frame) → value | error


Executes the frame’s resolution rule.
RESOLVE MUST be idempotent.
A.3.3 EMIT
EMIT(actor, signal) → propagation


Broadcasts a signal across a channel.
EMIT MUST respect channel constraints.
A.3.4 NEGOTIATE
NEGOTIATE(a, b, constraints) → agreement | null


Attempts to converge two Actor Nodes toward a shared state.
NEGOTIATE MUST terminate.

A.4 Structural Invariants
A.4.1 Deterministic Resolution
Every Context Frame MUST resolve deterministically within its declared scope.
A.4.2 No Implicit Authority
No Actor Node may assume authority without explicit delegation.
A.4.3 Channel Integrity
Semantic Channels MUST preserve:
- Intent
- Ordering
- Boundary conditions
Transformations are allowed but MUST NOT distort meaning.
A.4.4 Temporal Coherence
Operations MUST be evaluated in a temporally coherent order.
If coherence cannot be guaranteed, the system MUST enter a safe fallback state.

A.5 Compliance Matrix
| Requirement | MUST | Evidence | Notes | Status |
| --- | --- | --- | --- | --- |
| Context Frame determinism | Yes |  |  |  |
| Context Frame immutability | Yes |  |  |  |
| Actor Node authority limits | Yes |  |  |  |
| Semantic Channel intent preservation | Yes |  |  |  |
| Temporal coherence | Yes |  |  |  |



A.6 Reference Implementation Notes
These notes are non‑normative but recommended for implementers:
- Treat Context Frames as immutable after resolution.
- Prefer explicit over inferred semantics.
- Maintain a clear audit trail of BIND and NEGOTIATE operations.
- Avoid recursive NEGOTIATE chains deeper than 4 layers unless necessary.

A.7 Versioning
Appendix A follows semantic versioning:
- MAJOR increments: breaking changes to primitives
- MINOR increments: new primitives or expanded semantics
- PATCH increments: clarifications or error corrections
Current version: A.1.0
