---
type: asset-list
version: 2.0
date: 2026-09-11
project: BePal
---

# Asset List — BePal (Master Asset & Image Checklist)

เอกสารรวบรวมรายการ Asset ทั้งหมดของโปรเจกต์ BePal โดยเฉพาะ **รายการภาพ 2D ทั้งหมดที่ ธัญญรัตน์ (เดียร์) รับผิดชอบวาด** ตามธีม "Cozy yet Dangerous" และสไตล์ *Death Spiral* พร้อมรายการเสียง (Audio) สำหรับ ภูมิ (Pooh) และฟอนต์สำหรับ ซุง (Zunk)

---

## 🎨 1. รายการภาพ 2D ทั้งหมดสำหรับ เดียร์ (2D Art & UI Checklist)

### 1.1 สไปรต์สัตว์เลี้ยง (Abnormal Pets — 280×360 px, PNG Transparent)
สัตว์เลี้ยงแต่ละสายพันธุ์ต้องมีเอกลักษณ์ทางสรีระที่ "น่ารักแต่แอบผิดปกติ (Uncanny)" วาดให้ครบ 4 อารมณ์หลัก:

| รหัส Asset | ชื่อไฟล์ | คำอธิบาย & รายละเอียดทางภาพ | ขนาด | สถานะ |
| --- | --- | --- | --- | --- |
| **PET-01** | `pet/spr_mossling_idle.png` | **Mossling (Idle):** สิ่งมีชีวิตกึ่งพืช นุ่มฟูคล้ายตะไคร่น้ำ ดวงตากลมโตคู่โตจ้องมองผู้เล่น | 280×360 | 🔲 Not Started |
| **PET-02** | `pet/spr_mossling_happy.png` | **Mossling (Happy):** ดอกไม้ตูมบนหัวผลิบาน แก้มอมชมพู แสดงความพึงพอใจ | 280×360 | 🔲 Not Started |
| **PET-03** | `pet/spr_mossling_angry.png` | **Mossling (Angry):** ตะไคร่แห้งกรอบ มีหนามแหลมงอก ดวงตาหดเล็กลงอย่างระแวง | 280×360 | 🔲 Not Started |
| **PET-04** | `pet/spr_mossling_attack.png` | **Mossling (Attack):** สัตว์สะบัดหนามหรือปล่อยละอองสปอร์พิษจู่โจมตอน Dodge QTE | 280×360 | 🔲 Not Started |
| **PET-05** | `pet/spr_nibbleclaw_idle.png` | **Nibbleclaw (Idle):** สิ่งมีชีวิตคล้ายแมวผสมตัวกินมด มีกรงเล็บแหลมยาวซ่อนใต้ขน | 280×360 | 🔲 Not Started |
| **PET-06** | `pet/spr_nibbleclaw_happy.png` | **Nibbleclaw (Happy):** นอนหงายพุง คาบของเล่นเคี้ยวอย่างเชื่องช้า | 280×360 | 🔲 Not Started |
| **PET-07** | `pet/spr_nibbleclaw_angry.png` | **Nibbleclaw (Angry):** กางกรงเล็บแหลม ขนพอง แยกเขี้ยว ดวงตาเปลี่ยนเป็นสีแดงก่ำ | 280×360 | 🔲 Not Started |
| **PET-08** | `pet/spr_nibbleclaw_attack.png` | **Nibbleclaw (Attack):** ท่าพุ่งตะปบด้วยกรงเล็บคม บังคับเข้าสู่ Dodge QTE | 280×360 | 🔲 Not Started |
| **PET-09** | `pet/spr_blinkbun_idle.png` | **Blinkbun (Idle):** กระต่ายหูยาวขนสีม่วงอ่อน มี "ดวงตาที่สาม" ตรงกลางหน้าผาก | 280×360 | 🔲 Not Started |
| **PET-10** | `pet/spr_blinkbun_happy.png` | **Blinkbun (Happy):** หูทั้งสองลู่ลง ตาที่สามปิดสนิท ยิ้มอย่างสงบ | 280×360 | 🔲 Not Started |
| **PET-11** | `pet/spr_blinkbun_angry.png` | **Blinkbun (Angry):** หูตั้งชัน ตาที่สามเบิกกว้างปล่อยคลื่นแสงรบกวนสมาธิ | 280×360 | 🔲 Not Started |
| **PET-12** | `pet/spr_blinkbun_teleport.png` | **Blinkbun (Teleport):** ตัวกระต่ายโปร่งแสง/แตกเป็นภาพซ้อน ใช้ตอนเข็มวาร์ปตำแหน่ง | 280×360 | 🔲 Not Started |

