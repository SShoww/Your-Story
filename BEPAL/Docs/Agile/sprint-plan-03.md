# Sprint 3 Plan — Shelter Atmosphere, 2-Phase Care Mini-Games & Final Release

**Sprint Goal:** พัฒนาระบบบรรยากาศและสิ่งแวดล้อมห้อง 4 ทิศให้สมบูรณ์, เพิ่มมินิเกมสัมผัส 4 รูปแบบ (Feed, Pet, Play, Observe), ระบบเข็ม Sweet Spot, ระบบสรุปวัน Daily Summary Report Card และทดสอบ End-to-End QA เพื่อส่งมอบเกมสมบูรณ์ก่อนวันที่ 12 ตุลาคม 2026  
**ระยะเวลา:** 2026-09-29 — 2026-10-11 (2 สัปดาห์, เสร็จสิ้นก่อนกำหนดส่ง 12 ต.ค. 2026)  
**Status:** 🔲 Planned / Active Integration (13/54 SP Done ล่วงหน้าผ่าน PR #16)  
**Team:**
- วศิน ศรีวรกุล (โชว์) — Lead Programmer
- ปีย์ตะวัน แห่งหาญ (ซุง) — Game Designer
- ภูมิพัฒน์ ตามวงค์ (ภูมิ) — Flex (leans towards Design) / Audio Support
- ธัญญรัตน์ ติ๊บหน่อ (เดียร์) — 2D Art & UI Lead

---

## Sprint Backlog

| # | User Story / Task ID | User Story | ผู้รับผิดชอบ | MoSCoW | Estimate (SP) | Status |
|---|---|---|---|---|---|---|
| 1 | **US-15** | Prologue & Daily Doorstep Narrative Scripts | ภูมิ (Pooh / Design) | Must Have | 4 | ✅ Done (PR #16) |
| 2 | **TECH-05** | Unified DialogueBox Subsystem | โชว์ (Lead Prog) | Must Have | 4 | ✅ Done (PR #16) |
| 3 | **ART-02** | 4-Wall Panoramic Shelter Backgrounds & Porch | เดียร์ (2D Art Lead) | Must Have | 5 | 🔄 In Progress |
| 4 | **US-20** | 4-Wall Panoramic Shelter Navigation Engine | โชว์ (Lead Prog) | Must Have | 5 | ✅ Done (PR #16) |
| 5 | **US-13** | Distinct Pet Species Sprites (Nibbleclaw & Blinkbun) | เดียร์ (2D Art Lead) | Should Have | 4 | 🔲 Todo |
| 6 | **DES-02** | Behavior Cues & Inspectable Clues Design | ซุง (Game Designer) | Should Have | 3 | 🔄 In Progress |
| 7 | **US-14** | Atmospheric Cozy & Tension BGM | ภูมิ (Pooh / Audio) | Should Have | 3 | 🔲 Todo |
| 8 | **US-21** | Dynamic Wheel with Sweet Spots & Cue Integration | โชว์ & ซุง | Must Have | 6 | 🔲 Todo |
| 9 | **US-22** | Tactile Care Mini-Games Subsystem (4 Actions) | โชว์ & เดียร์ | Must Have | 8 | 🔲 Todo |
| 10 | **US-23** | Daily Summary Report Card & Night Rest | โชว์ & ซุง | Must Have | 5 | 🔲 Todo |
| 11 | **US-16** | Narrative Lore Secret Origin & Ending | ภูมิ (Pooh / Design) | Should Have | 4 | 🔲 Todo |
| 12 | **QA-02** | End-to-End Playtesting & Final Balance QA | ภูมิ (Pooh / Design) | Should Have | 3 | 🔲 Todo |

## Status Legend
- 🔲 Todo
- 🔄 In Progress
- 🔍 Review
- ✅ Done
- ❌ Blocked

## Workload Summary (54 SP Total)
- **วศิน (โชว์):** 21 SP (TECH-05: 4, US-20: 5, US-21: 4, US-22: 5, US-23: 3) — *9 SP Done, 12 SP Remaining*
- **ปีย์ตะวัน (ซุง):** 7 SP (DES-02: 3, US-21: 2, US-23: 2) — *In Progress / Ready*
- **ภูมิพัฒน์ (ภูมิ):** 14 SP (US-15: 4, US-14: 3, US-16: 4, QA-02: 3) — *4 SP Done, 10 SP Remaining*
- **ธัญญรัตน์ (เดียร์):** 12 SP (ART-02: 5, US-13: 4, US-22: 3) — *In Progress / Ready*

---

## Detailed Tasks

### US-15 — Prologue & Daily Doorstep Narrative Scripts [4 SP] ✅ Done
- [x] เขียนบทนำ Day 1 แนะนำสถานรับเลี้ยง พัสดุปริศนา และการแกะกล่อง Mossling (ภาษาอังกฤษ) [owner:: ภูมิ] [estimate:: 2] [status:: done]
- [x] เขียนบทสนทนาพัสดุมาส่งหน้าประตูตอนเช้า Day 2–5 และ Manifest ข้อมูล [owner:: ภูมิ] [estimate:: 2] [status:: done]

### TECH-05 — Unified DialogueBox Subsystem [4 SP] ✅ Done
- [x] พัฒนาคลาส `DialogueBox` รองรับ Typewriter reveal, คลิกเพื่อข้าม, dynamic line wrap [owner:: โชว์] [estimate:: 2] [status:: done]
- [x] รองรับปุ่ม `NEXT =>`, `SKIP >>` และ Prompt `[YES]/[NO]` พร้อมคีย์ลัด Y/N [owner:: โชว์] [estimate:: 1] [status:: done]
- [x] เขียน unit tests ทดสอบ DialogueBox ทั้งหมด 8 กรณีทดสอบ [owner:: โชว์] [estimate:: 1] [status:: done]

### US-20 — 4-Wall Panoramic Shelter Navigation Engine [5 SP] ✅ Done
- [x] พัฒนา `PanoramicRoomScreen` รองรับการหมุนรอบห้อง 360 องศา 4 ทิศ (Wall 1–4) ด้วยลูกศรและปุ่ม A/D [owner:: โชว์] [estimate:: 2] [status:: done]
- [x] วาง Hitbox สำรวจสิ่งของประจำผนัง (ชั้นอาหาร, ถังขยะ, โต๊ะเอกสาร, ประตู) เชื่อมต่อข้อความ DialogueBox [owner:: โชว์] [estimate:: 2] [status:: done]
- [x] ระบบคลิกสัตว์เลี้ยงที่ Wall 1 แสดงข้อความยืนยันก่อนตัดเข้าสู่โหมดแคร์ [owner:: โชว์] [estimate:: 1] [status:: done]

### ART-02 — 4-Wall Panoramic Shelter Backgrounds & Porch [5 SP]
- [ ] วาดภาพฉากพื้นหลังขนาด 1280×720 px ครบ 4 ผนัง: Wall 1 (Pet), Wall 2 (Pantry), Wall 3 (Desk), Wall 4 (Front Door) [owner:: เดียร์] [estimate:: 3] [status:: in-progress]
- [ ] วาดฉากหน้าบ้าน `bg_doorstep_morning.png` และฉากบทนำ `bg_prologue_intro.png` [owner:: เดียร์] [estimate:: 2] [status:: in-progress]

### US-13 — Distinct Pet Species Sprites (Nibbleclaw & Blinkbun) [4 SP]
- [ ] วาด Nibbleclaw ครบ 4 ท่า: Idle (แมวนอนเลียกรงเล็บ), Happy (ตาเป็นประกาย), Angry (กรงเล็บกางออก), Attack (พุ่งจู่โจม) [owner:: เดียร์] [estimate:: 2] [status:: todo]
- [ ] วาด Blinkbun ครบ 4 ท่า: Idle (กระต่ายหูยาว), Happy (หูตั้งกระดิก), Angry (ตาเรืองแสงม่วง), Teleport (เงาวาร์ป) [owner:: เดียร์] [estimate:: 2] [status:: todo]

### DES-02 — Behavior Cues & Inspectable Clues Design [3 SP]
- [ ] ออกแบบ Ambient & Dynamic Cues ของสัตว์ 3 สายพันธุ์ เชื่อมโยงกับ 4 Care Actions [owner:: ซุง] [estimate:: 2] [status:: in-progress]
- [ ] ออกแบบข้อความเบาะแสบนชั้นอาหาร Pantry Wall 2 และกระดาน Notice Board Wall 3 [owner:: ซุง] [estimate:: 1] [status:: in-progress]

### US-14 — Atmospheric Cozy & Tension BGM [3 SP]
- [ ] จัดหาและตั้งค่า Loop เสียงเพลง Cozy BGM ประจำห้อง Shelter (`bgm_shelter_cozy.mp3`) [owner:: ภูมิ] [estimate:: 2] [status:: todo]
- [ ] จัดหาเพลง Tension BGM สำหรับช่วงดูแลและ Dodge QTE [owner:: ภูมิ] [estimate:: 1] [status:: todo]

### US-21 — Dynamic Wheel with Sweet Spots & Cue Integration [6 SP]
- [ ] ออกแบบและคำนวณตำแหน่ง Golden Sweet Spot ($\pm 15^\circ$) ในแต่ละ Sector (+2 Satisfaction) [owner:: ซุง] [estimate:: 2] [status:: todo]
- [ ] พัฒนาระบบแสดงผล Sweet Spot บนวงล้อ Death Spiral และตรวจจับคะแนน [owner:: โชว์] [estimate:: 2] [status:: todo]
- [ ] เชื่อมต่อ Behavior Cues ของสัตว์ขึ้นบนแบนเนอร์หรือรอบวงล้อขณะหมุน [owner:: โชว์] [estimate:: 2] [status:: todo]

### US-22 — Tactile Care Mini-Games Subsystem (4 Actions) [8 SP]
- [ ] สร้างโครงสร้างอินเทอร์เฟซ `ICareMiniGame` และตัวจัดการสลับมินิเกม [owner:: โชว์] [estimate:: 2] [status:: todo]
- [ ] พัฒนามินิเกม 4 แบบ: Feed (Hold-to-pour), Pet (Mouse stroke), Play (Reflex catch), Observe (Focus lens) [owner:: โชว์] [estimate:: 3] [status:: todo]
- [ ] วาดภาพ Mini-Game Props: ชามข้าว, ขวดนม, มือลูบ, ลูกบอลของเล่น, แว่นขยาย [owner:: เดียร์] [estimate:: 3] [status:: todo]

### US-23 — Daily Summary Report Card & Night Rest [5 SP]
- [ ] พัฒนา `DailySummaryScreen` แสดงสถิติประจำวัน (Satisfaction, Health, Forced Retreat, Log Unlock) สไตล์ Papers, Please [owner:: โชว์] [estimate:: 2] [status:: todo]
- [ ] ออกแบบแบบฟอร์มเอกสาร Daily Report และข้อความประเมินผลประจำวัน [owner:: ซุง] [estimate:: 2] [status:: todo]
- [ ] ระบบ Fade to Black Night Rest ฟื้นฟูเลือด 3 HP ก่อนเริ่มวันถัดไป [owner:: โชว์] [estimate:: 1] [status:: todo]

### US-16 — Narrative Lore Secret Origin & Ending [4 SP]
- [ ] เขียนบทสรุปเนื้อเรื่องตอนจบของศูนย์วิจัย เปิดเผยความจริงของสัตว์ประหลาดและยาทดลอง [owner:: ภูมิ] [estimate:: 2] [status:: todo]
- [ ] เชื่อมต่อบทสรุปเนื้อเรื่องใน Run Summary หลังผ่านวันที่ 5 [owner:: ภูมิ & โชว์] [estimate:: 2] [status:: todo]

### QA-02 — End-to-End Playtesting & Final Balance QA [3 SP]
- [ ] ทดสอบเล่นเกมเต็มรูปแบบ 5 วัน ตรวจสอบความลื่นไหล บั๊ก และการแสดงผล [owner:: ภูมิ] [estimate:: 2] [status:: todo]
- [ ] ปรับจูน Balance ครั้งสุดท้ายร่วมกับซุง และทดสอบ Regression Test ครบถ้วนก่อนปล่อยเกม [owner:: ภูมิ & ซุง] [estimate:: 1] [status:: todo]

---

## Daily Notes & Standup Log

### 2026-09-29 (Sprint 3 Kickoff & Consolidation)
- **แผนงาน:** รวมงานส่วนที่เหลือเข้าสู่ Sprint 3 เพื่อเตรียมส่งมอบเกมสมบูรณ์ก่อนวันที่ 12 ตุลาคม 2026
- **สถานะเบื้องต้น:** ฟังก์ชันแกนหลัก 13 SP (DialogueBox, PanoramicRoomScreen, PrologueScreen, DoorstepScreen) ผ่านการผสานใน PR #16 เรียบร้อยแล้ว
- **เป้าหมายสัปดาห์นี้:** นำเข้า Art พื้นหลังห้อง 4 ทิศ และเริ่มพัฒนาระบบ Sweet Spot กับ Tactile Mini-Games

---

## Links
- [[BEPAL/Docs/Agile/01-product-backlog|Product Backlog]]
- [[BEPAL/Docs/Agile/02-sprint-backlog|Sprint Backlog]]
- [[BEPAL/Docs/Agile/03-kanban-board|Kanban Board]]
- [[BEPAL/Docs/Agile/04-Kanban-for-Obsidian|Obsidian Interactive Kanban]]
- [[BEPAL/Docs/Agile/sprint-plan-01|Sprint 1 Plan]]
- [[BEPAL/Docs/Agile/sprint-plan-02|Sprint 2 Plan]]
- [[BEPAL/Docs/GDD/00-concept|GDD Concept]]
- [[BEPAL/Docs/GDD/01-core-loop|GDD Core Loop]]
- [[BEPAL/Docs/GDD/02-scope-features|GDD Scope & Features]]
- [[BEPAL/Docs/GDD/03-mechanics|GDD Mechanics]]
- [[BEPAL/Docs/GDD/04-class-diagram|GDD Architecture]]
- [[BEPAL/Docs/GDD/05-asset-list|GDD Asset List]]
