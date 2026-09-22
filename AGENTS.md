# Repository Guidelines

## Project Overview

**BePal** is a single-player pet-care management simulation built with C# 12 and .NET 8 on the MonoGame DesktopGL framework.

The game explores learning the hidden rules, biological needs, and behavioral patterns of abnormal creatures through trial and error in an anomalous research shelter/daycare. Core design pillars:
- **Cozy yet Dangerous**: Warm, domestic shelter environment contrasted with uncanny, abnormal creatures possessing lethal potential.
- **Consequential Interaction**: Every care action yields observable consequences—care routines must be deduced through careful experimentation.
- **Simulation Loop**: A structured 3-day shift progression balancing shelter Energy (6 AP daily budget), Pet Stats (Health, Stomach, Clean, EXP), timed 10-attempt Care QTEs, high-stakes combat encounters (Toothless stray taming, multi-phase Merchant boss battle, Chapter Boss incursion), and economic survival (Gold subsidies, loans with 20% compound interest, and emergency revives).

The repository hosts two parallel implementations in the solution:
1. **BePalV2 (`CoPoject/BePalV2`) [PRIMARY / PRODUCTION]**: The active production architecture implementing the canonical **New GDD (v2.0)** and **New Agile (v2.0)** suites: 3-day loop, 6 AP energy budget, starter pets (Coco/Mossling, Sproutlet/Nibbleclaw, Gloomtail/Blinkbun), 10-attempt Care QTE with dynamic gimmicks (reverse rotation, shrinking zones), stray taming & boss combat engine with Clean < 50 combat refusal, 8-slot inventory, CleanUI borderless theme, and 56 dedicated unit tests.
2. **BePal (`CoPoject/BePal`) [LEGACY REFERENCE / V1 PROTOTYPE]**: The initial 5-day prototype featuring Samsara Room-style 4-wall panoramic navigation, baseline abnormal pets (Mossling, Nibbleclaw, Blinkbun), 3 HP player resource, and 83 unit tests. Maintained intact as an architectural and regression reference.

---

## Architecture & Data Flow

### High-Level Structure

The codebase enforces strict **Clean Architecture** and separation of concerns between the MonoGame engine presentation layer (`Game1.cs`, screen managers, UI rendering, input polling) and pure C# domain/gameplay state models (`Gameplay/`).

```
CoPoject/
├── CoPoject.slnx                  # Solution containing all V1 and V2 projects
├── BePalV2/                       # PRIMARY: Active GDD v2.0 Production Game
│   ├── Program.cs                 # Application entry point
│   ├── Game1.cs                   # MonoGame host, ScreenManager, CleanUI window chrome, screenshot runner
│   ├── Screens/                   # Modular IScreen hierarchy and screen transitions
│   │   ├── IScreen.cs             # Screen lifecycle contract (Update, Draw, Enter, Exit, IsOverlay)
│   │   ├── ScreenManager.cs       # Screen stack, modal overlays, and transition coordinator
│   │   ├── ScreenContext.cs       # Shared graphics, font, audio, and V2RunState container
│   │   ├── StripeWipeTransition.cs# Diagonal Venetian-blinds transition (0.85s default)
│   │   ├── MainMenuScreen.cs      # Title screen with New Game / Continue
│   │   ├── ChoosePetScreen.cs     # Starter pet adoption (Coco/Mossling, Sproutlet/Nibbleclaw, Gloomtail/Blinkbun)
│   │   ├── BaseHabitatScreen.cs   # Core shelter hub, pet status HUD, care actions, upgrade station, clinic
│   │   ├── CareQteScreen.cs       # 10-attempt radial Care QTE with precision tiers & dynamic gimmicks
│   │   ├── CombatArenaScreen.cs   # Real-time combat (Day 2 Toothless Taming, Day 3 Merchant Boss, Chapter Boss)
│   │   ├── CalmingQteScreen.cs    # Day 1 Thunderstorm panic calming mini-game
│   │   ├── ShopModalScreen.cs     # Merchant 5-item purchasing overlay
│   │   ├── InventoryOverlayScreen.cs # 8-slot item grid (Use / Equip)
│   │   ├── DailySummaryScreen.cs  # Nightly debriefing, grade calculation, and shift gold payout
│   │   └── EndingScreen.cs        # Narrative outcomes (Ending A: Sell, Ending B: Protector, Forced Defeat)
│   ├── UI/
│   │   ├── CleanUI.cs             # Modern borderless theme, rounded panels, pill badges, buttons
│   │   └── DialogueBox.cs         # Narrative typewriter display with [YES]/[NO] choice prompts
│   ├── Gameplay/                  # PURE C# DOMAIN (Zero MonoGame references)
│   │   ├── V2RunState.cs          # Multi-day state machine, phase router, daily stats tracker
│   │   ├── PetEntity.cs           # Pet instance, stat calculations, equipment, natural decay
│   │   ├── Enums.cs               # Species, CareActions, PrecisionTiers, DailyPhases, Grades, StoryEndings
│   │   ├── CareQteEngine.cs       # 10-attempt needle math, target zones, streaks, scoring, gimmicks
│   │   ├── CombatEngine.cs        # Attack telegraphs, dodge zones, counter-attack windows, CanPetFight check
│   │   ├── EnergyAccount.cs       # 6 AP daily energy budget management
│   │   ├── EconomyManager.cs      # Gold treasury, loans, 20% daily compound interest
│   │   ├── InventoryService.cs    # 8-slot inventory grid and item equipment rules
│   │   └── ItemDefinition.cs      # Item registry (Crab Apple, Sea Tea, Cloudy Glasses, Torn Notebook, etc.)
│   ├── Audio/
│   │   ├── IAudioService.cs       # Sound service abstraction
│   │   ├── AudioManager.cs        # Audio player with procedural tone synthesis fallback
│   │   ├── NullAudioService.cs    # Headless test stub
│   │   └── SoundEffectType.cs     # Sound cue taxonomy (Confirm, Success, Fail, Dodge, etc.)
│   └── Content/                   # Spritefonts and textures compiled via Content.mgcb
├── BePalV2.Tests/                 # Unit test suite (56 tests) for V2 domain, engines, and screens
├── BePal/                         # LEGACY REFERENCE: V1 Samsara 4-Wall Prototype
│   ├── Program.cs, Game1.cs
│   ├── Screens/                   # PanoramicRoomScreen, CareQteScreen, DodgeQteScreen, etc.
│   ├── Gameplay/                  # PrototypeRun, PetKind, ActionPattern, HarmType
│   └── UI/, Audio/, Content/
└── BePal.Tests/                   # Unit test suite (83 tests) for V1 legacy domain & transitions
```

