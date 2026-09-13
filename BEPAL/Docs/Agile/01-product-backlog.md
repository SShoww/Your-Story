---
type: agile-backlog
version: 3.0
date: 2026-09-13
project: BePal
---

# Product Backlog — BePal (Master Traceability & Story Points)

> รวม User Stories และ Technical Enablers ทั้งหมดของโปรเจกต์ BePal สอดคล้องกับเอกสาร Game Design Document (GDD 00–05), Master Asset List, สถาปัตยกรรม (ADR), และกระดาน Kanban Board ตลอดแผนการพัฒนา **4 Sprints (รวม 128 SP)**
> - **โชว์ (Show):** Lead Programmer
> - **ซุง (Zunk):** Game Designer
> - **ภูมิ (Pooh):** Flex (leans towards Design) / Audio Support
> - **เดียร์ (Dear):** 2D Art & UI Lead

---

## 1. GDD Feature Traceability Matrix (ตารางเชื่อมโยงกับ GDD)

| GDD Reference | ระบบ / ฟีเจอร์หลัก | Backlog Item IDs | Sprint | MoSCoW | ผู้รับผิดชอบหลัก |
| --- | --- | --- | --- | --- | --- |
| **GDD-00 / 01** | Daily Core Loop & Room Flow | US-01, US-06, US-20, TECH-05 | Sprint 1, 3 | **Must** | โชว์ (Lead Prog) |
| **GDD-01 / 03** | 4 Care Actions & 2-Phase Care Loop | US-02, US-03, US-21, US-22 | Sprint 1, 2, 4 | **Must** | ซุง & โชว์ |
| **GDD-01 / 03** | Attack System & Dodge QTE | US-04, US-08 | Sprint 1, 2 | **Must / Should** | โชว์ (Lead Prog) |
| **GDD-02 / 03** | Health, Hazard & Forced Retreat | US-05, US-09, US-23 | Sprint 1, 2, 4 | **Must / Should** | ซุง (Game Designer) |
| **GDD-01 / 03** | Survival Log Discovery System | US-06, ART-03 | Sprint 1, 4 | **Must / Could** | ซุง & เดียร์ |
| **GDD-00 / 02** | 2D Soft Art & 4-Wall Panoramic Room | US-07, US-13, ART-01, ART-02 | Sprint 2, 3 | **Must / Should** | เดียร์ (2D Art Lead) |
| **GDD-02 / 05** | Audio Feedback & Cozy/Tension BGM | US-11, US-12, US-14 | Sprint 2, 3 | **Should** | ภูมิ (Pooh / Audio) |
| **GDD-01 / 02** | Prologue, Doorstep & Narrative Scripts | US-15, US-16, TECH-05 | Sprint 3, 4 | **Must / Should** | ภูมิ (Pooh / Design) |
| **GDD-02 / 04** | Modular Architecture & Tests | TECH-01, TECH-02, TECH-03, TECH-04 | Sprint 2 | **Should** | โชว์ (Lead Prog) |
| **GDD-02 / 03** | Daily Summary Report & Night Rest | US-23, QA-02 | Sprint 4 | **Must / Should** | โชว์, ซุง, ภูมิ |

---

## 2. Product Backlog Items by MoSCoW & Sprint

### 2.1 Must Have (Core MVP — Sprint 1: รวม 29 SP) ✅ Done

