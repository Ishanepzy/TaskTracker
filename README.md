# TaskTracker

TaskTracker is a task management web application built with .NET 8 and C# 12. It provides user authentication, task CRUD (create, read, update, delete), assignee, prioritization logic and completion tracking.

## Features

- User registration, login and logout using ASP.NET Core Identity
- Task creation, editing, deletion and assignee
- Task prioritization via a dedicated service
- Completion tracking and simple metrics displayed via view components
- Database schema managed with Entity Framework Core migrations

## Technology stack

- .NET 8, C# 12
- ASP.NET Core MVC
- Entity Framework Core (EF Core)
- ASP.NET Core Identity

## Project structure (key folders/files)

- `Controllers/` - MVC controllers handling HTTP requests (`AccountController`, `TasksController`)
- `Models/` - domain and view models (`ApplicationUser`, `UserTask`, view models)
- `Services/` - business services such as `TaskPrioritizer`
- `Views/` - templates for UI
- `Migrations/` - EF Core migrations
- `Program.cs` - app startup and service registration

## Prerequisites

- .NET 8 SDK
- A supported database (default: SQLite or SQL Server configured via connection string)
- `dotnet-ef` tool to run migrations (optional):

```bash
dotnet tool install --global dotnet-ef
```

## Configuration

1. Update the database connection string in `appsettings.json` under `ConnectionStrings:DefaultConnection`.
2. Ensure identity settings (if any) are configured in `Program.cs`.

## Database setup

From the `TaskTracker` project directory run:

```bash
# apply migrations
dotnet ef database update
```

To add a migration (if needed):

```bash
dotnet ef migrations add <MigrationName>
```

## Run

From the project root:

```bash
dotnet run --project TaskTracker/TaskTracker.csproj
```

Open the app at `https://localhost:5001` or the URL shown in the console.

## Notes on authentication

The project uses ASP.NET Core Identity with an `ApplicationUser` model and controller-based auth flows (`AccountController`). Registration and login views are included under `Views/Account/`.