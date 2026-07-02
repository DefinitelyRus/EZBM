# EZBM.DesktopHost

This is the local ASP.NET Core Web API host project for the EZBM application. It acts as the local API layer between the React front-end application and the backend logic engine in `EZBM.Core`.

---

## Key Components

* **`Endpoints/`**: Map HTTP REST requests (GET/POST/PUT/DELETE) to backend controller actions (e.g. `InventoryController`, `SalesController`, `StaffController`, `AuthController`, `SettingsController`, `LogsController`).
* **`Program.cs`**: Registers core API routing tables, CORS rules, dependency injections, and triggers `DbManager.Initialize()` to ensure database creation on startup.

---

## How to Run

To run the local REST server in the development environment:

```bash
# Execute from the project folder
dotnet run --project EZBM.DesktopHost.csproj

# Alternatively, from the repository root
dotnet run --project Backend/EZBM.DesktopHost/EZBM.DesktopHost.csproj
```

The server runs on HTTP/HTTPS local host ports defined in `appsettings.json` and `Properties/launchSettings.json`.
You can access the auto-generated Swagger/OpenAPI documentation (in development mode) at `/openapi/v1.json` or through the mapped endpoints list in `Program.cs`.