| ID | User Story / Requirement | Acceptance Criteria (เกณฑ์การตรวจรับ) | SP | Sprint | ผู้รับผิดชอบหลัก |
| --- | --- | --- | --- | --- | --- |
| **US-01** | **Mouse Navigation & Core UI**<br>*As a player, I want to navigate between Main Menu, Help, Home, and Survival Log using mouse clicks.* | 1. ปุ่ม Start, Help, Quit, End Day, Survival Log ตอบสนองต่อการคลิกซ้าย<br>2. มีหน้าจอพื้นฐานครบ 6 หน้าจอ (Menu, Help, Home, Care, Dodge, Summary) | **4** | 1 | โชว์ (Lead Prog) |
| **US-02** | **Wheel-Based Care QTE Engine**<br>*As a player, I want to play a rotating wheel QTE with 4 actions (Feed, Play, Pet, Observe) and confirm with Spacebar.* | 1. เข็มหมุนที่ความเร็วคงที่ $\omega = 2.2 \text{ rad/s}$<br>2. วงล้อแบ่ง 4 Sectors ชัดเจน ($45^\circ, 135^\circ, 225^\circ, 315^\circ$)<br>3. ตรวจจับการกด Spacebar ในช่องแอ็กชันได้อย่างแม่นยำ | **5** | 1 | โชว์ (Lead Prog) |
| **US-03** | **Pet Behavior & Favor Rules Design**<br>*As a player, I want to learn pet preferences through trial and error, earning Satisfaction for correct choices.* | 1. ซุงกำหนดสเปกและกฎของสัตว์ 3 ชนิดแรก (Mossling, Nibbleclaw, Blinkbun)<br>2. กำหนดแต้ม Pet Favor: $+1$ แต้ม (สะสมครบ 3 แต้มจบ Session)<br>3. สัตว์ Trickster มีลูกเล่นเข็มวาร์ปสุ่มตำแหน่ง (Teleporting Marker) | **6** | 1 | ซุง (Game Designer) |
| **US-04** | **Attack System & Dodge QTE**<br>*As a player, I want to face reactive Dodge QTE when attacked by aggressive pets.* | 1. Nibbleclaw เข้าสู่สถานะ Attack เมื่อ Satisfaction ถึง 2 แต้ม<br>2. วงล้อเปลี่ยนเป็น Dodge Zone สีทองที่มุมด้านบน ($270^\circ$, กว้าง $\pm 30^\circ$)<br>3. กดทันหลบพ้นการโจมตี กดพลาดเสีย 1 Health | **5** | 1 | โชว์ (Lead Prog) |
| **US-05** | **Health, Damage & Forced Retreat Rules**<br>*As a player, I want to manage 3 Health per day and experience Forced Retreat when Health reaches 0.* | 1. เริ่มวันใหม่ด้วย Health 3 แต้มเสมอ<br>2. เมื่อเลือกผิดหรือหลบพลาดจะเสีย 1 Health<br>3. เมื่อ Health เหลือ 0 จะเกิด Forced Retreat จบวันทันทีและฟื้นฟูเลือดในวันใหม่ | **4** | 1 | ซุง (Game Designer) |
| **US-06** | **Survival Log Discovery Rules & Display**<br>*As a player, I want completed care sessions to permanently unlock pet rules in the Survival Log.* | 1. บันทึกจำนวนรอบที่ดูแลสัตว์แต่ละตัวสำเร็จ<br>2. ซุงเขียนข้อความ Action Pattern และคำแนะนำเพื่อปลดล็อกเมื่อครบ 3 Sessions | **5** | 1 | ซุง (Game Designer) |

---

### 2.2 Should Have (Deepening & Audio Polish — Sprint 2: รวม 45 SP) 🔄 Active

