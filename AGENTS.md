# Repository Guidelines

> 🚨 **AI AGENT CRITICAL DIRECTIVES**
> - **PRIMARY PRODUCTION**: `CoPoject/BePalV2` (Game) and `CoPoject/BePalV2.Tests` (Unit Tests).
> - **ACTIVE SOLUTION**: `CoPoject/CoPoject.slnx` contains only V2 projects. Legacy V1 prototype was isolated to git branch `feature/bepal-v1-legacy`.
> - **FAST TESTING**: Run `dotnet test CoPoject/BePalV2.Tests` during development (63 tests execute in ~0.3s).
> - **ZERO MONOGAME IN DOMAIN**: `Gameplay/` must NEVER import `Microsoft.Xna.Framework.*` or MonoGame types.
> - **CLEAN UI & DRAWING**: Use `CleanUI` design tokens and `MonoGame.Extended` primitives (`FillRectangle`, `DrawCircle`, `DrawLine`, `DrawRectangle`).
> - **ZERO WARNING POLICY**: Builds must compile with 0 errors and 0 warnings (`dotnet build CoPoject/CoPoject.slnx`).

---

## Project Overview

**BePal** is a single-player abnormal pet-care management simulation built with C# 12 and .NET 8 on the MonoGame DesktopGL framework.

The game explores discovering hidden rules, biological needs, and behavioral patterns of abnormal creatures through trial and error in an anomalous research shelter.
- **Core Loop**: A 3-day shift progression balancing shelter Energy (6 AP daily budget), Pet Stats (Health, Stomach, Clean, EXP), timed 10-attempt Care QTEs with dynamic gimmicks, real-time combat encounters (Day 2 Toothless stray taming, Day 3 multi-phase Merchant boss fight, Chapter Boss incursion), and economic survival (Gold subsidies, loans with 20% compound daily interest, emergency revives).
- **Core Atmosphere**: Cozy domestic shelter contrasted with lethal anomalous creatures. Every interaction yields observable consequences.

---

## Architecture & Data Flow

### High-Level Structure

The codebase strictly enforces **Clean Architecture** / Domain-Driven Design separating the MonoGame engine presentation layer from the pure C# domain layer.

```
CoPoject/
├── CoPoject.slnx                       # Modern VS XML solution (BePalV2 + BePalV2.Tests)
├── BePalV2/                            # PRIMARY PRODUCTION GAME
│   ├── Program.cs                      # WinExe entry point (Main)
│   ├── Game1.cs                        # MonoGame Game host, ScreenManager, screenshot QA runner
│   ├── Screens/                        # IScreen hierarchy, modals, and screen transitions
│   │   ├── IScreen.cs                  # Screen contract (Update, Draw, Enter, Exit, IsOverlay)
│   │   ├── ScreenManager.cs            # Stack-based screen manager & modal coordinator
│   │   ├── ScreenContext.cs            # Shared dependencies container (Graphics, SpriteBatch, Audio, RunState)
│   │   ├── StripeWipeTransition.cs     # Diagonal Venetian-blinds transition (0.85s, input-gated)
│   │   ├── MainMenuScreen.cs           # Title screen (New Game / Continue)
│   │   ├── ChoosePetScreen.cs          # Starter adoption (Coco, Sproutlet, Gloomtail)
│   │   ├── BaseHabitatScreen.cs        # Primary hub (pet HUD, care actions, upgrade station, clinic, door)
│   │   ├── CareQteScreen.cs            # 10-attempt radial Care QTE with dynamic gimmicks (Overlay)
│   │   ├── CombatArenaScreen.cs        # Real-time combat arena (Toothless taming, Merchant boss)
│   │   ├── CalmingQteScreen.cs         # Day 1 Thunderstorm panic calming mini-game
│   │   ├── ShopModalScreen.cs          # Merchant 5-item purchasing dialog (Overlay)
│   │   ├── InventoryOverlayScreen.cs   # 8-slot inventory grid modal (Overlay)
│   │   ├── DailySummaryScreen.cs       # Shift debrief, letter grade calculation, gold payout
│   │   └── EndingScreen.cs             # Narrative outcomes (Ending A Sell, Ending B Protector, Forced Defeat)
│   ├── UI/
│   │   ├── CleanUI.cs                  # Dark charcoal theme (#1E1E24), rounded panels, pill badges, buttons
│   │   └── DialogueBox.cs              # Typewriter text display with [YES]/[NO] prompts
│   ├── Gameplay/                       # PURE C# DOMAIN (Zero MonoGame references, 100% headless testable)
│   │   ├── V2RunState.cs               # Run aggregate root, phase router, daily stats tracker
│   │   ├── PetEntity.cs                # Virtual pet entity (Health, Stomach, Clean, EXP, natural decay)
│   │   ├── CareQteEngine.cs            # 10-attempt radial needle math, precision tiers, streaks, gimmicks
│   │   ├── CombatEngine.cs             # Combat wheel, telegraphs, dodge zones, counter-attacks, CanPetFight check
│   │   ├── EnergyAccount.cs            # 6 AP daily energy budget management
│   │   ├── EconomyManager.cs           # Gold treasury, 500G loans, 20% compound daily interest, doctor revives
│   │   ├── InventoryService.cs         # 8-slot fixed inventory grid and item equipment rules
│   │   ├── ItemDefinition.cs           # Consumables and equipment accessory definitions
│   │   ├── PlayerProgression.cs        # Skill tree levels (QTE Zone, AP Energy, Care Booster)
│   │   ├── EndlessEventManager.cs      # Scripted Day 1-3 events & procedural Day 4+ endless events
│   │   └── Enums.cs                    # Species, CareActions, PrecisionTiers, DailyPhases, Grades, StoryEndings
│   ├── Audio/
│   │   ├── IAudioService.cs            # Audio abstraction (semantic sound cues)
│   │   ├── AudioManager.cs             # Audio coordinator with procedural synthetic tone fallback
│   │   ├── NullAudioService.cs         # Headless test stub
│   │   └── SoundEffectType.cs          # Sound cue taxonomy
│   └── Content/                        # Asset sources (Content.mgcb, PrototypeFont.spritefont, pet sprites)
└── BePalV2.Tests/                      # UNIT TEST SUITE (63 tests, xUnit, .NET 8, pure headless)
```

