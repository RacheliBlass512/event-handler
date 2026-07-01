# Event Handler — Instructions for Claude Code

## What this project is
A home assignment (Experienced Developer / Team Leader level, 1 week): a centralized,
real-time field-event management system connecting a Central Agent (listens for events
from sensors/external systems/manual reports), a Central Server (business logic, state
machine, real-time delivery), and an Angular frontend (dispatcher + technician views).
Stack constraints from the assignment: **.NET**  + **Angular** (frontend, fixed) + **SQL Server** (fixed).

The full original assignment text lives in [`home work instructions.md`](home%20work%20instructions.md)
at the repo root (kept for reference, not part of the deliverable).

## Who you're working with
The user is the candidate being evaluated. During review they must explain and justify
every architectural decision and every code block as if they wrote it from scratch —
"the AI suggested it" is an explicitly unacceptable answer. Your job is to help them
**think and build**, not to quietly decide the architecture for them:
- Surface alternatives and trade-offs; don't just pick one and move on.
- Before treating something as "decided," make sure the user actually agrees with the
  reasoning — draft it, but don't bury a real decision in prose they haven't confirmed.
- Prefer asking when a choice is genuinely theirs to make (this is graded on *their*
  judgment), and act directly on plumbing/scaffolding that has no real judgment call.

## Living documents — keep these current
- **[docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)** — the architecture document required
  for submission. **Keep it short and skimmable — this is a hard constraint, not a
  nice-to-have.** Whenever a conversation produces or revisits an architectural decision
  (component design, protocol choice, auth mechanism, failure handling, data model, etc.),
  update the relevant section **and** append a one-line row to its Decision Log
  (decision, alternatives considered, why rejected, trade-off accepted). Do this
  proactively right after the discussion that produced the decision — don't wait to be
  asked, and don't let a decision go unrecorded. Only record decisions that matter; don't
  restate the assignment text, don't pad with prose, one or two lines per point.
- **[docs/REQUIREMENTS_CHECKLIST.md](docs/REQUIREMENTS_CHECKLIST.md)** — tracks graded
  deliverables from the assignment. Proactively check items off the moment they become
  true (code exists and works) — don't wait to be asked, and don't check something off
  based on a plan or discussion alone.
- **README.md "AI Tools Used" section** — the assignment requires briefly stating which
  AI tools were used and what was tested/changed/rejected. Keep this a short bullet list
  in the README (not a separate log file, not a table) — a few lines is enough.

## Tech stack
- Backend: **.NET Core** (chosen; see `docs/ARCHITECTURE.md` Decision Log — rationale
  still needs the user's confirmation/edits, see the log entry).
- Frontend: Angular (fixed by assignment).
- Database: SQL Server (fixed by assignment).
- Real-time transport: **TBD** (e.g. SignalR / WebSockets / SSE, plus a separate
  mechanism for alerts when the browser is closed).

## Testing
- The State Machine must have unit tests — explicit, non-negotiable requirement.

## Repo hygiene
- A Git repository with real history is itself a graded deliverable — don't squash the
  story of decisions away.
- Follow the standard commit rules: only commit when the user asks.
