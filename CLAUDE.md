# Task Management System — CLAUDE.md

## Resume Instruction
Say: **"resume Task Management System project"**
Claude reads this file and continues from where we left off.

---

## Project Location
`C:\Users\Shihab\Desktop\Project\Task-Management-System`

## Tech Stack
- .NET 10 | ASP.NET Core MVC | EF Core 10 | MS SQL Server (SQLEXPRESS)
- ASP.NET Core Identity (custom AccountController — NOT scaffolded Razor Pages)
- Bootstrap 5 | Bootstrap Icons | jQuery | SweetAlert2 | X.PagedList 10.5.9

## Architecture
3-project layered solution — Repository Pattern + Service Layer:
- `TaskManagementSystem.Core` → Entities, Enums, Exceptions, Interfaces, Services, ViewModels
- `TaskManagementSystem.Infrastructure` → ApplicationDbContext, TaskRepository, DbSeeder, Migrations
- `TaskManagementSystem.Web` → Controllers, Views, Middleware, wwwroot

Dependency flow (one direction): Web → Core ← Infrastructure

## Code Rules (enforce strictly)
- No business logic in controllers — delegate to ITaskService
- No try/catch in controllers — GlobalExceptionMiddleware handles it
- All DB calls are async (no .Result or .Wait())
- Verify UserId ownership in service before edit/delete
- Anti-forgery tokens on all POST forms
- Methods must stay below cyclomatic complexity 13
- No method longer than ~50 lines — extract private helpers
- Zero CS warnings (treat CS8618, CS0234 etc. as blockers)

## DB Connection
`Server=DESKTOP-L5LQLQ5\SQLEXPRESS;Database=TaskManagementSystemDb;Trusted_Connection=True;TrustServerCertificate=True;`

## Seed Credentials
- Email: admin@taskmanager.com
- Password: Admin@123

## Phase Status
- [x] Phase 1  — Solution + scaffolding + NuGet packages
- [x] Phase 2  — Core: entities, enums, interfaces, ViewModels, exceptions, TaskService
- [x] Phase 3  — Infrastructure: DbContext, TaskRepository, DbSeeder
- [x] Phase 4  — EF Migrations + database created
- [x] Phase 5  — TaskService (included in Phase 2)
- [x] Phase 6  — GlobalExceptionMiddleware + HomeController Error action
- [x] Phase 7  — AccountController (Login/Register/Logout)
- [x] Phase 8  — TaskController (9 endpoints, thin)
- [x] Phase 9  — All Razor views: Layout, Login, Register, Index, Create, Edit, _TaskTableRows, Error
- [x] Phase 10 — task.js: AJAX search, toggle, SweetAlert2 delete/toast
- [x] Phase 11 — Bonus: dashboard cards, pagination, CSV export, responsive Bootstrap 5 UI
- [x] Phase 12 — README.md, API_DOCUMENTATION.md, sql/schema.sql, CLAUDE.md

## Build Status
All 3 projects build with 0 errors, 0 CS warnings.
Only transitive NuGet NU1903 warning (EF Tools build-time dep) — not a code issue.

## Known Items / Future Work
- Run `dotnet run` from `src/TaskManagementSystem.Web` to start the app
- Migrations: `dotnet ef database update --project src/TaskManagementSystem.Infrastructure --startup-project src/TaskManagementSystem.Web`
- SQL script: `sql/schema.sql` (idempotent)
