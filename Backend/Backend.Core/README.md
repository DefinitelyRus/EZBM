# EZBM Backend Core

`Backend.Core` is a class library that defines the various entities used by the `Backend.DesktopHost` and other clients.

## Recreating `Backend.Core`

### 1. Create the `Backend.Core` folder

```bash
mkdir Backend/Backend.Core
cd Backend/Backend.Core
```

### 2. Create `Backend.Core.csproj`

```bash
dotnet new classlib
```

### 3. Install required NuGet Packages

These NuGet packages are needed for the entities defined in this project to function properly.

```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

- **`Microsoft.EntityFrameworkCore.Sqlite`:** Defines how the C# data structures translate to a SQLite schema and how `Backend.DesktopHost` interacts with it.
- **`Microsoft.EntityFrameworkCore.Design`:** Reads the C# classes, inspects the database context, and looks for the migration differences when changes are made to the data models.
- **`Microsoft.EntityFrameworkCore.Tools`:** Enacts the database migrations when changes are made to the data models. This package depends on `Design` to function.

### 4. Add `Backend.Core` to the `Backend` solution

```bash
dotnet sln ../Backend.slnx add Backend.Core/Backend.Core.csproj
```

## Database

The SQLite `.db` file path is resolved dynamically via `BusinessSettings` and `BusinessSettingsService`. By default, it resolves to:

```text
{Documents}/EZBM/business_data/data.db
```

The database path can be inspected or modified at runtime using the settings API (`/api/settings`) or updated in `{Documents}/EZBM/settings.json`. Database migrations apply automatically when the host launches, or manually via `dotnet ef database update`.
