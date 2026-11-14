# Tasks 261114-04

## Current Status

Phase 4 is currently marked on hiatus in the roadmap. However, backend models and data endpoints for employee attendance and basic payroll can be scaffolded to support future workforce management. All front-end tasks are excluded.

## Tasks Summary

1. Implement Attendance Entity and Database Schema
2. Implement Attendance Logging and Query Service
3. Implement Attendance Controller Endpoints
4. Implement Basic Payroll Calculation Engine and Query Endpoints

## Tasks

### Task 1: Implement Attendance Entity and Database Schema

The database has no table to store employee clock-in and clock-out entries.

Create the attendance domain model and add an EF Core migration.

1. Open `Backend/Backend.Core/Models/` and create `AttendanceRecord.cs`:
    - Add properties for `UserId`, `ActionType` (`In` or `Out`), `Timestamp`, and optional `Notes`.
2. Register `AttendanceRecords` DbSet in `Backend/Backend.Core/Data/AppDbContext.cs`.
3. Add EF Core migration for attendance schema.

```csharp
namespace Backend.Core.Models;

public class AttendanceRecord : Entity
{
    public long UserId { get; set; }
    public string ActionType { get; set; } = "In";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}
```

#### Things to Consider

- Restrict `ActionType` to predefined values (`In`, `Out`).
- Use UTC timestamps for consistency.

### Task 2: Implement Attendance Logging and Query Service

The system needs business logic to record clock actions and query logs by date range or employee.

Create `AttendanceService.cs` in `Backend.DesktopHost/Services/`.

1. Implement `ClockActionAsync(long userId, string actionType, string? notes)`.
2. Implement `GetAttendanceHistoryAsync(long? userId, DateTime? startDate, DateTime? endDate)`.

#### Things to Consider

- Verify user existence before recording attendance.
- Provide sensible defaults for date filtering.

### Task 3: Implement Attendance Controller Endpoints

The API needs HTTP endpoints to record attendance and query logs.

Create `AttendanceController.cs` in `Backend.DesktopHost/Controllers/`.

1. Add `POST /api/Attendance/clock` to record clock-in or clock-out.
2. Add `GET /api/Attendance/history` to query attendance records.

#### Things to Consider

- Return appropriate status codes (`201 Created` for clock actions).

### Task 4: Implement Basic Payroll Calculation Engine and Query Endpoints

The system needs basic calculation of hours worked and gross pay for employee shifts.

Create `PayrollService.cs` and `PayrollController.cs` in `Backend.DesktopHost`.

1. Create payroll calculation model / DTO `PayrollSummaryResponse`.
2. Pair clock-in and clock-out records to calculate total hours worked in a date range.
3. Apply hourly rate to determine gross pay.
4. Add `GET /api/Payroll/summary` accepting `userId`, `hourlyRate`, `startDate`, and `endDate`.

#### Things to Consider

- Handle unmatched clock-in records gracefully.

## Next Step(s)

1. Implement Task 1 (Attendance entity and migration).
2. Implement Task 2 (Attendance service).
3. Implement Task 3 (Attendance controller).
4. Implement Task 4 (Payroll calculation service and controller).

## Affected File(s)

1. `Backend/Backend.Core/Data/AppDbContext.cs`
2. `Backend/Backend.Core/Models/AttendanceRecord.cs`
3. `Backend/Backend.DesktopHost/Controllers/AttendanceController.cs`
4. `Backend/Backend.DesktopHost/Controllers/PayrollController.cs`
5. `Backend/Backend.DesktopHost/DTOs/AttendanceDtos.cs`
6. `Backend/Backend.DesktopHost/DTOs/PayrollDtos.cs`
7. `Backend/Backend.DesktopHost/Services/AttendanceService.cs`
8. `Backend/Backend.DesktopHost/Services/PayrollService.cs`
