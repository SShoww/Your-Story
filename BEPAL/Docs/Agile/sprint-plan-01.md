# Sprint 1 Plan — MVP Core Gameplay & Care QTE

**Sprint Goal:** พัฒนาแก่นการเล่น MVP ให้สมบูรณ์: ระบบวงล้อ Care QTE 4 ตัวเลือก, กลไกสัตว์โจมตีและ Dodge QTE, ระบบ Health / Forced Retreat, และสมุดบันทึก Survival Log
**ระยะเวลา:** 2026-09-01 — 2026-09-14
**Team:**
- วศิน ศรีวรกุล (โชว์) — Lead Programmer
- ปีย์ตะวัน แห่งหาญ (ซุง) — Game Designer
- ภูมิพัฒน์ ตามวงค์ (ภูมิ) — Flex (leans towards Design) / Audio Support
- ธัญญรัตน์ ติ๊บหน่อ (เดียร์) — 2D Art & UI Lead

## Sprint Backlog

| # | User Story | ผู้รับผิดชอบ | MoSCoW | Estimate (SP) | Status |
| --- | --- | --- | --- | --- | --- |
| 1 | As a player, I want to click so I can navigate menus and interact with pets | โชว์ & ซุง | Must Have | 4 | ✅ Done |
| 2 | As a player, I want to press Spacebar to execute Care QTE on the 4 actions | วศิน (โชว์) | Must Have | 5 | ✅ Done |
| 3 | As a player, I want to experience pet attacks and play Dodge QTE | วศิน (โชว์) | Must Have | 5 | ✅ Done |
| 4 | As a player, I want to see pet reactions and discover behavior rules | โชว์ & เดียร์ | Must Have | 6 | ✅ Done |
| 5 | As a player, I want Health depletion on failure and Forced Retreat at 0 HP | ซุง & เดียร์ | Must Have | 4 | ✅ Done |
| 6 | As a player, I want to unlock and review discovered pet data in Survival Log | โชว์ & ภูมิ | Must Have | 5 | ✅ Done |

## Status Legend
- 🔲 Todo
- 🔄 In Progress
- ✅ Done
- ❌ Blocked

---

## Detailed Tasks

### Story 1 & 2 — Care QTE Wheel & Input Handling
- [x] สร้างวงล้อ Care QTE 4 ส่วน (Feed, Play, Pet, Observe) [วศิน] [5] [status:: done]
- [x] ระบบหมุนเข็ม Wheel Marker (2.2 rad/s) และการคำนวณมุมด้วย Spacebar [วศิน] [3] [status:: done]
- [x] ตรวจจับคะแนน Pet Favor และเพิ่ม Satisfaction Bar จนเต็ม 3 แต้ม [วศิน] [3] [status:: done]

### Story 3 — Attack System & Dodge QTE
- [x] พัฒนาเงื่อนไขการโจมตีของสัตว์ตาม Action Pattern (Nibbleclaw) [วศิน] [4] [status:: done]
- [x] สร้างหน้าจอ Dodge QTE และตรวจจับตำแหน่งเข็มในโซนสีทอง (Dodge Zone) [วศิน] [4] [status:: done]

### Story 4 & 5 — Pet Emotions, Health & Forced Retreat
- [x] ออกแบบ Sprite สัตว์เลี้ยง 3 อารมณ์ (Idle, Happy, Angry) [เดียร์] [5] [status:: done]
- [x] จัดการระบบ Health 3 หน่วย และระบบจบวันฉุกเฉิน (Forced Retreat) [ซุง] [3] [status:: done]
- [x] ตารางการเล่น 5 Game Days และหน้าสรุป Run Summary [โชว์ & ซุง] [4] [status:: done]

### Story 6 — Survival Log
- [x] สร้างหน้าจอ Survival Log Overlay แสดงสถานะและ Action Pattern เมื่อครบ 3 รอบ [โชว์ & ภูมิ] [4] [status:: done]

---

## Sprint Review & Daily Notes

### Sprint 1 Completion Summary
- **สิ่งที่ทำสำเร็จ:** ต้นแบบเกม MonoGame BePal สามารถรันและเล่นลูปหลักได้ครบถ้วน ทั้ง 5 วัน, 3 สายพันธุ์สัตว์เลี้ยง (Mossling, Nibbleclaw, Blinkbun), วงล้อ Care QTE 4 ตัวเลือก, Dodge QTE, และ Survival Log
- **ข้อสังเกตและงานต่อยอดใน Sprint 2:**
  - แยกโค้ดจาก `Game1.cs` ให้เป็น Modular Screens และสร้างโปรเจกต์ Unit Test
  - เพิ่มความหลากหลายของงานภาพสัตว์เลี้ยงแต่ละตัว (Mossling, Nibbleclaw, Blinkbun)
  - นำเข้าเสียงประกอบ SFX และ BGM สไตล์ Cozy

---

## Links
- [[BEPAL/Docs/GDD/00-concept|GDD Concept]]
- [[BEPAL/Docs/GDD/01-core-loop|GDD Core Loop]]
- [[BEPAL/Docs/GDD/03-mechanics|GDD Mechanics]]
- [[BEPAL/Docs/Agile/01-product-backlog|Product Backlog]]
- [[BEPAL/Docs/Agile/02-sprint-backlog|Sprint Backlog]]
- [[BEPAL/Docs/Agile/03-kanban-board|Kanban Board]]
