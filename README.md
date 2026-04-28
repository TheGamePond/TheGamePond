# TheGamePond

Lean custom marketplace for The Game Pond.

## Stack

- ASP.NET Core MVC
- C#
- Entity Framework Core
- Razor Views
- Bootstrap 5.3.8
- PostgreSQL on a budget VPS by default
- SQL Server Express as an alternate database path
- Payment gateway checkout, with Stripe Checkout as the first temporary adapter

## Local Build

This workspace has a repo-local .NET SDK installed under `.dotnet/`.

PowerShell:

```powershell
$env:DOTNET_ROOT = Join-Path (Get-Location) '.dotnet'
$env:DOTNET_CLI_HOME = Join-Path (Get-Location) '.dotnet-home'
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:NUGET_PACKAGES = Join-Path (Get-Location) '.nuget\packages'
$env:APPDATA = Join-Path (Get-Location) '.appdata'
$env:LOCALAPPDATA = Join-Path (Get-Location) '.localappdata'
$env:PATH = "$env:DOTNET_ROOT;$env:PATH"
dotnet build TheGamePond.sln --configfile NuGet.Config
```

## Sprint 0 Foundation

Sprint 0 now includes:

- ASP.NET Core Identity wired to Entity Framework Core.
- PostgreSQL provider configured through `DefaultConnection`.
- Seeded `Owner`, `Admin`, and `Staff` roles in the initial migration.
- Optional first owner seed through `SeedAdmin__Email` and `SeedAdmin__Password`.
- Minimal admin login and protected admin dashboard shell.
- Shipping-only launch assumption.
- Payment-gateway-only checkout; no cash/COD/manual payment flow in the MVP.

See `docs/development/LOCAL_SETUP.md` for database migration and first-owner setup.

## Planning Docs

- `docs/planning/LEAN_CUSTOM_MARKETPLACE_PLAN.md`
- `docs/planning/SPRINT_PLAN.md`
- `docs/planning/ARCHITECTURE_DECISION.md`
- `docs/planning/SESSION_HANDOFF.md`
- `docs/development/LOCAL_SETUP.md`
- `docs/agents/README.md`
