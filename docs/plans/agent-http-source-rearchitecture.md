# Re-architect the Agent as a Web host — Option A: each HTTP source maps its own routes

## Context
Step 1 of the mandatory E2E flow (`External source → Agent → Server → Database → Dispatcher`)
requires the Agent to accept events from a real external boundary. The previous plan
(`this-is-the-only-resilient-pnueli.md`) proposed a self-hosted `System.Net.HttpListener` inside
`SensorHttpSource`; the user rejected that as a plaster — Kestrel is already a paid-for dependency
in this solution (`EventHandler.Server` is `Sdk.Web`), so hand-rolling a second, primitive HTTP
server in the Agent is not defensible in review.

Three re-architecture options were laid out (A: source owns its routes; B: one built-in ingress +
per-source normalizers; C: plain endpoint, no abstraction). **The user chose Option A**: the Agent
becomes an ASP.NET Core `WebApplication`, and each push-style source is the full owner of its own
HTTP surface (route, verb, payload shape, validation) via a new `IHttpEventSource` interface.
Trade-off accepted knowingly: maximum per-sensor flexibility, at the cost of each future push
source carrying its own HTTP handling (and auth later being per-route rather than one choke point).

Decisions carried over unchanged from the previous plan:
- **Auth deliberately deferred** (consistent with skeleton-plan.md §12).
- **`MockEventSource` deleted entirely** (class + DI registration; only referenced from Program.cs).
- Trust-boundary validation on incoming payloads **stays** — this is where raw external input enters.

Design refinement over the earlier Option-A sketch: push sources implement **only**
`IHttpEventSource` — they do NOT also implement `IEventSource` with no-op `StartAsync`/`StopAsync`.
Two sibling contracts, no dead methods: `IEventSource` = poll-style (self-driven loop),
`IHttpEventSource` = push-style (Kestrel-driven). `SourceHostService` stays as-is for poll sources
(it will iterate an empty collection until a poll source is registered — harmless, and it keeps the
poll extension point alive).

## Changes

**`backend/src/EventHandler.Agent/EventHandler.Agent.csproj`**
- SDK: `Microsoft.NET.Sdk.Worker` → `Microsoft.NET.Sdk.Web`. No new PackageReference — ASP.NET Core
  comes from the shared framework (same as the Server). The existing
  `Microsoft.Extensions.Hosting` package reference becomes redundant under `Sdk.Web` — remove it.
- Add `<InternalsVisibleTo Include="EventHandler.Agent.Tests" />`.

**New: `backend/src/EventHandler.Agent/Sources/IHttpEventSource.cs`**
```csharp
public interface IHttpEventSource
{
    string Name { get; }
    /// Push-style counterpart of IEventSource: instead of running its own loop, the source
    /// maps its endpoints onto the Agent's shared Kestrel host. Onboarding a push source:
    /// implement this, register in Program.cs DI, add config — no other file changes.
    void MapRoutes(IEndpointRouteBuilder app, IEventSink sink);
}
```

**`backend/src/EventHandler.Agent/Sources/SensorHttpSource.cs`** — implement for real as
`IHttpEventSource` (drop the `IEventSource` stub implementation):
- `MapRoutes` does `app.MapPost("/events/sensor", ...)` with `IncomingEventDto` as the bound
  parameter — ASP.NET Core model binding gives automatic `400` on malformed/unparseable JSON.
