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
- Stripe Checkout first, or Square Payments if selected later

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

## Planning Docs

- `docs/planning/LEAN_CUSTOM_MARKETPLACE_PLAN.md`
- `docs/planning/SPRINT_PLAN.md`
- `docs/planning/ARCHITECTURE_DECISION.md`
- `docs/planning/SESSION_HANDOFF.md`
- `docs/agents/README.md`
