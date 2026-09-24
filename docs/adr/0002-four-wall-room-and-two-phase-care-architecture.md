# 2. Four-Wall Panoramic Shelter and Two-Phase Care Architecture

Date: 2026-09-13

## Status

Superseded by ADR 0003 (BePalV2 Production Architecture)

## Context

Following the user experience review and wireframe design iterations, the initial prototype's single-screen room and single-action QTE wheel lacked environmental immersion and gameplay depth:
1. **Lack of Spatial Immersion**: The original `HomeScreen` was a static single view without physical context or reason for room exploration.
2. **Repetitive Care QTE**: The original QTE wheel only required timing spacebar presses into a single predetermined quadrant three times with explicit answers displayed to the player, offering minimal player deduction or varied physical feedback.
3. **Narrative & Progression Disconnect**: The game lacked a narrative entry point explaining why abnormal pets were delivered, how the protagonist's shelter started, and how days transitioned with emotional weight.

## Decision

We adopt an expanded architecture combining a **Samsara Room-inspired 4-wall panoramic environment**, a **unified dialogue and inspection subsystem**, and a **2-Phase Care Loop**:

### 1. Samsara Room-Style 4-Wall Panoramic Shelter
The pet shelter room is structured into 4 navigable panoramic walls with left/right rotation controls:
- **Wall 1 (Pet Zone)**: Active pet bed/cat tree with ambient behavior cues; clicking the pet opens confirmation dialogue (`[YES]` / `[NO]`) to initiate care.
- **Wall 2 (Prep & Pantry)**: Food shelves, wash basin, and trash receptacle providing inspectable clues regarding pet diets and sensitivities.
- **Wall 3 (Study Desk & Records)**: Desk with inspectable Survival Log notebook, bulletin board, and mystery notes regarding the mysterious sender.
- **Wall 4 (Front Door & Entrance)**: Doorstep where mysterious delivery packages arrive each morning, window looking outside, and the clock/calendar to trigger `End Day`.

### 2. Unified Dialogue and Inspection Subsystem (`DialogueBox`)
A reusable dialogue component with a typewriter text effect, instant text reveal on click, and `NEXT =>` progression:
- Handles the **Day 1 Prologue** (shop preparation, doorbell ring, mystery box arrival, opening Mossling).
- Handles **Days 2–5 Doorstep Delivery** sequences.
- Handles **Point-and-Click Object Inspection** across all 4 walls.
- Handles **Care Confirmation** with `[YES]` / `[NO]` prompts.
- Text content is architected in English first, with localization capability.

### 3. Two-Phase Care Loop
Caring for an abnormal pet is divided into two distinct, complementary phases:
- **Phase 1: Deduction & Approach (Behavior Cues & Dynamic Wheel)**:
  - The pet displays real-time behavioral cues (e.g. grumbling belly, trembling fur, dilated pupils).
  - The player times the Wheel Marker on a dynamic wheel featuring erratic speeds and a central golden **Sweet Spot**.
  - *Consequence*: Mismatched deduction results in immediate pet aggression (-1 HP or triggers Dodge QTE). Matching deduction advances to Phase 2.
- **Phase 2: Tactile Care Mini-Game (Tactile Execution)**:
  - Upon deduction success, the interaction transitions into a tactile micro-game corresponding to the chosen action:
    - **Feed**: Hold-to-pour pouring mechanic to reach a safe line.
    - **Pet**: Gentle mouse stroking within safe speed thresholds.
    - **Play**: Reflex-catch timing challenge as the pet pounces.
    - **Observe**: Focus-lens examination to spot anomalies.
  - *Consequence*: Execution success yields +1 or +2 (Sweet Spot) Satisfaction. Execution failure results in +0 Satisfaction without HP loss.

### 4. Progression, Risk vs. Reward, and Day Transitions
- **Daily Sessions**: Completing one session satisfies the minimum daily requirement and activates the `End Day` action. Players may risk additional care sessions on the active pet to unlock Survival Log entries before nightfall.
- **Day Transition**: Ending the day or suffering a Forced Retreat (0 HP) routes to a **Daily Summary Report Card** (tracking sessions, logged clues, physical condition), followed by a fade-to-black night rest before the next day's doorstep arrival.

## Consequences

- **Positive**:
  - Dramatically improves gameplay feel, replayability, and emotional resonance.
  - Clear separation between spatial navigation, dialogue presentation, and QTE minigames.
  - Maintains domain decoupling by keeping state rules in `PrototypeRun` and gameplay patterns in `PetCatalog`.
- **Negative / Costs**:
  - Requires additional 2D background art (4 wall perspectives).
  - Requires 4 distinct micro-game controller implementations under `Screens/MiniGames/`.
