---
type: asset-list
version: 2.0
date: 2026-09-20
project: BePal
---

# Asset List — BePal (Master Asset & Image Checklist v2.0)

เอกสารรวบรวมรายการ Asset ทั้งหมดของโปรเจกต์ BePal (v2.0) โดยเฉพาะ **รายการภาพ 2D ทั้งหมดที่ ธัญญรัตน์ (เดียร์) รับผิดชอบวาด**, **รายการเสียง SFX & BGM สำหรับ ปีย์ตะวัน (ซุง)**, และ**การออกแบบเกม ระบบ UI และฟอนต์สำหรับ ภูมิพัฒน์ (ภูมิ)** เพื่อรองรับลูปการเล่น 3 วันแบบสมบูรณ์

---

## 🎨 1. รายการภาพ 2D ทั้งหมดสำหรับ เดียร์ (2D Art)

### 1.1 สไปรต์สัตว์เลี้ยง (Abnormal Pets — 280×360 px, PNG Transparent)
สัตว์เลี้ยงแต่ละสายพันธุ์มีเอกลักษณ์ทางสรีระที่ "น่ารักแต่แอบผิดปกติ (Uncanny)" วาดให้ครบอารมณ์และท่าทางหลัก:

| รหัส Asset | ชื่อไฟล์ | คำอธิบาย & รายละเอียดทางภาพ | ขนาด | สถานะ |
| --- | --- | --- | --- | --- |
| **PET-01** | `pet/spr_coco_idle.png` | **Coco (Idle):** พืชชีวภาพตัวนุ่ม ตะไคร่น้ำฟู ดวงตากลมโตคู่โตจ้องมองผู้เล่น | 280×360 | 🔄 In Progress |
| **PET-02** | `pet/spr_coco_happy.png` | **Coco (Happy):** ดอกไม้ตูมบนหัวผลิบาน แก้มอมชมพู แสดงความพึงพอใจ | 280×360 | 🔄 In Progress |
| **PET-03** | `pet/spr_coco_angry.png` | **Coco (Angry):** ตะไคร่แห้งกรอบ มีหนามแหลมงอก ดวงตาหดเล็กลงอย่างระแวง | 280×360 | 🔄 In Progress |
| **PET-04** | `pet/spr_coco_hurt.png` | **Coco (Hurt):** ตัวลีบแบน กลีบดอกไม้ร่วงหล่นเมื่อโดนดาเมจหรือป่วย | 280×360 | 🔄 In Progress |
| **PET-05** | `pet/spr_sproutlet_idle.png` | **Sproutlet (Idle):** สิ่งมีชีวิตกึ่งสัตว์เลื้อยคลานสีเขียวสด หางงอกต้นอ่อน คล่องแคล่ว | 280×360 | 🔲 Not Started |
| **PET-06** | `pet/spr_sproutlet_happy.png`| **Sproutlet (Happy):** กระดิกหางต้นอ่อน กระโดดโลดเต้นอย่างร่าเริง | 280×360 | 🔲 Not Started |
| **PET-07** | `pet/spr_sproutlet_angry.png`| **Sproutlet (Angry):** ลำตัวพองออก ขู่ฟ่อ ใบไม้บนหางชี้ชันเป็นใบมีด | 280×360 | 🔲 Not Started |
| **PET-08** | `pet/spr_sproutlet_hurt.png` | **Sproutlet (Hurt):** นอนขดตัว ใบไม้เหี่ยวเฉาเมื่อได้รับบาดเจ็บ | 280×360 | 🔲 Not Started |
| **PET-09** | `pet/spr_gloomtail_idle.png` | **Gloomtail (Idle):** สัตว์แห่งเงามืด ขนสีม่วงเข้ม มีดวงตาสามดวงเปล่งประกาย | 280×360 | 🔲 Not Started |
| **PET-10** | `pet/spr_gloomtail_happy.png`| **Gloomtail (Happy):** หูทั้งสองลู่ลง ดวงตาปิดสนิท ยิ้มอย่างสงบ | 280×360 | 🔲 Not Started |
| **PET-11** | `pet/spr_gloomtail_angry.png`| **Gloomtail (Angry):** ตาที่สามเบิกกว้าง ปล่อยควันหมอกสีดำทมิฬรอบตัว | 280×360 | 🔲 Not Started |
| **PET-12** | `pet/spr_gloomtail_teleport.png`| **Gloomtail (Teleport):** เงาร่างโปร่งแสงแตกเป็นภาพซ้อน ใช้ตอนเคลื่อนไหวพริบตา | 280×360 | 🔲 Not Started |
| **PET-13** | `pet/spr_toothless_idle.png`| **Toothless (Idle):** สัตว์เลื้อยคลานสีดำทมิฬ ผิวลื่นไร้ฟัน ถุงกรดสีม่วงที่ลำคอ | 280×360 | 🔲 Not Started |
| **PET-14** | `pet/spr_toothless_angry.png`| **Toothless (Angry):** อ้าปากกว้าง ถุงกรดพองโต น้ำกรดเดือดเป็นฟองฟู่ | 280×360 | 🔲 Not Started |
| **PET-15** | `pet/spr_toothless_attack.png`| **Toothless (Attack):** ท่าสะบัดหางหรือพ่นกรดพิษในฉากต่อสู้ | 280×360 | 🔲 Not Started |
| **PET-16** | `pet/spr_toothless_tamed.png` | **Toothless (Tamed):** หมอบลงอย่างว่าง่าย เอาหัวคลอเคลียผู้เล่น | 280×360 | 🔲 Not Started |