---

### Key Modules & Subsystems (BePalV2 Primary)

1. **Host & CleanUI Window Chrome (`Program.cs`, `Game1.cs`, `UI/CleanUI.cs`)**:
   - Manages `GraphicsDeviceManager` (1280x720 backbuffer), `SpriteBatch`, and `ContentManager`.
   - Renders a borderless modern clean UI theme: dark charcoal palette (`#1E1E24`), rounded cards, crisp status bars, pill-shaped badges, and custom window dragging.
   - Contains the headless visual regression runner (`--screenshot`) writing 10 canonical PNG captures to `screenshots/v2/`.

2. **Presentation & Screen Lifecycle (`Screens/IScreen.cs`, `Screens/ScreenManager.cs`)**:
   - `ScreenManager` coordinates screen stack operations (`SetScreen`, `PushScreen`, `PopScreen`).
   - Modal screens (such as `CareQteScreen`, `ShopModalScreen`, `InventoryOverlayScreen`) specify `IsOverlay => true` and render on top of `BaseHabitatScreen`.
   - Transitions are handled by `StripeWipeTransition`: a geometric two-phase Venetian-blinds diagonal wipe (0.85s duration) that completely gates user input during transitions.

3. **Base Habitat Hub & Environment Objects (`Screens/BaseHabitatScreen.cs`)**:
   - **Controls & Hotkeys:** `WASD` / Mouse to navigate, `1-4` for care actions, `B` for Bag, `S` for Shop, and `E` key for End Day / Door action.
   - **Front Door:** Opening door to morning events, delivery packages, and doorstep visits ("Knock Knock !!").
   - **Doctor NPC Clinic:** 500 Gold medical revive when pet HP hits 0.
   - **Upgrade Station:** 3 skill tree branches (QTE Zone +15%, Max Energy +2 AP, Care Progress Booster +50%).
   - **Survival Desk:** Research discoveries (Pet Discovery & Disaster Log).

4. **Care QTE Engine (`Gameplay/CareQteEngine.cs`, `Screens/CareQteScreen.cs`)**:
   - Consists of **10 consecutive care attempts** per session.
   - Spacebar input evaluates marker proximity against target zones into three **Precision Tiers**: Perfect, Good, Miss.
   - Dynamic gimmicks: Reverse Rotation (Feed), Escaping Zone (Clean), Blinking Needle (Heal), and Shrinking Zone.
   - Evaluates a final **Care Grade** (S, A, B, C, F) determining daily gold payout and pet affection.

