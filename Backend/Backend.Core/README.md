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

### 4. Add the `Backend.Core` to the `Backend` solution

```bash
dotnet sln ../Backend.sln add Backend.Core/Backend.Core.csproj
```

## Database

<!-- TODO: Update when database handling is properly implemented. -->

The .db file's location can be changed during runtime, but is set to `"{Documents}/EZBM/business_data.db"` by default.
