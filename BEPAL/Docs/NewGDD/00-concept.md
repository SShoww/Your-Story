---
type: gdd-concept
version: 2.0
date: 2026-09-20
---

# BePal — Game Concept (v2)

## Elevator Pitch

**“BePal is a hardcore pet-care management simulation where domestic warmth collides with uncanny survival peril: care for abnormal creatures through rigorous time and energy budgeting, survive unexpected environmental disasters and lethal wild incursions, and confront moral dilemmas when shady merchants come knocking.”**

> **สโลแกนแนวคิด:** เกมจำลองการบริหารและดูแลสัตว์เลี้ยงผิดปกติ (Abnormal Pets) ที่ผสมผสานความน่ารักอบอุ่นของสัตว์เลี้ยง (Virtual Pet) เข้ากับความท้าทายและการเอาตัวรอดสุดขั้ว (Hardcore Survival) ทุกการตัดสินใจใช้พลังงานและเงินตรามีผลลัพธ์ถึงชีวิต ปกป้องพวกมันจากภัยพิบัติ สัตว์ป่าดุร้าย และข้อเสนออันดำมืดของพ่อค้าเร่

---

## Genre & Platform

- **Genre:** Hardcore Pet-Care Simulation | Resource Management | QTE Reflex Combat | Narrative Choice
- **Platform:** PC (Windows DesktopGL)
- **Engine:** MonoGame (.NET 8 C#)
- **Target Audience:** ผู้เล่นที่ชื่นชอบเกมดูแลสัตว์เลี้ยงสไตล์คลาสสิก (Tamagotchi / Digimon V-Pet) แต่ต้องการความตื่นเต้นระทึกขวัญ ความเสี่ยงสูง (High Stakes) และระบบการต่อสู้หลบหลีกที่ต้องอาศัยทักษะความแม่นยำ

---

## Core Pillars (เสาหลักการออกแบบ)

1. **Care × Hardcore Peril (การดูแลที่แฝงความตายรอบด้าน):**
   ไม่ใช่แค่การเลี้ยงสัตว์เพื่อความเพลิดเพลิน แต่เป็นสมรภูมิการจัดสรรทรัพยากรเพื่อความอยู่รอด สัตว์เลี้ยงมีค่าสเตตัสความต้องการพื้นฐาน (Stomach, Clean, Health) ที่ลดลงอย่างต่อเนื่อง และสามารถล้มป่วย บาดเจ็บ หรือเสียชีวิตได้จริงหากผู้เล่นละเลย

2. **Discrete Energy & Resource Economy (การบริหารพลังงานและเศรษฐกิจที่จำกัด):**
   ในแต่ละวัน ผู้เล่นได้รับโควตาพลังงานจำกัด (**6 Energy Points**) ทุกการกระทำ—ให้อาหาร ทำความสะอาด ฝึกซ้อม หรือรักษาพยาบาล—ล้วนกินพลังงาน การใช้เงิน (Gold) ซื้ออาหาร ยารักษา หรืออุปกรณ์เสริมจากร้านค้าต้องผ่านการคำนวณอย่างรอบคอบ

3. **Precision QTE Mechanics (ความแม่นยำในการตอบสนอง):**
   การดูแลทุกหมวดหมู่และการเผชิญหน้าในสถานการณ์ต่อสู้ควบคุมผ่านระบบ Quick Time Events (QTE) 10 ครั้งต่อรอบ ที่มีทั้ง **Perfect Zone** ($\pm 0.20 \text{ rad}$) และ **Good Zone** ($\pm 0.45 \text{ rad}$) รวมถึงระบบหลบหลีกการโจมตี (Dodge QTE) และการสวนกลับ (Counter-Attack)

4. **Consequential Moral Dilemmas (ทางเลือกและผลลัพธ์เชิงจริยธรรม):**
   เนื้อเรื่องขับเคลื่อนด้วยทางเลือกที่มีน้ำหนักจริง เช่น การตัดสินใจว่าจะยอมเสี่ยงชีวิตฝึกสัตว์ป่าดุร้าย หรือขับไล่มันไป และการเลือกว่าจะยอมขายสัตว์เลี้ยงที่ร่วมทุกข์ร่วมสุขมาเพื่อเงินก้อนโต หรือยืนหยัดปกป้องพวกมันจนนำไปสู่การปะทะกับบอส

---

## Setting & Worldbuilding (ฉากและบรรยากาศ)

### The Setting: Habitat Base Room (ห้องดูแลสัตว์เลี้ยง)
ศูนย์ดูแลสัตว์เลี้ยงขนาดกะทัดรัดที่ดัดแปลงเป็นพื้นที่อยู่อาศัยที่ปลอดภัย (Sanctuary) ภายในมีอุปกรณ์อำนวยความสะดวกครบครัน:
- **มุมพักผ่อนและให้อาหาร:** เบาะนอน ชามข้าว และแท่นสังเกตการณ์
- **คอนโซลบริหารจัดการ (HUD Console):** แสดงสถานะค่าพลังงาน (Energy), วันที่ (Day), จำนวนเงิน (Gold), และเลเวลของสัตว์เลี้ยง
- **ชั้นวางอุปกรณ์และตู้ยา:** จัดเก็บไอเทม อาหาร และอุปกรณ์สวมใส่
- **ประตูทางเข้า:** จุดเชื่อมต่อสู่โลกภายนอก ที่ซึ่งพัสดุ พ่อค้าเร่ หรือสิ่งมีชีวิตจรจัดจะแวะเวียนมา

### Protagonist: The Sanctuarist (ผู้ดูแลสถานพักพิง)
ผู้เล่นสวมบทบาทเป็นผู้ดูแลที่มีความมุ่งมั่นในการให้ที่พักพิงแก่สัตว์ประหลาดที่ไม่สามารถปรับตัวเข้ากับระบบนิเวศทั่วไปได้ ต้องรับมือทั้งปัญหาค่าใช้จ่าย ภัยพิบัติทางธรรมชาติ และการคุกคามจากภายนอก

---

## Starter Pets (สัตว์เลี้ยงเริ่มต้น 3 สายพันธุ์)

ผู้เล่นเริ่มต้นเกมด้วยการเลือกรับอุปการะสัตว์เลี้ยงเริ่มต้น 1 ใน 3 สายพันธุ์:

| สายพันธุ์ | บุคลิก & ลักษณะเด่น | สเตตัสเริ่มต้น | แอ็กชันที่ชอบ | สกิลติดตัว (Passive) |
| --- | --- | --- | --- | --- |
| **Coco (Mossling)** | พืชชีวภาพตัวนุ่ม ขี้อาย รักสงบ ตกใจง่าย | HP: 100 / St: 80 / Cl: 70 | **Feed, Clean** | *Photosynthesis:* ได้รับฟื้นฟู Health +5 อัตโนมัติเมื่อค่า Clean > 80 |
| **Sproutlet** | คล่องแคล่ว ว่องไว ซุกซน ชอบการเคลื่อนไหว | HP: 90 / St: 70 / Cl: 80 | **Train, Clean** | *Agile Reflex:* ขยายหน้าต่าง Perfect Zone ในมินิเกม Train ขึ้น +15% |
| **Gloomtail** | เงียบขรึม สัตว์แห่งเงามืด ลึกลับ อดทนสูง | HP: 110 / St: 60 / Cl: 60 | **Heal, Feed** | *Shadow Barrier:* ลดความเสียหายจากการพลาดใน Dodge QTE ลง 25% |

---

## The 3-Day Vertical Slice Narrative Arc

```mermaid
flowchart TD
    D1[Day 1: The Arrival & Disaster] -->|เลือก Starter Pet + เผชิญพายุ Thunderstorm| D2[Day 2: The Wild Infiltration]
    D2 -->|เสียงเคาะประตู 'Knock Knock' + รับมือ Toothless| D2_Choice{ทางเลือก: ขับไล่ หรือ เชื่อง?}
    D2_Choice -->|Chase| D2_End[Toothless หนีไป / จบวันอย่างสงบ]
    D2_Choice -->|Tame| D2_Combat[เข้าสู่ฉาก Combat Taming / สยบ Toothless เข้าทีม]
    D2_Combat --> D3[Day 3: The Traveling Merchant]
    D2_End --> D3
    D3 -->|พ่อค้าเร่เดินทางมาถึงพร้อมข้อเสนอซื้อสัตว์เลี้ยง| D3_Choice{ยอมขาย Toothless 5,000G?}
    D3_Choice -->|Sell| D3_EndingA[จบแบบ Bittersweet: ได้เงินมหาศาลแต่สูญเสียสัตว์เลี้ยง]
    D3_Choice -->|Refuse| D3_Boss[เข้าสู่ Boss Fight: Merchant Battle ปกป้องบ้าน]
    D3_Boss -->|Victory| D3_EndingB[จบแบบ Heroic: ปกป้องสัตว์เลี้ยงสำเร็จและขับไล่พ่อค้า]
```

1. **Day 1 — The Arrival & Disaster (การเริ่มต้นและภัยพิบัติ):**
   - ผู้เล่นเปิดร้าน เลือก Starter Pet ตัวแรก
   - เรียนรู้ระบบพื้นฐาน: การจัดสรรพลังงาน 6 แต้ม, การให้อาหาร (Feed), ทำความสะอาด (Clean), และฝึกฝน (Train)
   - **Disaster Event:** ช่วงบ่ายเกิดพายุฝนฟ้าคะนองรุนแรง (**Thunderstorm**) ส่งผลให้สัตว์เลี้ยงเกิดอาการตื่นตระหนก ค่า Clean ลดฮวบ และต้องบริหารพลังงานฉุกเฉินเพื่อปลอบประโลม

2. **Day 2 — The Wild Infiltration & Taming (ผู้มาเยือนปริศนา):**
   - มีเสียงเคาะประตูปริศนาดังขึ้น (**"Knock Knock !!"**)
   - พบสัตว์ประหลาดร่างดำดุร้ายที่มีกรดกัดกร่อน (**Toothless**) บุกเข้ามาในบริเวณบ้าน
   - **ทางเลือกสำคัญ:** 
     - *Chase:* ขับไล่ไปอย่างปลอดภัย (ไม่เสี่ยงเจ็บตัว แต่ไม่ได้สัตว์เพิ่ม)
     - *Tame:* เข้าสู่ระบบการต่อสู้และฝึกให้เชื่อง (Taming Encounter) ต้องหลบกรดพิษ (Acid Dodge) และสวนกลับจนได้เป็นสัตว์เลี้ยงตัวที่สอง

3. **Day 3 — The Traveling Merchant & The Moral Crossroads (พ่อค้าเร่และทางแยกแห่งจิตใจ):**
   - พ่อค้าเร่ปริศนาแวะมาเยือน เปิดร้านขายไอเทมพิเศษ (**Crab Apple, Sea Tea, Cloudy Glasses, Torn Notebook**)
   - พ่อค้าสังเกตเห็น Toothless และยื่นข้อเสนอซื้อตัวด้วยเงินสดมหาศาล (**5,000 Gold**)
   - **ทางแยกแห่งผลลัพธ์:**
     - *ยอมรับข้อเสนอ (Sell):* ได้รับเงิน 5,000G ปลดหนี้และจบวันด้วยความร่ำรวย แต่สูญเสีย Toothless ตลอดกาล
     - *ปฏิเสธข้อเสนอ (Refuse):* พ่อค้าเกิดความโกรธเกรี้ยว ปิดร้านและเปิดฉากโจมตี นำไปสู่การต่อสู้ระดับบอส (**Merchant Boss Fight**) ผู้เล่นต้องสั่งการสัตว์เลี้ยงเข้าต่อสู้จนคว้าชัยชนะ

---

## Summary of the 4-Phase Daily Loop

ในแต่ละ Game Day จะดำเนินผ่าน 4 ขั้นตอนตามลำดับที่รัดกุม:

1. **Phase 1: Narrative & Morning Event Phase:**
   ตรวจสอบข้อความแจ้งเตือนประจำวัน เหตุการณ์สุ่ม สภาพอากาศ (Weather/Disasters) และการมาเยือนของแขกหน้าประตู
2. **Phase 2: Care & QTE Action Phase:**
   ใช้แต้มพลังงาน (Energy Points) ทำกิจกรรมดูแล 4 หมวดหมู่ (Feed, Clean, Train, Heal) ผ่านมินิเกม QTE 10 ครั้ง เพื่อเพิ่มสเตตัสและเลเวล
3. **Phase 3: Defense & Resolution Phase:**
   รับมือกับสถานการณ์วิกฤต การเผชิญหน้ากับสัตว์ป่า หรือการต่อสู้ป้องกันบ้านด้วยระบบ Dodge & Counter QTE
4. **Phase 4: Progression & Summary Phase:**
   ตรวจสอบรายงานสรุปประจำวัน (Daily Summary Report) คำนวณค่าเสื่อมถอยของสถานะ (Daily Decay) และบันทึกข้อมูลความคืบหน้า

---

## Art & Audio Aesthetic Direction

### Art Style
- **Visuals:** 2D High-Contrast Stylized Pixel Art / Detailed Sprite Illustration ที่ความละเอียด Backbuffer 1280x720
- **Color Palette:** โทนสีห้องพักสไตล์ Creamy Pastel ผสม Warm Amber ให้ความรู้สึกเป็นมิตร แต่ตัดด้วยเฉดสี Acid Green, Void Purple และ Warning Crimson เมื่อเกิดภัยพิบัติหรือการต่อสู้
- **Expressive Cues:** สัตว์เลี้ยงมี Sprite แสดงอารมณ์ละเอียด (Idle, Hungry, Distressed, Angry, Joyful) พร้อม Visual VFX บอกใบ้จังหวะ QTE

### Audio Direction
- **Soundtrack:** ดนตรี Lo-Fi Acoustic Guitar / Warm Electric Piano สำหรับช่วงเวลาดูแลในห้องพัก และแปรเปลี่ยนเป็น Up-tempo Chiptune Synthwave ผสมจังหวะกลองเขย่าขวัญในฉากต่อสู้ Boss Fight
- **SFX:** เสียง Typewriter เวลาอ่านข้อความ, เสียงหยดน้ำ/สบู่ในโหมด Clean, เสียงเคี้ยวกรุบกรอบในโหมด Feed, และเสียงสะท้อนโลหะ (Metallic Parry) เมื่อกด Dodge สำเร็จ

---

## Inspiration & Competitor Analysis

| ผลงานอ้างอิง | องค์ประกอบที่นำมาปรับใช้ใน BePal | จุดแตกต่างและวิวัฒนาการของ BePal |
| --- | --- | --- |
| **Tamagotchi / Digimon V-Pet** | วงจรการให้อาหาร ทำความสะอาด ฝึกซ้อม และความเสี่ยงจากการปล่อยปละละเลย | เพิ่มระบบจังหวะ QTE แบบแอ็กชัน และการตัดสินใจเชิงกลยุทธ์ผ่าน Energy Budget |
| **Undertale / Deltarune** | ระบบการหลบหลีกการโจมตีแบบเรียลไทม์ (Dodge Mechanics) และบุคลิกบอสที่มีเอกลักษณ์ | เชื่อมโยงความพร้อมในการต่อสู้เข้ากับความสมบูรณ์ของค่าสเตตัสการดูแล |
| **Lobotomy Corporation** | ความตื่นเต้นระทึกขวัญของการจัดการสิ่งมีชีวิตผิดปกติที่มีอันตรายถึงตาย | บรรยากาศเน้นความ Cozy อบอุ่น สัมผัสได้ใกล้ชิด ไม่ใช่ความสยองขวัญเลือดสาดแบบโรงงานปิด |
| **Papers, Please** | การจัดสรรทรัพยากรที่ตึงเครียด รายงานสรุปผลประจำวัน และแรงกดดันทางการเงิน | ถ่ายทอดผ่านการดูแลสัตว์เลี้ยงที่มีชีวิตจิตใจ มีการตอบสนองทางอารมณ์ชัดเจน |
