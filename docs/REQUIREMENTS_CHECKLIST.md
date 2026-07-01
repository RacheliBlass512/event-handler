# Submission Requirements Checklist

Mirrors assignment section 7 (plus the bonus). Check items off only once they're actually
true — code exists and runs, not just discussed.

## Architecture Document
- [ ] Architectural diagram
- [ ] Component descriptions
- [ ] Agent details (architecture, comms, exposure, alternatives, auth, failure handling, onboarding new sources)
- [ ] State Machine (states, transitions, history)
- [ ] Data model
- [ ] Security mechanism
- [ ] Behavior during component failure
- [ ] Trade-offs for every major decision
- [ ] AI tools/sources used, and what was tested/changed/rejected

## E2E Implementation
- [ ] External source → Agent
- [ ] Agent → Server
- [ ] Server → Database
- [ ] Server → Dispatcher receives alert (real-time)

## Skeleton
- [ ] Clear structure for the rest of the system
- [ ] Defined layers
- [ ] Interfaces
- [ ] Stubs for not-yet-implemented parts

## Code Quality
- [ ] Git repository with meaningful history
- [ ] README
- [ ] Compile-ready
- [ ] Unit tests for the State Machine

## Bonus — Offline Mode (high-level description only)
- [ ] What data is stored locally and how
- [ ] How actions sync on reconnection
- [ ] How conflicts are handled
