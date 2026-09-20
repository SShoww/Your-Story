---
type: agile-backlog
version: 2.0
date: 2026-09-20
project: BePal
---

# Product Backlog — BePal (Master Traceability & Story Points v2.0)

> รวม User Stories, Requirements และ Technical Tasks ทั้งหมดของโปรเจกต์ BePal สอดคล้องกับเอกสาร Game Design Document (New GDD v2.0 ฉบับสมบูรณ์ 00–05), Master Asset List, สถาปัตยกรรม Clean Architecture, และกระดาน Kanban Board ตลอดแผนการพัฒนา **3 Sprints (รวม 128 SP, สิ้นสุดก่อน 12 ตุลาคม 2026)**
> - **โชว์ (Show):** Lead Programmer (60 SP)
> - **ซุง (Zunk):** Game Designer (28 SP)
> - **ภูมิ (Pooh):** Flex (leans towards Design) / Audio Support (21 SP)
> - **เดียร์ (Dear):** 2D Art & UI Lead (19 SP)

---

## 1. GDD Feature Traceability Matrix (ตารางเชื่อมโยงกับ New GDD v2.0)

| GDD Reference | ระบบ / ฟีเจอร์หลัก | Backlog Item IDs | Sprint | MoSCoW | ผู้รับผิดชอบหลัก |
| --- | --- | --- | --- | --- | --- |
| **GDD-00 / 01** | Starter Pets & 4-Phase Daily Loop | US-01, US-04, US-23, TECH-05 | Sprint 1, 3 | **Must** | โชว์ (Lead Prog) |
| **GDD-01 / 03** | 10-Attempt Care QTE & 4 Actions | US-02, US-03, US-05 | Sprint 1 | **Must** | โชว์ & ซุง |
| **GDD-02 / 03** | Day 1 Thunderstorm Disaster | US-06, ART-02 | Sprint 1, 3 | **Must / Should** | ซุง & เดียร์ |
| **GDD-01 / 03** | Day 2 Toothless Knock Knock & Combat Taming | US-08, US-09, US-10, ART-01 | Sprint 2 | **Must** | โชว์, ภูมิ, เดียร์ |
| **GDD-02 / 05** | Dynamic Sprites & Audio Production | US-07, US-11, US-12, US-16 | Sprint 2, 3 | **Should** | เดียร์, ภูมิ, โชว์ |
| **GDD-03 / 04** | Architecture, Tests & Screens | TECH-01, TECH-02, TECH-03, TECH-04 | Sprint 2 | **Should** | โชว์ (Lead Prog) |
| **GDD-01 / 03** | Day 3 Merchant Dilemma & 3-Phase Boss Fight | US-14, US-21, DES-03, ART-03 | Sprint 3 | **Must** | โชว์, ภูมิ, ซุง, เดียร์ |
| **GDD-03 / 04** | 8-Slot Inventory & Merchant Shop Catalog | US-20, DES-02, ART-04 | Sprint 3 | **Must / Should** | โชว์, ซุง, เดียร์ |
| **GDD-01 / 03** | Emergency Revive, 500G Loan & Endings | US-15, US-22, QA-02 | Sprint 3 | **Must** | โชว์, ภูมิ, ซุง |

---

## 2. Product Backlog Items by MoSCoW & Sprint

### 2.1 Sprint 1: MVP Core & Care Foundations (รวม 29 SP) ✅ Done

