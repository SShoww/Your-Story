# Repository Guidelines

## Project Overview

**BePal** is a single-player pet-care management simulation built with C# and .NET 8 on the MonoGame DesktopGL framework.

The game explores learning the hidden rules and behaviors of abnormal creatures through trial and error in a research shelter/daycare. Core design pillars:
- **Cozy yet Dangerous**: Warm, domestic shelter environment contrasted with uncanny, abnormal creatures possessing lethal potential.
- **Consequential Interaction**: Every care action yields observable consequences—care routines must be deduced through careful experimentation.
- **Simulation Loop**: A 5-day cycle where the player interacts with daily assigned abnormal pets (Mossling, Nibbleclaw, Blinkbun), balancing player Health (3 HP) across timed Care QTEs and Dodge QTEs while logging discoveries into the Survival Log.

## Architecture & Data Flow

### High-Level Structure

The codebase enforces separation of concerns between the MonoGame engine lifecycle (`Game1.cs`, rendering, input polling) and pure C# domain/gameplay state models (`Gameplay/`).

```
CoPoject/BePal/
├── Program.cs             # Application entry point
├── Game1.cs               # Graphics host, input polling, QTE math, screen rendering
├── Gameplay/
│   ├── PetKind.cs         # Abnormal pet taxonomy (Baseline, Attacker, Trickster)
│   └── PrototypeRun.cs    # Pure domain model: day progression, HP, sessions, log unlock
└── Content/               # Fonts and 2D sprite textures compiled via Content.mgcb
```

As specified in `docs/adr/0001-screen-and-care-qte-architecture.md`, UI views currently coordinated via the `Screen` enum (`Menu`, `Help`, `Home`, `Log`, `Care`, `Dodge`, `Summary`) are designed to be decomposed into dedicated `IScreen` classes (`MainMenuScreen`, `HomeScreen`, `CareQteScreen`, `DodgeQteScreen`, `SurvivalLogScreen`, `SummaryScreen`).

### Key Modules & Subsystems

1. **Host & Engine Lifecycle (`Program.cs`, `Game1.cs`)**:
   - Manages `GraphicsDeviceManager` (1280x720 backbuffer), `SpriteBatch`, and `ContentManager`.
   - Polls `KeyboardState` and `MouseState` each frame in `Update()`.
   - Renders visual elements via `SpriteBatch` in `Draw()` with translation matrix screen shake effects.

2. **Run Progression & Domain Logic (`Gameplay/PrototypeRun.cs`)**:
   - Tracks `DayNumber` (1 to 5), `Health` (3 HP, reset daily), `Satisfaction` (0 to 3 per session), `SessionsToday`, `ForcedRetreats`, and per-pet completion counts (`_completedSessions`).
   - Pure C# state with zero MonoGame or rendering dependencies.
   - Triggers `Forced Retreat` when damage reduces Health to 0, immediately restoring Health to 3 and advancing the day.
   - Unlocks pet entries in the Survival Log once 3 Pet-Care Sessions are completed for that pet.

3. **Care QTE Subsystem**:
   - Radial wheel divided into 4 sectors corresponding to the 4 Care Actions: **Feed (Appetite)**, **Play (Recreation)**, **Pet (Intimacy)**, and **Observe (Observation)**.
   - Needle marker rotates continuously at $2.2 \text{ rad/s}$ (`_angle = (_angle + 2.2f * dt) % Tau`).
   - Spacebar input (`QTE Confirmation`) evaluates marker proximity to action centers ($\pm\pi/6$ hit tolerance). Matching the pet's preferred action gives +1 Satisfaction; mismatch or dead-zone hit inflicts -1 HP.

4. **Pet Variations & Hazard Mechanics**:
   - **Mossling (`PetKind.Baseline`)**: Hazard Lv 1 (Physical harm). Prefers Feed. Steady wheel rotation.
   - **Nibbleclaw (`PetKind.Attacker`)**: Hazard Lv 2 (Aggressive). Prefers Play. Triggers a Dodge QTE attack after 2 Care successes.
   - **Blinkbun (`PetKind.Trickster`)**: Hazard Lv 3 (Teleporting). Prefers Pet. Marker teleports to a random wheel angle once per Care QTE.

5. **Dodge QTE Subsystem**:
   - Triggered when aggressive pets attack. The wheel changes to a warning state with a golden **Dodge Zone** centered at $1.5\pi$ (span $\pi/3$, tolerance $\pm\pi/6$).
   - Pressing Spacebar while inside the zone evades the attack; failure inflicts -1 HP.

6. **Automated Playtest & Screenshot Pipeline**:
   - Built-in headless verification runner activated with `--screenshot`, `--playtest`, or `BEPAL_SCREENSHOT=1`.
   - Steps through 18 discrete frames across all screens and writes canonical PNG captures (`01_menu.png` through `06_summary.png`) to `screenshots/`.

### Data Flow

