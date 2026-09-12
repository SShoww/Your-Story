# Sprint 2 Plan — Deepening, Death Spiral QTE & Systems Polish

**Sprint Goal:** พัฒนาและขัดเกลาระบบเกมเพลย์เชิงลึก: วงล้อ Care QTE สไตล์ Death Spiral พร้อมป้ายกระดาษฉีก, ระบบเสียง SFX ประกอบการเล่น, แยกสถาปัตยกรรม Pure C# Domain Models และติดตั้งระบบทดสอบอัตโนมัติ Unit Test (`BePal.Tests`)
**ระยะเวลา:** 2026-09-15 — 2026-09-28
**Team:**
- วศิน ศรีวรกุล (โชว์) — Lead Programmer
- ปีย์ตะวัน แห่งหาญ (ซุง) — Game Designer
- ภูมิพัฒน์ ตามวงค์ (ภูมิ) — Flex (leans towards Design) / Audio Support
- ธัญญรัตน์ ติ๊บหน่อ (เดียร์) — 2D Art & UI Lead

---

## Sprint Backlog

| # | User Story / Task ID | User Story | ผู้รับผิดชอบ | MoSCoW | Estimate (SP) | Status |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | **US-07** | As a player, I want visual pet reactions (Idle, Happy, Angry) during and after QTE attempts | เดียร์ (2D Art Lead) | Should Have | 4 | ✅ Done |
| 2 | **US-08** | As a player, I want an immersive circular QTE track with floating tags and scrap-paper badges | โชว์ (Lead Prog) | Should Have | 5 | 🔄 In Progress |
| 3 | **US-09** | As a player, I want clear indication of Hazard Level (1–3) and Harm Type (Physical/Mental) | ซุง (Game Designer) | Should Have | 3 | 🔄 In Progress |
| 4 | **US-10** | As a developer/QA, I want an automated test harness to capture high-res PNG screenshots of all screens | โชว์ (Lead Prog) | Should Have | 3 | 🔄 In Progress |
| 5 | **US-11** | As a player, I want audio feedback for confirmations, successes, misses, teleport, and alarms | ภูมิ (Pooh / Audio) | Should Have | 4 | 🔄 In Progress |
| 6 | **US-12** | As a player, I want sound effects to trigger reliably on corresponding gameplay actions | โชว์ (Lead Prog) | Should Have | 3 | 🔲 Todo |
| 7 | **ART-01** | As a player, I want styled scrap-paper action badges along the circular track | เดียร์ (2D Art Lead) | Should Have | 3 | 🔄 In Progress |
| 8 | **DES-01** | As a designer, I want a mathematical balance sheet for needle speed, sector spans, and dead zones | ซุง (Game Designer) | Should Have | 3 | 🔄 In Progress |
| 9 | **QA-01** | As a QA tester, I want to verify the mechanical rhythm, timing feel, and difficulty curve of QTEs | ภูมิ (Pooh / Design) | Should Have | 3 | 🔄 In Progress |
| 10 | **TECH-01** | As an architect, I want core gameplay models decoupled from MonoGame render loops | โชว์ (Lead Prog) | Should Have | 3 | 🔄 In Progress |
| 11 | **TECH-02** | As a developer, I want an automated xUnit test suite covering damage, sessions, and day cycles | โชว์ (Lead Prog) | Should Have | 4 | 🔲 Todo |
| 12 | **TECH-03** | As a developer, I want screen logic partitioned into independent classes | โชว์ (Lead Prog) | Should Have | 5 | 🔲 Todo |
| 13 | **TECH-04** | As a developer, I want a clean build without MGCB version mismatch warnings | โชว์ (Lead Prog) | Should Have | 2 | 🔲 Todo |

## Status Legend
- 🔲 Todo
- 🔄 In Progress
- 🔍 Review
- ✅ Done
- ❌ Blocked

