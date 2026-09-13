---

kanban-plugin: basic

---

# BePal — Interactive Kanban Board (Obsidian)

> 💡 **Obsidian Kanban Board:** ไฟล์นี้รองรับการแสดงผลแบบการ์ดลากวาง (Interactive Cards) ผ่าน Community Plugin **Kanban** ของ Obsidian
> - **Lead Programmer:** วศิน (โชว์) — `@โชว์`
> - **Game Designer:** ปีย์ตะวัน (ซุง) — `@ซุง`
> - **Flex (leans towards Design) / Audio Support:** ภูมิพัฒน์ (ภูมิ / Pooh) — `@ภูมิ`
> - **2D Art & UI Lead:** ธัญญรัตน์ (เดียร์) — `@เดียร์`

## 🗄️ Backlog

- [ ] **[T-27] Dynamic Wheel Sweet Spot & Cue Integration**<br>โชว์และซุงพัฒนาเข็ม Sweet Spot (+2 Satisfaction) และเชื่อมโยง Behavior Cues ใน Care QTE<br>👤 @โชว์ (ร่วมกับ @ซุง) 🏷️ #SP/6 #Gameplay/QTE #Priority/High
- [ ] **[T-28] Tactile Care Mini-Games (Feed, Pet, Play, Observe)**<br>โชว์และเดียร์พัฒนามินิเกมสัมผัสจริง 4 รูปแบบ (เทอาหาร, ลูบตัว, จับของเล่น, ส่องเลนส์) พร้อม Props<br>👤 @โชว์ (ร่วมกับ @เดียร์) 🏷️ #SP/8 #Gameplay/MiniGames #Priority/High
- [ ] **[T-29] Daily Summary Report Card & Night Rest**<br>โชว์พัฒนา DailySummaryScreen สไตล์ Papers, Please สรุปผลประจำวัน และระบบ Fade to Black พักผ่อนข้ามวัน<br>👤 @โชว์ (ร่วมกับ @ซุง) 🏷️ #SP/5 #Feature/Progression #Priority/High
- [ ] **[T-30] Narrative Lore Secret Origin & Ending**<br>ภูมิเขียนบทสรุปเนื้อเรื่องตอนจบของศูนย์วิจัย เปิดเผยความจริงเบื้องหลังพัสดุปริศนาทั้ง 5 วัน<br>👤 @ภูมิ 🏷️ #SP/4 #Design/Narrative #Priority/Medium
- [ ] **[T-31] Final Release Balance QA & Polish**<br>ภูมิทดสอบเล่นลูปเต็ม 5 วัน ตรวจสอบความสมดุลและความรู้สึกในการเล่นรอบสุดท้าย<br>👤 @ภูมิ 🏷️ #SP/3 #QA/Release #Priority/High


## 📋 Ready for Development

- [ ] **[T-23] Unified DialogueBox Subsystem**<br>โชว์สร้างคลาส DialogueBox รองรับ Typewriter effect, fast-reveal เมื่อคลิก, ปุ่ม Next และ Prompt [YES]/[NO]<br>👤 @โชว์ 🏷️ #SP/4 #Tech/UI #Priority/Critical
- [ ] **[T-24] 4-Wall Panoramic Shelter Navigation Engine**<br>โชว์พัฒนา PanoramicRoomScreen หมุนซ้าย-ขวา 4 ทิศ (สไตล์ Samsara Room) พร้อมคลิกสำรวจสิ่งของหาเบาะแส<br>👤 @โชว์ 🏷️ #SP/5 #Tech/Gameplay #Priority/Critical
- [ ] **[T-25] 4-Wall Panoramic Shelter Backgrounds (Wall 1–4) & Porch**<br>เดียร์วาดภาพฉาก 1280x720 px สำหรับ Wall 1 (Pet), Wall 2 (Pantry), Wall 3 (Desk), Wall 4 (Door) และชานเรือนหน้าบ้าน<br>👤 @เดียร์ 🏷️ #SP/5 #Art/Environment #Priority/Critical
- [ ] **[T-10] Prologue & Daily Doorstep Narrative Scripts**<br>ภูมิเขียนบทนำ Day 1 (เตรียมร้าน -> กล่องปริศนา -> เปิด Mossling) และบทพัสดุมาส่ง Day 2–5 เป็นภาษาอังกฤษ<br>👤 @ภูมิ 🏷️ #SP/4 #Design/Narrative #Priority/High
- [ ] **[T-26] Behavior Cues & Room Item Clues Design**<br>ซุงออกแบบภาษากายสัตว์ (Cues) และคำบอกใบ้บนชั้นอาหารและสิ่งของในห้อง 4 ทิศ<br>👤 @ซุง 🏷️ #SP/3 #Design/Mechanics #Priority/High
- [ ] **[T-15] 2D Sprites: Nibbleclaw & Blinkbun (8 ท่า)**<br>เดียร์วาดสไปรต์สัตว์เลี้ยงตัวที่ 2 และ 3 ครบ 4 อารมณ์ (Idle, Happy, Angry, Attack/Teleport) ขนาด 280x360 px<br>👤 @เดียร์ 🏷️ #SP/4 #Art/Sprites #Priority/High
- [ ] **[T-18] Audio Manager Integration into MonoGame**<br>โชว์นำไฟล์เสียง SFX ของภูมิมาเล่นในจังหวะกด Spacebar QTE, หลบพ้น, และถูกโจมตี<br>👤 @โชว์ (ร่วมกับ @ภูมิ) 🏷️ #SP/3 #Tech/Audio #Priority/Medium


## ⏳ In Progress

