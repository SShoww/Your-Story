---
type: gdd-class-diagram
version: 0.2
date: 2026-09-11
---

# Class Diagram — BePal Architecture

## Architecture Overview

โครงสร้างคลาสของ BePal ออกแบบตามหลักการแยกหน้าที่ (Separation of Concerns) สอดคล้องกับแนวทางใน `AGENTS.md` และ `CONTEXT.md` โดยแยกชั้นการจัดการหน้าจอ (Screens) ออกจากตรรกะการบริหารและการเล่น (Gameplay & Domain Models)

```mermaid
classDiagram
    class Game1 {
        -GraphicsDeviceManager _graphics
        -SpriteBatch _batch
        -ScreenManager _screenManager
        -PrototypeRun _run
        +Initialize()
        +LoadContent()
        +Update(GameTime)
        +Draw(GameTime)
    }

    class IScreen {
        <<interface>>
        +Update(GameTime, InputState)
        +Draw(SpriteBatch)
    }

    class MainMenuScreen {
        +Update(GameTime, InputState)
        +Draw(SpriteBatch)
    }

    class HomeScreen {
        -PetDefinition _activePet
        +Update(GameTime, InputState)
        +Draw(SpriteBatch)
    }

    class CareQteScreen {
        -float _angle
        -float _qteTimer
        -bool _teleported
        +Update(GameTime, InputState)
        +Draw(SpriteBatch)
        +ResolveConfirmation()
    }

    class DodgeQteScreen {
        -float _angle
        -float _dodgeZoneAngle
        +Update(GameTime, InputState)
        +Draw(SpriteBatch)
        +CheckDodgeSuccess() bool
    }

    class SurvivalLogScreen {
        +Update(GameTime, InputState)
        +Draw(SpriteBatch)
    }

    class SummaryScreen {
        +Update(GameTime, InputState)
        +Draw(SpriteBatch)
    }

    class PrototypeRun {
        +int DayNumber
        +int Health
        +int Satisfaction
        +int SessionsToday
        +int ForcedRetreats
        +PetKind ActivePet
        +bool CanEndDay
        +bool IsComplete
        +RecordCareSuccess()
        +CompleteSession()
        +TakeDamage() bool
        +EndDay()
        +CompletedSessions(PetKind) int
        +IsLogUnlocked(PetKind) bool
    }

    class PetDefinition {
        +string Name
        +PetKind Kind
        +int HazardLevel
        +HarmType HarmType
        +ActionPattern Pattern
        +CareAction PreferredAction
    }

    class ActionPattern {
        +CareAction[] Steps
        +bool HasDodgeAttack
        +int AttackAtSatisfaction
        +bool HasTeleportingMarker
    }

    class CareAction {
        <<enumeration>>
        Feed
        Play
        Pet
        Observe
    }

    class HarmType {
        <<enumeration>>
        Physical
        Mental
    }

    class PetKind {
        <<enumeration>>
        Baseline
        Attacker
        Trickster
    }

    Game1 --> PrototypeRun
    Game1 --> IScreen
    MainMenuScreen ..|> IScreen
    HomeScreen ..|> IScreen
    CareQteScreen ..|> IScreen
    DodgeQteScreen ..|> IScreen
    SurvivalLogScreen ..|> IScreen
    SummaryScreen ..|> IScreen
    HomeScreen --> PrototypeRun
    CareQteScreen --> PrototypeRun
    DodgeQteScreen --> PrototypeRun
    PrototypeRun --> PetDefinition
    PetDefinition --> PetKind
    PetDefinition --> HarmType
    PetDefinition --> ActionPattern
    ActionPattern --> CareAction
```
