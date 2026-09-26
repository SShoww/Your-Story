# Sprint 1 Plan — MVP Core Gameplay & Care Foundations (v2.0)

**Sprint Goal:** พัฒนาแก่นการเล่น MVP ให้สมบูรณ์: ระบบเลือก Starter Pet 3 สายพันธุ์, วงล้อ Care QTE 10 ครั้งต่อรอบ, การจัดสรรพลังงาน 6 AP, สเตตัส 4 มิติ และการรับมือพายุ Thunderstorm ใน Day 1  
**ระยะเวลา:** 2026-09-01 — 2026-09-14  
**Team:**
- วศิน ศรีวรกุล (โชว์) — Lead Programmer (14 SP)
- ปีย์ตะวัน แห่งหาญ (ซุง) — Flex (15 SP)
- ภูมิพัฒน์ ตามวงค์ (ภูมิ) — Game Designer & UI Lead (0 SP)
- ธัญญรัตน์ ติ๊บหน่อ (เดียร์) — 2D Art (0 SP)
**Total Velocity:** 29 SP (Done ✅)

---

## Sprint Backlog

| # | User Story / Task | ผู้รับผิดชอบ | MoSCoW | Estimate (SP) | Status |
| --- | --- | --- | --- | --- | --- |
| 1 | Starter Pet Selection & Base Habitat Room | โชว์ (Lead Prog) | Must Have | 4 | ✅ Done |
| 2 | 10-Attempt Care QTE Engine | โชว์ (Lead Prog) | Must Have | 5 | ✅ Done |
| 3 | Pet Stats Model & Mathematical Decay Rules | ซุง (Flex) | Must Have | 6 | ✅ Done |
| 4 | Discrete Energy Budget (6 AP) System | โชว์ (Lead Prog) | Must Have | 5 | ✅ Done |
| 5 | Sickness & Health Status Effects Design | ซุง (Flex) | Must Have | 5 | ✅ Done |
| 6 | Day 1 Thunderstorm Disaster & Calming QTE | ซุง (Flex) | Must Have | 4 | ✅ Done |

## Status Legend
- 🔲 Todo
- 🔄 In Progress
- 🔍 In Review
- ✅ Done
- ❌ Blocked

---

## Detailed Tasks

### Story 1 & 2 — Starter Pets & Care QTE Wheel Engine
- [x] สร้างหน้าต่างเลือกรับอุปการะ Starter Pet (Coco, Sproutlet, Gloomtail) [โชว์] [4] [status:: done]
- [x] พัฒนาวงล้อ QTE 10 ครั้ง เข็มหมุน 2.4 rad/s พร้อมตรวจจับ Perfect / Good Zone [โชว์] [5] [status:: done]
- [x] ตรวจจับคะแนนและเกรดสะสม พร้อมหลอด Day Progress (+10% / +15%) [โชว์] [3] [status:: done]

### Story 3 & 4 — Pet Stats & 6 AP Energy Economy
- [x] ออกแบบสูตรสเตตัส 4 มิติ: HP (0–100), Stomach, Clean, EXP/Level [ซุง] [3] [status:: done]
- [x] คำนวณสูตร Natural Decay (Stomach -20, Clean -15) และ Metabolic Burn (-5/-10) [ซุง] [3] [status:: done]
- [x] พัฒนาระบบโควตาพลังงาน 6 AP และการตัดเข้าสู่ Phase 3 เมื่อพลังงานหมด [โชว์] [5] [status:: done]

### Story 5 & 6 — Sickness Penalties & Day 1 Thunderstorm
- [x] ออกแบบตารางสถานะผิดปกติ Starving, Grimy, Infected และการหัก HP ข้ามวัน [ซุง] [5] [status:: done]
- [x] ออกแบบเหตุการณ์พายุ Thunderstorm ใน Day 1 (Clean -25, Stomach -10) [ซุง] [2] [status:: done]
- [x] พัฒนามินิเกมปลอบประโลมฉุกเฉิน Calming QTE 3 จังหวะ [ซุง] [2] [status:: done]

---

## Links
- [[BEPAL/Docs/NewAgile/01-product-backlog|Product Backlog]]
- [[BEPAL/Docs/NewAgile/02-sprint-backlog|Sprint Backlog]]
- [[BEPAL/Docs/NewAgile/sprint-plan-02|Sprint 2 Plan]]
