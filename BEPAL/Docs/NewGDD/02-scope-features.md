---
type: gdd-scope
version: 2.0
date: 2026-09-20
---

# BePal — Scope & Feature List

## Unique Selling Point (USP)

1. **Cozy Domestic Shelter with Hardcore Survival Peril:** บรรยากาศบ้านพักพิงที่อบอุ่นนุ่มนวล ผสมผสานความตึงเครียดของการเอาชีวิตรอด สัตว์เลี้ยงมีความต้องการทางกายภาพสมจริง และมีโอกาสเจ็บป่วยหรือเสียชีวิตได้หากปล่อยปละละเลย
2. **Discrete Energy Economy (6 AP Daily Budget):** ผู้เล่นต้องบริหารแต้มพลังงานจำกัด 6 แต้มในแต่ละวัน ตัดสินใจระหว่างการให้อาหาร ทำความสะอาด ฝึกซ้อม หรือเตรียมพร้อมรับมือวิกฤต
3. **10-Attempt Precision Care QTE Engine:** การดูแลสัตว์เลี้ยงวัดผลผ่านมินิเกมวงล้อความแม่นยำ 10 ครั้งต่อเซสชัน พร้อมระบบ Perfect Zone ($\pm 0.20 \text{ rad}$) และ Good Zone ($\pm 0.45 \text{ rad}$) ที่ผลักดันเวลาในเกมให้เดินหน้าเร็วขึ้นเมื่อเกิดข้อผิดพลาด
4. **Dynamic Combat & Boss Encounters:** การเผชิญหน้ากับสัตว์ป่าดุร้าย (Toothless) และการประลองระดับบอส 3 เฟสกับพ่อค้าเร่ (The Traveling Collector) ด้วยระบบ Dodge QTE และ Counter-Attack
5. **Consequential Moral Crossroads:** ทางเลือกเชิงศีลธรรมที่ส่งผลต่อเส้นทางเนื้อเรื่อง เช่น การเลือกว่าจะยอมขายสัตว์เลี้ยงเพื่อเงินก้อนโต 5,000G หรือสู้สุดชีวิตเพื่อปกป้องมิตรภาพ

---

## Feature List & Priorities

