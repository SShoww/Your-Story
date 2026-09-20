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
├── Screens/               # IScreen hierarchy, ScreenManager, transitions, and views
│   ├── IScreen.cs         # Modular screen lifecycle interface
│   ├── ScreenManager.cs   # Stack management, scene transitions, and input gating
│   ├── StripeWipeTransition.cs # Diagonal Venetian-blinds scene transition
│   ├── MainMenuScreen.cs  # Title screen and help overlay
│   ├── PanoramicRoomScreen.cs # 4-Wall Samsara Room habitat navigation
│   ├── CareQteScreen.cs   # Radial Care QTE wheel with 4 action sectors
│   ├── DodgeQteScreen.cs  # Evasive Dodge QTE mini-game
│   ├── PrologueScreen.cs  # Day 1 delivery crate unboxing sequence
│   ├── DoorstepScreen.cs  # Morning courier crate delivery sequence
│   ├── SurvivalLogScreen.cs # Research discovery journal overlay
│   └── SummaryScreen.cs   # Run debriefing and shift report
├── Gameplay/
│   ├── PetKind.cs         # Abnormal pet taxonomy (Baseline, Attacker, Trickster)
│   ├── PrototypeRun.cs    # Pure domain model: day progression, HP, sessions, log unlock
│   ├── PetCatalog.cs      # Pet definitions and species registry
│   ├── ActionPattern.cs   # Timing profiles and preferred actions
│   └── HarmType.cs        # Hazard classification (Physical, Mental)
├── UI/
│   └── DialogueBox.cs     # Typewriter narrative display and choice prompts
└── Content/               # Fonts and 2D sprite textures compiled via Content.mgcb
```

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

3. **Screen Management & Modular Views (`Screens/`)**:
   - `ScreenManager` coordinates screen lifecycle (`SetScreen`, `PushScreen`, `PopScreen`).
   - Modal screens (such as `SurvivalLogScreen`) specify `IsOverlay => true` and render on top of underlying shelter views.

4. **Scene Transition Subsystem (`Screens/StripeWipeTransition.cs`)**:
   - Two-phase geometric Venetian-blinds diagonal stripe wipe transition matching high-contrast graphic aesthetics.
   - **Phase 1 (Sweep In / Cover):** Alternating horizontal slats sweep across from left to right with bottom-leading diagonal slant, fully occluding the outgoing screen at midpoint.
   - **Midpoint Screen Swap:** Outgoing screen is replaced by incoming screen while 100% occluded.
   - **Phase 2 (Sweep Out / Reveal):** Slats clear from left to right with top-leading diagonal slant and smoothstep easing, revealing the incoming screen.
   - Configurable via `ScreenManager.TransitionDuration` (default `0.85f` seconds) and dynamic parameter overrides in `SetScreen()`. User input to underlying screens is gated during active transitions.

5. **Care QTE Subsystem (`Screens/CareQteScreen.cs`)**:
   - Radial wheel divided into 4 sectors corresponding to the 4 Care Actions: **Feed (Appetite)**, **Play (Recreation)**, **Pet (Intimacy)**, and **Observe (Observation)**.
   - Needle marker rotates continuously at $2.2 \text{ rad/s}$ (`_angle = (_angle + 2.2f * dt) % Tau`).
   - Spacebar input (`QTE Confirmation`) evaluates marker proximity to action centers ($\pm\pi/6$ hit tolerance). Matching the pet's preferred action gives +1 Satisfaction; mismatch or dead-zone hit inflicts -1 HP.

6. **Pet Variations & Hazard Mechanics**:
   - **Mossling (`PetKind.Baseline`)**: Hazard Lv 1 (Physical harm). Prefers Feed. Steady wheel rotation.
   - **Nibbleclaw (`PetKind.Attacker`)**: Hazard Lv 2 (Aggressive). Prefers Play. Triggers a Dodge QTE attack after 2 Care successes.
   - **Blinkbun (`PetKind.Trickster`)**: Hazard Lv 3 (Teleporting). Prefers Pet. Marker teleports to a random wheel angle once per Care QTE.

7. **Dodge QTE Subsystem (`Screens/DodgeQteScreen.cs`)**:
   - Triggered when aggressive pets attack. The wheel changes to a warning state with a golden **Dodge Zone** centered at $1.5\pi$ (span $\pi/3$, tolerance $\pm\pi/6$).
   - Pressing Spacebar while inside the zone evades the attack; failure inflicts -1 HP.

8. **Automated Playtest & Screenshot Pipeline**:
   - Built-in headless verification runner activated with `--screenshot`, `--playtest`, or `BEPAL_SCREENSHOT=1`.
   - Steps through 27 discrete frames across all screens and writes canonical PNG captures (`01_menu.png` through `09_transition.png`) to `screenshots/`.

### Data Flow

```
User Input (Keyboard / Mouse)
           │
           ▼
Game1.Update(GameTime)
   ├── ScreenManager Transition Check (gates input if transition active)
   ├── Active Screen Update & Radial QTE Angle Check
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
   ├── SpriteBatch Render Pass (HUD, Pet, Track, Needle, Tags)
   └── StripeWipeTransition Overlay Render Pass (when active)