| ID | User Story / Requirement | Acceptance Criteria (เกณฑ์การตรวจรับ) | SP | Sprint | ผู้รับผิดชอบหลัก |
| --- | --- | --- | --- | --- | --- |
| **US-07** | **Dynamic Pet Emotional Sprites (Mossling)**<br>*As a player, I want visual pet reactions (Idle, Happy, Angry) during and after QTE attempts.* | 1. เดียร์วาดอารมณ์สัตว์ Happy (ยกนิ้ว) และ Angry (กำหมัด) ของ Mossling<br>2. โชว์เขียนระบบ Reaction Timer (0.7s) สลับกลับสู่ Idle อัตโนมัติ<br>3. ในโหมด Dodge QTE สัตว์มีกรอบสีแดงเตือนภัย | **4** | 2 | เดียร์ (2D Art Lead) |
| **US-08** | **Death Spiral Circular Track & Needle**<br>*As a player, I want an immersive circular QTE track with floating tags and scrap-paper badges.* | 1. โชว์สร้างวงแหวนคู่สไตล์ Death Spiral ด้วย MonoGame.Extended<br>2. วาดเข็มหมุนสีขาวพร้อมหัวทอง และระบบตรวจจับมุม Hit/Miss ใน Sector<br>3. มี Floating Tags (`PERFECT!`, `DODGED!`, `MISSED!`) และ Screen Shake Matrix | **5** | 2 | โชว์ (Lead Prog) |
| **US-09** | **Hazard Level & Harm Type System Design**<br>*As a player, I want clear indication of Hazard Level (1–3) and Harm Type (Physical/Mental).* | 1. ซุงกำหนดสเกล Hazard Level (1–3) และนิยามความเสียหาย Physical vs. Mental<br>2. โชว์เชื่อมต่อค่าสถานะขึ้นแสดงบน HUD ด้านบนขวาและหน้าห้อง | **3** | 2 | ซุง (Game Designer) |
| **US-10** | **Playtest Screenshot & Capture Pipeline**<br>*As a developer/QA, I want an automated test harness to capture high-res PNG screenshots of all screens.* | 1. รองรับคำสั่ง `--screenshot` สำหรับรันและบันทึกภาพทุกหน้าจออัตโนมัติ<br>2. รองรับปุ่มคีย์ลัด `F12` บันทึกภาพลงในโฟลเดอร์ `screenshots/` ขณะเล่นจริง | **3** | 2 | โชว์ (Lead Prog) |
| **US-11** | **Sound Effects Production (8 SFX)**<br>*As a player, I want audio feedback for confirmations, successes, misses, teleport, and alarms.* | 1. ภูมิจัดหาและตัดต่อไฟล์เสียง `.wav` คุณภาพสูง 8 เสียงลงใน Staging ครบตาม GDD-05<br>2. ไฟล์เสียงผ่านการ Normalize ระดับเสียงเรียบร้อย ไม่แตกพร่า | **4** | 2 | ภูมิ (Pooh / Audio) |
| **US-12** | **Audio Engine Integration into MonoGame**<br>*As a player, I want sound effects to trigger reliably on corresponding gameplay actions.* | 1. โชว์เขียนระบบเล่นเสียง SFX ของภูมิเมื่อกด Spacebar ถูก, ผิด, หลบพ้น, และจบ Session<br>2. จัดการหน่วยความจำเสียงผ่าน SoundEffectInstance ไม่เกิด memory leak | **3** | 2 | โชว์ (Lead Prog) |
| **ART-01** | **Death Spiral UI Scrap Badges (6 Badges)**<br>*As a player, I want styled scrap-paper action badges along the circular track.* | 1. เดียร์วาดป้ายกระดาษฉีก: FEED, PLAY, PET, OBSERVE, DODGE ZONE, ATTACK!<br>2. ขนาด 128×52 px และ 160×56 px พร้อมขอบสีประจำหมวดหมู่ | **3** | 2 | เดียร์ (2D Art Lead) |
| **DES-01** | **QTE Balance Matrix & Timing Calculations**<br>*As a designer, I want a mathematical balance sheet for needle speed, sector spans, and dead zones.* | 1. ซุงคำนวณและระบุค่ามุม: Sector กว้าง $60^\circ$, Dead Zone $30^\circ$, Dodge Zone $\pm 30^\circ$<br>2. กำหนดตารางคะแนน Pet Favor ($+2, +1, +0, \text{Attack}$) เพื่อส่งต่อให้โชว์ | **3** | 2 | ซุง (Game Designer) |
| **QA-01** | **Lead QA Playtesting & Feel Feedback**<br>*As a QA tester, I want to verify the mechanical rhythm, timing feel, and difficulty curve of QTEs.* | 1. ภูมิทดสอบเล่นเกมจริง วัดความรู้สึกจังหวะกด Spacebar และความยากง่ายของ Hit Window<br>2. จัดทำบันทึกข้อเสนอแนะส่งให้ซุงและโชว์ปรับจูน | **3** | 2 | ภูมิ (Pooh / Design) |
| **TECH-01** | **Pure C# Domain Models Extraction**<br>*As an architect, I want core gameplay models decoupled from MonoGame render loops.* | 1. โชว์แยกคลาส `PrototypeRun`, `PetDefinition`, `ActionPattern`, `CareAction` ออกจาก `Game1.cs`<br>2. เป็น Pure C# Class ตามแนวทางใน `AGENTS.md` | **3** | 2 | โชว์ (Lead Prog) |
| **TECH-02** | **Automated Unit Testing (`BePal.Tests`)**<br>*As a developer, I want an automated xUnit test suite covering damage, sessions, and day cycles.* | 1. โชว์สร้างโปรเจกต์ `CoPoject/BePal.Tests` และเชื่อมต่อกับ `CoPoject.slnx`<br>2. เขียนเทสต์ครอบคลุม `TakeDamage`, `CompleteSession`, `ForcedRetreat`, `SurvivalLog` | **4** | 2 | โชว์ (Lead Prog) |
| **TECH-03** | **Modular Screen Hierarchy (`IScreen`)**<br>*As a developer, I want screen logic partitioned into independent classes.* | 1. โชว์สร้างอินเทอร์เฟซ `IScreen` และตัวจัดการ `ScreenManager`<br>2. แยก `HomeScreen`, `CareQteScreen`, `DodgeQteScreen`, `SurvivalLogScreen` ออกจาก `Game1.cs` | **5** | 2 | โชว์ (Lead Prog) |
| **TECH-04** | **Content Pipeline Warning Cleanups**<br>*As a developer, I want a clean build without MGCB version mismatch warnings.* | 1. โชว์คลีนอัพการอ้างอิง DLL ใน `Content.mgcb` ให้เข้ากับ `dotnet-mgcb` 3.8.4<br>2. บิลด์ผ่านโดยมี 0 Warnings และ 0 Errors | **2** | 2 | โชว์ (Lead Prog) |