- [ ] **[T-09] Pet Rules & Balance Matrix**<br>ซุงกำหนด Action Patterns และตารางตัวเลขสัตว์ 3 ชนิด (ความเร็วเข็ม 2.2 rad/s, องศา Sector 60 deg, แต้ม Pet Favor)<br>👤 @ซุง 🏷️ #SP/4 #Design/Balance #Priority/Critical
- [ ] **[T-11] SFX & BGM Production & Staging**<br>ภูมิจัดหา ตัดต่อ และ Normalize ไฟล์เสียง SFX และ BGM ลงใน docs/02_Assets/_candidates/sfx, music<br>👤 @ภูมิ 🏷️ #SP/4 #Audio/Staging #Priority/High
- [ ] **[T-12] Mossling Dedicated Finished Sprite (4 ท่า)**<br>เดียร์วาดภาพ Mossling ตัวจริง (280x360 px) ครบ 4 อารมณ์ (Idle ตะไคร่นุ่ม, Happy ดอกไม้บาน, Angry มีหนาม, Attack สะบัดหนาม)<br>👤 @เดียร์ 🏷️ #SP/4 #Art/Sprites #Priority/High
- [ ] **[T-13] Death Spiral Scrap Badges (6 แบบ)**<br>เดียร์วาดป้ายกระดาษฉีก FEED, PLAY, PET, OBSERVE, DODGE ZONE, และ ATTACK!<br>👤 @เดียร์ 🏷️ #SP/3 #Art/UI #Priority/High


## 🔍 Review & Testing

- [ ] **[T-05] Death Spiral QTE System Implementation**<br>โชว์พัฒนาวงล้อ 4 Sectors ด้วย MonoGame.Extended, เข็มหมุน, Dodge Zone สีทอง, Floating Tags, และ Screen Shake<br>👤 @โชว์ 🏷️ #SP/5 #Gameplay/Core #Status/Review
- [ ] **[T-06] Playtest Screenshot Automation Pipeline**<br>โชว์สร้างคำสั่ง `--screenshot` สำหรับแคปภาพทุกหน้าจออัตโนมัติ และปุ่ม `F12` บันทึกภาพ PNG ทันที<br>👤 @โชว์ 🏷️ #SP/3 #Tooling/QA #Status/Review
- [ ] **[T-07] HUD Badges & Health Alignment Polish**<br>โชว์และเดียร์จัดเลย์เอาต์ป้าย HEALTH 3.0, DAY 1/5, LIVES hearts, และข้อมูลสัตว์เลี้ยงไม่ทับขอบกล่อง<br>👤 @โชว์ (ร่วมกับ @เดียร์) 🏷️ #SP/2 #UI/Polish #Status/Review
- [ ] **[T-08] Lead QA Playtest & Timing Balance Report**<br>ภูมิทดสอบฟีลลิ่งการเล่นจริงในเกม ซุงตรวจสอบค่าตัวเลขใน Balance Sheet สรุปผลปรับแต่งความเร็วเข็มและ Hit Window<br>👤 @ภูมิ (ร่วมกับ @ซุง) 🏷️ #SP/3 #QA/Balancing #Status/Review


## ✅ Done

- [x] **[T-01] MVP 5-Day Core Loop Prototype**<br>โชว์สร้างแก่นการเล่น 5 วัน, จัดการ Health 3 หน่วย, และระบบ Forced Retreat สำเร็จ<br>👤 @โชว์ 🏷️ #SP/5 #Milestone/MVP
- [x] **[T-02] Concept & GDD 00–05 Master Documentation**<br>อัปเดตเอกสาร GDD, Master Asset List, Backlogs, และ CONTEXT.md ครบถ้วนตาม Canva Board<br>👤 @ทั้งทีม 🏷️ #SP/4 #Milestone/Docs
- [x] **[T-03] ADR Architecture Decision Records (0001 & 0002)**<br>บันทึกสถาปัตยกรรม IScreen และ 4-Wall Panoramic Shelter / 2-Phase Care Loop<br>👤 @โชว์ (ร่วมกับ @ซุง) 🏷️ #SP/3 #Milestone/Architecture
- [x] **[T-04] Basic Survival Log Screen**<br>ปลดล็อกและแสดงผลข้อมูลพฤติกรรมสัตว์เมื่อผ่านการดูแลครบ 3 Sessions<br>👤 @โชว์ 🏷️ #SP/3 #Milestone/SurvivalLog
- [x] **[T-14] Pure C# Domain Models Extraction**<br>โชว์แยกคลาส CareAction, PetDefinition, ActionPattern ออกจาก Game1 ให้เป็น Pure C# Decoupled<br>👤 @โชว์ 🏷️ #SP/3 #Tech/Architecture
- [x] **[T-17] Setup BePal.Tests Unit Test Project**<br>โชว์สร้างโปรเจกต์ xUnit .NET 8 และเขียนเทสต์ 7 รายการผ่านการรัน 100%<br>👤 @โชว์ 🏷️ #SP/4 #Tech/Testing

***

%% kanban:settings
```
{"kanban-plugin":"basic","tag-colors":[{"tagKey":"#Priority/Critical","color":"rgba(235, 60, 60, 0.45)"},{"tagKey":"#Priority/High","color":"rgba(245, 140, 50, 0.45)"},{"tagKey":"#Priority/Medium","color":"rgba(60, 150, 235, 0.45)"},{"tagKey":"#Priority/Low","color":"rgba(140, 140, 150, 0.3)"},{"tagKey":"#Status/Review","color":"rgba(240, 210, 60, 0.45)"},{"tagKey":"#Milestone/MVP","color":"rgba(70, 200, 120, 0.45)"}]}
```
%%
