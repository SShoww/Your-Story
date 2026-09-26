---

kanban-plugin: basic

---

# BePal — Interactive Kanban Board (Obsidian v2.0)

> 💡 **Obsidian Kanban Board:** ไฟล์นี้รองรับการแสดงผลแบบการ์ดลากวาง (Interactive Cards) ผ่าน Community Plugin **Kanban** ของ Obsidian
> - **Lead Programmer:** วศิน (โชว์) — `@โชว์`
> - **Flex:** ปีย์ตะวัน (ซุง) — `@ซุง`
> - **Game Designer & UI Lead:** ภูมิพัฒน์ (ภูมิ / Pooh) — `@ภูมิ`
> - **2D Art:** ธัญญรัตน์ (เดียร์) — `@เดียร์`

## 🗄️ Backlog

- [ ] **[T-15] Toothless Sprite Sheet (4 ท่า)**<br>เดียร์วาดสไปรต์ Toothless ครบ 4 ท่า (Idle, Angry, Attack, Tamed) ขนาด 280×360 px<br>👤 @เดียร์ 🏷️ #SP/3 #Art/Sprites #Priority/Critical
- [ ] **[T-17] Core Sound Effects Production (9 SFX)**<br>ซุงตัดต่อและ Normalize ไฟล์เสียง SFX 9 ไฟล์ (.wav) คุณภาพคมชัด<br>👤 @ซุง 🏷️ #SP/3 #Audio/SFX #Priority/High
- [ ] **[T-18] Lead QA Playtesting & QTE Timing Calibration**<br>ซุงทดสอบฟีลลิ่งการหลบกรดและสวนกลับ ตรวจสอบความลื่นไหลของจังหวะ QTE<br>👤 @ซุง 🏷️ #SP/3 #QA/Balancing #Priority/High
- [ ] **[T-27] 8-Slot Inventory & Item Consumption Subsystem**<br>โชว์พัฒนา InventoryOverlayScreen รองรับตารางเก็บไอเทม 8 ช่อง ป๊อปอัปตรวจสอบ และปุ่ม USE / EQUIP<br>👤 @โชว์ 🏷️ #SP/5 #Tech/Gameplay #Priority/Critical
- [ ] **[T-28] Merchant 3-Phase Boss Battle Engine**<br>โชว์พัฒนาบอสไฟต์พ่อค้า 1,000 HP แบ่ง 3 เฟส (Greed's Splash, Gold Gatling, Collector's Cane) พร้อมระบบสะสมเงินทอง<br>👤 @โชว์ 🏷️ #SP/5 #Tech/Combat #Priority/Critical
- [ ] **[T-29] Emergency Revive & 500G Loan Subsystem**<br>โชว์พัฒนา EmergencyReviveModal ชำระเงิน 500G หรือเซ็นสัญญาเงินกู้ดอกเบี้ย 20% ทบต้น<br>👤 @โชว์ 🏷️ #SP/4 #Tech/Economy #Priority/High
- [ ] **[T-30] Daily Summary Report Card & JSON Save System**<br>โชว์พัฒนา DailySummaryScreen สไตล์ Papers, Please คำนวณเกรด S–F และระบบเซฟความคืบหน้า JSON<br>👤 @โชว์ 🏷️ #SP/4 #Tech/Progression #Priority/High
- [ ] **[T-31] Day 3 Traveling Merchant Lore & Buyout Script**<br>ภูมิเขียนบทสนทนาพ่อค้า The Traveling Collector ยื่นข้อเสนอซื้อ Toothless 5,000G และบทพูดเมื่อปฏิเสธ<br>👤 @ภูมิ 🏷️ #SP/4 #Design/Narrative #Priority/High
- [ ] **[T-32] Endings A & B Epilogue Narrative Scripts**<br>ภูมิเขียนบทสรุปฉากจบ Ending A (The Wealthy Betrayal) และ Ending B (Sanctuary's Protector)<br>👤 @ภูมิ 🏷️ #SP/4 #Design/Narrative #Priority/High


## 📋 Ready for Development

- [ ] **[T-20] The Traveling Collector Boss Sprites (4 ท่าบอส)**<br>เดียร์วาดสไปรต์พ่อค้า 320×400 px: Idle, Talk, Angry, Flask, Gatling, Cane, Defeat<br>👤 @เดียร์ 🏷️ #SP/4 #Art/Sprites #Priority/Critical
- [ ] **[T-21] 5-Item Shop Catalog & 3 Equipment Balance**<br>ภูมิกำหนดราคาสินค้า 5 ชนิด (Crab Apple, Sea Tea, etc.) และผลลัพธ์อุปกรณ์ 3 ชิ้น (Ballet Shoes, Toy Knife, Faded Ribbon)<br>👤 @ภูมิ 🏷️ #SP/4 #Design/Balance #Priority/High
- [ ] **[T-22] Boss Combat 3 Phases Math & Ending Conditions**<br>ภูมิคำนวณสเกลพลังชีวิตบอส 1,000 HP ดาเมจแต่ละท่า และเงื่อนไขการแยกฉากจบ A และ B<br>👤 @ภูมิ 🏷️ #SP/3 #Design/Combat #Priority/High
- [ ] **[T-23] Base Habitat Background & Environments (5 ฉาก)**<br>เดียร์วาดภาพฉากหลัง 1280×720 px: Habitat Base, Porch, Storm, Combat Arena, และ Summary Table<br>👤 @เดียร์ 🏷️ #SP/5 #Art/Environment #Priority/Critical
- [ ] **[T-24] Item Icons & UI Dialogue Frames**<br>เดียร์วาดไอคอนไอเทม 8 รูปแบบ (48×48 px) กรอบ Dialogue Box และปุ่ม Choice Prompt<br>👤 @เดียร์ 🏷️ #SP/3 #Art/UI #Priority/High
- [ ] **[T-25] Atmospheric Cozy & Boss BGM Soundtrack (5 แทร็ก)**<br>ซุงมิกซ์เพลง BGM 5 แทร็ก (Cozy, Storm, Boss, Ending A, Ending B) วนลูปไร้รอยต่อ<br>👤 @ซุง 🏷️ #SP/3 #Audio/BGM #Priority/High
- [ ] **[T-26] End-to-End 3-Day Playtesting & Release QA**<br>ซุงทดสอบเล่นลูปเต็ม 3 วัน ตรวจสอบความยากง่ายของมินิเกม และความเสถียรของเกมรอบสุดท้าย<br>👤 @ซุง 🏷️ #SP/3 #QA/Release #Priority/Critical


## ⏳ In Progress

- [ ] **[T-09] Day 2 "Knock Knock !!" Narrative Script**<br>ภูมิเขียนบทสนทนาเสียงเคาะประตู รอยกรดม่วง และทางเลือก Chase vs Tame<br>👤 @ภูมิ 🏷️ #SP/4 #Design/Narrative #Status/InProgress
- [ ] **[T-10] Combat & Taming Balance Matrix**<br>ภูมิคำนวณตัวเลขความเร็วเข็ม 3.0 rad/s ดาเมจกรด 15 HP และอัตราเพิ่ม Tame Gauge<br>👤 @ภูมิ 🏷️ #SP/3 #Design/Balance #Status/InProgress
- [ ] **[T-16] Dynamic Pet Emotional Sprites (Coco 4 ท่า)**<br>เดียร์วาดสไปรต์ Coco 4 อารมณ์ (Idle, Happy, Angry, Hurt) ขนาด 280×360 px<br>👤 @เดียร์ 🏷️ #SP/4 #Art/Sprites #Priority/High


## ✅ Done

- [x] **[T-01] Starter Pet Selection Screen**<br>โชว์สร้างหน้าต่างเลือกรับอุปการะ 1 ใน 3 สัตว์เลี้ยงเริ่มต้น (Coco, Sproutlet, Gloomtail)<br>👤 @โชว์ 🏷️ #SP/4 #Milestone/MVP
- [x] **[T-02] 10-Attempt Care QTE Engine**<br>โชว์พัฒนาเข็มหมุน 2.4 rad/s พร้อมคำนวณ Perfect (<=0.20) และ Good (<=0.45) ครบ 10 ครั้ง<br>👤 @โชว์ 🏷️ #SP/5 #Milestone/MVP
- [x] **[T-03] Pet Stats & Decay Formulas**<br>ซุงกำหนดสูตร Natural Decay (Stomach -20, Clean -15) และ Metabolic Burn (-5/-10)<br>👤 @ซุง 🏷️ #SP/6 #Milestone/MVP
- [x] **[T-04] Discrete Energy 6 AP Budget**<br>โชว์พัฒนาระบบโควตา 6 AP หักตามคำสั่ง และตัดเข้าสู่ Phase 3 เมื่อหมด<br>👤 @โชว์ 🏷️ #SP/5 #Milestone/MVP
- [x] **[T-05] Sickness & Health Penalties**<br>ซุงออกแบบบทลงโทษสถานะ Starving, Grimy, และ Infected หัก HP ข้ามวัน<br>👤 @ซุง 🏷️ #SP/5 #Milestone/MVP
- [x] **[T-06] Day 1 Thunderstorm Disaster**<br>ซุงออกแบบมินิเกม Calming QTE 3 ครั้งเพื่อระงับความตื่นตระหนกยามพายุเข้า<br>👤 @ซุง 🏷️ #SP/4 #Milestone/MVP
- [x] **[T-07] Pure C# Domain Models**<br>โชว์แยกโมเดล PetEntity, PetStats, EnergyAccount ออกจาก MonoGame<br>👤 @โชว์ 🏷️ #SP/3 #Milestone/MVP
- [x] **[T-08] Automated BePal.Tests Suite**<br>โชว์สร้างชุดทดสอบ xUnit ครอบคลุมการคำนวณความเสียหายและการใช้พลังงาน<br>👤 @โชว์ 🏷️ #SP/4 #Milestone/MVP
- [x] **[T-11] Screen Hierarchy & State Transitions**<br>โชว์สร้าง IScreen และ ScreenManager รองรับการเปลี่ยนฉากแบบ StripeWipeTransition<br>👤 @โชว์ 🏷️ #SP/3 #Tech/Architecture #Milestone/Core
- [x] **[T-12] Content Pipeline & Automated Screenshot Harness**<br>โชว์พัฒนาระบบรัน Headless Screenshot Runner (--screenshot) แคปภาพ 27 เฟรม<br>👤 @โชว์ 🏷️ #SP/2 #Tooling/QA #Milestone/Core
- [x] **[T-13] Toothless Combat Taming Arena & QTE**<br>โชว์พัฒนา CombatArenaScreen แสดงหลอด Tame Gauge 0–100% และรับมือคลื่นการโจมตี Acid Spit<br>👤 @โชว์ 🏷️ #SP/5 #Tech/Combat #Priority/Critical
- [x] **[T-14] Acid Spit Dodge Zone & Counter-Attack**<br>โชว์สร้างระบบหลบกรดใน Dodge Zone สีทอง และสวนกลับแบบ Shrinking Ring ภายใน 0.3s<br>👤 @โชว์ 🏷️ #SP/5 #Tech/Combat #Priority/Critical
- [x] **[T-19] MonoGame Audio Engine Integration**<br>โชว์เชื่อมต่อระบบเล่นเสียง SoundEffectInstance ในหน้า Care QTE และฉากต่อสู้<br>👤 @โชว์ 🏷️ #SP/3 #Tech/Audio #Priority/Medium



%% kanban:settings
```json
{
  "kanban-plugin": "basic",
  "hide-tags": false,
  "tag-colors": [
    { "tagKey": "#Priority/Critical", "color": "#E53935" },
    { "tagKey": "#Priority/High", "color": "#FB8C00" },
    { "tagKey": "#Priority/Medium", "color": "#FDD835" },
    { "tagKey": "#Milestone/MVP", "color": "#43A047" },
    { "tagKey": "#Status/Review", "color": "#1E88E5" }
  ]
}
```
%%
