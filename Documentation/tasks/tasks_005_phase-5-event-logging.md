# Tasks 261114-05

## Current Status

Phase 5 introduces structured audit event logging across system operations (login failures, security events, admin actions, large stock adjustments). All front-end tasks are excluded.

## Tasks Summary

1. Implement System Event Log Model and Database Persistence
2. Implement Audit Logging Service
3. Implement Audit Log Query Controller Endpoints

## Tasks

### Task 1: Implement System Event Log Model and Database Persistence

The system does not persist security or operational audit events to the SQLite database.

Create the event log model and database schema in `Backend.Core`.

1. Open `Backend/Backend.Core/Common/` and create `LogLevel.cs` or `AuditEventType.cs`.
2. Open `Backend/Backend.Core/Models/` and create `AuditEventLog.cs`:
    - Add properties for `EventType`, `Message`, `Severity`, `Timestamp`, `Source`, and optional `UserId`.
3. Register `AuditEventLogs` DbSet in `Backend/Backend.Core/Data/AppDbContext.cs`.
4. Add EF Core migration for audit log schema.

```csharp
namespace Backend.Core.Models;

public class AuditEventLog : Entity
{
    public string EventType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = "Info";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Source { get; set; }
    public long? UserId { get; set; }
}
```

#### Things to Consider

- Optimize timestamp indexing for fast historical queries.

### Task 2: Implement Audit Logging Service

The system needs a centralized backend service to record events asynchronously.

Create `IAuditLogService` and `AuditLogService` in `Backend.DesktopHost/Services/`.

1. Add method `LogEventAsync(string eventType, string message, string severity, string? source, long? userId)`.
2. Add helper overloads for security warnings, stock adjustments, and administrative events.
3. Add method `GetLogsAsync(string? eventType, string? severity, DateTime? from, DateTime? to)`.

#### Things to Consider

- Ensure logging failures do not break primary business transactions.

### Task 3: Implement Audit Log Query Controller Endpoints

The API needs HTTP endpoints to inspect and filter stored audit logs.

Create `AuditLogsController.cs` in `Backend.DesktopHost/Controllers/`.

1. Add `GET /api/AuditLogs` with query parameters for `eventType`, `severity`, `from`, `to`, `page`, and `pageSize`.
2. Add `GET /api/AuditLogs/{id}` to fetch a specific event detail.

#### Things to Consider

- Restrict log modification: endpoints must be read-only (no PUT/DELETE).

## Next Step(s)

1. Implement Task 1 (Audit log model and migration).
2. Implement Task 2 (Audit logging service).
3. Implement Task 3 (Audit log query controller).

## Affected File(s)

1. `Backend/Backend.Core/Data/AppDbContext.cs`
2. `Backend/Backend.Core/Models/AuditEventLog.cs`
3. `Backend/Backend.DesktopHost/Controllers/AuditLogsController.cs`
4. `Backend/Backend.DesktopHost/DTOs/AuditLogDtos.cs`
5. `Backend/Backend.DesktopHost/Services/AuditLogService.cs`
6. `Backend/Backend.DesktopHost/Services/IAuditLogService.cs`
