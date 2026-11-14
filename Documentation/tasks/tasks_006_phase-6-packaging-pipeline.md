# Tasks 261114-06

## Current Status

Phase 6 redesigns the presentation layer. While the React UI components are front-end work, Phase 6 includes a backend packaging and build pipeline task: bundling compiled frontend assets into the final .NET single-file executable package. All UI design and frontend implementation tasks are excluded.

## Tasks Summary

1. Configure MSBuild Pipeline to Bundle Frontend Build Artifacts into .NET Host

## Tasks

### Task 1: Configure MSBuild Pipeline to Bundle Frontend Build Artifacts into .NET Host

The .NET publish build pipeline needs target automation to embed or copy compiled frontend build output into `wwwroot` for single-file deployment.

Update `Backend.DesktopHost.csproj` with target hooks to package web assets.

1. Open `Backend/Backend.DesktopHost/Backend.DesktopHost.csproj`.
2. Add target or item definitions ensuring `wwwroot` content files are included in published output.
3. Validate publish command produces working executable with static assets preserved.

#### Things to Consider

- Maintain single-file extraction or embedded static file serving options for production builds.

## Next Step(s)

1. Review and test MSBuild publish targets for static file inclusion.

## Affected File(s)

1. `Backend/Backend.DesktopHost/Backend.DesktopHost.csproj`
