# README

> *Author(s): DefinitelyRus, Google Gemini*

This is a respository for how things are made on the back-end... mainly because the author will forget soon after committing the code.

## Creating the project

The back-end service is split in 2 parts: the logic engine (`EZBM.Core`) and the desktop host (`EZBM.DesktopHost`).

The logic engine is a standard class library acting as the back-end's core which does all the data handling and calculations and all that.

The desktop host provides the local REST API layer so that the front-end server can fetch and interact with `EZBM.Core` without it modifying any data directly by itself.

```bash
# 1. Create a solution folder and step inside
mkdir EZBM
cd EZBM

# 2. Initialize a blank solution file
dotnet new sln -n EZBM

# 3. Create your back-end logic engine as a standard class library
dotnet new classlib -o EZBM.Core

# 4. Create your temporary desktop host (ASP.NET Web API)
dotnet new webapi -o EZBM.DesktopHost

# 5. Attach both projects to your solution file
dotnet sln add EZBM.Core/EZBM.Core.csproj
dotnet sln add EZBM.DesktopHost/EZBM.DesktopHost.csproj

# 6. Make the Host reference your core business logic project
dotnet add EZBM.DesktopHost/EZBM.DesktopHost.csproj reference EZBM.Core/EZBM.Core.csproj
```

### Adding Android support

When the front-end and back-end are done and ready to go, we can pivot to adding Android support.

```bash
dotnet new maui -o EZBM.MobileApp
```

Once that project is made, you need to:

1. Bundle your compiled static React production build (`dist` or `build` directory assets) directly into the `wwwroot` directory of that new project.

2. Link the mobile package to your existing backend rules engine.

```bash
dotnet sln add EZBM.MobileApp/EZBM.MobileApp.csproj
dotnet add EZBM.MobileApp/EZBM.MobileApp.csproj reference EZBM.Core/EZBM.Core.csproj
```

3. Instead of using controllers to route HTTP calls, use the MAUI `.NET 10` native `HybridWebView` to execute your `EZBM.Core` data methods directly out of physical device memory, bypassing local web server performance limitations on mobile entirely.

## Implementing Local Storage

The backend uses SQLite and Entity Framework Core (EF Core) for local storage as these don't require a separate database engine server installation, and stores everything in one file.

### Add NuGet Packages

```bash
# Move into the Core directory
cd EZBM.Core

# Install EF Core and the SQLite driver
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```
