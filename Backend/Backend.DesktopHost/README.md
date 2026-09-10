# EZBM Backend Desktop API Host

*Author(s): [DefinitelyRus](https://github.com/DefinitelyRus)*

`Backend.DesktopHost` provides the REST API controllers for the front-end interface and other clients.

Only one instance is meant to be run at a time per local network, as it handles the database for that network.

> [!IMPORTANT]
> This project depends on `Backend.Core`.

## Recreating `Backend.DesktopHost.csproj` and Dependencies

### 1. Create the `Backend.DesktopHost` folder

```bash
mkdir Backend/Backend.DesktopHost
cd Backend/Backend.DesktopHost
```

### 2. Create the `Backend.DesktopHost` project

```bash
dotnet new webapi
```

### 3. Install required NuGet packages

These NuGet packages are needed for certain functions of the API Host to work properly.

```bash
dotnet add Backend.DesktopHost.csproj package Microsoft.EntityFrameworkCore.Design
dotnet add Backend.DesktopHost.csproj package Microsoft.EntityFrameworkCore.Sqlite
dotnet add Backend.DesktopHost.csproj package Microsoft.AspNetCore.OpenApi
dotnet add Backend.DesktopHost.csproj package Swashbuckle.AspNetCore
```

- **`Microsoft.EntityFrameworkCore.Sqlite`:** Allows performing queries on a SQLite database in C# using LINQ instead of writing SQL queries.
- **`Microsoft.EntityFrameworkCore.Design`:** Reads the C# classes, inspects the database context, and looks for the migration differences when changes are made to the data models.
- **`Microsoft.AspNetCore.OpenApi`:** Generates OpenAPI (formerly Swagger) specification files for the API endpoints.
- **`Swashbuckle.AspNetCore`:** Generates a web UI for browsing, testing, and documenting the API endpoints.

### 4. Add the `Backend.DesktopHost` to the `Backend` solution

Required for running commands like `dotnet build` and `dotnet test`.

```bash
dotnet sln ../Backend.sln add Backend.DesktopHost.csproj
```

### 5. Add a reference to the `Backend.Core` project

Required for `dotnet` to find the entities defined in `Backend.Core` when building/running the project.

```bash
dotnet add Backend.DesktopHost.csproj reference ../Backend.Core/Backend.Core.csproj
```
