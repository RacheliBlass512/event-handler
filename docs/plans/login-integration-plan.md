# Plan: Implement Login (Backend + Frontend) + Tests + Seeded Users

## Context
The skeleton (`docs/plans/skeleton-plan.md`) already locked in the login **architecture**:
JWT bearer auth, EF Core repository pattern, `[Authorize(Roles=...)]` on protected endpoints,
DTOs, Angular `AuthService`/guards/interceptors/login form. Nothing here needs fresh design —
every interface, DTO and claim shape is already defined. What's missing is that ~5 method
bodies are `throw new NotImplementedException()` stubs, so login doesn't actually work yet, and
there's no seeded data to log in with. This plan fills in exactly those bodies, adds tests for
the new logic, and seeds dispatcher/technician users so the app is actually usable end-to-end.
Per your instruction, no extra layers beyond what's needed to make login correct.

## Backend — fill in the stubs (no new architecture)

1. **`UserRepository`** (`Infrastructure/Persistence/UserRepository.cs`): implement
   `GetByIdAsync`, `GetByUsernameAsync`, `ListTechniciansAsync` as plain EF Core queries against
   `AppDbContext.Users`. No tests needed for these — they're one-line EF calls, not custom logic.

2. **`JwtTokenService.GenerateToken`** (`Infrastructure/Auth/JwtTokenService.cs`): build a signed
   JWT (`HmacSha256`, key = `Jwt:Secret`) with `ClaimTypes.NameIdentifier = user.Id`,
   `ClaimTypes.Role = user.Role.ToString()` — matching what `EventsController.GetUserId()` /
   `GetRole()` already read — issuer/audience from config, expiry = now + `Jwt:LifetimeMinutes`.

3. **`AuthService.LoginAsync`** (`Application/AuthService.cs`): look up the user by username,
   verify the password via the existing `IPasswordHasher`, generate a token on success. Change
   `IAuthService.LoginAsync` / `LoginResult` return type to nullable (`LoginResult?`) — simplest
   way to signal bad credentials, no new exception type needed.

4. **`AuthController.Login`**: return `Unauthorized()` when the service returns `null`, `Ok(...)`
   otherwise.

5. **`DbSeeder.SeedAsync`** (`Infrastructure/DbSeeder.cs`): if `Users` table is empty, insert one
   dispatcher + two technicians with passwords hashed via `IPasswordHasher` (idempotent — skip if
   any user already exists).

6. **`Program.cs`**: in `Development`, after `app.Build()`, open a scope and call
   `db.Database.Migrate()` then `DbSeeder.SeedAsync()` — so a fresh clone has a working DB and
   login-able users with zero manual steps. `// ponytail:` comment noting auto-migrate-on-startup
   is fine for a 1-week dev skeleton, not a pattern to keep for production.

7. **EF migration**: install the `dotnet-ef` tool (not currently installed) and run
   `dotnet ef migrations add InitialCreate` — no migration exists yet for any entity.

8. **README**: document the seeded credentials (e.g. `dispatcher` / `tech1` / `tech2`, shared dev
   password) so login can be exercised immediately after cloning.

### Backend tests — new project `backend/tests/EventHandler.Server.Tests`
xUnit, `ProjectReference` to `EventHandler.Server`. Use **hand-written fakes** for
`IUserRepository` / `IPasswordHasher` / `IJwtTokenService` instead of adding a mocking library —
the interfaces are tiny (2-3 methods each), a mocking framework is more machinery than the job
needs.
- `AuthServiceTests`: valid credentials → token returned; wrong password → `null`; unknown
  username → `null`.
- `JwtTokenServiceTests`: build `IConfiguration` via `ConfigurationBuilder` with in-memory
  `Jwt:*` values, decode the generated token with `JwtSecurityTokenHandler` and assert the
  `NameIdentifier`/`Role` claims and expiry match.

## Frontend — already wired, no new code needed
`AuthService.login()` already POSTs to the real endpoint and stores state in a signal +
`localStorage`; the `Login` component already calls it and redirects by role. Once the backend
above works, the existing flow works end-to-end unchanged.

### Frontend tests
- **New ``**: using `provideHttpClientTesting`/`HttpTestingController` —
  `login()` posts to `${API_BASE_URL}/api/auth/login` and stores `AuthState` on success;
  `logout()` clears signal + localStorage; `isAuthenticated()` reflects `expiresAt`; `hasRole()`
  matches the stored role.
- **Expand `login.spec.ts`** (currently only a smoke test): invalid form does not call
  `authService.login` (spy); successful login navigates to `/dispatcher` or `/technician` per
  role; a mocked 401 sets `errorMessage` and clears `submitting`.

## Verification
1. `dotnet build` — solution-wide.
2. Run `EventHandler.Server`; confirm auto-migrate creates the DB and seeds users.
3. `POST /api/auth/login` with seeded dispatcher creds → 200 + JWT; wrong password → 401.
4. `dotnet test` — `EventHandler.Domain.Tests` + new `EventHandler.Server.Tests` green.
5. `npm test` (frontend Vitest) green.
6. **Live browser check**: run `dotnet run` on `EventHandler.Server` and `ng serve` on the
   frontend at the same time, open the app in a real browser, and log in as the seeded
   dispatcher and then as a seeded technician. Confirm each redirects to the correct dashboard,
   the JWT lands in `localStorage`, and a wrong password shows the form's error message. This is
   the step that actually proves login works end-to-end, not just that tests pass.
