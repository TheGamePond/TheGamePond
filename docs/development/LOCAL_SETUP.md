# The Game Pond Local Setup

Date: 2026-04-28

## Prerequisites

- .NET 8 SDK
- PostgreSQL for the default development path
- Visual Studio 2022 or newer, Rider, or VS Code

## Restore And Build

```powershell
dotnet restore TheGamePond.sln
dotnet build TheGamePond.sln
```

This repo also includes a local tool manifest for Entity Framework:

```powershell
dotnet tool restore
```

## Database

The default development connection string points to:

```text
Host=localhost;Port=5432;Database=the_game_pond_dev;Username=postgres;Password=postgres
```

Override it with an environment variable when needed:

```powershell
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Port=5432;Database=the_game_pond_dev;Username=postgres;Password=your-local-password"
```

Apply the Sprint 0 Identity migration:

```powershell
dotnet ef database update --project TheGamePond/TheGamePond.csproj --startup-project TheGamePond/TheGamePond.csproj
```

## Seed First Owner

The migration seeds the `Owner`, `Admin`, and `Staff` roles.

To create the first owner account, set these environment variables before running the app:

```powershell
$env:SeedAdmin__Email = "owner@example.com"
$env:SeedAdmin__Password = "ChangeThis123"
$env:SeedAdmin__DisplayName = "The Game Pond Owner"
```

Use a real strong password for shared development or production environments. Do not commit owner credentials.

## Run The App

```powershell
dotnet run --project TheGamePond/TheGamePond.csproj
```

Open `/Account/Login` to sign in, or `/Admin` to confirm that unauthenticated users are redirected to the admin login.