---

### 2.3 Must & Should Have (Atmosphere, Narrative & 4-Wall Shelter — Sprint 3: รวม 28 SP) 🔲 Draft

| ID | User Story / Requirement | Acceptance Criteria (เกณฑ์การตรวจรับ) | SP | Sprint | ผู้รับผิดชอบหลัก |
| --- | --- | --- | --- | --- | --- |
| **US-15** | **Prologue & Daily Doorstep Narrative Scripts**<br>*As a player, I want Day 1 shop prologue and Day 2-5 doorstep arrival stories.* | 1. ภูมิเขียนบทนำ Day 1 (เตรียมร้าน $\rightarrow$ กล่องปริศนา $\rightarrow$ เปิดเจอ Mossling)<br>2. เขียนบทพัสดุมาส่งหน้าประตูตอนเช้า Day 2–5 เป็นภาษาอังกฤษ | **4** | 3 | ภูมิ (Pooh / Design) |
| **TECH-05** | **Unified DialogueBox Subsystem**<br>*As a player, I want smooth typewriter text rendering with fast-reveal and [YES]/[NO] prompts.* | 1. โชว์สร้างคลาส `DialogueBox` รองรับ Typewriter effect, คลิกเพื่อเร่งแสดงผลเต็มทันที, ปุ่ม Next และ Prompt `[YES]/[NO]`<br>2. ใช้งานร่วมกันทั้ง Prologue, Doorstep, Room Inspection และ Pet Confirmation | **4** | 3 | โชว์ (Lead Prog) |
| **ART-02** | **4-Wall Panoramic Shelter Backgrounds & Porch**<br>*As a player, I want 4 connected wall backgrounds for the shelter room.* | 1. เดียร์วาดภาพฉาก 1280×720 px: Wall 1 (Pet Zone), Wall 2 (Pantry), Wall 3 (Desk), Wall 4 (Door)<br>2. วาดฉาก `bg_prologue_intro.png` และ `bg_doorstep_morning.png` | **5** | 3 | เดียร์ (2D Art Lead) |
| **US-20** | **4-Wall Panoramic Shelter Navigation Engine**<br>*As a player, I want to rotate 360 degrees around the shelter room with left/right arrows and inspect items.* | 1. โชว์พัฒนา `PanoramicRoomScreen` พร้อมปุ่มลูกศรหมุนซ้าย-ขวา 4 ทิศ (Wall 1–4 สไตล์ Samsara Room)<br>2. วาง Hitbox คลิกสำรวจสิ่งของ (ชั้นอาหาร, ถังขยะ, ประตู) เปิดข้อความใน DialogueBox<br>3. คลิกสัตว์เลี้ยงที่ Wall 1 ขึ้นข้อความถาม `[YES]/[NO]` เพื่อเริ่มดูแล | **5** | 3 | โชว์ (Lead Prog) |
| **US-13** | **Distinct Pet Species Sprites (Nibbleclaw & Blinkbun)**<br>*As a player, I want Nibbleclaw and Blinkbun to have completely distinct visual designs.* | 1. เดียร์วาด Nibbleclaw ครบ 4 ท่า (Idle, Happy, Angry, Attack)<br>2. เดียร์วาด Blinkbun ครบ 4 ท่า (Idle, Happy, Angry, Teleport) ขนาด 280×360 px | **4** | 3 | เดียร์ (2D Art Lead) |
| **DES-02** | **Behavior Cues & Inspectable Clues Design**<br>*As a designer, I want clear behavior cues and room item clues for player deduction.* | 1. ซุงออกแบบ Ambient & Dynamic Cues (ท้องร้อง, สั่น, ตาโต) เชื่อมโยงกับ 4 Care Actions<br>2. เขียนคำอธิบาย Clues บนชั้นอาหารและของในห้องเพื่อบอกใบ้ผู้เล่น | **3** | 3 | ซุง (Game Designer) |
| **US-14** | **Atmospheric Cozy & Tension BGM**<br>*As a player, I want warm acoustic music in the shelter and suspenseful cues during care.* | 1. ภูมิจัดหาและตั้งค่า Loop เพลง BGM ห้องพัก (`bgm_shelter_cozy.mp3`) และเพลงตอนแคร์<br>2. โชว์เชื่อมต่อระบบเล่นเพลงแบบวนลูปไร้รอยต่อ | **3** | 3 | ภูมิ (Pooh / Audio) |