### Key Modules

- **`V2RunState`**: Run aggregate root orchestrating multi-day loop, phases (`MorningEvent`, `DaytimeCare`, `AfternoonEncounter`, `NightSummary`), `ActivePet`, `ToothlessPet`, `EnergyAccount`, `EconomyManager`, `InventoryService`, `PlayerProgression`, and `StoryEnding`.
- **`PetEntity`**: Virtual pet domain model. Enforces stat limits (0–100) on Health, Stomach, and Clean. Calculates derived statuses (`IsStarving`, `IsGrimy`, `IsInfected`, `IsIncapacitated`), metabolic action burn (-5 stomach non-feed, -10 train), and overnight natural decay (-20 stomach, -15 clean).
- **`CareQteEngine`**: Mathematical simulation of 10-attempt radial needle. Evaluates angle proximity to target zones into precision tiers (`Perfect`, `Good`, `Miss`), streak multipliers, action gimmicks (Feed reverse, Clean escaping zone, Heal flickering needle), letter grades (`S`, `A`, `B`, `C`, `F`), and gold payouts.
- **`CombatEngine`**: Real-time combat simulation governing needle angular speed, dynamic dodge zones, 0.35s counter-attack windows, parry timing, and boss phases. Enforces **Combat Readiness Requirement**: `CombatEngine.CanPetFight(PetEntity)` requires `Clean >= 50` and `Health > 0` (pets refuse to fight when grimy).
- **`ScreenManager` & `IScreen`**: Manages the screen navigation stack (`SetScreen`, `PushScreen`, `PopScreen`). Supports transparent overlays (`IsOverlay => true`) rendered above `BaseHabitatScreen`. Coordinates `StripeWipeTransition` (0.85s diagonal wipe) which gates user input during transitions.
- **`CleanUI`**: Presentation design tokens for borderless modern UI: dark charcoal canvas (`#1E1E24`), rounded cards, crisp status bars, pill-shaped badges, and custom window chrome.

### Data Flow

