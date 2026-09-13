---
type: gdd-scope
version: 0.3
date: 2026-09-13
---

# BePal — Scope & Feature List

## Unique Selling Point (USP)

1. **Cozy Aesthetics with Uncanny Hardcore Mechanics:** บรรยากาศบ้านและศูนย์พักพิงที่อบอุ่นนุ่มนวล แต่แฝงไปด้วยสัตว์เลี้ยงที่มีสรีระหรือพฤติกรรมผิดปกติ (Uncanny) และอันตรายถึงชีวิต
2. **Observation & Consequential Interaction:** แก่นการเล่นเน้นการสังเกตและลองผิดลองถูกที่มีผลลัพธ์ตามมาอย่างชัดเจน—ทุกการกระทำที่ผิดพลาดจะถูกลงโทษด้วยดาเมจ และทุกการกระทำที่ถูกต้องจะนำไปสู่ข้อมูลใน Survival Log
3. **Samsara Room 4-Wall Exploration:** ห้องพักสัตว์ 4 ทิศ (Wall 1–4) หมุนสำรวจ ซ้าย-ขวา เพื่ออ่านเบาะแสของกิน ตรวจสอบสมุดบันทึก หรือเตรียมตัวดูแล
4. **2-Phase Care Loop with Tactile Mini-Games:** ไม่ใช่แค่กด Spacebar ดูวงล้อ แต่เป็นการอ่านภาษากาย กะจังหวะ Sweet Spot และลงมือดูแลจริงผ่านมินิเกม 4 รูปแบบ (เทอาหาร, ลูบตัว, เล่นของเล่น, ส่องแว่นขยาย)

## Feature List & Priorities

| # | Feature | Priority | รายละเอียด / Acceptance Criteria |
| --- | --- | --- | --- |
| 1 | **Day 1 Prologue & Story Intro** | **Must** | ฉากเล่าเรื่องเปิดร้าน แนะนำตัวละคร รับกล่องพัสดุปริศนา และเปิดกล่องพบ Mossling ผ่าน Unified Dialogue Box |
| 2 | **4-Wall Panoramic Shelter Room** | **Must** | ระบบห้อง 4 ทิศ (Wall 1 Pet, Wall 2 Pantry, Wall 3 Desk, Wall 4 Door) หมุนมุมมองซ้าย-ขวา พร้อมจุดคลิกสำรวจสิ่งของหาเบาะแส |
| 3 | **Unified Dialogue & Inspection System** | **Must** | กล่องข้อความสไตล์ Visual Novel: Typewriter effect, คลิกกล่องเพื่อแสดงข้อความเต็มทันที, ปุ่ม Next และรองรับ Prompt `[YES] / [NO]` |
| 4 | **2-Phase Care Loop (Deduction & Mini-Games)** | **Must** | **Phase 1:** สังเกต Behavior Cues + หมุน Dynamic Wheel พร้อม Sweet Spot สีทอง<br>**Phase 2:** มินิเกมสัมผัสจริง 4 รูปแบบ (Feed: เทอาหาร, Pet: ลูบตัว, Play: จับของเล่น, Observe: ส่องเลนส์) |
| 5 | **Pet Behavior & Action Patterns** | **Must** | สัตว์เลี้ยง 3 ชนิด (Mossling, Nibbleclaw, Blinkbun) พร้อม Dynamic Cues และรูปแบบความชอบที่ผู้เล่นต้องอนุมาน |
| 6 | **Dodge QTE Challenge** | **Must** | เมื่อสัตว์เข้าสู่สถานะโจมตี วงล้อจะเปลี่ยนเป็น Dodge QTE ที่มี Dodge Zone สีทอง หากกดพลาดจะเสีย Health |
| 7 | **Health, Forced Retreat & Risk/Reward** | **Must** | Health 3 หน่วย ดูแลสำเร็จ 1 ครั้งปลดล็อกปุ่ม [End Day] ผู้เล่นเลือกได้ว่าจะจบวันหรือเสี่ยงดูแลต่อเพื่อปลด Log หาก HP=0 เกิด Forced Retreat |
| 8 | **Daily Summary Report Card** | **Must** | หน้าสรุปผลประจำวันสไตล์ Papers, Please สรุปจำนวน Session สำเร็จ, ข้อมูล Log ใหม่ และสถานะร่างกาย ก่อนเข้าสู่ฉากพักผ่อนกลางคืน |
| 9 | **Doorstep Arrival Scene (Days 2–5)** | **Must** | ฉากรับพัสดุกล่องใหม่หน้าประตูในทุกเช้าก่อนเข้าสู่ห้องดูแล |
| 10 | **Survival Log System** | **Must** | สมุดบันทึกข้อมูลพฤติกรรมสัตว์ ปลดล็อก Action Pattern แบบสมบูรณ์เมื่อดูแลสัตว์ตัวนั้นสำเร็จครบ 3 Sessions |
| 11 | **Audio Feedback & BGM** | **Should** | เสียง SFX ตอนพิมพ์ดีด, เสียงเทอาหาร, เสียงลูบตัว, เสียง QTE ถูก/ผิด, และเพลงบรรเลง BGM Cozy |
| 12 | **Hazard & Harm Classification** | **Should** | แสดงข้อมูล Hazard Level (1–3), Harm Type (Physical/Mental) บนสมุดและหน้าจอ |
| 13 | **Caretaker Daily Upgrades** | **Could** | ระบบพัฒนาความสามารถตัวละครเมื่อจบวัน (ขยาย Safe Zone, เพิ่มความทนทาน) |
| 14 | **Save / Load System** | **Could** | บันทึกความคืบหน้าของวันและข้อมูล Survival Log ลงไฟล์ JSON |

## Out of Scope — สิ่งที่ไม่ทำในเฟสนี้

- **No Weapon Arsenal / Combat Mechanics:** ไม่มีการใช้อาวุธ ดาบ หรือปืนใดๆ ทั้งสิ้น—ปฏิสัมพันธ์กับสัตว์ทำผ่านการดูแลและหลบหลีกเท่านั้น
- **No Free-roam Platforming:** ไม่มีระบบเดินกระโดดแบบ Platformer—เป็น Point-and-Click บริหารจัดการในห้อง 4 ทิศ
- **No Complex Multi-frame Animations:** ใช้ Sprite แสดงอารมณ์หลัก (Idle, Happy, Angry, Cues) ผสม Procedural Transform/Shake
- **No Pure Horror / Blood Splatter:** หลีกเลี่ยงภาพเลือดสาดเพื่อรักษาแก่น "Cozy yet Dangerous"

## Risks & Mitigation

| ความเสี่ยง | ผลกระทบ | แนวทางป้องกันและแก้ไข |
| --- | --- | --- |
| **ภาระงานด้านภาพฉาก 4 ทิศ** | งาน Art ส่งช้า | ออกแบบ Perspective ภายในห้องให้เรียบง่าย วาดเป็น 4 มุมต่อเนื่องกัน และใช้ Placeholder ชัดเจนระหว่างพัฒนา |
| **Mini-Game 4 แบบใช้เวลาขัดเกลานาน** | Pacing ของเกมสะดุด | ออกแบบฟิสิกส์และการควบคุมของ Mini-game ให้เรียบง่ายและกระชับ (เล่นไม่เกิน 2–3 วินาทีต่อรอบ) |
| **ความซับซ้อนของการเปลี่ยนสถานะหน้าจอ** | เกิดบั๊กในการเปลี่ยน Scene | รวมศูนย์การจัดการผ่าน `ScreenManager` และใช้ `DialogueBox` เป็นตัวกลางในการทริกเกอร์เหตุการณ์ |