---

### 1.2 ฉากหลังและบรรยากาศ (Backgrounds & Scenes — 1280×720 px)
เน้นโทนสีอบอุ่น Cozy (ครีม, ไม้อ่อน, เขียวมิ้นต์, แสงโคมไฟสีส้ม) ขัดแย้งกับความอันตรายของสัตว์:

| รหัส Asset | ชื่อไฟล์ | คำอธิบาย & บรรยากาศ | ขนาด | สถานะ |
| --- | --- | --- | --- | --- |
| **BG-01A** | `bg/bg_wall1_pet.png` | **Wall 1 (Pet Zone):** ผนังโซนสัตว์เลี้ยง มีคอนโดแมว เบาะนอน ถาดรอง และพื้นที่สัตว์พักผ่อน | 1280×720 | 🔲 Not Started |
| **BG-01B** | `bg/bg_wall2_pantry.png` | **Wall 2 (Prep & Pantry):** ผนังครัว/ชั้นเตรียมอาหาร มีชั้นวางขวดอาหาร ถังขยะ อ่างล้าง | 1280×720 | 🔲 Not Started |
| **BG-01C** | `bg/bg_wall3_desk.png` | **Wall 3 (Study Desk):** ผนังโต๊ะทำงานวิจัย มีโคมไฟ สมุด Survival Log กระดานแปะโน้ต | 1280×720 | 🔲 Not Started |
| **BG-01D** | `bg/bg_wall4_door.png` | **Wall 4 (Front Door):** ผนังประตูหน้าร้าน มีหน้าต่างมองเห็นข้างนอก ปฏิทิน/นาฬิกาจบวัน | 1280×720 | 🔲 Not Started |
| **BG-02** | `bg/bg_prologue_intro.png` | **ฉากแนะนำตัวละคร (Prologue):** บรรยากาศหน้าร้านเปิดใหม่พร้อมตัวละครโบกมือต้อนรับ | 1280×720 | 🔲 Not Started |
| **BG-03** | `bg/bg_doorstep_morning.png` | **ชานเรือนหน้าบ้าน (Porch):** ฉากตอนเช้าสำหรับเปิดกล่องพัสดุปริศนา มีแสงแดดอ่อนๆ ยามเช้า | 1280×720 | 🔲 Not Started |
| **BG-04** | `bg/bg_daily_summary.png` | **ฉากใบรายงานประจำวัน (Daily Summary):** โต๊ะมืดพร้อมแผ่นกระดาษรายงานผลสไตล์ Papers, Please | 1280×720 | 🔲 Not Started |
| **BG-05** | `bg/bg_qte_vignette.png` | **ขอบมืดรอบจอ (Vignette Overlay):** เงาสีดำ/น้ำเงินเข้มขอบจอ เพื่อขับเน้นวงล้อ QTE กลางจอ | 1280×720 | 🔲 Not Started |
| **BG-06** | `bg/bg_danger_vignette.png` | **ขอบแดงเตือนภัย (Danger Overlay):** แถบสีแดงเลือดกระพริบเบาๆ รอบจอขณะเข้าสู่สถานะ Attack / Dodge | 1280×720 | 🔲 Not Started |
---

### 1.3 ป้ายแอ็กชันวงล้อ QTE สไตล์ Death Spiral (Scrap-Paper Badges)
ป้ายข้อความสไตล์กระดาษฉีก/ป้ายแปะ ขอบสีคมชัด ตัวหนังสือหนาอ่านง่าย:

