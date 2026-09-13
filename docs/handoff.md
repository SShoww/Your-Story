# BePal — Session Handoff Document

**Date:** 2026-09-13  
**Target Branch:** `Develop`  
**Current Status:** All GDD, Agile, ADR, and CONTEXT documentation fully synchronized and verified with passing unit tests (7/7). Ready for Milestone 1 implementation.

---

## 1. Context & Design Decisions Summary

During this session, we conducted an in-depth design distillation (`grill-me`) based on user wireframes and narrative design notes, establishing the following core system improvements:

### Key Architectural & Gameplay Decisions
1. **Narrative & Progression Flow:**
   - **Day 1:** Full prologue sequence (dream/shop opening preparation $\rightarrow$ mystery package arrives at doorstep $\rightarrow$ unboxing reveals Mossling).
   - **Days 2–5:** Doorstep morning delivery sequence with new daily abnormal pet before entering the shelter.
2. **Samsara Room-Style 4-Wall Panoramic Shelter:**
   - Shelter room is divided into 4 navigable panoramic walls with left/right rotation controls (`[◄ Left]` and `[Right ►]`):
     - **Wall 1 (Pet Zone):** Active pet bed/cat tree with ambient behavior cues; clicking pet opens prompt `[YES]/[NO]` to initiate care.
     - **Wall 2 (Prep & Pantry):** Food shelves, sink, and trash providing inspectable clues on dietary preferences.
     - **Wall 3 (Study Desk):** Desk with clickable Survival Log notebook and mystery research notes.
     - **Wall 4 (Front Door):** Entrance door, window, and clock/calendar for the `[End Day]` action.
3. **Unified Dialogue & Inspection System (`DialogueBox`):**
   - Reusable dialogue component with typewriter effect, click-to-fast-reveal, `NEXT =>` button, and `[YES]/[NO]` prompt buttons.
   - Used uniformly across Prologue, Doorstep arrival, room item inspection, and pet confirmation.
   - Text is architected in **English first**.
4. **Flexible Care Progression (Risk vs. Reward):**
   - Completing 1 Pet-Care Session unlocks the `[End Day]` button on Wall 4 / HUD.
   - The player can safely end the day or risk additional care sessions on the active pet to gather Survival Log entries before nightfall.
5. **2-Phase Care Loop:**
   - **Phase 1 (Deduction & Dynamic Wheel):** Read pet Behavior Cues (e.g. grumbling belly, trembling fur, dilated pupils) in real-time, time the Wheel Marker on a dynamic wheel featuring erratic speeds and a central golden **Sweet Spot** ($\pm 15^\circ$ awarding +2 Satisfaction).
   - **Phase 2 (Tactile Care Mini-Games):** Correct Phase 1 deduction transitions into a tactile micro-game (2–3 seconds):
     - **Feed:** Hold-to-pour pouring mechanic to hit a safe line.
     - **Pet:** Mouse stroke interaction within gentle speed thresholds.
     - **Play:** Reflex catch timing challenge as pet pounces.
     - **Observe:** Focus lens inspection to spot anomalies.
   - **Consequence Rules:**
     - Phase 1 wrong deduction = immediate pet aggression (-1 HP or Dodge QTE).
     - Phase 2 execution failure = +0 Satisfaction (no HP loss; fair and non-punitive).
     - Phase 2 execution success = +1 Satisfaction (+2 if Phase 1 hit Sweet Spot).
6. **Day Conclusion & Transition:**
   - Ending the day or Forced Retreat (0 HP) routes to a **Daily Summary Report Card** (Papers, Please style), followed by a fade-to-black night rest and morning doorstep arrival.

---

## 2. Updated Artifacts & References

All architectural and game design documentation has been updated to reflect these decisions:

- **ADR:**
  - `docs/adr/0002-four-wall-room-and-two-phase-care-architecture.md` (Newly created)
  - `docs/adr/0001-screen-and-care-qte-architecture.md` (Existing baseline)