```
User Input (Keyboard / Mouse)
           │
           ▼
Game1.Update(GameTime)
   ├── ScreenManager Transition Gate (blocks input during StripeWipeTransition)
   ├── Active Screen / Modal Overlay Update Pass
   │        │
   │        ▼
   ├── Presentation-to-Domain Delegation:
   │     ├── CareQteScreen      ──> CareQteEngine.SubmitAttempt()
   │     ├── CombatArenaScreen  ──> CombatEngine.SubmitDodge() / SubmitCounter()
   │     ├── BaseHabitatScreen  ──> CombatEngine.CanPetFight() (Clean >= 50 check)
   │     │                      ──> PetEntity.ExecuteCareAction()
   │     │                      ──> EnergyAccount.Spend(1)
   │     └── ShopModalScreen    ──> EconomyManager.SpendGold() & InventoryService.AddItem()
   │
   └── Domain State Mutation (Pure C#)
         ├── V2RunState.RecordCareSessionOutcome()
         ├── PetEntity.ApplyNaturalDecay()
         └── EconomyManager.CompoundDailyInterest()
           │
           ▼
Game1.Draw(GameTime)
   ├── CleanUI Panel & Window Chrome Pass
   ├── Screen Stack Draw Pass (BaseHabitatScreen + Modal Overlays)
   └── StripeWipeTransition Overlay Pass (when active)
```

---

## Key Directories

- `CoPoject/BePalV2/`: Main game source code.
  - `Gameplay/`: Pure domain logic, engines, state machines, and mathematical formulas (Zero MonoGame references).
  - `Screens/`: Game screens, overlays, transitions, and screen coordinator.
  - `UI/`: Reusable presentation components (`CleanUI`, `DialogueBox`).
  - `Audio/`: Sound abstraction, procedural waveform synthesizer, audio manager.
  - `Content/`: Asset sources, textures, fonts, and MGCB pipeline config.
- `CoPoject/BePalV2.Tests/`: Unit tests (63 tests) covering all domain engines, game loop, economy, combat, and progression.
- `BEPAL/Docs/NewGDD/`: Canonical Game Design Document v2.0 suite (`00-concept.md` to `05-asset-list.md`).
- `BEPAL/Docs/NewAgile/`: Sprint plans (01–03), product backlog, sprint backlog, and Kanban board (128 SP workload).
- `docs/`: Repository governance docs (`gitflow-workflow.md`, `handoff.md`, `adr/`, `agents/`, `02_Assets/`).
- `screenshots/v2/`: Canonical visual regression captures (`01_menu.png` through `10_ending.png`).

---

## Development Commands

Run all commands from the repository root:

### Restore & Build
```powershell
# Restore NuGet packages across solution
dotnet restore CoPoject/CoPoject.slnx

# Restore local dotnet tools (MGCB)
dotnet tool restore

# Build solution (Pre-merge rule: 0 errors, 0 warnings)
dotnet build CoPoject/CoPoject.slnx
```

### Run
```powershell
# Launch interactive game (1280x720 windowed)
dotnet run --project CoPoject/BePalV2
```

### Test
```powershell
# Run all unit tests (63 tests in ~0.3s)
dotnet test CoPoject/BePalV2.Tests

# Run filtered tests
dotnet test CoPoject/BePalV2.Tests --filter "FullyQualifiedName~CombatEngine"
```

### Lint & Format
```powershell
# Verify code formatting matches conventions
dotnet format CoPoject/CoPoject.slnx --verify-no-changes

# Auto-format codebase
dotnet format CoPoject/CoPoject.slnx
```

### Visual Regression QA Harness
```powershell
# Run automated playtest harness (captures 12 canonical PNG frames to screenshots/v2/ in ~4s)
dotnet run --project CoPoject/BePalV2 -- --screenshot
```

---

## Code Conventions & Common Patterns

### Formatting & Syntax
- C# 12 / .NET 8.
- 4-space indentation, no tabs.
- File-scoped namespaces: `namespace BePalV2.Gameplay;`.
- Target-typed `new()` expressions: `private readonly V2RunState _run = new();`.
- Nullable reference types enabled everywhere (`#nullable enable`).

### Naming Conventions
- **Classes, Interfaces, Structs, Enums**: PascalCase (`CareQteEngine`, `IScreen`, `PetEntity`).
- **Methods, Properties**: PascalCase (`ExecuteCareAction()`, `CurrentEnergy`).
- **Private Fields**: `_camelCase` prefix (`_screenStack`, `_audioManager`).
- **Enum Values**: PascalCase (`PrecisionTier.Perfect`, `DailyPhase.DaytimeCare`).
- **Unit Tests**: `[UnitOrAction]_[Scenario]_[ExpectedResult]` (e.g., `ToothlessTaming_SuccessfulDodgeAndCounter_FillsTameGauge`).

### Architectural Invariants (Mandatory)
1. **Zero MonoGame Dependency in Domain**:
   - Files under `CoPoject/BePalV2/Gameplay/` must NEVER import `Microsoft.Xna.Framework.*` or `MonoGame.Extended`.
   - Domain logic must remain 100% testable headlessly in `BePalV2.Tests` without GPU or audio device requirements.