| รหัส Asset | ชื่อไฟล์ | คำอธิบาย & สีหลัก | ขนาด | สถานะ |
| --- | --- | --- | --- | --- |
| **UI-01** | `ui/badges/badge_feed.png` | ป้ายกระดาษฉีก **FEED** (Appetite) — ขอบสีเขียวมรกต (`#48CD82`) | 128×52 | 🔲 Not Started |
| **UI-02** | `ui/badges/badge_play.png` | ป้ายกระดาษฉีก **PLAY** (Recreation) — ขอบสีฟ้าคราม (`#41AAF5`) | 128×52 | 🔲 Not Started |
| **UI-03** | `ui/badges/badge_pet.png` | ป้ายกระดาษฉีก **PET** (Intimacy) — ขอบสีชมพูกุหลาบ (`#F573A5`) | 128×52 | 🔲 Not Started |
| **UI-04** | `ui/badges/badge_observe.png` | ป้ายกระดาษฉีก **OBSERVE** (Observation) — ขอบสีม่วงลาเวนเดอร์ (`#B47DF5`) | 128×52 | 🔲 Not Started |
| **UI-05** | `ui/badges/badge_dodge_zone.png` | ป้ายกระดาษฉีก **DODGE ZONE** (Space to Evade) — สีทองสว่าง (`#FFD241`) | 156×52 | 🔲 Not Started |
| **UI-06** | `ui/badges/badge_attack_alert.png` | ป้ายเตือนภัยฉุกเฉิน **ATTACK!** — พื้นสีแดงเลือด ขอบขาว ตัวหนาเด่นชัด | 160×56 | 🔲 Not Started |

---

### 1.4 ไอคอนสถานะและองค์ประกอบ HUD (HUD & Status Icons)

| รหัส Asset | ชื่อไฟล์ | คำอธิบาย | ขนาด | สถานะ |
| --- | --- | --- | --- | --- |
| **UI-07** | `ui/hud/ic_heart_full.png` | หัวใจพลังชีวิตเต็ม (1 Health) — สไตล์ Pixel หรือ Soft Vector สีแดงชมพู | 36×36 | 🔲 Not Started |
| **UI-08** | `ui/hud/ic_heart_empty.png` | หัวใจสูญเสีย (Lost Health) — หัวใจสีเทาเข้ม รอยแตกหรือเส้นโครงร่าง | 36×36 | 🔲 Not Started |
| **UI-09** | `ui/hud/ic_energy_pip.png` | สัญลักษณ์พลังงานประจำวัน (Daily Care Energy) | 32×32 | 🔲 Not Started |
| **UI-10** | `ui/hud/ic_satisfaction_pip_on.png` | แต้มความพอใจที่ได้แล้ว (Satisfaction Pip On) — สี่เหลี่ยมหรือดวงไฟสีเขียวสว่าง | 28×28 | 🔲 Not Started |
| **UI-11** | `ui/hud/ic_satisfaction_pip_off.png`| แต้มความพอใจที่ยังไม่ได้ (Satisfaction Pip Off) — กรอบสีเทาหม่น | 28×28 | 🔲 Not Started |
| **UI-12** | `ui/hud/frame_badge_hud.png` | กรอบป้ายพื้นหลัง HUD สไตล์ Death Spiral (สำหรับใส่ Health และ Day/Pet) | 280×72 | 🔲 Not Started |

---

### 1.5 วัตถุประกอบฉากและอุปกรณ์เนื้อเรื่อง (Props & Story Items)

| รหัส Asset | ชื่อไฟล์ | คำอธิบาย | ขนาด | สถานะ |
| --- | --- | --- | --- | --- |
| **PROP-01** | `props/box_mystery_closed.png` | **กล่องพัสดุปริศนา (ปิด):** กล่องกระดาษลังปิดเทปกาว มีรูเจาะระบายอากาศข้างกล่อง | 180×180 | 🔲 Not Started |
| **PROP-02** | `props/box_mystery_shaking.png`| **กล่องพัสดุปริศนา (กำลังสั่น):** กล่องกระดาษแง้มเล็กน้อย มีแสงหรือตาแอบมองออกมา | 180×180 | 🔲 Not Started |
| **PROP-03** | `props/box_mystery_open.png` | **กล่องพัสดุปริศนา (เปิดแล้ว):** กล่องลังเปิดฝาออก ว่างเปล่า (สัตว์ออกมาแล้ว) | 180×180 | 🔲 Not Started |

---

### 1.6 สมุดบันทึกและหน้าต่างเมนู (Survival Log & Menu UI)