## Workload Summary (45 SP Total)
- **วศิน (โชว์):** 25 SP (US-08: 5, US-10: 3, US-12: 3, TECH-01: 3, TECH-02: 4, TECH-03: 5, TECH-04: 2)
- **ปีย์ตะวัน (ซุง):** 6 SP (US-09: 3, DES-01: 3)
- **ภูมิพัฒน์ (ภูมิ):** 7 SP (US-11: 4, QA-01: 3)
- **ธัญญรัตน์ (เดียร์):** 7 SP (US-07: 4, ART-01: 3)

---

## Detailed Tasks

### US-07 — Dynamic Pet Emotional Sprites (Mossling) [4 SP]
- [x] วาด Sprite อารมณ์ของ Mossling ครบ 4 ท่า (Idle, Happy, Angry, Attack) ขนาด 280×360 px [owner:: เดียร์] [estimate:: 2] [status:: done]
- [x] เขียน Reaction Timer (0.7s) ในโค้ดเกมเพลย์เพื่อสลับภาพอารมณ์ Happy/Angry กลับสู่ Idle อัตโนมัติ [owner:: โชว์] [estimate:: 1] [status:: done]
- [x] เชื่อมต่อกรอบสีแดงเตือนภัย (Danger Vignette / Border) เมื่อเข้าสู่โหมด Dodge QTE [owner:: โชว์] [estimate:: 1] [status:: done]

### US-08 — Death Spiral Circular Track & Needle [5 SP]
- [x] พัฒนาวงแหวนคู่เรขาคณิต (Outer & Inner Track) ด้วย MonoGame.Extended [owner:: โชว์] [estimate:: 2] [status:: done]
- [x] ปรับจูนระบบเข็มหมุนสีขาวพร้อมหัวทอง และการคำนวณตำแหน่งองศา Hit/Miss ในรัศมี 4 Sectors [owner:: โชว์] [estimate:: 1] [status:: done]
- [ ] เพิ่มเอฟเฟกต์ Floating Tags (`PERFECT!`, `DODGED!`, `MISSED!`) และระบบ Screen Shake Translation Matrix [owner:: โชว์] [estimate:: 2] [status:: in-progress]

### US-09 — Hazard Level & Harm Type System Design [3 SP]
- [x] กำหนดสเปกสเกล Hazard Level (1–3) และนิยามผลกระทบ Physical vs. Mental ใน GDD-02 และ GDD-03 [owner:: ซุง] [estimate:: 2] [status:: done]
- [ ] เชื่อมต่อค่าสถานะ Hazard Badge และประเภทความเสียหายขึ้นบน HUD ด้านบนขวาและหน้าห้อง [owner:: โชว์] [estimate:: 1] [status:: in-progress]

### US-10 — Playtest Screenshot & Capture Pipeline [3 SP]
- [x] พัฒนาระบบ Automated Playtest Harness รองรับคำสั่ง `--screenshot` บันทึกภาพครบ 18 เฟรมลง `screenshots/` [owner:: โชว์] [estimate:: 2] [status:: done]
- [ ] เพิ่มคีย์ลัด `F12` สำหรับบันทึกภาพหน้าจอแบบเรียลไทม์ขณะเล่นเกม [owner:: โชว์] [estimate:: 1] [status:: in-progress]

### US-11 — Sound Effects Production (8 SFX) [4 SP]
- [ ] จัดหาและคัดเลือกไฟล์เสียง SFX 8 เสียง (Confirm, Success, Fail, Teleport, Warning, Evade, Chime, Box) ตาม GDD-05 [owner:: ภูมิ] [estimate:: 2] [status:: in-progress]
- [ ] ตัดต่อ, ปรับแต่งความถี่ และ Normalize ระดับเสียง `.wav` คุณภาพสูงลง Staging Folder [owner:: ภูมิ] [estimate:: 2] [status:: in-progress]