| # | Feature | Priority | รายละเอียด / Acceptance Criteria |
| --- | --- | --- | --- |
| 1 | **Starter Pet Selection** | **Must** | ผู้เล่นเลือกสัตว์เลี้ยงเริ่มต้น 1 ใน 3 สายพันธุ์ (Coco/Mossling, Sproutlet, Gloomtail) ที่มีสเตตัสเริ่มต้น สกิลติดตัว (Passive) และความชอบต่างกัน |
| 2 | **4-Phase Daily Loop** | **Must** | ระบบเวลาเดินหน้า 3 วัน แบ่งเป็น 4 เฟสชัดเจน: Morning Event $\rightarrow$ Care & QTE $\rightarrow$ Defense/Resolution $\rightarrow$ Daily Summary & Rest |
| 3 | **Discrete Energy Budget (6 AP)** | **Must** | โควตาพลังงาน 6 AP ต่อวัน การกระทำแต่ละอย่างหัก AP อย่างแม่นยำ และรีเซ็ตกลับเป็น 6 แต้มในเช้าวันใหม่ |
| 4 | **4 Core Care Actions** | **Must** | ระบบคำสั่ง 4 หมวด: Feed (เติม Stomach), Clean (ขจัดคราบ), Train (เร่ง EXP), Heal (รักษา HP และสถานะผิดปกติ) |
| 5 | **10-Attempt Precision QTE Wheel** | **Must** | วงล้อ QTE 10 ครั้งต่อรอบ เข็มหมุนคงที่ $2.4 \text{ rad/s}$ ตรวจจับ Perfect ($\le 0.20$), Good ($\le 0.45$), Miss ($> 0.45$) พร้อมเกรด S–F |
| 6 | **Day Progress Dynamic Scaling** | **Must** | หลอดเวลาประจำวันเพิ่ม $+10\%$ เมื่อกดสำเร็จ และ $+15\%$ เมื่อกดพลาด สะท้อนความตื่นตระหนกที่เร่งให้เวลาหมดเร็วขึ้น |
| 7 | **Day 1 Thunderstorm Disaster** | **Must** | เหตุการณ์พายุฝนฟ้าคะนอง สัตว์เลี้ยงตื่นตระหนก ค่า Clean ลดฮวบ และมีมินิเกม Calming QTE ฉุกเฉินเพื่อปลอบโยน |
| 8 | **Day 2 Toothless Knock Knock Encounter** | **Must** | เหตุการณ์สัตว์ป่ากรดเคาะประตู ทางเลือกขับไล่ (Chase) หรือสยบเข้าทีม (Tame) ผ่านมินิเกมหลบกรด Acid Dodge & Counter |
| 9 | **Day 3 Merchant Shop & 5,000G Buyout Dilemma** | **Must** | พ่อค้าเร่มาเยือน เปิดร้านค้า 5 ชิ้น พร้อมบทสนทนายื่นข้อเสนอซื้อ Toothless 5,000G นำไปสู่ Ending A หรือ Boss Fight |
| 10 | **3-Phase Merchant Boss Battle** | **Must** | การต่อสู้ระดับบอส 3 เฟส: Phase 1 (Acid Flasks), Phase 2 (Gold Gatling), Phase 3 (Collector's Cane) พร้อมระบบสวนกลับ |
| 11 | **Gold Economy & 8-Slot Inventory** | **Must** | กระเป๋าเก็บของ 8 ช่อง ระบบเงิน Gold (เงินเริ่ม 150G, เงินสนับสนุน 100G/วัน, โบนัสเกรด, ซื้อไอเทมร้านค้า) |
| 12 | **Equipment System** | **Should** | ไอเทมสวมใส่ 3 ชนิด (Ballet Shoes, Toy Knife, Faded Ribbon) มอบบัฟถาวรแก่สัตว์เลี้ยง |
| 13 | **Incapacitation & 500G Revive Loan** | **Must** | เมื่อ HP เหลือ 0 เข้าสู่สภาวะหมดสภาพ ต้องจ่าย 500G เพื่อกู้ชีพ หรือเซ็นสัญญาเงินกู้ดอกเบี้ย 20% ทบต้น |
| 14 | **Daily Summary Report Card** | **Must** | หน้าสรุปผลประจำวันสไตล์ Papers, Please แสดงเกรดการดูแล บันทึก Log ใหม่ และรายได้ประจำวัน |
| 15 | **Survival Log Research Journal** | **Should** | หน้าต่างสมุดบันทึกข้อมูลและกฎพฤติกรรมสัตว์เลี้ยง ปลดล็อกข้อมูลเมื่อดูแลสำเร็จครบตามเงื่อนไข |
| 16 | **Dynamic Audio Feedback & BGM** | **Should** | เสียง SFX ในการกด QTE (Perfect, Good, Miss, Dodge, Parry) และเพลงประกอบ BGM บรรยากาศ Cozy ในห้อง และเพลงระทึกขวัญตอนบอส |
| 17 | **Endings A & B Cinematic Screens** | **Must** | หน้าต่างบทสรุปฉากจบ Ending A (The Wealthy Betrayal) และ Ending B (Sanctuary's Protector) |

---

## Out of Scope — สิ่งที่ไม่ทำในเฟสนี้

- **No Real-Time 3D Graphics:** ใช้ภาพกราฟิก 2D Pixel Art / High-Contrast Illustration 1280x720 เท่านั้น
- **No Multiplayer / Online Leaderboards:** มุ่งเน้นประสบการณ์เล่นคนเดียว (Single-Player Simulation)
- **No Weapon Inventory / Free Combat:** การต่อสู้ทำผ่านวงล้อจังหวะ QTE สั่งการสัตว์เลี้ยง ไม่มีการควบคุมตัวละครเดินยิงหรือฟันดาบอิสระ
- **No Endless Lifespan / Procedural Days:** โครงสร้างจำกัดที่ 3-Day Vertical Slice ที่มีความเข้มข้นทางเนื้อเรื่องสูงสุด

---

## Risks & Mitigation

| ความเสี่ยง | ผลกระทบ | แนวทางป้องกันและแก้ไข |
| --- | --- | --- |
| **ความซับซ้อนของ 10-Attempt QTE Math** | ผู้เล่นรู้สึกยากหรือง่ายเกินไป | กำหนดองศาและรัศมีเป้าหมายชัดเจน (Perfect $\pm 0.20$, Good $\pm 0.45$) พร้อมระบบปรับจูนค่าความเร็วเข็มใน config |
| **ความลื่นไหลของการสลับหน้าจอ (Screen Transition)** | เกิดสถานะค้างระหว่างฉากหลักและฉากต่อสู้ | รวมศูนย์การเปลี่ยนหน้าจอทั้งหมดผ่าน `ScreenManager` และใช้ `ScreenContext` ถ่ายโอนข้อมูลแบบ Pure C# |
| **สมดุลทางเศรษฐกิจและการเงิน (Gold Economy)** | ผู้เล่นเงินหมดจนติด Deadlock ในการกู้ชีพ | มีระบบเงินกู้ฉุกเฉิน (Emergency Loan 500G) คิดดอกเบี้ย 20% รองรับไม่ให้เกิด Hard Lock |
| **ภาระงานด้านภาพสไปรต์สัตว์เลี้ยงหลายอารมณ์** | กราฟิกเสร็จไม่ทันตามกำหนด | ออกแบบโครงสร้าง Sprite Sheet หลัก 4 ท่า (Idle, Happy, Angry, Attack/Teleport) และใช้ Procedural Shake ช่วยเสริมอารมณ์ |
