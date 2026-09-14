---
type: gdd-class-diagram
version: 0.4
date: 2026-09-14
---

# Class Diagram — BePal Architecture

## Architecture Overview

โครงสร้างคลาสของ BePal ออกแบบตามหลักการแยกหน้าที่ (Separation of Concerns) สอดคล้องกับแนวทางใน `AGENTS.md` และ `CONTEXT.md` โดยแยกชั้นการจัดการหน้าจอ (Screens), ระบบสลับฉาก (Scene Transitions), ระบบกล่องข้อความ (Dialogue), มินิเกมสัมผัส (Mini-Games) ออกจากตรรกะและสถานะของเกม (Gameplay & Domain Models)

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

    class ScreenManager {
        -Stack~IScreen~ _screens
        -StripeWipeTransition _activeTransition
        +ScreenContext Context
        +bool TransitionsEnabled
        +float TransitionDuration
        +StripeWipeTransition ActiveTransition
        +PushScreen(IScreen)
        +PopScreen()
        +SetScreen(IScreen, bool, float?)
        +CompleteTransition()
        +ShowMenu()
        +ShowPrologue()
        +ShowDoorstep()
        +ShowRoom(int)
        +BeginCare()
        +BeginDodge()
        +ShowSurvivalLog()
        +ShowSummary()
        +Fail(string)
        +AdvanceDay(string)
    }

    class StripeWipeTransition {
        +float Duration
        +float Elapsed
        +float Progress
        +int StripeHeight
        +int GapHeight
        +int Period
        +float Slant
        +Color StripeColor
        +Color BackgroundColor
        +bool IsActive
        +bool IsCovered
        +IScreen FromScreen
        +IScreen ToScreen
        +Update(float)
        +CalculateStripeBounds(float, int) (int, int)
        +Draw(SpriteBatch, Texture2D, int, int)
    }

    class IScreen {
        <<interface>>
        +bool IsOverlay
        +Update(GameTime)
        +Draw(GameTime, SpriteBatch)
    }

    class DialogueBox {
        -Queue~string~ _dialogueLines
        -string _currentLine
        -float _charTimer
        -int _revealedChars
        -bool _isFullTextRevealed
        -bool _showPrompt
        +bool IsFinished
        +Update(GameTime, MouseState)
        +Draw(SpriteBatch)
        +SetDialogue(IEnumerable~string~)
        +SetPrompt(string question, Action onYes, Action onNo)
        +SkipTypewriter()
    }

    class PrologueScreen {
        -DialogueBox _dialogueBox
        -int _sceneIndex
        -Texture2D _crateTexture
        +Update(GameTime)
        +Draw(GameTime, SpriteBatch)
    }

    class DoorstepScreen {
        -DialogueBox _dialogueBox
        -int _day
        -Texture2D _courierBoxTexture
        +Update(GameTime)
        +Draw(GameTime, SpriteBatch)
    }

    class PanoramicRoomScreen {
        -PanoramicRoomModel _roomModel
        -DialogueBox _dialogueBox
        -int _currentWall
        +RotateLeft()
        +RotateRight()
        +Update(GameTime)
        +Draw(GameTime, SpriteBatch)
    }

    class PanoramicRoomModel {
        -int _currentWallIndex
        +int CurrentWallIndex
        +RotateLeft()
        +RotateRight()
        +SetWall(int)
        +GetWallTitle(int) string
        +GetItemsForWall(int) List~InspectableItem~
        +GetPetBehaviorCue(PetKind) string
    }

    class CareQteScreen {
        -ICareMiniGame _activeMiniGame
        -float _needleAngle
        -float _needleSpeed
        -int _targetSector
        +Update(GameTime)
        +Draw(GameTime, SpriteBatch)
    }

    class DodgeQteScreen {
        -float _dodgeAngle
        -float _needleAngle
        -float _timeRemaining
        +Update(GameTime)
        +Draw(GameTime, SpriteBatch)
    }

    class DailySummaryScreen {
        -DailyReportCard _reportCard
        -bool _isAcknowledged
        +Update(GameTime)
        +Draw(GameTime, SpriteBatch)
    }

    class SurvivalLogScreen {
        -int _selectedPetIndex
        +Update(GameTime)
        +Draw(GameTime, SpriteBatch)
    }

    class ICareMiniGame {
        <<interface>>
        +bool IsCompleted
        +int Score
        +Update(GameTime)
        +Draw(GameTime, SpriteBatch)
    }

    class FeedMiniGame {
        -Rectangle _foodBowlRect
        -bool _isFoodPrepared
        +Update(GameTime)
        +Draw(GameTime, SpriteBatch)
    }

    class PetMiniGame {
        -Vector2 _touchPosition
        -float _comfortMeter
        +Update(GameTime)
        +Draw(GameTime, SpriteBatch)
    }

    class PlayMiniGame {
        -Vector2 _toyPosition
        -float _reactionTimer
        +Update(GameTime)
        +Draw(GameTime, SpriteBatch)
    }

    class ObserveMiniGame {
        -Rectangle _focusLensRect
        -float _insightGauge
        +Update(GameTime)
        +Draw(GameTime, SpriteBatch)
    }

    class PrototypeRun {
        -int _dayNumber
        -int _health
        -int _satisfaction
        -int _sessionsToday
        -int _forcedRetreats
        -PetKind _activePet
        -Dictionary~PetKind, int~ _completedSessions
        +int DayNumber
        +int Health
        +int Satisfaction
        +int SessionsToday
        +int ForcedRetreats
        +PetKind ActivePet
        +bool IsComplete
        +TakeDamage() bool
        +RecordCareSuccess(int)
        +CompleteSession()
        +EndDay()
        +IsLogUnlocked(PetKind) bool
    }

    class PetDefinition {
        +PetKind Kind
        +string Name
        +int HazardLevel
        +HarmType HarmType
        +ActionPattern Pattern
    }

    class ActionPattern {
        +CareAction PreferredAction
        +float NeedleSpeed
        +float SweetSpotTolerance
        +bool HasErraticNeedle
    }

    class BehaviorCue {
        +string Description
        +string VisualClue
        +CareAction ImpliedAction
    }

    Game1 --> ScreenManager
    ScreenManager --> StripeWipeTransition : manages
    StripeWipeTransition --> IScreen : transitions between
    ScreenManager --> IScreen
    PrologueScreen ..|> IScreen
    DoorstepScreen ..|> IScreen
    PanoramicRoomScreen ..|> IScreen
    CareQteScreen ..|> IScreen
    DodgeQteScreen ..|> IScreen
    DailySummaryScreen ..|> IScreen
    SurvivalLogScreen ..|> IScreen

    PrologueScreen --> DialogueBox
    DoorstepScreen --> DialogueBox
    PanoramicRoomScreen --> DialogueBox
    PanoramicRoomScreen --> PanoramicRoomModel
    CareQteScreen --> ICareMiniGame
    FeedMiniGame ..|> ICareMiniGame
    PetMiniGame ..|> ICareMiniGame
    PlayMiniGame ..|> ICareMiniGame
    ObserveMiniGame ..|> ICareMiniGame

    PanoramicRoomScreen --> PrototypeRun
    CareQteScreen --> PrototypeRun
    PrototypeRun --> PetDefinition
    PetDefinition --> BehaviorCue
```