| ID | User Story / Requirement | Acceptance Criteria (เกณฑ์การตรวจรับ) | SP | Sprint | ผู้รับผิดชอบหลัก |
| --- | --- | --- | --- | --- | --- |
| **US-01** | **Starter Pet Selection & Base Habitat Room**<br>*As a player, I want to choose my starter pet and enter the habitat room.* | 1. เลือกรับอุปการะ 1 ใน 3 สัตว์เลี้ยง (Coco, Sproutlet, Gloomtail)<br>2. หน้าจอ Habitat Room แสดงผลสเตตัสเริ่มต้นครบถ้วน | **4** | 1 | โชว์ (Lead Prog) |
| **US-02** | **10-Attempt Care QTE Engine**<br>*As a player, I want to execute 10 consecutive QTE attempts for Feed, Clean, Train, and Heal.* | 1. เข็มหมุน $\omega = 2.4 \text{ rad/s}$ ตรวจจับ Perfect ($\le 0.20$) และ Good ($\le 0.45$)<br>2. ตัวนับ Attempt 1–10 และ Day Progress สะสม $+10\%/+15\%$ | **5** | 1 | โชว์ (Lead Prog) |
| **US-03** | **Pet Stats Model & Mathematical Decay Rules**<br>*As a designer, I want mathematical formulas for HP, Stomach, Clean, and natural daily decay.* | 1. ซุงกำหนดสูตร Natural Decay: Stomach -20, Clean -15<br>2. กำหนดสูตร Metabolic Burn: $-5$ ต่อแอ็กชัน (Train -10) | **6** | 1 | ซุง (Game Designer) |
| **US-04** | **Discrete Energy Budget (6 AP) System**<br>*As a player, I want to manage 6 Energy Points per day to allocate actions.* | 1. เริ่มต้นวันใหม่ด้วย 6 AP เสมอ<br>2. หักค่า AP ตามคำสั่ง (Feed 1, Clean 1, Train 1, Heal 1 หรือ 2)<br>3. ตัดเข้าสู่ Phase 3 เมื่อ AP = 0 | **5** | 1 | โชว์ (Lead Prog) |
| **US-05** | **Sickness & Health Status Effects Design**<br>*As a designer, I want clear conditions and penalties for Starving, Grimy, and Infected states.* | 1. กำหนดเกณฑ์ Starving (Stomach = 0) เสีย HP ข้ามวัน<br>2. กำหนดเกณฑ์ Grimy (Clean < 50) และ Infected (Clean < 25) | **5** | 1 | ซุง (Game Designer) |
| **US-06** | **Day 1 Thunderstorm Disaster & Calming QTE**<br>*As a player, I want to face the thunderstorm disaster and soothe my distressed pet.* | 1. เกิดเหตุการณ์พายุ Clean ลด -25 และ Stomach ลด -10<br>2. มินิเกม Calming QTE 3 ครั้ง ฟื้นฟู Health +15 และระงับความเครียด | **4** | 1 | ซุง (Game Designer) |

---

### 2.2 Sprint 2: Toothless Taming, Combat Arena & Systems Polish (รวม 45 SP) 🔄 Active