---

### 1.2 สไปรต์ NPC & ศัตรู (NPC & Enemy Sprites — 320×400 px)

| รหัส Asset | ชื่อไฟล์ | คำอธิบาย & ท่าทาง | ขนาด | สถานะ |
| --- | --- | --- | --- | --- |
| **NPC-01** | `npc/spr_merchant_idle.png` | **The Traveling Collector (Idle):** ชายร่างสูงในชุดคลุมทมิฬ หมวกปีกกว้าง รอยยิ้มฟันทอง | 320×400 | 🔲 Not Started |
| **NPC-02** | `npc/spr_merchant_talk.png` | **The Traveling Collector (Talk):** ยื่นมือสวมถุงมือหนังออกมาข้างหน้าในจังหวะยื่นข้อเสนอ 5,000G | 320×400 | 🔲 Not Started |
| **NPC-03** | `npc/spr_merchant_angry.png` | **The Traveling Collector (Angry):** หน้าตามืดครึ้ม แววตาสีแดงก่ำ ชักดาบซ่อนในไม้เท้า | 320×400 | 🔲 Not Started |
| **NPC-04** | `npc/spr_merchant_flask.png` | **Boss Attack Phase 1:** ท่าเหวี่ยงขวดกรดแก้ว 3 ขวด | 320×400 | 🔲 Not Started |
| **NPC-05** | `npc/spr_merchant_gatling.png`| **Boss Attack Phase 2:** เปิดกลไกกระเป๋า ยิงห่ากระสุนเหรียญทองคำ | 320×400 | 🔲 Not Started |
| **NPC-06** | `npc/spr_merchant_cane.png` | **Boss Attack Phase 3:** ท่ากระหน่ำฟาดไม้เท้าหัวกะโหลกพร้อมภาพลวงตา | 320×400 | 🔲 Not Started |
| **NPC-07** | `npc/spr_merchant_defeat.png` | **Boss Defeated:** เสื้อผ้าขาดรุ่งริ่ง ทรุดตัวคุกเข่าลงกับพื้น ยอมจำนน | 320×400 | 🔲 Not Started |

---

### 1.3 ฉากหลังและบรรยากาศ (Backgrounds & Scenes — 1280×720 px)