```
User Input (Keyboard / Mouse)
           │
           ▼
Game1.Update(GameTime)
   ├── Screen Bounds / Radial QTE Angle Check
   ├── QTE Confirmation (Spacebar)
   │        │
   │        ▼
   ├── PrototypeRun Mutation
   │     ├── RecordCareSuccess() -> Satisfaction++
   │     ├── CompleteSession()   -> SessionCount++, Satisfaction=0
   │     └── TakeDamage()        -> Health--, check Forced Retreat
   │
   └── Visual Timers & Tag FX
         ├── _shakeTime, _reactionTime (pet emotions)
         └── FloatingTag animations (lifetime, drift, alpha)
           │
           ▼
Game1.Draw(GameTime)
   └── SpriteBatch Render Pass (HUD, Pet, Track, Needle, Tags)
```

## Key Directories

- `CoPoject/`: Solution root containing `CoPoject.slnx`.
- `CoPoject/BePal/`: Primary game executable project (`BePal.csproj`).
  - `Gameplay/`: Pure domain models, entities, and state rules decoupled from MonoGame (`PrototypeRun.cs`, `PetKind.cs`).
  - `Content/`: Asset sources and pipeline configs (`Content.mgcb`, `PrototypeFont.spritefont`, `pet/` textures).
  - `pipeline-references/`: Precompiled MonoGame.Extended pipeline assemblies for build-time asset processing.
  - `.config/`: Tool manifest (`dotnet-tools.json`) pinning `dotnet-mgcb` CLI tools.
- `BEPAL/Docs/`: Game design documentation and agile planning.
  - `GDD/`: Concept, mechanics, core loop, class diagrams, and asset flow (`01-core-loop.md`, `04-class-diagram.md`).
  - `Agile/`: Sprint plans, sprint backlogs, kanban setup, and MoSCoW breakdowns.
- `docs/`: Repository governance and architectural documentation.
  - `adr/`: Architectural Decision Records (`0001-screen-and-care-qte-architecture.md`).
  - `agents/`: Domain guidelines, GitHub issue tracking conventions, and triage labels.
- `screenshots/`: Playtest artifacts and automated visual test captures (`01_menu.png` - `06_summary.png`).

## Development Commands

Run all commands from the repository root:

### Restore & Build
```powershell
# Restore NuGet packages and local .NET tools (dotnet-mgcb)
dotnet restore CoPoject/CoPoject.slnx

# Compile the game and run the content pipeline
dotnet build CoPoject/CoPoject.slnx
```

### Run
```powershell
# Launch the game interactively
dotnet run --project CoPoject/BePal

# Run automated playtest harness (captures screenshots and exits in ~4s)
dotnet run --project CoPoject/BePal -- --screenshot
```

### Test
```powershell
# Execute test suite (once test project CoPoject/BePal.Tests is present)
dotnet test CoPoject/CoPoject.slnx
```

### Tooling & Pipeline Commands
```powershell
# Restore .NET local tools explicitly
dotnet tool restore --configfile CoPoject/BePal/.config/dotnet-tools.json

# Build content assets manually via MonoGame Content Builder
dotnet mgcb CoPoject/BePal/Content/Content.mgcb
```

### Issue Tracking via GitHub CLI (`gh`)
```powershell
# List open issues
gh issue list --state open

# View issue with comments
gh issue view <number> --comments

# Create new issue
gh issue create --title "..." --body "..."
```

## Code Conventions & Common Patterns

### Formatting & Syntax
- C# 12 / .NET 8 syntax.
- 4-space indentation.
- File-scoped namespaces (`namespace BePal.Gameplay;`).
- Target-typed `new()` expressions (`private PrototypeRun _run = new();`).
- Use `#nullable enable` when introducing new classes and annotate reference types properly.

### Naming Conventions
- **Types & Methods**: `PascalCase` (`PrototypeRun`, `RecordCareSuccess`, `PetKind`).
- **Parameters & Local Variables**: `camelCase` (`gameTime`, `actionIdx`, `dodgeCenter`).
- **Private Fields**: `_camelCase` (`_graphics`, `_run`, `_angle`, `_tags`).
- **Constants & Action Arrays**: `PascalCase` or `UPPERCASE` (`Actions`, `ActionNeeds`, `Tau`).

### Domain Vocabulary (Mandatory)
Adhere strictly to canonical terms defined in `CONTEXT.md` (avoid synonyms):
- **Pet-Care Session** (*avoid*: round, encounter)
- **Satisfaction** (*avoid*: object bar, happiness bar)
- **Action Pattern** (*avoid*: pet rule, behaviour pattern)
- **Care QTE** (*avoid*: skill check, normal QTE)
- **Care Action**: Feed, Play, Pet, Observe (*avoid*: examine)
- **Pet Favor**: Very Effective (+2), Effective (+1), Neutral (+0), Rejection/Attack
- **Dodge QTE** (*avoid*: attack QTE)
- **Dodge Zone**: Safe zone on the Dodge QTE wheel
- **Wheel Marker** (*avoid*: needle, arrow)
- **Teleporting Marker**: Erratic marker jump modifier
- **QTE Confirmation**: Spacebar input to submit an attempt (*avoid*: click to confirm)
- **Health (HP)**: Player life resource (3 HP per day)
- **Forced Retreat**: Early end of a Game Day when HP hits 0 (*avoid*: game over)
- **Survival Log** (*avoid*: journal, notebook)

