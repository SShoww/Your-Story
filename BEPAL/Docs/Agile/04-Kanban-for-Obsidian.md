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

- [ ] **[T-20] Mystery Box Scene Integration**<br>ประกอบฉากรับกล่องพัสดุหน้าบ้านประจำวัน นำภาพกล่องของเดียร์และบทสนทนาของภูมิมาต่อเข้าระบบ Typewriter<br>👤 @โชว์ (ร่วมกับ @เดียร์) 🏷️ #SP/4 #Feature/Narrative #Priority/Medium
- [ ] **[T-21] Caretaker End-Day Upgrades Design**<br>ซุงออกแบบตารางบัฟตัวละครเมื่อจบวัน (ขยายช่อง QTE Hitbox, เพิ่มความทนทาน, ลดดาเมจ)<br>👤 @ซุง 🏷️ #SP/4 #Design/Progression #Priority/Medium
- [ ] **[T-22] Save / Load JSON Persistence Engine**<br>ซุงกำหนดสเปกข้อมูล โชว์พัฒนาโค้ดบันทึกความคืบหน้าของวัน สถิติสัตว์ และ Survival Log ลงไฟล์ JSON<br>👤 @โชว์ (ร่วมกับ @ซุง) 🏷️ #SP/4 #Tech/System #Priority/Low


## 📋 Ready for Development

- [ ] **[T-15] 2D Sprites: Nibbleclaw & Blinkbun (8 ท่า)**<br>เดียร์วาดสไปรต์สัตว์เลี้ยงตัวที่ 2 และ 3 ครบ 4 อารมณ์ (Idle, Happy, Angry, Attack/Teleport) ขนาด 280x360 px<br>👤 @เดียร์ 🏷️ #SP/6 #Art/Sprites #Priority/High
- [ ] **[T-16] 2D Backgrounds: Pet Room & Porch**<br>เดียร์วาดภาพฉากหลังห้องพักพิงสัตว์ (1280x720) และชานเรือนหน้าบ้านตอนเช้า ในสไตล์ Soft Cozy<br>👤 @เดียร์ 🏷️ #SP/4 #Art/Environment #Priority/High
- [ ] **[T-17] Setup BePal.Tests Unit Test Project**<br>โชว์สร้างโปรเจกต์ xUnit และเขียนเทสต์ครอบคลุม PrototypeRun, Damage, Satisfaction, Day Rollover<br>👤 @โชว์ 🏷️ #SP/4 #Tech/Testing #Priority/High
- [ ] **[T-18] Audio Manager Integration into MonoGame**<br>โชว์นำไฟล์เสียง 8 SFX ของภูมิมาเล่นในจังหวะกด Spacebar QTE, หลบพ้น, และถูกโจมตี<br>👤 @โชว์ (ร่วมกับ @ภูมิ) 🏷️ #SP/4 #Tech/Audio #Priority/Medium
- [ ] **[T-19] Modular Screens Refactoring (IScreen)**<br>โชว์แยก Screen Classes (MainMenu, Home, CareQte, DodgeQte, SurvivalLog, Summary) ออกจาก Game1.cs<br>👤 @โชว์ 🏷️ #SP/5 #Tech/Refactor #Priority/Medium


## ⏳ In Progress

- [ ] **[T-09] Pet Rules & Balance Matrix**<br>ซุงกำหนด Action Patterns และตารางตัวเลขสัตว์ 3 ชนิด (ความเร็วเข็ม 2.2 rad/s, องศา Sector 60 deg, แต้ม Pet Favor)<br>👤 @ซุง 🏷️ #SP/4 #Design/Balance #Priority/Critical
- [ ] **[T-10] Mystery Box Dialogue Script (Day 1–5)**<br>ภูมิเขียนบทสนทนาและคำบรรยายฉากเปิดกล่องพัสดุปริศนาหน้าบ้าน พร้อมวางปม Lore ยาทดลองและความลับศูนย์วิจัย<br>👤 @ภูมิ 🏷️ #SP/3 #Design/Narrative #Priority/High
- [ ] **[T-11] SFX & BGM Production & Staging**<br>ภูมิจัดหา ตัดต่อ และ Normalize ไฟล์เสียง 8 SFX และ 2 BGM ลงใน docs/02_Assets/_candidates/sfx, music<br>👤 @ภูมิ 🏷️ #SP/4 #Audio/Staging #Priority/High
- [ ] **[T-12] Mossling Dedicated Finished Sprite (4 ท่า)**<br>เดียร์วาดภาพ Mossling ตัวจริง (280x360 px) ครบ 4 อารมณ์ (Idle ตะไคร่นุ่ม, Happy ดอกไม้บาน, Angry มีหนาม, Attack สะบัดหนาม)<br>👤 @เดียร์ 🏷️ #SP/4 #Art/Sprites #Priority/High
- [ ] **[T-13] Death Spiral Scrap Badges (6 แบบ)**<br>เดียร์วาดป้ายกระดาษฉีก FEED, PLAY, PET, OBSERVE, DODGE ZONE, และ ATTACK!<br>👤 @เดียร์ 🏷️ #SP/3 #Art/UI #Priority/High
- [ ] **[T-14] Pure C# Domain Models Extraction**<br>โชว์แยกคลาส CareAction, PetDefinition, ActionPattern ออกจาก Game1 ให้เป็น Pure C# พร้อมรับค่า Balance จากซุง<br>👤 @โชว์ 🏷️ #SP/3 #Tech/Architecture #Priority/Critical