| รหัส Asset | ชื่อไฟล์ | คำอธิบาย & บรรยากาศ | ขนาด | สถานะ |
| --- | --- | --- | --- | --- |
| **BG-01** | `bg/bg_title_screen.png` | **Title Screen:** หน้าไตเติลศูนย์พักพิงยามค่ำคืน แสงไฟสลัว อบอุ่นแต่ดูลึกลับ | 1280×720 | 🔲 Not Started |
| **BG-02** | `bg/bg_habitat_base.png` | **Habitat Base Room:** ห้องดูแลสัตว์เลี้ยง โต๊ะวางอุปกรณ์ เบาะนอน ชามข้าว แสงแดดส่อง | 1280×720 | 🔲 Not Started |
| **BG-03** | `bg/bg_thunderstorm.png` | **Thunderstorm Event:** ห้องดูแลสัตว์ยามพายุเข้า ท้องฟ้าภายนอกมืดมิด ลมพัดรอยแยกหลังคา | 1280×720 | 🔲 Not Started |
| **BG-04** | `bg/bg_porch_morning.png` | **ชานเรือนหน้าประตู (Porch):** ฉากพบรอยกรดและรับพัสดุยามเช้า | 1280×720 | 🔲 Not Started |
| **BG-05** | `bg/bg_combat_arena.png` | **ลานต่อสู้หน้าบ้าน (Combat Arena):** ลานดินหน้าศูนย์พักพิง บรรยากาศตึงเครียด | 1280×720 | 🔲 Not Started |
| **BG-06** | `bg/bg_merchant_shop.png` | **เกวียนร้านค้าพ่อค้าเร่:** แผงขายของลึกลับ แขวนขวดแก้ว โซ่ตรวน และกรงขัง | 1280×720 | 🔲 Not Started |
| **BG-07** | `bg/bg_daily_summary.png` | **โต๊ะทำงานสรุปผล (Daily Summary):** โต๊ะไม้พร้อมโคมไฟส่องใบรายงานสไตล์ Papers, Please | 1280×720 | 🔲 Not Started |

---

### 1.4 ป้ายแอ็กชันวงล้อ QTE (Action & Timing Badges)

| รหัส Asset | ชื่อไฟล์ | คำอธิบาย & สีหลัก | ขนาด | สถานะ |
| --- | --- | --- | --- | --- |
| **UI-01** | `ui/badges/badge_feed.png` | ป้าย **FEED** — ขอบสีเขียวมรกต (`#48CD82`) | 128×52 | 🔲 Not Started |
| **UI-02** | `ui/badges/badge_clean.png` | ป้าย **CLEAN** — ขอบสีฟ้าสดใส (`#41AAF5`) | 128×52 | 🔲 Not Started |
| **UI-03** | `ui/badges/badge_train.png` | ป้าย **TRAIN** — ขอบสีส้มอำพัน (`#F5A741`) | 128×52 | 🔲 Not Started |
| **UI-04** | `ui/badges/badge_heal.png` | ป้าย **HEAL** — ขอบสีแดงกุหลาบ (`#F54868`) | 128×52 | 🔲 Not Started |
| **UI-05** | `ui/badges/badge_dodge.png` | ป้าย **DODGE ZONE** — สีทองสว่าง (`#FFD241`) | 156×52 | 🔲 Not Started |
| **UI-06** | `ui/badges/badge_counter.png` | ป้าย **COUNTER!** — สีเขียวสะท้อนแสง (`#52FF78`) | 156×52 | 🔲 Not Started |

---

### 1.5 ไอคอนสถานะและองค์ประกอบ HUD (HUD & Status Icons)

| รหัส Asset | ชื่อไฟล์ | คำอธิบาย | ขนาด | สถานะ |
| --- | --- | --- | --- | --- |
| **HUD-01** | `ui/hud/icon_energy_pip.png` | ดวงแก้วพลังงาน AP (เขียวสว่างเมื่อมีแต้ม / มืดดับเมื่อใช้แล้ว) | 32×32 | 🔲 Not Started |
| **HUD-02** | `ui/hud/icon_gold_coin.png` | เหรียญทองคำเปล่งประกายประกายแสง | 32×32 | 🔲 Not Started |
| **HUD-03** | `ui/hud/bar_hp_fill.png` | แถบพลังชีวิต Health Bar สีแดงทับทิม | 240×24 | 🔲 Not Started |
| **HUD-04** | `ui/hud/bar_stomach_fill.png` | แถบความอิ่ม Stomach Bar สีส้ม | 240×24 | 🔲 Not Started |
| **HUD-05** | `ui/hud/bar_clean_fill.png` | แถบความสะอาด Clean Bar สีฟ้า | 240×24 | 🔲 Not Started |
| **HUD-06** | `ui/hud/badge_level_frame.png`| กรอบเหรียญตราแสดงเลเวลปัจจุบัน Lv. 1–10 | 48×48 | 🔲 Not Started |

