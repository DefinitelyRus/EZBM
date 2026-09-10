# EZBM Backend

Make sure `dotnet-ef` is installed.

## Recreating the `Backend` and its sub-projects

### 1. Create `Backend.sln`

This will create the solution file `Backend.sln`, which ties together the `Backend.Core` and `Backend.DesktopHost` projects.

```bash
dotnet new sln
```

### 2. Create the `Backend.Core` project

Follow the instructions at [`Backend.Core/README.md`](Backend.Core/README.md).

### 3. Create the `Backend.DesktopHost` project

Follow the instructions at [`Backend.DesktopHost/README.md`](Backend.DesktopHost/README.md).

### 4. Install `dotnet-ef`

`dotnet-ef` is the command-line tool for Entity Framework Core, the library used to allow writing SQL database queries in C#.

#### Option A: Install globally

This will install `dotnet-ef` globally, allowing it to be used in any project, but you may encounter version mismatches if you use different versions of EF Core in other projects.

```bash
dotnet tool install --global dotnet-ef
```

#### Option B: Install per-project

This will install `dotnet-ef` only for the current project.

```bash
dotnet new tool-manifest
dotnet tool install dotnet-ef
```

You must also restore `dotnet-ef` each time you open the project in a new computer or clear your environment.

```bash
dotnet tool restore
```

> [!NOTE]
> This is a one-time action per environment and will not affect other projects.

> [!IMPORTANT]
> After restoring, you may need to restart your terminal.

### 5. Create new migrations

Creates a new migration file based on the changes in `Backend.Core.csproj`.

This is needed for when you first create the project or when you make changes to the data models.

```bash
dotnet ef migrations add InitialCreate --project Backend.Core --startup-project Backend.DesktopHost
```

### 6. Create the database

Creates or updates the database schema based on the migration files.

```bash
dotnet ef database update --project Backend.Core --startup-project Backend.DesktopHost
```
