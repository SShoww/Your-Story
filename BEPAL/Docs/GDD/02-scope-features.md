---
type: gdd-scope
version: 0.2
date: 2026-09-11
---

# BePal — Scope & Feature List

## Unique Selling Point (USP)

1. **Cozy Aesthetics with Uncanny Hardcore Mechanics:** บรรยากาศบ้านและศูนย์พักพิงที่อบอุ่นนุ่มนวล แต่แฝงไปด้วยสัตว์เลี้ยงที่มีสรีระหรือพฤติกรรมผิดปกติ (Uncanny) และอันตรายถึงชีวิต
2. **Observation & Consequential Interaction:** แก่นการเล่นเน้นการสังเกตและลองผิดลองถูกที่มีผลลัพธ์ตามมาอย่างชัดเจน—ทุกการกระทำที่ผิดพลาดจะถูกลงโทษด้วยดาเมจ และทุกการกระทำที่ถูกต้องจะนำไปสู่ข้อมูลใน Survival Log
3. **Wheel-based Care & Dodge QTE:** ระบบการดูแลสัตว์ผ่านวงล้อ 4 ตัวเลือก พร้อมจังหวะสกิลเช็คที่ปรับเปลี่ยนตามพฤติกรรมสัตว์ เช่น เข็มวาร์ปสุ่มตำแหน่ง หรือการโจมตีสวนกลับที่ต้องหลบใน Dodge Zone สีทอง

## Feature List & Priorities

| # | Feature | Priority | รายละเอียด / Acceptance Criteria |
| --- | --- | --- | --- |
| 1 | **Care QTE Wheel System** | **Must** | วงล้อ 4 ตัวเลือก (Feed, Play, Pet, Observe) เข็มหมุนวนตรวจจับตำแหน่ง และกดยืนยันด้วย Spacebar |
| 2 | **Pet Behavior & Action Patterns** | **Must** | สัตว์เลี้ยง 3 ชนิดแรก (Mossling, Nibbleclaw, Blinkbun) ที่มีรูปแบบพฤติกรรมและเงื่อนไขความชอบต่างกัน |
| 3 | **Dodge QTE Challenge** | **Must** | เมื่อสัตว์เข้าสู่สถานะจู่โจม วงล้อจะเปลี่ยนเป็น Dodge QTE ที่มี Dodge Zone สีทอง หากกดพลาดจะเสีย Health |
| 4 | **Health & Forced Retreat** | **Must** | ผู้เล่นมี Health 3 หน่วย หากหมดจะเกิด Forced Retreat จบวันฉุกเฉินและฟื้นฟู Health ในวันถัดไป |
| 5 | **Survival Log System** | **Must** | สมุดบันทึกข้อมูลพฤติกรรมสัตว์ ปลดล็อก Action Pattern แบบสมบูรณ์เมื่อดูแลสัตว์ตัวนั้นสำเร็จครบ 3 Sessions |
| 6 | **Daily Progression (Game Day 1–5)** | **Must** | วงจรการเล่น 5 วัน มีตารางสัตว์ประจำวัน (Day Schedule) และหน้าสรุป Run Summary เมื่อจบเกม |
| 7 | **Core UI & Screen Flow** | **Must** | หน้าจอ Main Menu, Help Screen, Main Pet Room, QTE Overlay, Survival Log Overlay, Run Summary |
| 8 | **Hazard & Harm Classification** | **Should** | แสดงข้อมูลหน้าห้อง: Hazard Level (ระดับ 1–3), Harm Type (Physical / Mental), และ Care Requirement |
| 9 | **Audio Feedback & BGM** | **Should** | เสียง SFX ตอนกด QTE ถูก/ผิด, เสียงเข็มวาร์ป, เสียงเตือน Dodge, และเพลงบรรเลง BGM สไตล์ Cozy |
| 10 | **Arrival / Unboxing Scene** | **Could** | ฉากรับกล่องพัสดุปริศนาหน้าบ้านในแต่ละวันพร้อมข้อความบรรยายสั้นๆ (Typewriter effect) |
| 11 | **Caretaker Daily Upgrades** | **Could** | ระบบพัฒนาความสามารถตัวละครเมื่อจบวัน (ขยายช่องเข็มสำเร็จ, เพิ่มแถบเลือด/ความทนทาน) |
| 12 | **Save / Load System** | **Could** | บันทึกความคืบหน้าของวันและข้อมูล Survival Log ลงไฟล์ JSON |

## Out of Scope — สิ่งที่ไม่ทำในเฟสนี้

- **No Weapon Arsenal / Combat Mechanics:** ไม่มีการใช้อาวุธ ดาบ หรือปืนใดๆ ทั้งสิ้น—ปฏิสัมพันธ์กับสัตว์ทำผ่านการดูแลและหลบหลีกเท่านั้น
- **No Free-roam Platforming:** ไม่มีระบบกระโดดหรือเดินสำรวจฉากแพลตฟอร์ม—เป็นระบบ Point-and-Click บริหารจัดการในห้อง
- **No Complex Multi-frame Animations:** ไม่ทำแอนิเมชันความละเอียดสูงหลายเฟรม เพื่อควบคุมภาระงานศิลป์ (ใช้ Sprite อารมณ์หลัก: Idle, Happy, Angry, Attack)
- **No Pure Horror / Blood Splatter:** หลีกเลี่ยงภาพเลือดสาดหรือความสยองขวัญสุดโต่ง เพื่อรักษาแก่น "Cozy yet Dangerous"

## Risks & Mitigation

| ความเสี่ยง                                | ผลกระทบ                          | แนวทางป้องกันและแก้ไข                                                                            |
| ----------------------------------------- | -------------------------------- | ------------------------------------------------------------------------------------------------ |
| **ภาระงานด้าน Art และ Sprite ไม่ทันเวลา** | เกมขาดความหลากหลายทางภาพ         | นำเทคนิค Procedural Shape / Color Tint มาช่วยเสริม และใช้ Sprite Sheet อารมณ์หลัก 3-4 ท่า        |
| **ระบบ QTE เล่นยากหรือน่าเบื่อเกินไป**    | ผู้เล่นรู้สึกหงุดหงิดหรือไม่สนุก | ปรับขนาดของ Hitbox และความเร็วการหมุนของเข็มให้ตอบสนองดี และเพิ่มลูกเล่นอย่าง Teleporting Marker |
| **ระยะเวลาพัฒนาจำกัด**                    | ฟีเจอร์หลุดจาก Scope             | ยึดมั่นตามเกณฑ์ Must-Have ก่อนเสมอ เลื่อนระบบ Upgrade และ Save/Load เป็นเฟสเสริม                 |

