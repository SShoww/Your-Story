# BePal

BePal is a single-player pet-care management simulation built with C# 12 and .NET 8 on MonoGame DesktopGL. The game explores learning the hidden rules, biological needs, and behavioral patterns of abnormal creatures through trial and error within an anomalous research shelter and daycare.

The primary production implementation is **BePalV2** (`CoPoject/BePalV2`), adhering strictly to the canonical **New GDD (v2.0)** specifications and architecture (superseding the V1 prototype).

---

## Core Philosophy

**Cozy yet Dangerous**:
The contrast between a warm, domestic shelter environment and uncanny, abnormal creatures possessing lethal potential.
_Avoid_: Pure horror, cute pet sim

**Consequential Interaction**:
Every care action, resource allocation, and narrative choice yields observable consequences—trial and error is required to deduce safe care routines.
_Avoid_: Arbitrary punishment, inconsequential branching

**Tactical Resource & Energy Budgeting**:
Balancing shelter Energy (6 AP daily budget), player Gold, and 4-tier Pet Stats across a high-density 3-day shift progression.

---

## Language & Domain Terms

### Energy & Resource Management

**Energy (AP)**:
The player's discrete daily action budget (3–6 AP per day, default 6 AP), spent on care actions (1 AP) and heavy training (2 AP).
_Avoid_: Stamina, mana, turns

**Economy & Gold**:
The shelter treasury in Gold currency.
- **Daily Subsidy**: Regular morning stipend (+100 Gold).
- **Merchant Loan**: Emergency credit when funds are insufficient, accruing 20% compound daily interest.
- **Doctor Revive**: Emergency medical revive (500 Gold) via Doctor NPC when pet Health drops to 0.

---

### Virtual Pet Domain

**Pet Stats**:
The 4-dimensional virtual pet attributes:
- **Health (0–100 HP)**: Core vitality; reducing to 0 causes pet incapacitation requiring emergency revive.
- **Stomach (0–100)**: Satiety level; drops by -20 on day change, -5 per non-feed action, -10 on Train. Reaching 0 causes the Starving status effect (-10 HP daily, -2 HP per action).
- **Clean (0–100)**: Hygiene and immunity; drops by -15 on day change. Falling below 50 triggers the Grimy status effect and Combat Refusal. Falling below 25 causes Infected status (-15 HP daily).
- **EXP / Level**: Pet growth progression (Lv. 1–10); leveling increases Max HP and counter-attack damage.
_Avoid_: Hunger points, dirtiness, happiness bar

**Starter Pets & Nicknames**:
- **Coco / Mossling** (ไอ่แดง): Balanced bio-plant creature, soft, shy. Passive: *Photosynthesis* (+5 HP auto-heal when Clean > 80). Favored actions: Feed, Clean.
- **Sproutlet / Nibbleclaw** (ไอ่ซุง): Agile feline-anteater with sharp claws. Passive: *Agile Reflex* (+15% Perfect zone window in Train). Favored actions: Train, Clean.
- **Gloomtail / Blinkbun** (ไอ่เขียว): Dark mysterious shadow rabbit with third eye. Passive: *Shadow Barrier* (-25% Dodge QTE damage penalty). Favored actions: Heal, Feed.

**Stray Ally**:
- **Toothless**: Acidic wild stray reptile encountered on Day 2; can be tamed or chased away. Passive: *Ferocious* (2x counter-attack damage).

**Care Actions**:
The four core daytime care commands executed from the console:
- **Feed**: Sustenance restoring Stomach and granting EXP (costs 1 AP).
- **Clean**: Bathing/scrubbing to restore Clean and prevent infection or combat refusal (costs 1 AP).
- **Train**: Intensive conditioning granting high EXP at the cost of Stomach (costs 1 AP).
- **Heal**: Medical treatment restoring Health and removing ailments (costs 1–2 AP).
_Avoid_: Examine, observe, scrub, play, pet

---

### Care QTE & Precision Mechanics

**Care QTE**:
The 10-attempt radial needle mini-game driven by `CareQteEngine` assessing care action accuracy.
_Avoid_: Skill check, mini-game wheel, normal QTE

**Precision Tiers**:
Timing windows evaluated on Spacebar input:
- **Perfect**: Sweet spot ($\pm 0.20$ rad), awards maximum score and increments streak counter.
- **Good**: Outer band ($\pm 0.45$ rad), awards standard score and preserves streak counter.
- **Miss**: Outside active zones, awards zero points and breaks streak.

**Care Grade**:
Overall session evaluation rating (**S**, **A**, **B**, **C**, **F**) calculated from final QTE score, determining gold payout (10–50 Gold) and pet affection.

