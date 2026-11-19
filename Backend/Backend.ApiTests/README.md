# EZBM Backend API Tests

`Backend.ApiTests` is a dedicated console application that runs end-to-end integration tests against all `Backend.DesktopHost` REST API endpoints and prints plain-word results to the terminal.

> [!NOTE]
> The test suite spins up an isolated in-process host using a temporary SQLite database and a temporary `settings.json` file. Running it will not affect your real database or user settings, and temporary files are cleaned up on exit.

## Running the Tests

Run the full test suite from the repository root with a single command:

```powershell
dotnet run --project Backend/Backend.ApiTests/Backend.ApiTests.csproj
```

The runner exits with code `0` when all tests pass, or `1` if any test fails.

## Test Modules

Each module under `Modules/` exercises a group of related endpoints:

- **`AuthTests.cs` (`/api/Auth`):** Default admin login, invalid password rejection, session token verification, and password changes.
- **`ProductsAndStockTests.cs` (`/api/Products`, `/api/ProductStocks`, `/api/ProductTransactions`):** Product creation, listing, lookup, deletion, stock additions, batch checks, and paginated/filtered transaction history.
- **`PosAndPaymentTests.cs` (`/api/Pos`, `/api/Payments`):** Physical product checkout with stock deduction, unlimited-stock service checkout, insufficient stock rollback, and cash payment change calculation.
- **`AttendanceAndPayrollTests.cs` (`/api/Attendance`, `/api/Payroll`):** Employee clock-in/clock-out logging, attendance history queries, and payroll shift calculation.
- **`AuditLogsAndSettingsTests.cs` (`/api/AuditLogs`, `/api/Settings`):** Recording and filtering audit events, reading settings, validating `.db` path updates, and resetting to default.

## Recreating `Backend.ApiTests.csproj` and Dependencies

### 1. Create the `Backend.ApiTests` project

```bash
mkdir Backend/Backend.ApiTests
cd Backend/Backend.ApiTests
dotnet new console
```

### 2. Install required NuGet packages

```bash
dotnet add Backend.ApiTests.csproj package Microsoft.AspNetCore.Mvc.Testing
```

- **`Microsoft.AspNetCore.Mvc.Testing`:** Provides `WebApplicationFactory<TEntryPoint>` to host `Backend.DesktopHost` in-process and issue HTTP requests through `HttpClient`.

### 3. Add a reference to `Backend.DesktopHost`

```bash
dotnet add Backend.ApiTests.csproj reference ../Backend.DesktopHost/Backend.DesktopHost.csproj
```
