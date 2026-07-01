# Architecture Document — Real-Time Field Event Management System

> Living document, kept short on purpose. Full reasoning for each decision lives in the
> [Decision Log](#decision-log) — sections above it stay to-the-point.

## 1. Overview
Centralizes field-event handling (sensors, external systems, manual reports) that today
happens by phone: intake → assignment → real-time dispatcher/technician communication →
closure. Events are small (title, description, source, location); this is a real-time
messaging problem, not a data/file processing one.

## 2. Diagram
_TBD_

## 3. Components

**Central Agent** — background service collecting events from sources, forwarding to the
Server.
- Architecture: _TBD_ · Comms to Server: _TBD_ · External exposure: _TBD_
- Source auth: _TBD_ · Behavior if Server is down: _TBD_ · Onboarding a new source: _TBD_

**Central Server (Backend)** — .NET Core / ASP.NET Core (Decision Log #1).
- Owns: event intake, State Machine, permissions, technician presence, alerts,
  assignment/transfer, persistence.

**Frontend (Angular)** — per-role views, real-time updates. Delivery mechanism: _TBD_

## 4. Users
- **Dispatcher:** sees all events, assigns/transfers/closes/prioritizes, alerts even when
  browser is closed, dashboard of technicians/events.
- **Technician:** sees only assigned events, updates status, requests events, notes to
  dispatcher, alerts even when browser is closed.

## 5. Real-Time
- Mode A (open): live updates, no refresh. Mode B (closed): push alert → deep link.
- Server tracking of connect/disconnect: _TBD_ · Mode transition handling: _TBD_ ·
  Subscription lifecycle: _TBD_

## 6. State Machine
- States: Initial → Assigned → (≥1 intermediate) → Closed / Canceled. Exact set: _TBD_
- Transitions defined in code; every change stamped with time + user; history shown in UI.

## 7. Data Model
_TBD — Event, EventStatusHistory, User (Dispatcher/Technician), Source,
Assignment/Transfer, at minimum._

## 8. Security
- User auth: _TBD_ · Source auth: _TBD_ · Server-enforced permissions: _TBD_ ·
  Transport encryption: _TBD_

## 9. Failure Handling
- Server down: _TBD_ · Agent/source unreachable: _TBD_ · Technician disconnects
  mid-assignment: _TBD_

## 10. Offline Mode (Bonus)
- Local storage: _TBD_ · Sync on reconnect: _TBD_ · Conflict handling: _TBD_

## Decision Log
The real record — one row per decision that matters, added the moment it's made.

| # | Decision | Alternatives | Rejected because | Trade-off accepted |
|---|----------|--------------|-------------------|---------------------|
| 1 | Backend: .NET Core (ASP.NET Core, C#) | Python (FastAPI/Django + separate real-time layer) | _needs your reason_ | _needs your input_ |

## Open Decisions
- [x] Backend platform → #1 (rationale still needs your input)
- [ ] Agent architecture, deployment shape
- [ ] Agent ↔ Server transport
- [ ] Agent external exposure
- [ ] Source auth
- [ ] Agent buffering/retry when Server is down
- [ ] New-source onboarding
- [ ] Mode A transport (SignalR / WebSockets / SSE)
- [ ] Mode B delivery (e.g. Web Push)
- [ ] Presence tracking
- [ ] State Machine states/transitions
- [ ] Data model
- [ ] User auth / identity model
- [ ] Transport encryption
- [ ] Offline mode (bonus)
