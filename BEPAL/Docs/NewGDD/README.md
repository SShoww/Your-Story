---
type: gdd-index
version: 2.0
date: 2026-09-20
---

# BePal — Game Design Document Suite (v2.0)

## Executive Summary (บทสรุปผู้บริหาร)

เอกสารชุดนี้เป็น **Game Design Document (GDD v2.0)** ฉบับสมบูรณ์สำหรับเกม **BePal** ซึ่งได้รับการพัฒนาและยกระดับอย่างก้าวกระโดดจากเอกสารต้นแบบเดิม (GDD v1) โดยอ้างอิงและสังเคราะห์ข้อมูลจากชุดเอกสารนำเสนอการออกแบบเกม (Presentation Slides) จำนวน 64 สไลด์ ร่วมกับการยกระดับความลึกซึ้งทางคณิตศาสตร์ (Mathematical Deepening), การวางบทสนทนาและโครงสร้างเนื้อเรื่องที่สมบูรณ์ (Full Narrative Branching), ระบบการต่อสู้ระดับบอสแบบหลายเฟส (Multi-Phase Boss Battles), และสถาปัตยกรรมทางเทคนิคสำหรับเฟรมเวิร์ก MonoGame (.NET 8 C#)

> **การสงวนเอกสารเดิม (Preservation Notice):**  
> เอกสาร GDD ดั้งเดิมทั้งหมดในโฟลเดอร์ `BEPAL/Docs/GDD/` จะถูกเก็บรักษาไว้โดยไม่มีการลบหรือแก้ไขใดๆ เพื่อใช้เป็นบันทึกประวัติการพัฒนา (Legacy Baseline) โดยเอกสารชุดใหม่ในโฟลเดอร์ `BEPAL/Docs/NewGDD/` จะทำหน้าที่เป็น **Single Source of Truth (เอกสารอ้างอิงหลัก)** สำหรับการออกแบบและพัฒนาเกมทั้งหมดนับจากนี้เป็นต้นไป

---

## Complete Documentation Index (สารบัญเอกสารทั้งหมด)

| เอกสาร | รหัส | หัวข้อหลักและขอบเขตเนื้อหา | ลิงก์ภายใน |
| --- | --- | --- | --- |
| **00-concept.md** | `gdd-concept` | ภาพรวมเกม, สโลแกน, เสาหลัก 4 ด้าน, สายพันธุ์สัตว์เลี้ยงเริ่มต้น (Coco, Sproutlet, Gloomtail), สุนทรียศาสตร์งานภาพและเสียง | [[BEPAL/Docs/NewGDD/00-concept|00-concept.md]] |
| **01-core-loop.md** | `gdd-core-loop` | ลูปการเล่นรายวัน 4 เฟส (Narrative, Care, Defense, Progression), ไทม์ไลน์ Day 1–3, แผนผัง State Machine และเงื่อนไขการแพ้ชนะ | [[BEPAL/Docs/NewGDD/01-core-loop|01-core-loop.md]] |
| **02-core-rules-and-stats.md** | `gdd-mechanics-rules` | โครงสร้างสเตตัสสัตว์เลี้ยง (HP, Stomach, Clean, EXP/Level), สูตรคณิตศาสตร์การเสื่อมถอย, ระบบพลังงาน 6 AP, สภาวะหมดสภาพและเงินกู้ฉุกเฉิน | [[BEPAL/Docs/NewGDD/02-core-rules-and-stats|02-core-rules-and-stats.md]] |
| **03-care-and-qte-systems.md** | `gdd-care-qte` | กิจกรรมการดูแล 4 หมวด (Feed, Clean, Train, Heal), มินิเกม QTE 10 ครั้งต่อรอบ, องศาความแม่นยำ Perfect/Good/Miss, สูตรเวลาเดินหน้า (+10%/+15%) และเกรด S–F | [[BEPAL/Docs/NewGDD/03-care-and-qte-systems|03-care-and-qte-systems.md]] |
| **04-events-and-encounters.md** | `gdd-events-encounters` | ภัยพิบัติพายุ Day 1, เหตุการณ์เคาะประตู Day 2 "Knock Knock !!", สัตว์ร้าย Toothless, ทางเลือก Chase vs Tame, ระบบการต่อสู้เพื่อสยบสัตว์ | [[BEPAL/Docs/NewGDD/04-events-and-encounters|04-events-and-encounters.md]] |
| **05-merchant-and-combat.md** | `gdd-merchant-combat` | พ่อค้าเร่ Day 3, ทางแยกทางศีลธรรมขาย Toothless 5,000G vs ปฏิเสธ, บอสไฟต์ 3 เฟส (Greed's Splash, Gold Gatling, Cane Strike), ฉากจบ Ending A & B | [[BEPAL/Docs/NewGDD/05-merchant-and-combat|05-merchant-and-combat.md]] |
| **06-economy-shop-inventory.md** | `gdd-economy-inventory` | ระบบเศรษฐกิจ Gold, ราคาสินค้า 5 ชนิดในร้านค้า, ตารางอุปกรณ์สวมใส่ (Ballet Shoes, Toy Knife, Faded Ribbon), กระเป๋า 8 ช่องและระบบอินสเปกเตอร์ | [[BEPAL/Docs/NewGDD/06-economy-shop-inventory|06-economy-shop-inventory.md]] |
| **07-ui-ux-flow.md** | `gdd-ui-ux` | แผนผัง UI Wireframes ครบทุกหน้าจอ (Title, Dialogue Box, Choose Pet, Base Room HUD, Care Wheel, Combat Arena, Revive Modal, Summary Card) | [[BEPAL/Docs/NewGDD/07-ui-ux-flow|07-ui-ux-flow.md]] |
| **08-technical-architecture.md** | `gdd-technical-architecture` | สถาปัตยกรรม MonoGame C# 12, การแยก Domain Logic จาก Engine, โครงสร้างคลาส, แผนภาพ State Machine, JSON Save Schema, และแผนผัง Migration 5 เฟส | [[BEPAL/Docs/NewGDD/08-technical-architecture|08-technical-architecture.md]] |

---

## Comparison Matrix: Legacy GDD v1 vs New GDD v2

ตารางเปรียบเทียบการเปลี่ยนแปลงและยกระดับระหว่างเอกสารดั้งเดิมและเอกสารฉบับใหม่:

| มิติการออกแบบ | Legacy GDD (v1) | New GDD (v2) | ประโยชน์และผลลัพธ์ที่ยกระดับ |
| --- | --- | --- | --- |
| **ขอบเขตการเล่น (Scope)** | ลูป 5 วันแบบหลวมๆ วนรับสัตว์แปลกหน้าประตู | **3-Day High-Density Vertical Slice** ที่มีโครงเรื่องเข้มข้น มีจุดเริ่มต้น จุดวิกฤต และไคลแมกซ์ชัดเจน | Pacing กระชับ สนุก ตื่นเต้น และเหมาะแก่การนำเสนอหรือทดสอบ Alpha/Demo |
| **ระบบสเตตัสสัตว์เลี้ยง** | มีเพียง Health 3 แต้ม และ Satisfaction Bar 3 แต้ม | **สเตตัสเสมือนจริง 4 มิติ:** Health (0–100), Stomach (0–100), Clean (0–100), และ Level/EXP | มอบความรู้สึกของการเป็น Virtual Pet จริง มีความผูกพันและต้องใส่ใจดูแลรอบด้าน |
| **ทรัพยากรและการบริหาร** | เล่นได้เรื่อยๆ ไม่จำกัดครั้งจนกว่าเลือดจะหมด | **Discrete Energy Budget (6 AP/วัน)** จัดสรรการกระทำอย่างมีกลยุทธ์ | เกิดการวางแผน (Resource Management) ที่ท้าทาย ทุกแอ็กชันมีความหมาย |
| **ระบบมินิเกม Care QTE** | หมุนเข็มสุ่มเลือก 1 ใน 4 ช่อง (Feed, Play, Pet, Observe) | **10-Attempt Session** แยกมินิเกมตามหมวดหมู่ (Feed, Clean, Train, Heal) พร้อมสูตรคำนวณ Perfect/Good/Miss | ควบคุมจังหวะได้แม่นยำ ท้าทายฝีมือผู้เล่น และมีระบบสะสมคะแนนสตรีค |
| **สูตรเวลา (Day Progress)** | เวลาไม่เดินหน้าตามผลลัพธ์ | **Day Progress Scaling:** สำเร็จ $+10\%$, พลาด $+15\%$ (เวลาเร่งเร็วขึ้นเมื่อพลาด) | เพิ่มความกดดันทางอารมณ์และสะท้อนความตื่นตระหนกได้อย่างแยบยล |
| **ระบบการต่อสู้ (Combat)** | มีเพียง Dodge QTE รับการโจมตีแบบตั้งรับ | **ระบบต่อสู้เต็มรูปแบบ:** มี Boss HP, Telegraphs, Dodge Zones, และหน้าต่าง Counter-Attack สวนกลับ | มีความตื่นเต้นแบบเกมแอ็กชัน สามารถสยบสัตว์และต่อสู้ป้องกันบ้านได้ |
| **เนื้อเรื่องและทางเลือก** | รับกล่องเปิดดูสัตว์ ไม่มีการตัดสินใจเชิงจริยธรรม | **Moral Dilemmas:** ทางเลือกขับไล่หรือสยบ Toothless, ทางเลือกขายสัตว์เลี้ยง 5,000G หรือสู้บอส | เพิ่มคุณค่าการเล่นซ้ำ (Replayability) และสร้างผลกระทบทางอารมณ์ |
| **ระบบเศรษฐกิจและไอเทม** | ไม่มีระบบเงินและร้านค้า | **ระบบเศรษฐกิจ Gold สมบูรณ์แบบ:** รายได้สนับสนุน, ค่ารักษา, ร้านค้า 5 ชนิด, กระเป๋า 8 ช่อง และอุปกรณ์สวมใส่ | เพิ่มความลึกในการวางแผนการเงินและความหลากหลายของบิลด์สัตว์เลี้ยง |

---

## Guidelines for Developers & Designers

1. **สำหรับ Game Designers:**
   - ใช้ค่าตัวเลขและสูตรคำนวณใน `02-core-rules-and-stats.md` และ `03-care-and-qte-systems.md` เป็นฐานในการ Balance ตัวเลข
   - สามารถขยายเนื้อเรื่องไปยัง Days 4–5 ได้ในอนาคตโดยใช้สถาปัตยกรรมลูป 4 เฟสใน `01-core-loop.md`
2. **สำหรับ Lead Programmers:**
   - ศึกษาสถาปัตยกรรมและ Migration Roadmap ใน `08-technical-architecture.md`
   - พัฒนาโมเดลในโฟลเดอร์ `CoPoject/BePal/Gameplay/` ให้เป็น Pure C# เพื่อให้ผ่านชุดทดสอบใน `CoPoject/BePal.Tests/`
3. **สำหรับ UI & 2D Artists:**
   - ยึดสัดส่วนความละเอียดและเลย์เอาต์ตาม Wireframes ใน `07-ui-ux-flow.md`
   - ออกแบบชุดสไปรต์สัตว์เลี้ยงให้มีอารมณ์ Idle, Hungry, Distressed, Happy, และ Hurt เพื่อรองรับ Status Effects