## 🔍 Review & Testing

- [ ] **[T-05] Death Spiral QTE System Implementation**<br>โชว์พัฒนาวงล้อ 4 Sectors ด้วย MonoGame.Extended, เข็มหมุน, Dodge Zone สีทอง, Floating Tags, และ Screen Shake<br>👤 @โชว์ 🏷️ #SP/5 #Gameplay/Core #Status/Review
- [ ] **[T-06] Playtest Screenshot Automation Pipeline**<br>โชว์สร้างคำสั่ง `--screenshot` สำหรับแคปภาพทุกหน้าจออัตโนมัติ และปุ่ม `F12` บันทึกภาพ PNG ทันที<br>👤 @โชว์ 🏷️ #SP/3 #Tooling/QA #Status/Review
- [ ] **[T-07] HUD Badges & Health Alignment Polish**<br>โชว์และเดียร์จัดเลย์เอาต์ป้าย HEALTH 3.0, DAY 1/5, LIVES hearts, และข้อมูลสัตว์เลี้ยงไม่ทับขอบกล่อง<br>👤 @โชว์ (ร่วมกับ @เดียร์) 🏷️ #SP/2 #UI/Polish #Status/Review
- [ ] **[T-08] Lead QA Playtest & Timing Balance Report**<br>ภูมิทดสอบฟีลลิ่งการเล่นจริงในเกม ซุงตรวจสอบค่าตัวเลขใน Balance Sheet สรุปผลปรับแต่งความเร็วเข็มและ Hit Window<br>👤 @ภูมิ (ร่วมกับ @ซุง) 🏷️ #SP/3 #QA/Balancing #Status/Review


## ✅ Done

- [x] **[T-01] MVP 5-Day Core Loop Prototype**<br>โชว์สร้างแก่นการเล่น 5 วัน, จัดการ Health 3 หน่วย, และระบบ Forced Retreat สำเร็จ<br>👤 @โชว์ 🏷️ #SP/5 #Milestone/MVP
- [x] **[T-02] Concept & GDD 00–05 Master Documentation**<br>อัปเดตเอกสาร GDD, Master Asset List, Backlogs, และ CONTEXT.md ครบถ้วนตาม Canva Board<br>👤 @ทั้งทีม 🏷️ #SP/4 #Milestone/Docs
- [x] **[T-03] ADR Architecture Decision Record**<br>บันทึกสถาปัตยกรรม docs/adr/0001-screen-and-care-qte-architecture.md<br>👤 @โชว์ (ร่วมกับ @ซุง) 🏷️ #SP/2 #Milestone/Architecture
- [x] **[T-04] Basic Survival Log Screen**<br>บันทึกและแสดงผลพฤติกรรมสัตว์เมื่อดูแลครบ 3 รอบ<br>👤 @โชว์ 🏷️ #SP/3 #Milestone/Feature

***

%% kanban:settings
```
{"kanban-plugin":"basic","tag-colors":[{"tagKey":"#Priority/Critical","color":"rgba(235, 60, 60, 0.45)"},{"tagKey":"#Priority/High","color":"rgba(245, 140, 50, 0.45)"},{"tagKey":"#Priority/Medium","color":"rgba(60, 150, 235, 0.45)"},{"tagKey":"#Priority/Low","color":"rgba(140, 140, 150, 0.3)"},{"tagKey":"#Status/Review","color":"rgba(240, 210, 60, 0.45)"},{"tagKey":"#Milestone/MVP","color":"rgba(70, 200, 120, 0.45)"}]}
```
%%