5. **Combat & Taming Arena (`Gameplay/CombatEngine.cs`, `Screens/CombatArenaScreen.cs`)**:
   - **Combat Readiness Requirement:** `CombatEngine.CanPetFight(PetEntity)` enforces that pets must have **Clean >= 50** and **Health > 0**. Pets with Clean < 50 refuse to fight until cleaned.
   - **Day 2 Toothless Taming:** Stray encounter with dodge and counter-attack mechanics.
   - **Day 3 Merchant Boss Battle:** 3 distinct phases (Greed's Splash, Gold Gatling, Collector's Cane).
   - **Final Day Chapter Boss Incursion:** Overwhelming boss attack causing a canonical **Forced Defeat** that transitions to the vertical slice conclusion.

6. **Daily Loop & Run Progression (`Gameplay/V2RunState.cs`)**:
   - Manages the **3-Day Cycle** across 4 daily phases: MorningEvent, DaytimeCare, AfternoonEncounter, NightSummary.
   - Tracks active starter pet (Coco, Sproutlet, or Gloomtail) and optional tamed ally (Toothless).

7. **Energy & Economy Subsystems (`EnergyAccount.cs`, `EconomyManager.cs`)**:
   - **Energy (AP)**: Strict 6 AP daily budget (costs 1 AP per care action, 2 AP for heavy training).
   - **Economy**: Player treasury in Gold. +100G daily subsidy, emergency loans with 20% compound daily interest.

8. **Inventory Subsystem (`InventoryService.cs`, `ItemDefinition.cs`)**:
   - 8-slot fixed inventory grid supporting consumables (Crab Apple, Sea Tea, Cloudy Glasses, Torn Notebook, Caffeine Tonic) and equipment (Ballet Shoes, Toy Knife, Faded Ribbon).

---

### Data Flow

```
User Input (Keyboard / Mouse)
           │
           ▼
Game1.Update(GameTime)
   ├── ScreenManager Transition Check (gates input during StripeWipe)
   ├── Active Screen / Modal Overlay Update
   │        │
   │        ▼
   ├── Presentation to Domain Delegation:
   │     ├── CareQteScreen      ──> CareQteEngine.SubmitAttempt()
   │     ├── CombatArenaScreen  ──> CombatEngine.SubmitDodge() / SubmitCounter()
   │     ├── BaseHabitatScreen  ──> CombatEngine.CanPetFight() (Clean >= 50 check)
   │     │                      ──> V2RunState.ActivePet.ExecuteCareAction()
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
   ├── CleanUI Panel & Window Chrome Render Pass
   ├── Active Screen Draw Pass (BaseHabitatScreen + Modal Stack)
   └── StripeWipeTransition Overlay Render Pass (when active)
```

---

### Legacy Prototype Reference (BePal V1)

The V1 prototype (`CoPoject/BePal/`) explores an alternate baseline design:
- **5-Day Loop**: Interacting with daily assigned abnormal pets (**Mossling**, **Nibbleclaw**, **Blinkbun**).
- **Player Health (3 HP)**: Mistakes inflict physical/mental damage. Reducing HP to 0 causes a **Forced Retreat** (advances day immediately).
- **Samsara Room 4-Wall Navigation (`PanoramicRoomScreen.cs`)**: 360-degree shelter rotation (Pet Zone, Pantry, Study Desk, Shift Control).
- **Legacy Care QTE (`CareQteScreen.cs`)**: 4-sector radial wheel with single-confirm evaluation against pet preferred action.

---

## Key Directories

- `CoPoject/`: Solution root containing `CoPoject.slnx`.
- `CoPoject/BePalV2/`: **Primary game project** (`BePalV2.csproj`).
  - `Screens/`: Modular screens, `ScreenManager`, `ScreenContext`, and `StripeWipeTransition`.
  - `Gameplay/`: Pure domain models, engines, and state rules decoupled from MonoGame (`V2RunState.cs`, `CareQteEngine.cs`, `CombatEngine.cs`).
  - `UI/`: Reusable UI components (`CleanUI.cs`, `DialogueBox.cs`).
  - `Audio/`: Audio service and synthetic procedural waveform fallback (`AudioManager.cs`).
  - `Content/`: Asset sources and pipeline configs (`Content.mgcb`, `PrototypeFont.spritefont`).