```

## Key Directories

- `CoPoject/`: Solution root containing `CoPoject.slnx`.
- `CoPoject/BePal/`: Primary game executable project (`BePal.csproj`).
  - `Screens/`: Modular screens, `ScreenManager`, and `StripeWipeTransition`.
  - `Gameplay/`: Pure domain models, entities, and state rules decoupled from MonoGame (`PrototypeRun.cs`, `PetKind.cs`).
  - `UI/`: Reusable UI components (`DialogueBox.cs`).
  - `Content/`: Asset sources and pipeline configs (`Content.mgcb`, `PrototypeFont.spritefont`, `pet/` textures).
  - `pipeline-references/`: Precompiled MonoGame.Extended pipeline assemblies for build-time asset processing.
  - `.config/`: Tool manifest (`dotnet-tools.json`) pinning `dotnet-mgcb` CLI tools.
- `CoPoject/BePal.Tests/`: Unit test suite (45 tests, .NET 8 xUnit) covering domain logic, navigation, transitions, and UI.
- `BEPAL/Docs/`: Game design documentation and agile planning.
  - `NewGDD/`: **Active Game Design Document Suite (v2.0)** strictly adhering to the canonical 6-document + README structure (`00-concept.md` through `05-asset-list.md`, `README.md`) synthesizing the 64 presentation slides: 4-phase daily loop, 6 AP energy budget, 10-attempt care QTEs, Toothless taming encounter, multi-phase Merchant boss battle, and full economic/item system.
  - `NewAgile/`: **Active Agile Management Suite (v2.0)** mirroring canonical Agile structure (`01-product-backlog.md`, `02-sprint-backlog.md`, `03-kanban-board.md`, `04-Kanban-for-Obsidian.md`, `sprint-plan-01.md` through `sprint-plan-03.md`, `meeting-notes/`, `README.md`) with mathematically balanced 128 SP workload.
  - `GDD/`: Legacy baseline specifications (v1) preserved intact for historical reference.
  - `Agile/`: Legacy sprint plans, sprint backlogs, kanban setup, and MoSCoW breakdowns (v1).
- `docs/`: Repository governance and architectural documentation.
  - `gitflow-workflow.md`: Complete Gitflow branching, PR, and release specification.
  - `handoff.md`: Cross-session developer progress and delivery tracker.
  - `adr/`: Architectural Decision Records (`0001-screen-and-care-qte-architecture.md`, `0002-four-wall-room-and-two-phase-care-architecture.md`).
  - `agents/`: Domain guidelines, GitHub issue tracking conventions, and triage labels.
- `screenshots/`: Playtest artifacts and automated visual test captures (`01_menu.png` - `09_transition.png`).

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
# Execute complete unit test suite (45 tests)
dotnet test CoPoject/CoPoject.slnx
```

### Tooling & Pipeline Commands
```powershell
# Restore .NET local tools explicitly
dotnet tool restore --configfile CoPoject/BePal/.config/dotnet-tools.json

# Build content assets manually via MonoGame Content Builder
dotnet mgcb CoPoject/BePal/Content/Content.mgcb
```

### Gitflow Workflow & Branching Rules
This repository strictly follows the [Atlassian Gitflow Workflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow) standard (full specification in `docs/gitflow-workflow.md`):

1. **Core Branches**:
   - **`main`**: Production release history only. Direct commits strictly forbidden. Every commit is tagged (`vX.Y.Z`).
   - **`Develop`**: Continuous integration branch. All features branch from and merge back into `Develop`.
2. **Supporting Branches**:
   - **`feature/<topic>`**: Branch from `Develop`, merge back to `Develop` via PR with `--no-ff`. Delete branch after merge.
   - **`release/v<version>`**: Branch from `Develop` for stabilization/bugfixes only. Merge into both `main` (with tag) and `Develop`. Delete branch.
   - **`hotfix/v<version>`**: Branch from `main` to address critical production issues. Merge into both `main` (with tag) and `Develop`. Delete branch.
3. **Pre-Merge Validation**:
   - Must build with 0 errors: `dotnet build CoPoject/CoPoject.slnx`
   - Must pass all unit tests: `dotnet test CoPoject/CoPoject.slnx`
   - Must pass screenshot harness: `dotnet run --project CoPoject/BePal -- --screenshot`

### Issue Tracking via GitHub CLI (`gh`) & MCP
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
- **Types & Methods**: `PascalCase` (`PrototypeRun`, `RecordCareSuccess`, `PetKind`, `StripeWipeTransition`).
- **Parameters & Local Variables**: `camelCase` (`gameTime`, `actionIdx`, `dodgeCenter`, `normY`).
- **Private Fields**: `_camelCase` (`_graphics`, `_run`, `_angle`, `_tags`, `_activeTransition`).
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

## Testing & QA

### Test Strategy & Structure
- Place automated unit tests in dedicated project: `CoPoject/BePal.Tests` (targeting `net8.0`).
- Use **xUnit** with standard asserts.
- Test domain logic headlessly: because classes under `Gameplay/`, `UI/`, and transition math (`StripeWipeTransition`) have zero MonoGame/graphics device dependencies, test all game rules, day advances, damage calculations, session unlocks, and transition bounds without a graphics device or window.

### Automated Visual Regression QA
- Execute `dotnet run --project CoPoject/BePal -- --screenshot` to run the automated playtest.
- Inspect the output in `screenshots/` (`01_menu.png` through `09_transition.png`) to verify UI layout, HUD text, dynamic pet emotion sprites, narrative screens, and scene transitions without manual playthrough.