2. **Encapsulated State Mutation**:
   - Domain models mutate state via explicit domain methods (`ExecuteCareAction()`, `Spend()`, `ApplyNaturalDecay()`) rather than public property setters.
3. **Synchronous Single-Threaded Game Loop**:
   - MonoGame update/draw loop runs synchronously at 60 FPS on the main thread. Do not dispatch async threads that touch domain state or MonoGame resources.
4. **Service Container Pattern (`ScreenContext`)**:
   - Dependencies (`GraphicsDevice`, `SpriteBatch`, `SpriteFont`, `IAudioService`, `V2RunState`, `ScreenManager`) are bundled in `ScreenContext` and passed down to screens upon initialization.
5. **Null Object Pattern for Headless Execution**:
   - `NullAudioService` implements `IAudioService` to safely stub audio in unit tests and automated CI runs.
6. **Error Handling & Preconditions**:
   - Domain methods validate invariants with fail-fast exceptions (`InvalidOperationException` or `ArgumentException`).
   - Presentation layer gracefully recovers from asset load failures (e.g., generating procedural 1x1 textures or procedural sound tones).

---

## Important Files

### Entry Points & Host
- `CoPoject/BePalV2/Program.cs`: WinExe entry point (`Main()`).
- `CoPoject/BePalV2/Game1.cs`: MonoGame `Game` host, backbuffer initialization (1280x720), `ScreenManager` setup, CleanUI window chrome, and `--screenshot` QA runner.

### Build & Project Config
- `CoPoject/CoPoject.slnx`: Modern Visual Studio XML solution file referencing `BePalV2` and `BePalV2.Tests`.
- `CoPoject/BePalV2/BePalV2.csproj`: Production project configuration (.NET 8, MonoGame DesktopGL, MonoGame.Extended 6.1.1, asset tools restore target).
- `CoPoject/BePalV2/.config/dotnet-tools.json`: Local manifest for `dotnet-mgcb` tool (v3.8.4).
- `CoPoject/BePalV2/Content/Content.mgcb`: MonoGame Content Builder pipeline file.

### Key Domain Engines (`CoPoject/BePalV2/Gameplay/`)
- `V2RunState.cs`: Central game state machine and run orchestrator.
- `PetEntity.cs`: Virtual pet health, stomach, clean stats, and natural decay formulas.
- `CareQteEngine.cs`: Radial Care QTE needle angle math, precision tiers, and scoring.
- `CombatEngine.cs`: Real-time combat telegraphs, dodge zones, counter-attacks, and readiness check.
- `EnergyAccount.cs`: 6 AP daily energy budget logic.
- `EconomyManager.cs`: Treasury, loan interest compounding, and revives.

### Key Presentation Screens (`CoPoject/BePalV2/Screens/`)
- `ScreenManager.cs`: Screen stack and modal transition coordinator.
- `BaseHabitatScreen.cs`: Main gameplay shelter hub and environment interactions.
- `CareQteScreen.cs`: 10-attempt radial Care QTE wheel overlay.
- `CombatArenaScreen.cs`: Real-time combat encounters (Toothless and Merchant Boss).
- `CleanUI.cs`: Modern borderless design tokens and 2D drawing primitives.

---

## Runtime & Tooling Preferences

