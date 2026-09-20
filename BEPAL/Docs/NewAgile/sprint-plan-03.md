# Sprint 3 Plan — Merchant Shop, 3-Phase Boss Battle & Release (v2.0)

**Sprint Goal:** พัฒนาระบบวันที่ 3 สู่การปล่อยเกมที่สมบูรณ์: ระบบร้านค้าของพ่อค้าเร่, กระเป๋าเก็บของ 8 ช่อง, ทางเลือกขายสัตว์ 5,000G vs ปฏิเสธ, บอสไฟต์ 3 เฟส (Greed's Splash, Gold Gatling, Collector's Cane), ระบบกู้ชีพฉุกเฉิน 500G, ฉากจบ Ending A & B และการส่งมอบเกม  
**ระยะเวลา:** 2026-09-29 — 2026-10-11  
**Team:**
- วศิน ศรีวรกุล (โชว์) — Lead Programmer (21 SP)
- ปีย์ตะวัน แห่งหาญ (ซุง) — Game Designer (7 SP)
- ภูมิพัฒน์ ตามวงค์ (ภูมิ) — Flex (leans towards Design) / Audio Support (14 SP)
- ธัญญรัตน์ ติ๊บหน่อ (เดียร์) — 2D Art & UI Lead (12 SP)
**Total Velocity:** 54 SP (Planned 🔲)

---

## Sprint Backlog

| # | User Story / Task | ผู้รับผิดชอบ | MoSCoW | Estimate (SP) | Status |
| --- | --- | --- | --- | --- | --- |
| 1 | 8-Slot Inventory & Item Consumption | โชว์ (Lead Prog) | Must Have | 5 | 🔲 Planned |
| 2 | Merchant 3-Phase Boss Battle Engine | โชว์ (Lead Prog) | Must Have | 5 | 🔲 Planned |
| 3 | Emergency Revive & 500G Loan Subsystem | โชว์ (Lead Prog) | Must Have | 4 | 🔲 Planned |
| 4 | Daily Summary Report Card & JSON Save | โชว์ (Lead Prog) | Must Have | 4 | 🔲 Planned |
| 5 | Final Release Build & Integration Polish | โชว์ (Lead Prog) | Must Have | 3 | 🔲 Planned |
| 6 | 5-Item Shop Catalog & 3 Equipment Balance | ซุง (Game Designer) | Should Have | 4 | 🔲 Planned |
| 7 | Boss Combat 3 Phases Math & Ending Conditions | ซุง (Game Designer) | Must Have | 3 | 🔲 Planned |
| 8 | Day 3 Traveling Merchant Lore & Buyout Script | ภูมิ (Flex / Audio) | Must Have | 4 | 🔲 Planned |
| 9 | Endings A & B Epilogue Narrative Scripts | ภูมิ (Flex / Audio) | Must Have | 4 | 🔲 Planned |
| 10 | Atmospheric Cozy & Boss BGM Soundtrack | ภูมิ (Flex / Audio) | Should Have | 3 | 🔲 Planned |
| 11 | End-to-End 3-Day Playtesting & Release QA | ภูมิ (Flex / Audio) | Must Have | 3 | 🔲 Planned |
| 12 | Base Habitat Background & Environments | เดียร์ (2D Art Lead) | Must Have | 5 | 🔲 Planned |
| 13 | The Traveling Collector Boss Sprites | เดียร์ (2D Art Lead) | Must Have | 4 | 🔲 Planned |
| 14 | Item Icons & UI Dialogue Frames | เดียร์ (2D Art Lead) | Should Have | 3 | 🔲 Planned |

## Status Legend
- 🔲 Todo
- 🔄 In Progress
- 🔍 In Review
- ✅ Done
- ❌ Blocked

---

## Detailed Tasks

### Lead Programmer (โชว์ — 21 SP)
- [ ] พัฒนา `InventoryOverlayScreen` รองรับไอเทม 8 ช่อง และปุ่ม USE / EQUIP [โชว์] [5] [status:: todo]
- [ ] พัฒนาตรรกะ Boss Fight 1,000 HP 3 เฟส พร้อมระบบเก็บเหรียญทอง [โชว์] [5] [status:: todo]
- [ ] พัฒนา `EmergencyReviveModal` ชำระเงิน 500G หรือสัญญากู้ยืมเงินดอกเบี้ย 20% [โชว์] [4] [status:: todo]
- [ ] พัฒนา `DailySummaryScreen` รายงานสรุปผลรายวัน และระบบเซฟโหลด JSON [โชว์] [4] [status:: todo]
- [ ] รวมระบบทั้งหมด ตรวจสอบความเสถียร และเตรียมบิลด์เวอร์ชันสุดท้าย [โชว์] [3] [status:: todo]

### Game Designer (ซุง — 7 SP)
- [ ] กำหนดราคาสินค้า 5 ชนิด และสเปกอุปกรณ์ 3 ชิ้นลงในไฟล์ Config [ซุง] [4] [status:: todo]
- [ ] กำหนดสเกลเลือดบอส 1,000 HP ดาเมจแต่ละเฟส และเงื่อนไขฉากจบ Ending A/B [ซุง] [3] [status:: todo]

### Flex Design & Audio (ภูมิ — 14 SP)
- [ ] เขียนบทสนทนายื่นข้อเสนอซื้อ Toothless 5,000G และบทพูดปฏิเสธ [ภูมิ] [4] [status:: todo]
- [ ] เขียนบทสรุปส่งท้ายฉากจบ Ending A (ทรยศ) และ Ending B (ผู้ปกป้อง) [ภูมิ] [4] [status:: todo]
- [ ] จัดหาและมิกซ์เพลงประกอบ BGM 5 แทร็ก (Cozy, Storm, Boss, Endings) [ภูมิ] [3] [status:: todo]
- [ ] ดำเนินการทดสอบเล่นรอบสมบูรณ์ 3 วันทั้งสองเส้นทาง และบันทึกผล QA [ภูมิ] [3] [status:: todo]

### 2D Art & UI Lead (เดียร์ — 12 SP)
- [ ] วาดภาพฉากหลัง 1280×720 px: Base Room, Porch, Storm, Arena, Summary [เดียร์] [5] [status:: todo]
- [ ] วาดสไปรต์พ่อค้า The Traveling Collector 320×400 px ครบทุกท่าบอส [เดียร์] [4] [status:: todo]
- [ ] วาดไอคอนไอเทม 8 แบบ (48×48 px) กรอบ Dialogue Box และปุ่ม Choice [เดียร์] [3] [status:: todo]

---

## Links
- [[BEPAL/Docs/NewAgile/01-product-backlog|Product Backlog]]
- [[BEPAL/Docs/NewAgile/02-sprint-backlog|Sprint Backlog]]
- [[BEPAL/Docs/NewAgile/03-kanban-board|Kanban Board]]
- [[BEPAL/Docs/NewGDD/03-mechanics|New GDD Mechanics]]