---

### 1.6 ไอเทมและอุปกรณ์ (Items & Equipment Icons — 48×48 px)

| รหัส Asset | ชื่อไฟล์ | คำอธิบาย & การใช้งาน | ขนาด | สถานะ |
| --- | --- | --- | --- | --- |
| **ITM-01** | `ui/items/item_crab_apple.png` | **Crab Apple:** แอปเปิลทะเลสีแดงสด ฟื้นฟู 18 HP / 20 Stomach | 48×48 | 🔲 Not Started |
| **ITM-02** | `ui/items/item_sea_tea.png` | **Sea Tea:** ชาน้ำทะเลสีฟ้าใส ขยาย Dodge Zone 20% | 48×48 | 🔲 Not Started |
| **ITM-03** | `ui/items/item_cloudy_glasses.png`| **Cloudy Glasses:** แว่นตากรอบเงิน มอบโล่ป้องกัน 2 ครั้ง | 48×48 | 🔲 Not Started |
| **ITM-04** | `ui/items/item_torn_notebook.png` | **Torn Notebook:** สมุดวิจัยขาดวิ่น บัฟ EXP Train +50% | 48×48 | 🔲 Not Started |
| **ITM-05** | `ui/items/item_caffeine_tonic.png`| **Caffeine Tonic:** ยาชูกำลังสีอำพัน ฟื้นฟู 2 AP ทันที | 48×48 | 🔲 Not Started |
| **EQP-01** | `ui/items/eqp_ballet_shoes.png` | **Ballet Shoes:** รองเท้าบัลเลต์สีชมพู ขยาย Perfect Zone +15% | 48×48 | 🔲 Not Started |
| **EQP-02** | `ui/items/eqp_toy_knife.png` | **Toy Knife:** มีดพลาสติกของเล่น เพิ่มพลังโจมตีสวนกลับ +35% | 48×48 | 🔲 Not Started |
| **EQP-03** | `ui/items/eqp_faded_ribbon.png` | **Faded Ribbon:** ริบบิ้นสีซีด ลดอัตราความสกปรก Clean Decay 30% | 48×48 | 🔲 Not Started |

---

### 1.7 กล่องข้อความและบทสนทนา (Dialogue & Choice Modals)

| รหัส Asset | ชื่อไฟล์ | คำอธิบาย | ขนาด | สถานะ |
| --- | --- | --- | --- | --- |
| **UI-07** | `ui/dialogue/frame_textbox.png` | กรอบกล่องข้อความสไตล์โปร่งแสง ขอบไม้โมเดิร์น | 1160×180 | 🔲 Not Started |
| **UI-08** | `ui/dialogue/btn_choice_prompt.png`| กรอบปุ่มกดเลือกคำตอบบทสนทนา `[ YES ]` / `[ NO ]` | 240×64 | 🔲 Not Started |
| **UI-09** | `ui/dialogue/btn_next_arrow.png` | ลูกศรกะพริบแจ้งเตือนคลิกเพื่อไปต่อ | 32×32 | 🔲 Not Started |

---

## 🔊 2. รายการเสียง SFX & BGM สำหรับ ซุง (Flex)

### 2.1 Sound Effects (SFX — WAV 44.1kHz 16-bit PCM Stereo)