| ID | User Story / Requirement | Acceptance Criteria (เกณฑ์การตรวจรับ) | SP | Sprint | ผู้รับผิดชอบหลัก |
| --- | --- | --- | --- | --- | --- |
| **US-07** | **Dynamic Pet Emotional Sprites (Coco 4 ท่า)**<br>*As a player, I want expressive sprites for Idle, Happy, Angry, and Hurt.* | 1. เดียร์วาด Coco ครบ 4 อารมณ์ ขนาด 280×360 px<br>2. รองรับการแสดงผลแอนิเมชัน procedural bounce และ reaction timer | **4** | 2 | เดียร์ (2D Art Lead) |
| **US-08** | **Day 2 "Knock Knock !!" Narrative Script & Clues**<br>*As a player, I want an immersive narrative buildup for the mysterious wild encounter.* | 1. ภูมิเขียนบทสนทนาเสียงเคาะประตู รอยกรดม่วง และทางเลือก Chase vs Tame<br>2. ข้อความสลับแสดงผลผ่าน DialogueBox รองรับ fast-reveal | **4** | 2 | ภูมิ (Flex / Audio) |
| **US-09** | **Toothless Combat Taming Arena & QTE**<br>*As a player, I want an action-packed combat arena to tame Toothless.* | 1. โชว์สร้างหน้าจอ `CombatArenaScreen`<br>2. แสดงหลอด Tame Gauge 0–100% และรับมือคลื่นการโจมตี Acid Spit | **5** | 2 | โชว์ (Lead Prog) |
| **US-10** | **Acid Spit Dodge Zone & Counter-Attack Window**<br>*As a player, I want to dodge lethal acid and execute counter-attacks.* | 1. Dodge Zone สีทองที่มุมด้านบน ($\pm 0.30 \text{ rad}$)<br>2. Shrinking Ring Counter-Attack กดภายใน 0.3s เพื่อเพิ่ม Tame Gauge +25% | **5** | 2 | โชว์ (Lead Prog) |
| **US-11** | **Core Sound Effects Production (9 SFX)**<br>*As a player, I want high-quality SFX for QTE hits, misses, parry, acid, and coins.* | 1. ภูมิตัดต่อและ Normalize เสียง SFX 9 ไฟล์ (.wav) ครบตาม GDD-05<br>2. เสียงคมชัด ไม่แตกพร่า ระดับเสียงสม่ำเสมอ | **3** | 2 | ภูมิ (Flex / Audio) |
| **US-12** | **MonoGame Audio Engine Integration**<br>*As a player, I want sound effects to trigger seamlessly on actions.* | 1. โชว์เขียนระบบเล่นเสียง SFX ผ่าน SoundEffectInstance<br>2. เชื่อมต่อเสียงในหน้า Care QTE และฉากต่อสู้ | **3** | 2 | โชว์ (Lead Prog) |
| **ART-01** | **Toothless Sprite Sheet (4 ท่า)**<br>*As a player, I want visually striking sprites for Toothless.* | 1. เดียร์วาด Toothless ครบ 4 ท่า: Idle, Angry, Attack, Tamed ขนาด 280×360 px<br>2. ลายเส้น Uncanny น่าเกรงขามแต่เชื่องได้ | **3** | 2 | เดียร์ (2D Art Lead) |
| **DES-01** | **Combat & Taming Balance Matrix**<br>*As a designer, I want mathematical balancing for dodge timing and damage.* | 1. ซุงคำนวณและระบุความเร็วเข็ม 3.0 rad/s, ดาเมจกรด 15 HP, อัตราเพิ่ม Tame Gauge<br>2. จัดทำ Balance Sheet รองรับการปรับจูน | **3** | 2 | ซุง (Game Designer) |
| **QA-01** | **Lead QA Playtesting & QTE Timing Calibration**<br>*As a QA tester, I want to verify the mechanical rhythm and difficulty of taming.* | 1. ภูมิทดสอบเล่นฉากสยบ Toothless วัดความรู้สึกของจังหวะกดหลบและสวนกลับ<br>2. จัดทำรายงานข้อเสนอแนะส่งให้ซุงและโชว์ปรับแต่ง | **3** | 2 | ภูมิ (Flex / Audio) |
| **TECH-01** | **Pure C# Domain Models Extraction**<br>*As an architect, I want core gameplay models decoupled from MonoGame render loops.* | 1. โชว์แยกคลาส `PetEntity`, `PetStats`, `EnergyAccount`, `EconomyManager`<br>2. เป็น Pure C# Class ตามแนวทางใน `AGENTS.md` และ `04-class-diagram.md` | **3** | 2 | โชว์ (Lead Prog) |
| **TECH-02** | **Automated Unit Testing (`BePal.Tests` Suite)**<br>*As a developer, I want an automated xUnit test suite covering rules and stats.* | 1. โชว์สร้างชุดทดสอบ xUnit ครอบคลุมการลดสเตตัส, การใช้พลังงาน, การคำนวณเกรด QTE<br>2. รันผ่าน 100% แบบ headless | **4** | 2 | โชว์ (Lead Prog) |
| **TECH-03** | **Screen Hierarchy & State Transitions (`ScreenManager`)**<br>*As a developer, I want modular screen management with transition wipes.* | 1. โชว์สร้างสถาปัตยกรรม `IScreen` และ `ScreenManager`<br>2. รองรับ Full Screen Swap และ Modal Overlays | **3** | 2 | โชว์ (Lead Prog) |
| **TECH-04** | **Content Pipeline & Automated Screenshot Harness**<br>*As a developer, I want build verification and automated visual regression test.* | 1. คลีนอัพ `Content.mgcb` ให้บิลด์ผ่าน 0 Warnings 0 Errors<br>2. ระบบ `--screenshot` บันทึกภาพครบทุกหน้าจออย่างแม่นยำ | **2** | 2 | โชว์ (Lead Prog) |

---

### 2.3 Sprint 3: Merchant Shop, 3-Phase Boss Battle & Release (รวม 54 SP) 🔲 Planned