### State Management & Separation
- **Domain Decoupling**: Keep domain logic in pure C# classes under `CoPoject/BePal/Gameplay/`. Do not pass MonoGame types (`SpriteBatch`, `GraphicsDevice`, `GameTime`) into domain models.
- **Deterministic Mutators**: Domain models should expose explicit mutator methods (`RecordCareSuccess()`, `TakeDamage()`) rather than public setters.
- **Transient Presentation State**: Visual effects (shake time, reaction timers, floating text tags) are updated using elapsed frame seconds: `float dt = (float)gameTime.ElapsedGameTime.TotalSeconds`.

### Error Handling & Concurrency
- **Graceful Bounds Checking**: Never allow invalid index lookup or empty collection access to crash the 60 FPS loop. Use nullable return values (`int? GetHoveredActionIndex()`).
- **In-Game Consequence over Exceptions**: Gameplay mistakes (missed needle timing, wrong care action) trigger in-game state transitions (`TakeDamage()`, `ForcedRetreats`), never runtime exceptions.
- **Synchronous Execution**: The game loop runs synchronously on the main thread at 60 FPS. Do not dispatch background threads that touch MonoGame resources or `GraphicsDevice`.

### Dependency Injection & Modularization
- `Game1` is the composition root. When introducing new subsystems or decomposing screens into `IScreen`, inject dependencies via constructors or through MonoGame's `Game.Services` container (`IServiceProvider`).

## Important Files

| File Path | Description |
| --- | --- |
| `CoPoject/CoPoject.slnx` | Solution definition organizing the projects |
| `CoPoject/BePal/BePal.csproj` | Main project file (.NET 8, `WinExe`, package references, tool restore target) |
| `CoPoject/BePal/Program.cs` | Application entry point |
| `CoPoject/BePal/Game1.cs` | MonoGame engine lifecycle, screen rendering, QTE logic, and playtest harness |
| `CoPoject/BePal/Gameplay/PrototypeRun.cs` | Core simulation state: days, health, satisfaction, retreats, log unlocking |
| `CoPoject/BePal/Gameplay/PetKind.cs` | Pet enumeration (`Baseline`, `Attacker`, `Trickster`) |
| `CoPoject/BePal/Content/Content.mgcb` | Content pipeline asset build definitions |
| `CoPoject/BePal/.config/dotnet-tools.json` | .NET tool manifest declaring `dotnet-mgcb` 3.8.4 |
| `CONTEXT.md` | Domain glossary and ubiquitous language definitions |
| `docs/adr/0001-screen-and-care-qte-architecture.md` | ADR for screen decomposition and Care QTE architecture |
| `docs/agents/issue-tracker.md` | GitHub CLI conventions for issue and PRD tracking |
| `docs/agents/triage-labels.md` | Triage label mappings (`needs-triage`, `ready-for-agent`, etc.) |
| `BEPAL/Docs/GDD/01-core-loop.md` | Core daily gameplay loop flowcharts and specifications |
| `BEPAL/Docs/GDD/04-class-diagram.md` | Class diagrams and planned screen interface architecture |

## Runtime/Tooling Preferences

- **Runtime Target**: .NET 8.0 SDK (`<TargetFramework>net8.0</TargetFramework>`).
- **Application Type**: DesktopGL cross-platform Windows/Linux/macOS desktop application (`<OutputType>WinExe</OutputType>`).
- **Package Manager**: NuGet via the standard `dotnet` CLI.
- **Tooling Constraints**:
  - Solution uses XML-based `.slnx` format.
  - The project restores .NET local tools (`dotnet tool restore`) via an MSBuild target `RestoreDotnetTools` before resolving packages.
  - MonoGame.Extended pipeline DLL is loaded from `pipeline-references/MonoGame.Extended.Content.Pipeline.dll`.
  - Issue tracker workflows rely on `gh` CLI.

## Testing & QA

### Test Strategy & Structure
- Place automated unit tests in a dedicated project: `CoPoject/BePal.Tests` (targeting `net8.0`).
- Use **xUnit** or **NUnit** with FluentAssertions or standard asserts.
- Test domain logic headlessly: because classes under `Gameplay/` (e.g. `PrototypeRun`) have zero MonoGame/graphics dependencies, test all game rules, day advances, damage calculations, and session unlocks without a graphics device or window.

### Test Naming Convention
Follow the `UnitOfWork_StateUnderTest_ExpectedBehavior` pattern:
```csharp
[Fact]
public void TakeDamage_WhenHealthReachesZero_ForcesRetreatAndRestoresHealth()

[Fact]
public void CompleteSession_WhenCalled_IncrementsSessionsTodayAndResetsSatisfaction()

[Fact]
public void IsLogUnlocked_WhenCompletedSessionsUnderThree_ReturnsFalse()
```

### Automated Visual Regression QA
- Execute `dotnet run --project CoPoject/BePal -- --screenshot` to run the 18-frame automated playtest.
- Inspect the output in `screenshots/` (`01_menu.png` through `06_summary.png`) to verify UI layout, HUD text, dynamic pet emotion sprites, and screen transitions without manual playthrough.