- **Domain Vocabulary:**
  - `CONTEXT.md` (Added 4-Wall Panoramic Shelter, 2-Phase Care Loop, Behavior Cue, Golden Sweet Spot, Tactile Care Mini-Game, Daily Summary Report, Unified Dialogue Box)
- **Game Design Documents (GDD):**
  - `BEPAL/Docs/GDD/00-concept.md` (Updated Starting Point, Narrative Goal, Samsara Room reference)
  - `BEPAL/Docs/GDD/01-core-loop.md` (Updated daily loop Mermaid diagram, scene breakdown, controls)
  - `BEPAL/Docs/GDD/02-scope-features.md` (Updated feature priorities to Must-Have)
  - `BEPAL/Docs/GDD/03-mechanics.md` (Updated state machine, 4-wall navigation, 2-phase care rules)
  - `BEPAL/Docs/GDD/04-class-diagram.md` (Added `DialogueBox`, `PanoramicRoomScreen`, `ICareMiniGame` hierarchy)
  - `BEPAL/Docs/GDD/05-asset-list.md` (Added 4-wall backgrounds, dialogue UI, mini-game props, audio SFX)
- **Agile & Backlog Planning:**
  - `BEPAL/Docs/Agile/01-product-backlog.md` (Updated Traceability Matrix and Sprint 3 & 4 tasks; total 128 SP)
  - `BEPAL/Docs/Agile/02-sprint-backlog.md` (Updated Sprint 3 & 4 tables and capacity matrix)
  - `BEPAL/Docs/Agile/03-kanban-board.md` (Updated role workflows, Kanban board, task tracking table)
  - `BEPAL/Docs/Agile/04-Kanban-for-Obsidian.md` (Fixed Obsidian Kanban plugin formatting and synced cards)
  - `BEPAL/Docs/Agile/sprint-plan-02.md` (Marked completed technical tasks as Done)

---

## 3. Codebase State & Verification

- **Solution:** `CoPoject/CoPoject.slnx`
- **Build Status:** Builds with 0 errors and 0 warnings (`dotnet build CoPoject/CoPoject.slnx`).
- **Test Suite:** `CoPoject/BePal.Tests` passes 7/7 tests (`dotnet test CoPoject/CoPoject.slnx`).
- **Screenshot Harness:** Automated 18-frame visual test runs and completes cleanly (`dotnet run --project CoPoject/BePal -- --screenshot`).

---

## 4. Immediate Next Steps for Next Session

The next agent should begin **Sprint 3 / Milestone 1** implementation:

1. **Implement `DialogueBox` Subsystem (`TECH-05` / `T-23`):**
   - Create `DialogueBox.cs` supporting typewriter character reveal, click/Spacebar fast reveal, `NEXT =>` button, and `[YES]/[NO]` prompt buttons.
   - Text rendering using `PrototypeFont.spritefont` with proper line wrapping.
2. **Implement `PrologueScreen.cs` and `DoorstepScreen.cs` (`US-15` / `T-10`):**
   - Connect the Day 1 prologue narrative (Intro $\rightarrow$ Mystery Box $\rightarrow$ Open Mossling).
   - Wire transitions into `ScreenManager`.
3. **Implement `PanoramicRoomScreen.cs` (`US-20` / `T-24`):**
   - 4-wall panoramic navigation with left/right rotation arrows.
   - Object inspection hitboxes on Wall 2 (Pantry), Wall 3 (Survival Log desk), Wall 4 (Front Door).
   - Pet click confirmation prompt on Wall 1 to enter `CareQteScreen`.

---

## 5. Suggested Skills for Next Agent

- `tdd`: Use test-driven development when building stateful components (e.g. `DialogueBox` text queue / typewriter timing and `PanoramicRoomScreen` wall index wrapping).
- `karpathy-guidelines`: Adhere to surgical, minimal, non-overengineered changes when extending MonoGame screens and UI components.
- `to-issues`: Use if breaking down Sprint 3 and Sprint 4 backlog items into trackable GitHub CLI issues.