**Dynamic QTE Gimmicks**:
Behavioral disruptions during Care QTE sessions:
- **Reverse Rotation**: Needle reverses direction abruptly (Feed mode).
- **Escaping Zone**: Target zone evades needle movement (Clean mode).
- **Blinking Needle**: Target zone or needle disappears intermittently (Heal mode).
- **Shrinking Zone**: Target zone narrows as attempt count advances.

---

### Combat & Encounters

**Combat Readiness Requirement**:
Rule enforced by `CombatEngine.CanPetFight(PetEntity)`: pets must have **Clean >= 50** and **Health > 0**. Pets with Clean < 50 refuse to fight (**Combat Refusal**) until cleaned.

**Combat Arena**:
Real-time reflex combat encounter screen driven by `CombatEngine`:
- **Telegraphed Attack**: Visual indicator of incoming enemy strike.
- **Dodge Zone**: Golden safe arc where Spacebar reflex avoids incoming damage.
- **Counter-Attack Window**: Ring-convergence opening enabling reactive damage against the opponent.
- **Parry Window**: Advanced timing window for negating attacks and reflecting damage.

**Encounters & Bosses**:
- **Day 1 Thunderstorm**: Natural disaster causing panic and dirtiness; resolved via Calming QTE.
- **Day 2 Toothless Taming**: Wild stray encounter featuring the [Chase] vs [Tame] moral dilemma.
- **Day 3 Merchant Boss Battle**: 3-phase confrontation when refusing to sell Toothless for 5,000 Gold:
  - *Phase 1 (Cane Strike / Greed's Splash)*: Acid flask projectiles (HP 1000 -> 700).
  - *Phase 2 (Coin Barrage / Gold Gatling)*: Rapid coin projectiles with +2G bonus per dodge (HP 700 -> 300).
  - *Phase 3 (Golden Rage / Collector's Cane)*: Teleporting strikes and purple parry windows (HP 300 -> 0).
- **Chapter Boss Incursion**: Final Day base invasion ending in a canonical **Forced Defeat / Forced Retreat**.

---

### Daily Loop & Progression

**4-Phase Daily Loop**:
The structured shift progression across each day:
1. **Morning Event (Phase 1)**: Weather briefings, hazard warnings, and doorstep arrivals ("Knock Knock !!").
2. **Daytime Care (Phase 2)**: 6 AP energy budgeting across Feed, Clean, Train, Heal, and Base facilities.
3. **Afternoon Encounter / Defense (Phase 3)**: Disasters, wild taming, or boss combat.
4. **Night Summary / Progression (Phase 4)**: Natural decay application (Stomach -20, Clean -15), sickness checks, and report card payout.

**Narrative Outcomes & Endings**:
- **Ending A (Sell / Betrayal)**: Accept Merchant's 5,000 Gold buyout; financial security at the cost of your companion.
- **Ending B (Protector / Heroic)**: Defeat Merchant in 3-phase combat to defend your sanctuary and companions.
- **Vertical Slice Finale (Forced Defeat)**: Chapter boss overwhelms sanctuary; retreat to inner vault leading into full game storyline.

---

### Base Shelter & Facilities

**Base Habitat Screen**:
The unified 1280x720 interactive sanctuary screen (`BaseHabitatScreen`) featuring interactive stations:
- **Front Door**: Red double doors for morning events, deliveries, and doorstep visitors ("Knock Knock !!").
- **Doctor NPC Clinic**: Emergency medical revive for 500 Gold when pet HP hits 0.
- **Upgrade Station**: 3-branch skill tree:
  - *QTE Focus*: Hit zone $+15\%$ (200 Gold).
  - *Stamina Tree*: Max energy $+2$ AP (300 Gold).
  - *Care Booster*: Care stat return $+50\%$ (250 Gold).
- **Survival Desk**: Research station containing Pet Discovery and Disaster Log records.

**Inventory Grid**:
8-slot fixed grid supporting consumable items and equipment accessories.
_Avoid_: Backpack, bag

**Controls & Hotkeys**:
- `WASD` / Mouse: Navigate and interact with stations.
- `Spacebar`: QTE confirm, Dodge reflex, and Counter-attack trigger.
- `E`: End Day / open Front Door.
- `1`, `2`, `3`, `4`: Care action shortcuts (`1` Train, `2` Feed, `3` Clean, `4` Heal).
- `B`: Open Inventory Grid.
- `S`: Open Merchant Shop.

**CleanUI**:
Modern dark borderless design system (`CleanUI.cs`) utilizing a charcoal palette (`#1E1E24`), rounded cards, pill badges, and high-contrast typography.