### US-12 — Audio Engine Integration into MonoGame [3 SP]
- [ ] สร้างระบบจัดการเสียง `AudioManager` รองรับการเล่นเสียง SFX ผ่าน `SoundEffectInstance` [owner:: โชว์] [estimate:: 2] [status:: todo]
- [ ] เชื่อมต่อการเล่นเสียง SFX ให้สัมพันธ์กับการกด Spacebar ยืนยัน, ทายถูก, โดนดาเมจ, และหลบพ้น [owner:: โชว์] [estimate:: 1] [status:: todo]

### ART-01 — Death Spiral UI Scrap Badges (6 Badges) [3 SP]
- [ ] วาดป้ายกระดาษฉีก 4 แอ็กชันหลัก: FEED (เขียว), PLAY (ฟ้า), PET (ชมพู), OBSERVE (ม่วง) ขนาด 128×52 px [owner:: เดียร์] [estimate:: 2] [status:: in-progress]
- [ ] วาดป้ายพิเศษ: DODGE ZONE (สีทอง 156×52 px) และ ATTACK! (สีแดงเลือด 160×56 px) [owner:: เดียร์] [estimate:: 1] [status:: in-progress]

### DES-01 — QTE Balance Matrix & Timing Calculations [3 SP]
- [ ] คำนวณขนาดมุมวงล้อ: ความเร็ว $\omega = 2.2 \text{ rad/s}$, Sector กว้าง $60^\circ$, Dead Zone $30^\circ$, Dodge Zone $\pm 30^\circ$ [owner:: ซุง] [estimate:: 2] [status:: in-progress]
- [ ] จัดทำตารางคะแนน Pet Favor ($+2, +1, +0, \text{Attack}$) ส่งต่อให้ทีมโปรแกรมเมอร์ปรับจูน [owner:: ซุง] [estimate:: 1] [status:: in-progress]

### QA-01 — Lead QA Playtesting & Feel Feedback [3 SP]
- [ ] ดำเนินการทดสอบ Playtest วงล้อ Care QTE และ Dodge QTE เพื่อวัดจังหวะและความรู้สึก (Hit Window Rhythm) [owner:: ภูมิ] [estimate:: 2] [status:: in-progress]
- [ ] รวบรวมข้อเสนอแนะและจัดทำรายงาน QA Feedback ส่งต่อให้ซุงและโชว์ปรับแต่งความยากง่าย [owner:: ภูมิ] [estimate:: 1] [status:: in-progress]

### TECH-01 — Pure C# Domain Models Extraction [3 SP]
- [ ] แยกโมเดลข้อมูลเกมเพลย์ `CareAction`, `PetDefinition`, `ActionPattern` ออกจาก `Game1.cs` มาไว้ที่ `Gameplay/` [owner:: โชว์] [estimate:: 2] [status:: in-progress]
- [ ] ปรับปรุง `PrototypeRun` ให้เป็น Pure C# State Machine ตามแนวทางใน `AGENTS.md` [owner:: โชว์] [estimate:: 1] [status:: in-progress]

### TECH-02 — Automated Unit Testing (`BePal.Tests`) [4 SP]
- [ ] สร้างโปรเจกต์ `CoPoject/BePal.Tests` (xUnit, .NET 8) และผูกเข้ากับ `CoPoject.slnx` [owner:: โชว์] [estimate:: 1] [status:: todo]
- [ ] เขียน Unit Tests ครอบคลุมการคำนวณ Health, การเกิด Forced Retreat, การบันทึก Session และการปลดล็อก Survival Log [owner:: โชว์] [estimate:: 3] [status:: todo]

### TECH-03 — Modular Screen Hierarchy (`IScreen`) [5 SP]
- [ ] ออกแบบอินเทอร์เฟซ `IScreen` และตัวจัดการ `ScreenManager` ตามสถาปัตยกรรม ADR-0001 [owner:: โชว์] [estimate:: 2] [status:: todo]
- [ ] แตกแยกหน้าจอ `MainMenuScreen`, `HomeScreen`, `CareQteScreen`, `DodgeQteScreen`, `SurvivalLogScreen`, `SummaryScreen` ออกจาก `Game1.cs` [owner:: โชว์] [estimate:: 3] [status:: todo]