---

### 2.4 Must Have (2-Phase Care Mini-Games & Release — Sprint 4: รวม 26 SP) 🔲 Draft

| ID | User Story / Requirement | Acceptance Criteria (เกณฑ์การตรวจรับ) | SP | Sprint | ผู้รับผิดชอบหลัก |
| --- | --- | --- | --- | --- | --- |
| **US-21** | **Dynamic Wheel with Sweet Spots & Cue Integration**<br>*As a player, I want Phase 1 care to feature dynamic wheel speeds, behavior cues, and golden sweet spots.* | 1. โชว์เชื่อมต่อ Behavior Cues ของซุงในหน้า Care QTE<br>2. เพิ่ม Golden Sweet Spot ($\pm 15^\circ$) ตรงกลางช่องแอ็กชัน เมื่อกดโดนจะได้รับ +2 Satisfaction<br>3. ปรับจูนเข็มตามความเร่งของ Nibbleclaw และการวาร์ปของ Blinkbun | **6** | 4 | โชว์ (4 SP) & ซุง (2 SP) |
| **US-22** | **Tactile Care Mini-Games Subsystem (4 Actions)**<br>*As a player, I want Phase 2 tactile mini-games for Feed, Pet, Play, and Observe.* | 1. โชว์พัฒนาอินเทอร์เฟซ `ICareMiniGame` และมินิเกม 4 แบบ (Feed: Hold-to-pour, Pet: Mouse stroke, Play: Reflex, Observe: Lens)<br>2. เดียร์วาด Props มินิเกม (ชาม, ขวด, มือ, ของเล่น, เลนส์)<br>3. กฎ Consequence: ผ่านได้ +1/+2 Satisfaction พลาดได้ +0 ไม่เสีย HP | **8** | 4 | โชว์ (5 SP) & เดียร์ (3 SP) |
| **US-23** | **Daily Summary Report Card & Night Rest**<br>*As a player, I want an end-of-day summary report card and night rest transition.* | 1. โชว์พัฒนา `DailySummaryScreen` (สไตล์ Papers, Please) แสดงสถิติและข้อมูล Log ใหม่<br>2. ระบบ Fade to Black Night Rest พักผ่อนและฟื้นฟูเลือดเป็น 3 ก่อนเข้าสู่เช้าวันใหม่ | **5** | 4 | โชว์ (3 SP) & ซุง (2 SP) |
| **US-16** | **Narrative Lore Secret Origin & Ending**<br>*As a player, I want to uncover the secret origin of the abnormal pets and reach a story conclusion.* | 1. ภูมิเขียนบทสรุปเนื้อเรื่องตอนจบ เปิดเผยปมปริศนายาทดลองและศูนย์วิจัยลับ<br>2. หน้าต่าง Run Summary แสดงบทสรุปเนื้อเรื่องเมื่อผ่านครบ 5 วัน | **4** | 4 | ภูมิ (Pooh / Design) |
| **QA-02** | **End-to-End Playtesting & Final Balance QA**<br>*As a QA lead, I want end-to-end playtesting of the full 5-day loop across all 4 walls and mini-games.* | 1. ภูมิทดสอบเล่นลูปเต็ม 5 วัน ตรวจสอบความยากง่ายของมินิเกมและสัตว์ทุกตัว<br>2. ตรวจสอบว่าไม่มี Bug หรือข้อความตกหล่น | **3** | 4 | ภูมิ (Pooh / Design) |

