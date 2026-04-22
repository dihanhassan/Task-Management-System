# Task Management System

A production-grade task management web application built with ASP.NET Core MVC (.NET 10), Entity Framework Core, MS SQL Server, and jQuery.

---

## Features

- **User Authentication** — Register, login, logout with ASP.NET Core Identity
- **Task CRUD** — Create, view, edit, and delete your tasks
- **AJAX Search** — Real-time task search without page reload
- **Toggle Complete** — Mark tasks done/pending with a single click (AJAX)
- **Filters & Sorting** — Filter by status, sort by due date or priority
- **Dashboard** — Live counts: Total, Completed, Pending, Overdue tasks
- **Pagination** — 10 tasks per page
- **CSV Export** — Download all your tasks as a CSV file
- **SweetAlert2** — Polished delete confirmation and toast notifications
- **Responsive UI** — Works on mobile, tablet, and desktop
- **Global Error Handling** — Friendly error pages for every error type

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 10) |
| Database | MS SQL Server |
| ORM | Entity Framework Core 10 |
| Authentication | ASP.NET Core Identity |
| Frontend | Bootstrap 5, jQuery, SweetAlert2, Bootstrap Icons |
| Architecture | 3-project layered: Core / Infrastructure / Web |

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- MS SQL Server (local instance or SQL Express)
- Visual Studio 2022 / VS Code / Rider

---

## Setup Instructions

### 1. Clone the repository

```bash
git clone <your-repo-url>
cd Task-Management-System
```

### 2. Configure the database connection

Open `src/TaskManagementSystem.Web/appsettings.json` and update the connection string to match your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=TaskManagementSystemDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Common server values:
- Local SQL Server: `Server=.`
- SQL Express: `Server=.\SQLEXPRESS` or `Server=MACHINE-NAME\SQLEXPRESS`
- LocalDB: `Server=(LocalDB)\MSSQLLocalDB`

### 3. Apply database migrations

```bash
dotnet ef database update \
  --project src/TaskManagementSystem.Infrastructure \
  --startup-project src/TaskManagementSystem.Web
```

Or use the included SQL script (see below).

### 4. Run the application

```bash
dotnet run --project src/TaskManagementSystem.Web
```

Open your browser at: `https://localhost:7xxx` (port shown in terminal)

---

## Default Admin Account

The application seeds an admin user on first run:

| Field | Value |
|---|---|
| Email | `admin@taskmanager.com` |
| Password | `Admin@123` |

You can change these in `appsettings.json` under the `Seed` section.

---

## Using the SQL Script (alternative to migrations)

A complete idempotent SQL script is included at `sql/schema.sql`. Run it against your SQL Server instance:

```bash
sqlcmd -S YOUR_SERVER -d master -i sql/schema.sql
```

Or open it in SQL Server Management Studio and execute it.

---

## Project Structure

```
TaskManagementSystem.sln
├── src/
│   ├── TaskManagementSystem.Core/         Domain layer (pure contracts)
│   │   ├── Entities/                      TaskItem entity
│   │   ├── Enums/                         Priority, TaskFilter, TaskSort
│   │   ├── Exceptions/                    Custom domain exceptions
│   │   ├── Interfaces/                    ITaskRepository, ITaskService
│   │   └── ViewModels/                    All view models and DTOs
│   │
│   ├── TaskManagementSystem.Infrastructure/   Data + business logic layer
│   │   ├── Data/                          ApplicationDbContext
│   │   ├── Helpers/                       TaskMapper, CsvExportHelper
│   │   ├── Migrations/                    EF Core migrations
│   │   ├── Repositories/                  TaskRepository
│   │   ├── Seed/                          DbSeeder (admin user)
│   │   └── Services/                      TaskService (business logic)
│   │
│   └── TaskManagementSystem.Web/          Presentation layer
│       ├── Controllers/                   AccountController, TaskController
│       ├── Middleware/                    GlobalExceptionMiddleware
│       ├── Views/                         All Razor pages
│       └── wwwroot/js/task.js             AJAX interactions
│
└── sql/
    └── schema.sql                         Full SQL schema script
```

---

## Architecture Principles Applied

- **SOLID** — Single responsibility per class, interfaces for all dependencies
- **Repository Pattern** — Data access isolated behind `ITaskRepository`
- **Service Layer** — Business logic in `TaskService`, not in controllers
- **Dependency Injection** — All services registered in `Program.cs`
- **Async/Await** — Every database operation is non-blocking
- **User Isolation** — Each user only sees and modifies their own tasks
- **Global Exception Handling** — Middleware catches all unhandled exceptions
- **Low Complexity** — No method exceeds cyclomatic complexity of 5

---

## Time Spent

Approximately **6–8 hours** including architecture, implementation, and documentation.