- **SDK / Runtime**: .NET 8.0 SDK (with C# 12 support). `RollForward: Major` is configured.
- **Package Manager**: NuGet via standard `dotnet` CLI (`dotnet restore`).
- **No Third-Party Mocking**: Do not add Moq, NSubstitute, or other mocking libraries. Instantiate pure domain classes directly or use manual stubs (`NullAudioService`).
- **Rendering & Primitives**: Presentation uses `MonoGame.Framework.DesktopGL` (OpenGL/SDL2) and `MonoGame.Extended` (v6.1.1) primitives (`batch.FillRectangle`, `batch.DrawCircle`, `batch.DrawLine`).
- **Display Target**: 1280x720 backbuffer, per-monitor V2 DPI awareness via `app.manifest`.
- **Audio Fallback**: OpenAL device runtime. `AudioManager` provides automatic fallback to procedural PCM synthetic tones when audio hardware or .xnb files are missing.
- **Gitflow Branching**: Follow Atlassian Gitflow (`main`, `Develop`, `feature/*`, `release/*`, `hotfix/*`). See `docs/gitflow-workflow.md`.

---

## Testing & QA

### Unit Testing Framework
- **Runner & Framework**: xUnit 2.6.2 with `Microsoft.NET.Test.Sdk` 17.8.0.
- **Suite Location**: `CoPoject/BePalV2.Tests/` (10 test files, 63 unit tests, 100% pass rate).
- **Execution Performance**: Entire suite runs in ~0.3s.
- **Testing Pattern**: Arrange-Act-Assert with direct domain instantiation. Zero third-party mocks, zero async delays, zero MonoGame framework imports.

### Test Coverage Areas
1. **Game Loop & Progression** (`V2RunStateTests.cs`, `NewGddAlignmentTests.cs`): 3-day loop, phases, story endings, emergency loans.
2. **QTE Mechanics** (`CareQteEngineTests.cs`): Needle angles, precision zones, streaks, gimmicks, grading scale.
3. **Combat Simulation** (`CombatEngineTests.cs`): Clean >= 50 combat refusal check, boss phase transitions, dodge windows, counter-attacks, damage math.
4. **Pet Biology & Decay** (`PetEntityTests.cs`): Natural overnight decay, hunger burn, status damage, starvation, revival.
5. **Economy & Energy** (`EnergyAccountTests.cs`, `EconomyManagerTests.cs`, `InventoryServiceTests.cs`): Action points spending, treasury interest, item consumption.
6. **Hub Interactions** (`BaseHabitatTests.cs`, `SmokeTests.cs`): Base habitat state and smoke verifications.

### Automated Visual Regression QA
- Run `dotnet run --project CoPoject/BePalV2 -- --screenshot` to execute the automated playtest harness.
- Frame counter in `Game1.cs` captures 12 sequential high-resolution PNG frames across all game states into `screenshots/v2/` in ~4 seconds and safely exits.
- Pre-merge requirement: Must execute cleanly without exceptions.

---

## Domain Vocabulary (Mandatory)

Always use canonical terminology from `BEPAL/Docs/NewGDD/` (avoid synonyms):

| Canonical Term | Avoid Synonyms | Meaning / Context |
|---|---|---|
| **Energy (AP)** | Stamina, mana, turns | Player action budget (6 AP default daily budget). |
| **Pet Stats** | Hunger points, dirtiness | Health (0–100), Stomach (0–100), Clean (0–100), Level / EXP. |
| **Coco / Mossling** (ไอ่แดง) | Red pet, plant pet | Balanced bio-plant starter pet (Photosynthesis passive). |
| **Sproutlet / Nibbleclaw** (ไอ่ซุง) | Log pet, cat pet | Agile feline-anteater starter pet (Agile Reflex passive). |
| **Gloomtail / Blinkbun** (ไอ่เขียว) | Green pet, bunny | Shadow rabbit starter pet with third eye (Shadow Barrier passive). |
| **Toothless** | Stray dog, stray cat | Day 2 tameable stray pet encounter. |
| **Care QTE** | Skill check, mini-game wheel | 10-attempt radial mini-game managed by `CareQteEngine`. |
| **Dynamic Gimmicks** | QTE modifiers | Reverse rotation (Feed), Escaping zone (Clean), Blinking needle (Heal), Shrinking zone. |
| **Precision Tiers** | Hit ratings | **Perfect** (sweet spot), **Good** (outer band), **Miss** (dead zone). |
| **Care Grade** | Rating, score | Final session evaluation: **S**, **A**, **B**, **C**, **F**. |
| **Care Actions** | Tasks, chores | **Feed**, **Clean**, **Play**, **Rest**, **Train** (costs 1–2 AP). |
| **Combat Refusal** | Strike, unready | Pets with **Clean < 50** refuse to fight until cleaned. |
| **Combat Arena** | Battle screen | Real-time combat with telegraphs, **Dodge Zones**, and **Counter-Attack Windows**. |
| **Merchant Boss** | Shopkeeper boss | Day 3 boss encounter with Phase 1 (Cane), Phase 2 (Coins), Phase 3 (Rage). |
| **Chapter Boss Incursion** | Final boss | Final encounter resulting in canonical **Forced Defeat / Forced Retreat**. |
| **Gold** | Money, coins, cash | Shelter currency (+100G daily subsidy, 500G loan at 20% compound interest, 500G doctor revive). |
| **Upgrade Station** | Tech tree, workbench | Shelter upgrade station (QTE Zone, Max Energy, Care Progress Booster). |
| **Inventory Grid** | Bag, backpack | 8-slot fixed grid for consumables and equipment accessories. |
| **CleanUI** | UI theme, GUI | Modern dark charcoal borderless presentation theme. |