| รหัส Asset | ชื่อไฟล์ | คำอธิบาย | ขนาด | สถานะ |
| --- | --- | --- | --- | --- |
| **MENU-01** | `ui/book/survival_log_frame.png` | **กรอบสมุด Survival Log:** สมุดบันทึกกางออก 2 หน้า สไตล์แฟ้มวิจัย/บันทึกอบอุ่น | 1020×580 | 🔲 Not Started |
| **MENU-02** | `ui/buttons/btn_primary_normal.png` | ปุ่มมาตรฐาน (CARE, END DAY, SURVIVAL LOG) สไตล์ไม้หรือการ์ดอบอุ่น | 260×60 | 🔲 Not Started |
| **MENU-03** | `ui/buttons/btn_primary_hover.png` | ปุ่มมาตรฐานสถานะ Hover (เรืองแสงขอบสีทอง) | 260×60 | 🔲 Not Started |
| **MENU-04** | `ui/buttons/btn_close_icon.png` | ปุ่มปิดหน้าต่างกากบาท (X) สำหรับปิด Survival Log | 44×44 | 🔲 Not Started |
### 1.7 กล่องข้อความและระบบบทสนทนา (Dialogue & Inspection UI)

| รหัส Asset | ชื่อไฟล์ | คำอธิบาย | ขนาด | สถานะ |
| --- | --- | --- | --- | --- |
| **DIA-01** | `ui/dialogue/frame_dialogue_box.png` | กรอบกล่องข้อความด้านล่าง สไตล์การ์ด Cozy ขอบมน รองรับ Typewriter text | 980×160 | 🔲 Not Started |
| **DIA-02** | `ui/dialogue/btn_dialogue_next.png` | ปุ่มลูกศร `NEXT =>` สำหรับไปข้อความถัดไป | 110×40 | 🔲 Not Started |
| **DIA-03** | `ui/dialogue/btn_choice_yes.png` | ปุ่มตัวเลือก `[ YES ]` สำหรับยืนยันการดูแลสัตว์ | 100×40 | 🔲 Not Started |
| **DIA-04** | `ui/dialogue/btn_choice_no.png` | ปุ่มตัวเลือก `[ NO ]` สำหรับปฏิเสธ/กลับห้อง | 100×40 | 🔲 Not Started |
| **DIA-05** | `ui/nav/btn_arrow_left.png` | ปุ่มลูกศรหมุนห้องไปทางซ้าย `[◄ Left]` | 50×80 | 🔲 Not Started |
| **DIA-06** | `ui/nav/btn_arrow_right.png` | ปุ่มลูกศรหมุนห้องไปทางขวา `[Right ►]` | 50×80 | 🔲 Not Started |

---

### 1.8 อุปกรณ์มินิเกมสัมผัสจริง (Tactile Mini-Game Props)

| รหัส Asset | ชื่อไฟล์ | คำอธิบาย | ขนาด | สถานะ |
| --- | --- | --- | --- | --- |
| **MG-01** | `props/mg_feed_bowl.png` | ชามอาหารสัตว์พร้อมแถบขีด Safe Line สีเขียว | 200×160 | 🔲 Not Started |
| **MG-02** | `props/mg_feed_bottle.png` | ขวดเทสารอาหาร/ยาเหลว (Hold Space to pour) | 120×180 | 🔲 Not Started |
| **MG-03** | `props/mg_pet_hand.png` | ไอคอนเคอร์เซอร์รูปมือลูบสัมผัสขนสัตว์ | 64×64 | 🔲 Not Started |
| **MG-04** | `props/mg_play_toy.png` | ของเล่นห้อยแกว่ง (ลูกบอลหญ้า / กิ่งไม้เรืองแสง) | 90×90 | 🔲 Not Started |
| **MG-05** | `props/mg_observe_lens.png` | กรอบเลนส์แว่นขยายส่องสังเกตจุดผิดปกติ | 160×160 | 🔲 Not Started |

## 🔊 2. รายการเสียง SFX & BGM สำหรับ ภูมิ (Pooh)

