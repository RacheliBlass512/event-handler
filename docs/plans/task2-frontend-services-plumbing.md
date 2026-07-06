# Task 2 (detailed) — Frontend: API + SignalR plumbing (core services)

Detailed plan for **Big Task 2** of
[e2e-realtime-dispatcher-alert.md](e2e-realtime-dispatcher-alert.md). Scope: **services only,
no feature UI** — the dispatcher view is Task 3.

## Goal
The Angular app can (a) fetch existing events and (b) receive live `EventCreated` pushes, exposed
through one consumable data flow the Task 3 UI will read.

## Current state (verified)
- `EventsApi.list()` (`core/api/events.api.ts`) already does `GET /api/events` → `EventDto[]`.
  After Task 1 the endpoint returns real data. **Nothing consumes it yet.**
- `SignalrService` (`core/realtime/signalr.service.ts`) connects with the JWT and auto-reconnects,
  but: nobody calls `connect()`, and it only has stub `onEventUpdated`/`onAlert` (payload `unknown`).
  **No `onEventCreated`.**
- Backend (Task 1) sends message name **`"EventCreated"`** with a full `EventDto` payload
  (`NotificationService.cs:38`). Frontend `EventDto` already mirrors it (`core/models/event.model.ts`).

## Decisions (locked with the user)
1. **Dedicated Store** (signal-based) owns the events list: seed from `list()` once, then fold live
   pushes in. Consistent with the existing signal pattern (`AuthService`).
2. **Stream shape = RxJS Observable/Subject** at the `SignalrService` boundary (`eventCreated$`).
   The Store subscribes and folds into a `signal` list. Observable = occurrences over time; signal =
   current state.
3. **Connection lifecycle = auth-driven**, owned by an **`effect` inside the Store** that watches
   `AuthService.currentUser`: user present → `connect()` + seed load; user gone → `disconnect()` +
   clear.
4. **Connect for any authenticated user** — the effect gates on presence, not role.
   - `ponytail:` known ceiling — the effect lives in the (lazy) dispatcher Store, so in practice the
     connection opens when the dispatcher view first injects the Store. If technician realtime is
     added later, lift the connection lifecycle into a root `ConnectionManager` service so it truly
     fires for every authenticated user regardless of which view is open.
5. **Remove the dead `onEventUpdated`/`onAlert` stubs** — the backend throws `NotImplemented` for
   those on this flow and nothing consumes them. Re-add as Observables when those pushes go live.

---

## Subtasks

### 2.1 — `SignalrService`: typed `eventCreated$` + fix registration timing
File: `core/realtime/signalr.service.ts`
- Add a private `Subject<EventDto>`; expose `readonly eventCreated$ = subject.asObservable()`.
- In `connect()`, after building the connection, register
  `connection.on('EventCreated', (dto: EventDto) => subject.next(dto))` **before** `start()`, so the
  handler exists when the first push arrives (today's stubs register on a possibly-null connection —
  latent bug).
- Remove `onEventUpdated` / `onAlert` (decision 5).
- Keep `connect()` idempotent (already guards on existing connection) and `disconnect()`.

Check: the payload type is `EventDto` (imported from `core/models`).

### 2.2 — `DispatcherEventsStore`: seed + live, signal list
New file: `core/state/dispatcher-events.store.ts` (`@Injectable({ providedIn: 'root' })`)
- Private `signal<EventDto[]>([])`; expose `readonly events = _events.asReadonly()`.
- `load()` — calls `EventsApi.list()`, sets the signal from the result.
- Subscribe once to `signalr.eventCreated$`: **prepend** the new event, **dedup by `id`** (guard: if
  an event with the same id already exists, skip — prevents a visible duplicate if a push races the
  seed load).
- **`effect`** (in the constructor's injection context) reading `authService.currentUser()`:
  - present → `signalr.connect()` + `load()`
  - null → `signalr.disconnect()` + reset the signal to `[]`
- Minimal error handling on the seed: on `list()` error, log and keep the list empty (read-only path,
  no data-loss risk). Expose an optional `readonly error = signal<string | null>(null)` for Task 3 to
  render — small, vetoable.

Ordering note: newest-first (prepend). Seed order comes from the server; Task 3 can sort for display.

### 2.3 — Test (one runnable check)
New file: `core/state/dispatcher-events.store.spec.ts` (Jasmine / `ng test`)
- Seed load populates `events` from a stubbed `EventsApi`.
- A push via a stub `eventCreated$` prepends and is deduped by id.
- Covers the only non-trivial logic in this task (the seed+merge+dedup). Framework already present;
  no new deps.

---

## What this task deliberately does NOT do
- No component/UI changes (Task 3).
- No new `EventsApi` methods — `list()` is all this flow needs; the assign/transfer/etc. methods stay
  as-is.
- No web-push / Mode B, no technician realtime.

## Done when
- `ng build` clean, `ng test` green (incl. 2.3).
- Logging in as a dispatcher (manually, in Task 3 or a quick harness) opens the SignalR connection and
  `events` seeds from the API; a posted event reaches `eventCreated$` and lands at the head of the
  list. Full visible verification is Task 3.
