2# Plan: Complete the mandatory E2E flow — external event → real-time dispatcher alert

## Context
The assignment requires exactly **one** flow implemented end-to-end (the rest may stay
skeleton):

1. An external source sends an event to the **Agent**
2. The Agent forwards it to the **Server**
3. The Server saves it to the **database**
4. The **dispatcher receives a real-time alert**

Exploration confirms **steps 1–3 are already fully wired and working**, and **step 4 is the
only missing link**. This plan finishes step 4 and builds the dispatcher view so the alert is
visible.

### What already works (do not rebuild)
- **Step 1** — `SensorHttpSource` (`backend/src/EventHandler.Agent/Sources/SensorHttpSource.cs`)
  exposes `POST /events/sensor` on the Agent's Kestrel host, validates required fields.
- **Step 2** — `EventIngestionService` → `HttpServerClient` (`ServerClient/HttpServerClient.cs`)
  POSTs to `api/intake/events` on the Server (`Server:BaseUrl`, default `:5027`).
- **Step 3** — `IntakeController` → `EventService.CreateFromIntakeAsync`
  (`Application/EventService.cs:28`) → `EventRepository.AddAsync` + `UnitOfWork.SaveChangesAsync`
  against real SQL Server (migrations applied on startup in Development). This is the one
  implemented service method.
- **Transport for step 4** — SignalR is configured: `AddSignalR()` + `MapHub<EventsHub>("/hubs/events")`
  (`Program.cs:49,116`), JWT-over-query-string for the hub (`Program.cs:77-90`). `EventsHub`
  auto-joins every connected dispatcher to the `role:Dispatcher` group. `PresenceTracker` is real.
  Frontend `SignalrService` (`frontend/src/app/core/realtime/signalr.service.ts`) already connects
  with the JWT. `@microsoft/signalr` is installed.

### The gap (step 4)
- `NotificationService` (`Application/NotificationService.cs`) is all `NotImplementedException`,
  injects `IPresenceTracker` + `IWebPushSender` but **not** `IHubContext<EventsHub>`, so it can't
  push. `INotificationService` has no "event created" method.
- `EventService.CreateFromIntakeAsync` never calls the notification service.
- No frontend component consumes `SignalrService`; the dispatcher dashboard is an empty stub.
- Server read path (`ListForUserAsync`, repo read methods) is still `NotImplementedException`, so
  the dashboard can't load existing events yet.

### Locked decisions
- **Delivery: Mode A only** — broadcast `EventCreated` to the `role:Dispatcher` SignalR group.
  Web-push (Mode B, closed browser) stays the documented stub.
- **Dashboard: seed + live** — load existing events from the server read path on init, then
  prepend live-pushed events. This implements the dispatcher read path (which the rest of the app
  needs anyway).

---

## How this plan is structured
This is an **overview only**. The remaining work is real programming, so it's split into **three
big tasks**, executed one at a time. For each big task we will, separately:
**plan it in detail (break into subtasks) → you review → implement → commit.** This document
just fixes the big-task boundaries and what each one covers; the subtask-level detail is deferred
to each task's own planning pass.

The three tasks are ordered so each builds on a working predecessor: backend push must exist
before the frontend can receive it; the SignalR/API plumbing must exist before the UI can render.

---

## Big Task 1 — Backend: real-time push + dispatcher read path
**Goal:** the Server actually pushes a new event to connected dispatchers, and can list events.
Covers:
- **SignalR handling (push to front):** implement `NotificationService` (`Application/NotificationService.cs`,
  today all `NotImplementedException`) to push over `IHubContext<EventsHub>` to the `role:Dispatcher`
  group; add a created-event method to `INotificationService`; call it from
  `EventService.CreateFromIntakeAsync` after the DB save.
- **IPresenceTracker handling:** presence is fed by the hub and already tracked; this task decides
  and wires how the notification path uses it (Mode A = broadcast to the connected `role:Dispatcher`
  group; presence stays the seam for the deferred Mode B / offline case).
- **Read path (seeds the events table):** implement `EventRepository` read methods +
  `EventService.ListForUserAsync` so `GET /api/events` returns events for a dispatcher.
- **Tests:** update `EventServiceTests` (its `ThrowingNotificationService` fails once the create
  path notifies) and cover the new behavior.

## Big Task 2 — Frontend: API + SignalR plumbing (core services)
**Goal:** the Angular app can fetch events and receive live pushes — services only, no feature UI yet.
Covers:
- **API queries handling:** wire `EventsApi` (`core/api/events.api.ts`) into a consumable
  data flow for the dispatcher (currently defined but consumed by nothing).
- **SignalR handling:** extend `SignalrService` (`core/realtime/signalr.service.ts`) with a typed
  created-event stream (`onEventCreated`), and own the connect/reconnect/disconnect lifecycle for a
  logged-in dispatcher.

## Big Task 3 — Frontend: dispatcher view (login + events table + live notification)
**Goal:** the dispatcher sees events and gets alerted in real time — the visible end of the flow.
Covers:
- **Style login:** finish the login screen styling — the app's front door, currently unstyled.
- **Show events table:** replace the empty dispatcher dashboard stub with a real events table,
  seeded from the API (Task 2) and reusing existing shared UI (`badge`/`stat-card`, status/priority pipes).
- **Show notification when a new one comes:** on the `onEventCreated` push (Task 2), prepend the new
  event to the table and raise a visible in-app alert (banner/toast) — no page refresh.

---

## End-to-end verification (after all three tasks)
1. `dotnet test` + `ng test`/`ng build` clean.
2. Start Server (`:5027`), Agent (`:5100`), `ng serve` (`:4200`); confirm ports/CORS match.
3. Log in as the seeded **dispatcher**; dashboard shows existing events (SignalR connected).
4. POST a sample event to the Agent:
   `curl -X POST http://localhost:5100/events/sensor -H "Content-Type: application/json" -d @tools/sample-events/valid-event.json`
5. Watch the full chain: Agent forwards → new row in `Events` (SQL) → event appears live on the
   dispatcher dashboard with a notification, no refresh.
6. Negative check: `tools/sample-events/missing-title.json` → Agent `400`, nothing persisted.

## Out of scope (unchanged skeleton)
Web-push / Mode B delivery, agent→server auth, and the assign/transfer/status/close/notes service
methods remain stubs — none are on this flow.