- `CoPoject/BePalV2.Tests/`: Unit test suite (**56 tests**, .NET 8 xUnit) covering V2 domain logic, engines, economy, and screens.
- `CoPoject/BePal/`: **Legacy prototype project** (`BePal.csproj`).
- `CoPoject/BePal.Tests/`: Legacy unit test suite (**83 tests**, .NET 8 xUnit) covering V1 prototype rules and transitions.
- `BEPAL/Docs/`:
  - `NewGDD/`: **Active Game Design Document Suite (v2.0)** (`00-concept.md` through `05-asset-list.md`, `README.md`) detailing the 3-day loop, 6 AP energy budget, 10-attempt care QTE, Toothless taming, 3-phase Merchant boss battle, and full economy.
  - `NewAgile/`: **Active Agile Management Suite (v2.0)** (`01-product-backlog.md`, `02-sprint-backlog.md`, `03-kanban-board.md`, `04-Kanban-for-Obsidian.md`, `sprint-plan-01.md` through `sprint-plan-03.md`, `README.md`) with mathematically balanced 128 SP workload.
  - `GDD/` & `Agile/`: Preserved legacy v1 specifications for historical reference.
- `docs/`: Governance documentation (`gitflow-workflow.md`, `handoff.md`, `adr/`, `agents/`).
- `screenshots/`:
  - `v2/`: Canonical visual regression captures for BePalV2 (`01_menu.png` through `10_ending.png`).
  - Legacy captures for BePal V1 (`01_menu.png` through `09_transition.png`).

---

## Development Commands

Run all commands from the repository root:

### Restore & Build
```powershell
# Restore NuGet packages and local .NET tools across the entire solution
dotnet restore CoPoject/CoPoject.slnx

# Compile both V1 and V2 projects and run the content pipeline (Must build with 0 warnings, 0 errors)
dotnet build CoPoject/CoPoject.slnx
```

### Run
```powershell
# Launch Primary Game (BePalV2) interactively
dotnet run --project CoPoject/BePalV2

# Launch Legacy Prototype (BePal V1) interactively
dotnet run --project CoPoject/BePal
```

### Test
```powershell
# Execute complete unit test suite across both projects (139 tests: 56 V2 + 83 V1)
dotnet test CoPoject/CoPoject.slnx

# Execute only BePalV2 tests (56 tests)
dotnet test CoPoject/BePalV2.Tests

# Execute only BePal legacy tests (83 tests)
dotnet test CoPoject/BePal.Tests
```

### Automated Visual Regression QA
```powershell
# Run BePalV2 automated playtest harness (captures 10 frames to screenshots/v2/ and exits in ~4s)
dotnet run --project CoPoject/BePalV2 -- --screenshot

# Run BePal V1 automated playtest harness (captures 9 frames to screenshots/ and exits in ~4s)
dotnet run --project CoPoject/BePal -- --screenshot
```

### Tooling & Pipeline Commands
```powershell
# Restore .NET local tools explicitly (dotnet-mgcb)
dotnet tool restore --configfile CoPoject/BePalV2/.config/dotnet-tools.json

# Build content assets manually via MonoGame Content Builder
dotnet mgcb CoPoject/BePalV2/Content/Content.mgcb
```

---

## Gitflow Workflow & Branching Rules

This repository strictly follows the [Atlassian Gitflow Workflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow) standard (full specification in `docs/gitflow-workflow.md`):

1. **Core Branches**:
   - **`main`**: Production release history only. Direct commits strictly forbidden. Every commit is tagged (`vX.Y.Z`).
   - **`Develop`**: Continuous integration branch. All features branch from and merge back into `Develop`.
2. **Supporting Branches**:
   - **`feature/<topic>`**: Branch from `Develop`, merge back to `Develop` via PR or non-fast-forward merge (`--no-ff`). Delete branch after merge.
   - **`release/v<version>`**: Branch from `Develop` for stabilization only. Merge into both `main` (with tag) and `Develop`. Delete branch.
   - **`hotfix/v<version>`**: Branch from `main` to address critical production bugs. Merge into both `main` (with tag) and `Develop`. Delete branch.
3. **Mandatory Pre-Merge Validation (Full-Solution Rigor)**:
   - Must build with 0 errors and 0 warnings: `dotnet build CoPoject/CoPoject.slnx`
   - Must pass all **139 unit tests**: `dotnet test CoPoject/CoPoject.slnx`
   - Must pass V2 visual regression screenshot harness: `dotnet run --project CoPoject/BePalV2 -- --screenshot`

---

## Code Conventions & Common Patterns

### Formatting & Syntax
- C# 12 / .NET 8 syntax.
- 4-space indentation.
- File-scoped namespaces (`namespace BePalV2.Gameplay;`).
- Target-typed `new()` expressions (`private V2RunState _run = new();`).
- Explicit `#nullable enable` on all classes and properly annotated reference types.