### TECH-04 — Content Pipeline Warning Cleanups [2 SP]
- [ ] ตรวจสอบและอัปเดตพาธการอ้างอิง DLL ของ MonoGame.Extended ใน `Content.mgcb` ให้ตรงกับ `dotnet-mgcb` 3.8.4 [owner:: โชว์] [estimate:: 1] [status:: todo]
- [ ] รัน Content Pipeline Build เพื่อยืนยันว่าบิลด์ผ่านด้วย 0 Warnings และ 0 Errors [owner:: โชว์] [estimate:: 1] [status:: todo]

---

## Daily Notes & Standup Log

### 2026-09-15 (Day 1 — Sprint Planning & Kickoff)
- **เมื่อวาน:** สรุปผลการส่งมอบ Sprint 1 MVP สำเร็จครบ 29 SP, ตรวจสอบภาพจับหน้าจอ Playtest ผ่านฉลุย
- **วันนี้:** ประชุมวางแผน Sprint 2 มุ่งเน้นการขัดเกลาเชิงลึก (Deepening), เริ่มงานวาดภาพป้ายกระดาษฉีก ART-01 และร่างโครงสร้าง Pure C# Domain Models TECH-01
- **Blocked:** ไม่มี

### 2026-09-18 (Day 4 — Mid-Sprint Sync)
- **เมื่อวาน:** เดียร์ส่งมอบ Sprite อารมณ์ Mossling (US-07 เสร็จสมบูรณ์), โชว์เรนเดอร์วงล้อคู่ Death Spiral และระบบจับภาพหน้าจออัตโนมัติสำเร็จ อยู่ระหว่าง Review
- **วันนี้:** ภูมิเริ่มบันทึกและตัดต่อไฟล์เสียง SFX 8 เสียงลง Staging, ซุงสรุปค่าตัวเลข Balance Sheet ส่งให้ทีมโปรแกรมเมอร์
- **Blocked:** รอไฟล์เสียง SFX ตัวจริงจากภูมิเพื่อเริ่มต่อเข้า AudioManager (US-12)

### 2026-09-22 (Day 8 — Architecture & Polish Check)
- **เมื่อวาน:** ซุงส่งมอบตาราง Balance Matrix และ Hazard Level สเปกครบถ้วน, โชว์เริ่มแยก Pure Domain Models ออกจาก Game1
- **วันนี้:** โชว์เตรียมสร้างโปรเจกต์ `BePal.Tests` (TECH-02) เพื่อเขียน Unit Tests สำหรับ Domain Logic, ภูมิดำเนินการทดสอบ Playtest และรวบรวมฟีดแบ็กความรู้สึกการกด QTE
- **Blocked:** ไม่มี

---

## Links
- [[BEPAL/Docs/GDD/00-concept|GDD Concept]]
- [[BEPAL/Docs/GDD/01-core-loop|GDD Core Loop]]
- [[BEPAL/Docs/GDD/02-scope-features|GDD Scope & Features]]
- [[BEPAL/Docs/GDD/03-mechanics|GDD Mechanics]]
- [[BEPAL/Docs/GDD/04-class-diagram|GDD Architecture]]
- [[BEPAL/Docs/GDD/05-asset-list|GDD Asset List]]
- [[BEPAL/Docs/Agile/01-product-backlog|Product Backlog]]
- [[BEPAL/Docs/Agile/02-sprint-backlog|Sprint Backlog]]
- [[BEPAL/Docs/Agile/03-kanban-board|Kanban Board]]
- [[BEPAL/Docs/Agile/04-Kanban-for-Obsidian|Obsidian Interactive Kanban]]
- [[BEPAL/Docs/Agile/sprint-plan-01|Sprint 1 Plan]]
