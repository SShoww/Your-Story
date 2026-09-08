# Repository Guidelines

## Project Structure & Module Organization

This is a .NET 8 MonoGame DesktopGL game. The solution is at
`CoPoject/CoPoject.slnx`; the application project is
`CoPoject/BePal/BePal.csproj`.

- `Program.cs` is the application entry point.
- `Game1.cs` contains the main MonoGame game loop and rendering setup.
- `Content/Content.mgcb` defines the MonoGame content-pipeline assets.
- `pipeline-references/` contains required MonoGame.Extended pipeline DLLs.
- `Icon.ico`, `Icon.bmp`, and `app.manifest` provide Windows application metadata.

Keep gameplay code under `CoPoject/BePal/`, grouping new features into focused
folders (for example, `Entities/Player.cs` or `Screens/MainMenuScreen.cs`). Keep
source assets and their content-pipeline definitions in `Content/`.

## Build, Test, and Development Commands

Run commands from the repository root:

```powershell
dotnet restore CoPoject/CoPoject.slnx  # Restore NuGet packages and local tools
dotnet build CoPoject/CoPoject.slnx    # Compile the game and content pipeline
dotnet run --project CoPoject/BePal    # Build and launch the DesktopGL game
```

The project restores .NET tools as part of its build, so the first build may take
longer. There is currently no automated test project; add one alongside the
application when introducing testable game logic.

## Coding Style & Naming Conventions

Use four-space indentation and standard C# conventions: PascalCase for types,
methods, and public members; camelCase for parameters and local variables; and
`_camelCase` for private fields. Keep MonoGame lifecycle behavior in the
appropriate overrides (`Initialize`, `LoadContent`, `Update`, `Draw`). Prefer
small classes with one clear responsibility and avoid putting unrelated systems
directly in `Game1`.

## Testing Guidelines

Place future unit tests in a dedicated project such as `CoPoject/BePal.Tests`.
Name test files after the unit under test (for example, `PlayerTests.cs`) and
tests as behavior statements, such as `Update_WhenEscapePressed_ExitsGame`.
Run all tests with `dotnet test CoPoject/CoPoject.slnx` once a test project exists.

## Commit & Pull Request Guidelines

History currently uses short imperative subjects (for example, `Initialize
MonoGame BePal project`). Continue that style: begin with a verb, keep the first
line focused, and separate unrelated changes into distinct commits. Pull requests
should state the gameplay or technical change, list validation performed, link any
relevant issue, and include screenshots or a short recording for visible game
changes.

## Agent skills

### Issue tracker

Issues and PRDs are tracked in this repository's GitHub Issues. See `docs/agents/issue-tracker.md`.

### Triage labels

Uses the default labels: `needs-triage`, `needs-info`, `ready-for-agent`, `ready-for-human`, and `wontfix`. See `docs/agents/triage-labels.md`.

### Domain docs

Single-context layout: root `CONTEXT.md` and `docs/adr/`. See `docs/agents/domain.md`.