- Business validation via `internal static bool TryValidate(IncomingEventDto dto, out string? error)`
  — rejects missing/blank `SourceName`, `SourceEventId`, `Title`, `Description`, `Location`.
  Invalid → `Results.BadRequest(error)`. (This replaces the old plan's `TryParseAndValidate`; JSON
  parsing itself is now the framework's job.)
- Valid → `await sink.PublishAsync(dto, ct)` → `Results.Accepted(value: result)` returning the
  `IntakeResponseDto`. `HttpServerClient.SendAsync` never throws (verified: it catches, logs, and
  returns `Accepted:false`), so the handler needs no try/catch around it. `Accepted:false` is the
  expected result until Server persistence (step 2/3) exists — not a bug here.

**`backend/src/EventHandler.Agent/Sources/IEventSink.cs`** and
**`Ingestion/EventIngestionService.cs`** — `PublishAsync` returns `Task<IntakeResponseDto>` instead
of `Task` (it already awaits `IServerClient.SendAsync` and discards the result; stop discarding).
One-line change each; no other callers once `MockEventSource` is gone.

**`backend/src/EventHandler.Agent/Sources/MockEventSource.cs`** — delete.

**`backend/src/EventHandler.Agent/Sources/IEventSource.cs`** — update the onboarding doc comment:
two source flavors now (poll → `IEventSource`, push → `IHttpEventSource`), same onboarding story
for each.

**`backend/src/EventHandler.Agent/Program.cs`**
- `Host.CreateApplicationBuilder` → `WebApplication.CreateBuilder`.
- Keep existing DI (HttpClient/`IServerClient`, `IEventSink`, `SourceHostService`).
- Remove `MockEventSource` registration; add `AddSingleton<IHttpEventSource, SensorHttpSource>()`.
- After `builder.Build()`: resolve `IEventSink` once, loop `GetServices<IHttpEventSource>()`,
  call `MapRoutes(app, sink)` on each. Update the onboarding comment to reflect both flavors.
- Add `public partial class Program { }` at the bottom (required marker for
  `WebApplicationFactory<Program>` integration tests).

**`backend/src/EventHandler.Agent/appsettings.json`**
- Remove `Sources:Mock`. Pin the Agent's listen address so it can't collide with the Server:
  `"Urls": "http://localhost:5100"`.

**New: `backend/tests/EventHandler.Agent.Tests/`** — mirror
`backend/tests/EventHandler.Domain.Tests/EventHandler.Domain.Tests.csproj` exactly (xUnit 2.9.3,
`Microsoft.NET.Test.Sdk` 17.14.1, `xunit.runner.visualstudio`, `coverlet.collector`,
`<Using Include="Xunit" />`; ProjectReference → `EventHandler.Agent`), plus
`Microsoft.AspNetCore.Mvc.Testing` for the integration test. Add to the solution file. Two small
test files:
1. `SensorHttpSourceValidationTests` — unit tests on `TryValidate`: valid DTO passes; blank
   `Title` / `SourceEventId` rejected.
2. `SensorHttpEndpointTests` — one `WebApplicationFactory<Program>` test class that replaces
   `IServerClient` with a stub via `WithWebHostBuilder` (the DI seam already exists): malformed
   JSON → 400 (model binding); missing `Title` → 400 (validation); valid payload → 202 with an
   `IntakeResponseDto` body. This is the testability payoff of the re-architecture — worth showing.

**New: sensor simulator script — `tools/send-event.ps1` + `tools/sample-events/`**
The manual way to exercise the trust boundary: a small PowerShell script that reads an event from
an easily-editable JSON file and POSTs it to the Agent — simulating a real external sensor.
- `tools/send-event.ps1` (~10 lines): params `-File` (default `tools/sample-events/valid-event.json`)
  and `-Url` (default `http://localhost:5100/events/sensor`); reads the file raw, sends it with
  `Invoke-WebRequest -Method Post -ContentType application/json`, prints status code + response
  body. Uses `Invoke-WebRequest` (not `Invoke-RestMethod`) so a `400` still shows the error body
  instead of throwing opaquely.
- `tools/sample-events/valid-event.json` — a complete `IncomingEventDto` payload (edit freely to
  simulate different events).
- `tools/sample-events/missing-title.json` — same payload minus `Title`, to demo the validation
  `400` path.
Sending raw file bytes (not re-serializing through PowerShell objects) is deliberate: it lets you
also test broken JSON by just breaking the file.

## Verification
1. `dotnet build` — solution compiles after the SDK swap and interface split.
2. `dotnet test` — new Agent tests pass alongside existing Domain/Server suites.
3. Run Server and Agent side by side; simulate a sensor:
   ```powershell
   .\tools\send-event.ps1                                                  # valid → 202
   .\tools\send-event.ps1 -File tools\sample-events\missing-title.json     # → 400 with error body
   ```
   Expect `202` with `IntakeResponseDto` (`Accepted:false` until Server persistence lands — that's
   step 2/3, not a defect here). Editing `valid-event.json` and re-running simulates any event
   shape; corrupting the JSON syntax demos the model-binding `400`.
4. Agent logs show the event flowing source → sink → `HttpServerClient`; Server logs show the
   intake hit.
5. Check off `External source → Agent` in `docs/REQUIREMENTS_CHECKLIST.md` (line 18) once confirmed.
