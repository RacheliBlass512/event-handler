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
- **[docs/REQUIREMENTS_CHECKLIST.md](docs/REQUIREMENTS_CHECKLIST.md)** — tracks graded
  deliverables from the assignment. Proactively check items off the moment they become
  true (code exists and works) — don't wait to be asked, and don't check something off
  based on a plan or discussion alone.
- **docs/plans/** — all implementation plans go here (not the default
  `~/.claude/plans/` location). Give each plan a descriptive filename, not an
  auto-generated slug.

## Tech stack
- Backend: **.NET Core** (chosen; rationale still needs the user's confirmation/edits).
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

## Consistency
- Favor consistency across the codebase — if keeping things consistent requires an
  architectural or global change, make it rather than working around it locally.