---

## 3. Workload Summary by Member & Sprint (Verified Math Matrix)

ตารางสรุป Story Points ตามบทบาทและ Sprint ที่ผ่านการตรวจสอบทางคณิตศาสตร์ให้ผลรวมตรงกันสมบูรณ์ทุกแกน:

| สมาชิก | บทบาท (Role) | Sprint 1 (Done) | Sprint 2 (Active) | Sprint 3 (Draft) | Sprint 4 (Draft) | รวมทั้งโปรเจกต์ (SP) |
| --- | --- | --- | --- | --- | --- | --- |
| **วศิน (โชว์)** | **Lead Programmer** | 14 SP | 25 SP | 9 SP | 12 SP | **60 SP** |
| **ปีย์ตะวัน (ซุง)** | **Game Designer** | 15 SP | 6 SP | 3 SP | 4 SP | **28 SP** |
| **ภูมิพัฒน์ (ภูมิ / Pooh)** | **Flex (Design & Audio)** | 0 SP | 7 SP | 7 SP | 7 SP | **21 SP** |
| **ธัญญรัตน์ (เดียร์)** | **2D Art & UI Lead** | 0 SP | 7 SP | 9 SP | 3 SP | **19 SP** |
| **รวม Story Points ต่อ Sprint** | - | **29 SP** | **45 SP** | **28 SP** | **26 SP** | **128 SP** |

---

## 4. Links
- [[BEPAL/Docs/Agile/02-sprint-backlog|Sprint Backlog]]
- [[BEPAL/Docs/Agile/03-kanban-board|Kanban Board]]
- [[BEPAL/Docs/Agile/04-Kanban-for-Obsidian|Obsidian Interactive Kanban]]
- [[BEPAL/Docs/GDD/00-concept|GDD Concept]]
- [[BEPAL/Docs/GDD/01-core-loop|GDD Core Loop]]
- [[BEPAL/Docs/GDD/02-scope-features|GDD Scope & Features]]
- [[BEPAL/Docs/GDD/03-mechanics|GDD Mechanics]]
- [[BEPAL/Docs/GDD/04-class-diagram|GDD Architecture]]
- [[BEPAL/Docs/GDD/05-asset-list|GDD Asset List]]