### Architectural Invariants (Mandatory)
1. **Zero MonoGame Dependency in Domain**:
   - Classes under `Gameplay/` (`PetEntity`, `CareQteEngine`, `CombatEngine`, `EnergyAccount`, `EconomyManager`, `InventoryService`, `V2RunState`) must **never** import `Microsoft.Xna.Framework.*` or reference `SpriteBatch`, `GraphicsDevice`, or `GameTime`.
   - All domain logic must be 100% testable headlessly in `BePalV2.Tests`.
2. **Deterministic State Mutation**:
   - State mutations must occur through explicit domain methods (`ExecuteCareAction()`, `Spend()`, `ApplyNaturalDecay()`) rather than public property setters.
3. **Synchronous Single-Threaded Game Loop**:
   - The game loop runs synchronously on the main thread at 60 FPS. Do not dispatch background threads that touch MonoGame resources or `GraphicsDevice`.
4. **CleanUI Design Consistency**:
   - Window and UI elements must use the `CleanUI` design tokens (dark charcoal backgrounds, pill badges, rounded panels, high-contrast text).

---

## Domain Vocabulary (Mandatory)

### Primary V2 Production Vocabulary

Adhere strictly to canonical terminology from `BEPAL/Docs/NewGDD/` (avoid synonyms):
- **Energy (AP)**: Player daily action budget (3–6 AP per day) (*avoid*: stamina, mana, turns).
- **Pet Stats**: Health (0–100 HP), Stomach (0–100), Clean (0–100), EXP / Level (*avoid*: hunger points, dirtiness).
- **Starter Pets & Nicknames**:
  - **Coco / Mossling** (ไอ่แดง): Balanced bio-plant creature, soft, shy. Photosynthesis passive.
  - **Sproutlet / Nibbleclaw** (ไอ่ซุง): Agile feline-anteater with sharp claws. Agile Reflex passive.
  - **Gloomtail / Blinkbun** (ไอ่เขียว): Dark mysterious shadow rabbit with third eye. Shadow Barrier passive.
- **Stray Ally**: **Toothless** (Day 2 Stray Pet Encounter, tameable ally).
- **Care QTE**: 10-attempt radial mini-game driven by `CareQteEngine` (*avoid*: skill check, mini-game wheel).
- **Dynamic QTE Gimmicks**: Reverse rotation, Escaping zone, Blinking needle, Shrinking zone.
- **Precision Tiers**: **Perfect** (sweet spot, +streak), **Good** (outer band, maintains streak), **Miss** (dead zone, breaks streak).
- **Care Grade**: Overall session rating: **S**, **A**, **B**, **C**, **F** based on accuracy.
- **Care Actions**: **Feed**, **Clean**, **Play**, **Rest**, **Train** (costs 1–2 AP).
- **Combat Readiness**: Pets with **Clean < 50** refuse to fight (**Combat Refusal**).
- **Combat Arena**: Real-time combat driven by `CombatEngine` with telegraphs, golden **Dodge Zones**, and **Counter-Attack Windows**.
- **Merchant Boss Fight**: Day 3 encounter with 3 phases: **Phase 1 (Cane Strike)**, **Phase 2 (Coin Barrage)**, **Phase 3 (Golden Rage)**.
- **Chapter Boss Incursion**: Final Day boss fight ending in a canonical **Forced Defeat / Forced Retreat**.
- **Economy**: **Gold** (currency), **Daily Subsidy** (+100G), **Merchant Loan** (500G with 20% daily compound interest), **Doctor Revive** (500G to revive pet at 0 HP).
- **Upgrade Station**: Base facility with 3 upgrade branches (QTE Zone, Max Energy, Care Progress Booster).
- **Inventory Grid**: 8-slot fixed grid with consumable items and equipment accessories (*avoid*: backpack, bag).
- **Controls**: `WASD` / Mouse to navigate, `Spacebar` for QTE/Dodge/Counter, `E` for End Day, `1-4` for Care shortcuts, `B` for Bag, `S` for Shop.
- **CleanUI**: Modern dark borderless theme with pill badges and rounded cards.

### Legacy V1 Reference Vocabulary

Historical terms specific to the V1 prototype (`CoPoject/BePal/`):
- **Pet-Care Session**: Single care interaction with legacy pet.
- **Satisfaction**: 0 to 3 session progress bar.
- **Player Health (3 HP)**: Player life resource (inflicted by mis-timed needles).
- **Forced Retreat**: Early end of Day when player HP hits 0.
- **V1 Abnormal Pets**: Mossling (Baseline), Nibbleclaw (Attacker), Blinkbun (Trickster).
- **Samsara Navigation**: 4-Wall Panoramic Room (Pet Zone, Pantry, Study Desk, Front Door).
- **Survival Log**: Research discovery journal.
