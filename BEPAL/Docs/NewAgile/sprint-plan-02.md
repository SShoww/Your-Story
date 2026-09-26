# Sprint 2 Plan — Toothless Taming, Combat Arena & Systems Polish (v2.0)

**Sprint Goal:** พัฒนาระบบการเผชิญหน้าและสยบสัตว์ป่า Toothless: เหตุการณ์เสียงเคาะประตู "Knock Knock !!", สนามต่อสู้ Combat Arena, ระบบหลบกรด Acid Dodge & Counter-Attack, ระบบเสียง SFX 9 เสียง และการทดสอบระบบอัตโนมัติ  
**ระยะเวลา:** 2026-09-15 — 2026-09-28  
**Team:**
- วศิน ศรีวรกุล (โชว์) — Lead Programmer (25 SP)
- ปีย์ตะวัน แห่งหาญ (ซุง) — Flex (6 SP)
- ภูมิพัฒน์ ตามวงค์ (ภูมิ) — Game Designer & UI Lead (7 SP)
- ธัญญรัตน์ ติ๊บหน่อ (เดียร์) — 2D Art (7 SP)
**Total Velocity:** 45 SP (Active 🔄 — 25/45 SP Done)

---

## Sprint Backlog

| # | User Story / Task | ผู้รับผิดชอบ | MoSCoW | Estimate (SP) | Status |
| --- | --- | --- | --- | --- | --- |
| 1 | Dynamic Pet Emotional Sprites (Coco 4 ท่า) | เดียร์ (2D Art) | Should Have | 4 | 🔄 In Progress |
| 2 | Day 2 "Knock Knock !!" Narrative Script | ภูมิ (Game Designer & UI Lead) | Must Have | 4 | 🔄 In Progress |
| 3 | Toothless Combat Taming Arena & QTE | โชว์ (Lead Prog) | Must Have | 5 | ✅ Done |
| 4 | Acid Spit Dodge Zone & Counter-Attack | โชว์ (Lead Prog) | Must Have | 5 | ✅ Done |
| 5 | Core Sound Effects Production (9 SFX) | ซุง (Flex) | Should Have | 3 | 🔲 Todo |
| 6 | MonoGame Audio Engine Integration | โชว์ (Lead Prog) | Should Have | 3 | ✅ Done |
| 7 | Toothless Sprite Sheet (4 ท่า) | เดียร์ (2D Art) | Must Have | 3 | 🔲 Todo |
| 8 | Combat & Taming Balance Matrix | ภูมิ (Game Designer & UI Lead) | Should Have | 3 | 🔄 In Progress |
| 9 | Lead QA Playtesting & QTE Calibration | ซุง (Flex) | Should Have | 3 | 🔲 Todo |
| 10 | Pure C# Domain Models Extraction | โชว์ (Lead Prog) | Should Have | 3 | ✅ Done |
| 11 | Automated Unit Testing (`BePal.Tests`) | โชว์ (Lead Prog) | Should Have | 4 | ✅ Done |
| 12 | Screen Hierarchy & State Transitions | โชว์ (Lead Prog) | Should Have | 3 | ✅ Done |
| 13 | Content Pipeline & Screenshot Harness | โชว์ (Lead Prog) | Should Have | 2 | ✅ Done |

## Status Legend
- 🔲 Todo
- 🔄 In Progress
- 🔍 In Review
- ✅ Done
- ❌ Blocked

---

## Detailed Tasks

### Story 1, 7 & 2 — Sprites & Narrative Script
- [ ] วาด Coco ครบ 4 อารมณ์ (Idle, Happy, Angry, Hurt) ขนาด 280×360 px [เดียร์] [4] [status:: in_progress]
- [ ] วาด Toothless ครบ 4 ท่า (Idle, Angry, Attack, Tamed) [เดียร์] [3] [status:: todo]
- [ ] เขียนบทสนทนาเสียงกระแทกประตู รอยกรดม่วง และทางเลือก Chase vs Tame [ภูมิ] [4] [status:: in_progress]

### Story 3, 4 & 8 — Combat Arena & Taming Mechanics
- [x] พัฒนา `CombatArenaScreen` แสดงหลอด Tame Gauge 0–100% [โชว์] [5] [status:: done]
- [x] พัฒนาระบบหลบกรด Acid Spit ใน Dodge Zone สีทอง และสวนกลับ [โชว์] [5] [status:: done]
- [ ] คำนวณความเร็วเข็ม 3.0 rad/s ดาเมจกรด 15 HP และอัตราเพิ่ม Tame Gauge [ภูมิ] [3] [status:: in_progress]

### Story 5, 6 & 9 — Audio Staging & QA Playtesting
- [ ] จัดหา ตัดต่อ และ Normalize ไฟล์เสียง SFX 9 ไฟล์ (.wav) [ซุง] [3] [status:: todo]
- [x] พัฒนาระบบเล่นเสียง SFX ผ่าน SoundEffectInstance ใน MonoGame [โชว์] [3] [status:: done]
- [ ] ทดสอบเล่นฉากสยบ Toothless และให้ข้อเสนอแนะปรับแต่งจังหวะ QTE [ซุง] [3] [status:: todo]

### Technical Tasks — Architecture, Tests & Tooling
- [x] สกัดโมเดลโดเมนบริสุทธิ์ `PetEntity`, `PetStats`, `EnergyAccount` ออกจาก Engine [โชว์] [3] [status:: done]
- [x] สร้างชุดทดสอบ xUnit ครอบคลุมการคำนวณและสเตตัสใน `CoPoject/BePal.Tests` [โชว์] [4] [status:: done]
- [x] สร้างระบบจัดการหน้าจอ `IScreen` และ `ScreenManager` [โชว์] [3] [status:: done]
- [x] สร้างระบบทดสอบภาพอัตโนมัติ `--screenshot` แคปภาพครบ 27 เฟรม [โชว์] [2] [status:: done]

---

## Links
- [[BEPAL/Docs/NewAgile/01-product-backlog|Product Backlog]]
- [[BEPAL/Docs/NewAgile/02-sprint-backlog|Sprint Backlog]]
- [[BEPAL/Docs/NewAgile/sprint-plan-03|Sprint 3 Plan]]