| ID | User Story / Requirement | Acceptance Criteria (เกณฑ์การตรวจรับ) | SP | Sprint | ผู้รับผิดชอบหลัก |
| --- | --- | --- | --- | --- | --- |
| **US-20** | **8-Slot Inventory & Item Consumption Subsystem**<br>*As a player, I want an 8-slot inventory to manage items and equip gear.* | 1. โชว์พัฒนา `InventoryOverlayScreen` รองรับไอเทม 8 ช่อง<br>2. ป๊อปอัปคำอธิบายไอเทม และปุ่ม USE / EQUIP ทำงานถูกต้อง | **5** | 3 | โชว์ (Lead Prog) |
| **US-21** | **Merchant 3-Phase Boss Battle Engine**<br>*As a player, I want an epic 3-phase boss encounter when refusing the merchant.* | 1. โชว์พัฒนาตรรกะ Boss Fight 1,000 HP: Phase 1 (Acid Flasks), Phase 2 (Gold Gatling), Phase 3 (Collector's Cane)<br>2. ระบบสะสมเงิน $+2\text{G}$ เมื่อหลบเหรียญทองใน Phase 2 | **5** | 3 | โชว์ (Lead Prog) |
| **US-22** | **Emergency Revive & 500G Loan Subsystem**<br>*As a player, I want an emergency medical modal when HP hits 0 with debt loans.* | 1. โชว์พัฒนา `EmergencyReviveModal` ชำระเงิน 500G หรือเซ็นสัญญาเงินกู้<br>2. ระบบคิดดอกเบี้ยทบต้น 20% ต่อวัน และเงื่อนไขยึดสัตว์เลี้ยง | **4** | 3 | โชว์ (Lead Prog) |
| **US-23** | **Daily Summary Report Card & JSON Save System**<br>*As a player, I want end-of-day summary reports and persistent save progress.* | 1. โชว์พัฒนา `DailySummaryScreen` คำนวณเกรด S–F และมอบเงินรางวัล<br>2. เซฟและโหลดข้อมูลลง `savegame.json` ครบตาม Schema | **4** | 3 | โชว์ (Lead Prog) |
| **TECH-05** | **Final Release Build & Integration Polish**<br>*As a developer, I want a polished vertical slice executable ready for demo.* | 1. เชื่อมต่อระบบทั้งหมดอย่างไร้รอยต่อ ปราศจากบั๊กค้างหรือแครช<br>2. บิลด์ Release และทดสอบรอบสุดท้ายผ่านเกณฑ์ทั้งหมด | **3** | 3 | โชว์ (Lead Prog) |
| **DES-02** | **5-Item Shop Catalog & 3 Equipment Balance**<br>*As a designer, I want balanced pricing and stats for shop items and gear.* | 1. ซุงกำหนดราคาสินค้า 5 ชนิด (Crab Apple, Sea Tea, Cloudy Glasses, etc.)<br>2. กำหนดบัฟและข้อเสียเปรียบของอุปกรณ์ 3 ชิ้น (Ballet Shoes, Toy Knife, Faded Ribbon) | **4** | 3 | ซุง (Game Designer) |
| **DES-03** | **Boss Combat 3 Phases Math & Ending Conditions**<br>*As a designer, I want mathematically balanced boss HP and win/loss branches.* | 1. ซุงกำหนดพลังชีวิตบอส 1,000 HP แบ่ง 3 เฟส และดาเมจการโจมตีแต่ละท่า<br>2. กำหนดเงื่อนไขฉากจบ Ending A (ขาย 5,000G) และ Ending B (ปราบพ่อค้า) | **3** | 3 | ซุง (Game Designer) |
| **US-14** | **Day 3 Traveling Merchant Lore & 5,000G Buyout Script**<br>*As a player, I want dramatic dialogue for the moral dilemma offer.* | 1. ภูมิเขียนบทสนทนาพ่อค้า The Traveling Collector ยื่นข้อเสนอ 5,000G<br>2. หน้าต่างตัวเลือก `[ YES ]` / `[ NO ]` และบทสนทนาเมื่อปฏิเสธ | **4** | 3 | ภูมิ (Flex / Audio) |
| **US-15** | **Endings A & B Epilogue Narrative Scripts**<br>*As a player, I want poignant epilogue texts for both narrative conclusions.* | 1. ภูมิเขียนบทสรุปฉากจบ Ending A (The Wealthy Betrayal) ความร่ำรวยที่โดดเดี่ยว<br>2. เขียนบทสรุป Ending B (Sanctuary's Protector) ชัยชนะและการปกป้องบ้าน | **4** | 3 | ภูมิ (Flex / Audio) |
| **US-16** | **Atmospheric Cozy & Boss Battle BGM Soundtrack**<br>*As a player, I want immersive music tracks for base room, storm, and boss fight.* | 1. ภูมิจัดหาและมิกซ์เพลง BGM 5 แทร็ก (Cozy, Storm, Boss, Ending A, Ending B)<br>2. ตั้งค่า Loop ไร้รอยต่อ | **3** | 3 | ภูมิ (Flex / Audio) |
| **QA-02** | **End-to-End 3-Day Playtesting & Release QA**<br>*As a QA lead, I want full playthrough verification across all 3 days and branches.* | 1. ภูมิทดสอบเล่นลูปเต็ม 3 วัน ทั้งสายขายสัตว์เลี้ยง และสายสู้บอส<br>2. ตรวจสอบข้อความตกหล่น ตัวเลขสมดุล และยืนยันความเสถียรของเกม | **3** | 3 | ภูมิ (Flex / Audio) |
| **ART-02** | **Base Habitat Background & Porch/Arena Environments**<br>*As a player, I want immersive background art for shelter room and arena.* | 1. เดียร์วาดภาพฉาก 1280×720 px: Habitat Base, Porch, Storm, Combat Arena, Summary<br>2. จัดโทนสี Cozy yet Dangerous ตามทิศทางศิลป์ | **5** | 3 | เดียร์ (2D Art Lead) |
| **ART-03** | **The Traveling Collector Sprite Sheet (4 ท่าบอส)**<br>*As a player, I want dynamic sprites for the merchant boss in all 3 phases.* | 1. เดียร์วาดสไปรต์พ่อค้า 320×400 px: Idle, Talk, Angry, Flask, Gatling, Cane, Defeat<br>2. ออกแบบบุคลิกน่าเกรงขาม รอยยิ้มฟันทองลึกลับ | **4** | 3 | เดียร์ (2D Art Lead) |
| **ART-04** | **Item Icons & UI Dialogue Frames**<br>*As a player, I want clean pixel icons for items, equipment, and dialogue boxes.* | 1. เดียร์วาดไอคอนไอเทม 8 รูปแบบ (48×48 px)<br>2. วาดกรอบ Dialogue Box, ปุ่ม Choice Prompt, และป้าย Action Badges | **3** | 3 | เดียร์ (2D Art Lead) |

---

## 3. Workload Summary by Member & Sprint (Verified Math Matrix)

ตารางสรุป Story Points ตามบทบาทและ Sprint ที่ผ่านการตรวจสอบความถูกต้องทางคณิตศาสตร์ตรงกันสมบูรณ์ทุกแกน (รวมทั้งสิ้น 3 Sprints = 128 SP):

| สมาชิก | บทบาท (Role) | Sprint 1 (Done) | Sprint 2 (Active) | Sprint 3 (Planned) | รวมทั้งโปรเจกต์ (SP) |
| --- | --- | --- | --- | --- | --- |
| **วศิน (โชว์)** | **Lead Programmer** | 14 SP | 25 SP | 21 SP | **60 SP** |
| **ปีย์ตะวัน (ซุง)** | **Game Designer** | 15 SP | 6 SP | 7 SP | **28 SP** |
| **ภูมิพัฒน์ (ภูมิ / Pooh)** | **Flex (Design & Audio)** | 0 SP | 7 SP | 14 SP | **21 SP** |
| **ธัญญรัตน์ (เดียร์)** | **2D Art & UI Lead** | 0 SP | 7 SP | 12 SP | **19 SP** |
| **รวม Story Points ต่อ Sprint** | - | **29 SP** | **45 SP** | **54 SP** | **128 SP** |

---

## 4. Links
- [[BEPAL/Docs/NewAgile/02-sprint-backlog|Sprint Backlog]]
- [[BEPAL/Docs/NewAgile/03-kanban-board|Kanban Board]]
- [[BEPAL/Docs/NewAgile/04-Kanban-for-Obsidian|Obsidian Interactive Kanban]]
- [[BEPAL/Docs/NewGDD/00-concept|New GDD Concept]]
- [[BEPAL/Docs/NewGDD/01-core-loop|New GDD Core Loop]]
- [[BEPAL/Docs/NewGDD/02-scope-features|New GDD Scope & Features]]
- [[BEPAL/Docs/NewGDD/03-mechanics|New GDD Mechanics]]
- [[BEPAL/Docs/NewGDD/04-class-diagram|New GDD Architecture]]
- [[BEPAL/Docs/NewGDD/05-asset-list|New GDD Asset List]]