| รหัส Asset | ชื่อไฟล์ | จังหวะใช้งาน & คำอธิบาย | ประเภท | สถานะ |
| --- | --- | --- | --- | --- |
| **SFX-01** | `sfx/sfx_qte_confirm.wav` | เสียงคลิกยืนยัน Spacebar ตอนเข็มหมุนอยู่ในช่อง | SFX (0.15s) | 🔲 Not Started |
| **SFX-02** | `sfx/sfx_qte_success.wav` | เสียงกระดิ่ง/คอร์ดอบอุ่นเมื่อเลือก Care Action ถูกต้อง (+1) | SFX (0.4s) | 🔲 Not Started |
| **SFX-03** | `sfx_qte_fail.wav` | เสียงทู่หรือเสียงข่วนเมื่อเลือกผิด / เสีย 1 Health | SFX (0.35s) | 🔲 Not Started |
| **SFX-04** | `sfx_qte_teleport.wav` | เสียง Whoosh มิติวาร์ปเมื่อเข็มของ Blinkbun กระโดดสุ่มมุม | SFX (0.25s) | 🔲 Not Started |
| **SFX-05** | `sfx_dodge_warning.wav` | เสียงสัญญาณเตือนภัยฉุกเฉินตอน Nibbleclaw เริ่ม Attack | SFX (0.5s) | 🔲 Not Started |
| **SFX-06** | `sfx_dodge_success.wav` | เสียง Swoosh หลบพ้นการโจมตีใน Dodge Zone | SFX (0.3s) | 🔲 Not Started |
| **SFX-07** | `sfx_session_complete.wav`| เสียง Chime ดนตรีสั้นเมื่อ Satisfaction เต็ม 3 แต้มจบ Session | SFX (0.8s) | 🔲 Not Started |
| **SFX-08** | `sfx_box_open.wav` | เสียงเปิดฝากล่องกระดาษลังรับสัตว์ใหม่หน้าบ้าน | SFX (0.4s) | 🔲 Not Started |
| **SFX-09** | `sfx_typewriter_click.wav` | เสียงแป้นพิมพ์ดีดตอนข้อความตัวอักษรวิ่งขึ้นใน DialogueBox | SFX (0.05s) | 🔲 Not Started |
| **SFX-10** | `sfx_liquid_pour.wav` | เสียงเทของเหลวลงชามอาหาร (Feed Mini-Game) | SFX (Loop) | 🔲 Not Started |
| **SFX-11** | `sfx_pet_purr.wav` | เสียงครางพอใจเมื่อลูบตัวสัตว์อย่างนุ่มนวล (Pet Mini-Game) | SFX (0.6s) | 🔲 Not Started |
| **SFX-12** | `sfx_lens_focus.wav` | เสียงเลนส์หมุนโฟกัสจับจุดผิดปกติ (Observe Mini-Game) | SFX (0.3s) | 🔲 Not Started |
| **BGM-01** | `music/bgm_shelter_cozy.mp3` | ดนตรี Acoustic / Lofi อบอุ่น ฟังสบายในห้องหลัก (Seamless Loop) | BGM (~2 min) | 🔲 Not Started |
| **BGM-02** | `music/bgm_qte_suspense.mp3` | ดนตรีตึงเครียดเบาๆ แฝงความลึกลับระหว่าง Care/Dodge QTE | BGM (~1.5 min) | 🔲 Not Started |
| **BGM-03** | `music/bgm_night_ambience.mp3` | เสียงบรรยากาศกลางคืน (เสียงลม/ฝน/นาฬิกา) ช่วงพักผ่อน | BGM (~1 min) | 🔲 Not Started |

---

## 🔤 3. รายการฟอนต์สำหรับ ซุง (Zunk)

| รหัส Asset | ชื่อไฟล์ | คำอธิบาย & วัตถุประสงค์ | แหล่งที่มา | สถานะ |
| --- | --- | --- | --- | --- |
| **FNT-01** | `PrototypeFont.spritefont` | ฟอนต์ Arial Black สำหรับตัวเลข HUD และข้อความ Prototype | System Font | ✅ Completed |
| **FNT-02** | `fnt_ui_cozy.ttf` | ฟอนต์ตัวหนังสือมน อบอุ่น สไตล์ Cozy อ่านง่ายสำหรับบทสนทนาและ UI | Google Fonts | 🔲 Not Started |