| รหัส Audio | ชื่อไฟล์ | บริบทการเล่น | คำอธิบายอารมณ์เสียง |
| --- | --- | --- | --- |
| **SFX-01** | `sfx/sfx_typewriter_key.wav` | เล่นเมื่อตัวอักษรขึ้นใน DialogueBox | เสียงแป้นพิมพ์ดีดกลไก กระชับ นุ่มนวล |
| **SFX-02** | `sfx/sfx_qte_perfect.wav` | กด Spacebar โดน Perfect Zone | เสียงปิ๊งแก้วใสคมชัด ดังก้องน่าพึงพอใจ |
| **SFX-03** | `sfx/sfx_qte_good.wav` | กด Spacebar โดน Good Zone | เสียงเคาะไม้เสียงทุ้มปานกลาง แสดงผลสำเร็จ |
| **SFX-04** | `sfx/sfx_qte_miss.wav` | กด Spacebar หลุดออกนอกเป้าหมาย | เสียงหึ่งทึบ (Buzzer) สั้นๆ แสดงความผิดพลาด |
| **SFX-05** | `sfx/sfx_dodge_success.wav` | กดหลบพ้นการโจมตีใน Dodge Zone | เสียงลมพัดวูบ (Whoosh) อย่างรวดเร็ว |
| **SFX-06** | `sfx/sfx_counter_hit.wav` | กด Counter-Attack โดนเป้าหมาย | เสียงกระทบโลหะสะท้อนดังกังวาน (Parry Clash) |
| **SFX-07** | `sfx/sfx_acid_spit.wav` | Toothless หรือบอสสาดกรด | เสียงของเหลวเดือดปะทุและสาดกระเซ็น (Sizzle Splat) |
| **SFX-08** | `sfx/sfx_coin_gatling.wav` | บอสยิงปืนกลเหรียญทอง Phase 2 | เสียงเหรียญทองพุ่งกระทบรัวเร็วต่อเนื่อง |
| **SFX-09** | `sfx/sfx_cane_strike.wav` | บอสฟาดไม้เท้า Phase 3 | เสียงไม้เนื้อแข็งฟาดกระทบพื้นอย่างหนักหน่วง |

### 2.2 Background Music (BGM — MP3/OGG Looping)

| รหัส Audio | ชื่อไฟล์ | บริบทของฉาก | สไตล์และอารมณ์ดนตรี |
| --- | --- | --- | --- |
| **BGM-01** | `music/bgm_shelter_cozy.mp3` | หน้า Habitat Base Room ตอนกลางวัน | อะคูสติกกีตาร์ผสมเปียโนไฟฟ้า นุ่มนวล สบายใจ สไตล์ Lo-Fi |
| **BGM-02** | `music/bgm_thunderstorm.mp3` | เหตุการณ์พายุฝน Day 1 | ดนตรีตึงเครียด แทรกเสียงฟ้าร้องคำรามและฝนตกหนัก |
| **BGM-03** | `music/bgm_boss_merchant.mp3` | ฉากประลองบอสพ่อค้าเร่ Day 3 | Up-tempo Chiptune Synthwave ผสมกลองรัวเร็ว ระทึกขวัญ |
| **BGM-04** | `music/bgm_ending_bittersweet.mp3`| ฉากจบ Ending A (ขาย Toothless) | เมโลดี้เปียโนเดี่ยว เงียบเหงา ว้าเหว่ |
| **BGM-05** | `music/bgm_ending_heroic.mp3` | ฉากจบ Ending B (ปกป้องบ้านสำเร็จ) | ดนตรีออร์เคสตราอบอุ่น เปี่ยมด้วยความหวังและมิตรภาพ |

---

## 🔤 3. รายการฟอนต์และ UI Specs สำหรับ ภูมิ (Game Designer & UI Lead)

| รหัส Font | ชื่อไฟล์ | บริบทการใช้งาน | สไตล์ฟอนต์ |
| --- | --- | --- | --- |
| **FNT-01** | `fnt_title_display.ttf` | ชื่อเกมไตเติล, หัวข้อบอสไฟต์, คำประกาศเกรด | ตัวหนา Serif กึ่งลึกลับ มีเอกลักษณ์สูง |
| **FNT-02** | `fnt_ui_cozy.ttf` | กล่องข้อความบทสนทนา, คำอธิบายไอเทม, HUD | Sans-serif มน อบอุ่น อ่านง่ายสบายตา |

---

## 🔄 4. Asset Pipeline & Ingestion Flow

```mermaid
flowchart LR
    A[Staging Candidates<br>docs/02_Assets/_candidates/] --> B[Asset Review by Lead & Designer]
    B --> C[MonoGame Content Builder<br>Content.mgcb Processing]
    C --> D[Compiled XNB / Raw Assets<br>CoPoject/BePal/Content/]
    D --> E[Runtime Ingestion<br>ContentManager.Load<T>()]
```
