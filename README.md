# Event Handler — Real-Time Field Event Management System

Home assignment implementation: a centralized system for handling field events (from
sensors, external systems, manual reports) in real time, from intake through dispatch to
closure.

## Status
Early scaffolding. Architecture and requirements are being drafted before implementation
begins — see the docs below.

## Docs
- [Architecture & decision log](docs/ARCHITECTURE.md) — component design, state machine,
  data model, security, failure handling, and the reasoning behind every major choice.
- [Requirements checklist](docs/REQUIREMENTS_CHECKLIST.md) — tracks graded deliverables.
- [Original assignment](home%20work%20instructions.md) — kept for reference.

## Tech stack
- Backend: .NET Core (ASP.NET Core, C#)
- Frontend: Angular (fixed by assignment)
- Database: SQL Server (fixed by assignment)
- Real-time transport: TBD — see [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)

## Getting started

### Backend
```
cd backend/src/EventHandler.Server
dotnet run
```
On startup (Development only) the app applies EF Core migrations and seeds three dev users if
the `Users` table is empty. Seeded login credentials (shared dev password: `Passw0rd!`):

| Username     | Role       |
|--------------|------------|
| `dispatcher` | Dispatcher |
| `tech1`      | Technician |
| `tech2`      | Technician |

### Frontend
```
cd frontend
npm install
npm start
```
Log in at `http://localhost:4200/login` with any of the seeded credentials above.
