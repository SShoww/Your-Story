---
type: gdd-core-loop
version: 2.0
date: 2026-09-20
---

# BePal — Core Loop & Gameplay Flow (v2)

## High-Level Daily Loop

วงจรการเล่นหลักของ BePal ถูกออกแบบให้เป็นวัฏจักรประจำวัน (Daily Cycle) ที่ผู้เล่นต้องบริหารจัดการเวลา พลังงาน และสถานะของสัตว์เลี้ยง ท่ามกลางวิกฤตและเหตุการณ์ไม่คาดฝัน

```mermaid
flowchart TD
    Start([Start Day]) --> Phase1[Phase 1: Narrative & Morning Event<br>แจ้งเตือนสภาพอากาศ / เหตุการณ์พิเศษ / แขกหน้าประตู]
    
    Phase1 --> Phase2[Phase 2: Care & QTE Action Phase<br>จัดสรร 6 Energy Points: Feed / Clean / Train / Heal]
    
    Phase2 --> CheckEnergy{Energy หมด หรือ<br>Progress เต็ม 100%?}
    CheckEnergy -->|ยังไม่หมด| Phase2
    CheckEnergy -->|หมด หรือ จบกิจกรรม| Phase3[Phase 3: Defense & Resolution Phase<br>รับมือภัยพิบัติ / การบุกรุก / บอสไฟต์]
    
    Phase3 --> CheckSurvive{สัตว์เลี้ยงหรือผู้เล่น<br>HP เหลือ 0 หรือไม่?}
    CheckSurvive -->|HP = 0| RevivePrompt[Incapacitated / กู้ชีพฉุกเฉิน เสีย 500G]
    RevivePrompt --> Phase4
    CheckSurvive -->|รอดชีวิต| Phase4[Phase 4: Progression & Summary Phase<br>คำนวณ Daily Decay / สรุปผล Daily Report]
    
    Phase4 --> CheckDays{จบวันที่ 3 หรือยัง?}
    CheckDays -->|ยังไม่จบ| NextDay[ก้าวสู่วันถัดไป / ฟื้นฟู Energy 6 แต้ม]
    NextDay --> Start
    CheckDays -->|จบวันที่ 3| Ending[Story Conclusion / End of Vertical Slice]
```

---

## Detailed Breakdown of the 4-Phase Daily Cycle

### Phase 1: Narrative & Morning Event Phase (ยามเช้าและข่าวสาร)
1. **Morning Briefing:** เมื่อเริ่มต้นวันใหม่ ระบบจะแสดงหัวข้อข่าวหรือข้อความบันทึกประจำวัน แจ้งสภาพอากาศและเหตุการณ์สุ่ม
2. **Environmental Hazard Notification:** หากมีภัยพิบัติ (เช่น Thunderstorm ใน Day 1) หน้าจอจะแสดงแถบคำเตือนสีแดงกะพริบ แจ้งเตือนผลกระทบต่อสเตตัสสัตว์เลี้ยง
3. **Doorstep Arrival Check:** ในวันที่กำหนด (เช่น Day 2 มีเสียง "Knock Knock !!", Day 3 มีรถเข็นพ่อค้าเร่มาจอด) จะมีไอคอนแจ้งเตือนให้ผู้เล่นตรวจสอบเหตุการณ์ภายนอก

### Phase 2: Care & QTE Action Phase (การดูแลและบริหารพลังงาน)
1. **Energy Budgeting:** ผู้เล่นได้รับแต้มพลังงานเริ่มต้น **6 Energy Points (AP)** ต่อวัน
2. **Action Selection:** ผู้เล่นเลือกทำกิจกรรมการดูแลจาก 4 หมวดหมู่หลัก:
   - **Feed (ให้อาหาร):** ใช้ 1 AP ฟื้นฟูค่า Stomach และให้ EXP
   - **Clean (ทำความสะอาด):** ใช้ 1 AP ขจัดสิ่งสกปรก ฟื้นฟูค่า Clean ป้องกันการเจ็บป่วย
   - **Train (ฝึกฝนทักษะ):** ใช้ 1 AP เพิ่มระดับเลเวลและ EXP สูงสุด แต่ลดค่า Stomach
   - **Heal (รักษาพยาบาล):** ใช้ 1 AP (หรือร่วมกับไอเทมยา) ฟื้นฟูค่า Health ที่สูญเสีย
3. **10-Attempt Mini-Game Execution:** แต่ละแอ็กชันจะตัดเข้าสู่หน้าจอมินิเกมวงล้อ QTE จำนวน 10 ครั้งติดต่อกัน ผู้เล่นต้องจับจังหวะกด Spacebar ในโซน Perfect หรือ Good
4. **Day Progress Accumulation:** การกดสำเร็จแต่ละครั้งจะเพิ่มหลอดความคืบหน้าของวัน (`Day Progress += 10%`) หากพลาดหลอดจะเพิ่มชดเชย (`+15%`) เพื่อผลักดันให้เวลาเดินหน้าไปสู่ช่วงถัดไป

### Phase 3: Defense & Resolution Phase (การเผชิญหน้าและการต่อสู้)
1. **Trigger Condition:** เกิดขึ้นเมื่อผู้เล่นใช้ Energy ครบตามโควตา หรือเกิดเหตุการณ์บังคับตามเนื้อเรื่องประจำวัน
2. **Dynamic Encounters:**
   - **Day 1:** แก้ไขสถานการณ์พายุฟ้าคะนอง สัตว์เลี้ยงตกใจกลัว ต้องทำจังหวะกดปลอบประโลม (Calm QTE)
   - **Day 2 (Toothless Encounter):** สัตว์ป่ากรดพิษบุกเข้ามา ผู้เล่นต้องเลือกว่าจะขับไล่ (Chase) หรือฝึกให้เชื่อง (Tame Combat QTE)
   - **Day 3 (Merchant Boss Battle):** หากปฏิเสธข้อเสนอขายสัตว์เลี้ยง พ่อค้าจะเปลี่ยนร่างเข้าสู่การต่อสู้ 3 เฟส
3. **Combat Loop:** สัตว์ศัตรูจะส่งคลื่นการโจมตี (Telegraphed Attacks) ผู้เล่นต้องกด Spacebar ในโซนหลบหลีก (Dodge Zone) และกดสวนกลับ (Counter-Attack) เมื่อเกิดจังหวะเปิดช่องโหว่

### Phase 4: Progression & Summary Phase (การสรุปผลและฟื้นฟู)
1. **Daily Stat Decay:** หักลบค่าสเตตัสความต้องการพื้นฐานตามสูตรประจำวัน (Stomach -20, Clean -15)
2. **Sickness & Penalty Check:** หากค่า Clean < 50 หรือ Stomach = 0 จะเกิดอาการป่วยและหักค่า Health ทันที
3. **Daily Summary Report Card:** นำเสนอผลประเมินเกรด (S, A, B, C, F) สรุปคะแนนสะสม โบนัสเงินรางวัล (Gold Reward) และปลดล็อกบันทึก Survival Log
4. **Night Rest:** บันทึกข้อมูลเซฟเกม (Auto-save) ตัดเข้าสู่ฉากกลางคืน และรีเซ็ตพลังงานกลับเป็น 6 แต้มสำหรับวันถัดไป

---

## The 3-Day Vertical Slice Progression Timeline

| วันที่ | Phase 1 (Event) | Phase 2 (Care Focus) | Phase 3 (Encounter / Conflict) | Phase 4 (Resolution) |
| --- | --- | --- | --- | --- |
| **Day 1** | - รับสัตว์เลี้ยงเริ่มต้น (Coco/Sproutlet/Gloomtail)<br>- แจ้งเตือนพายุ Thunderstorm | - ทำความคุ้นเคยกับการใช้ 6 AP<br>- เลี้ยงดูตามความชอบของ Starter Pet | - **Thunderstorm Disaster:** ลมพายุพัดสิ่งสกปรกเข้ามา ค่า Clean ลดลง ต้องจัดการสถานการณ์ฉุกเฉิน | - สรุปคะแนนวันแรก<br>- รับเงินสนับสนุนก้อนแรก 100G |
| **Day 2** | - เสียงประหลาดหน้าประตู **"Knock Knock !!"**<br>- ร่องรอยคราบกรดสีม่วง | - บริหารพลังงานเพื่อเตรียมความพร้อมรับมือสิ่งไม่คาดคิด | - **Toothless Wild Encounter:**<br>ทางเลือก: ขับไล่ (Chase) หรือ ปราบให้อยู่หมัด (Tame)<br>- มินิเกมหลบกรด Acid Dodge & Counter | - ปลดล็อก Toothless เข้าเป็นสมาชิกที่สองในฟาร์ม (กรณี Tame)<br>- อัปเดต Survival Log |
| **Day 3** | - พ่อค้าเร่เดินทางมาถึง<br>- เปิดระบบร้านค้า (Merchant Shop) | - จับจ่ายซื้อไอเทมบัฟและอาหารฟื้นพลัง<br>- ฝึกฝนสัตว์เลี้ยงเพื่อเตรียมพร้อม | - **The Merchant's Dilemma:**<br>ทางเลือก: ขาย Toothless แลกเงิน 5,000G หรือ ปฏิเสธ<br>- **Merchant Boss Fight (หากปฏิเสธ):** ต่อสู้ 3 เฟส | - **Ending A:** ร่ำรวยแต่เดียวดาย (หากขาย)<br>- **Ending B:** ชัยชนะของมิตรภาพ (หากชนะบอส) |

---

## State Machine Diagram

```mermaid
stateDiagram-v2
    [*] --> MainMenu
    MainMenu --> IntroCutscene : Start Game
    IntroCutscene --> ChoosePetScreen : Select Starter Pet
    
    ChoosePetScreen --> BaseRoom : Pet Selected
    
    state BaseRoom {
        [*] --> IdleHUD : แสดงค่าพลังงาน 6 AP และสเตตัส
        IdleHUD --> CareMiniGame_Feed : คลิก Feed (ใช้ 1 AP)
        IdleHUD --> CareMiniGame_Clean : คลิก Clean (ใช้ 1 AP)
        IdleHUD --> CareMiniGame_Train : คลิก Train (ใช้ 1 AP)
        IdleHUD --> CareMiniGame_Heal : คลิก Heal (ใช้ 1 AP)
        IdleHUD --> ShopScreen : คลิก Merchant (เฉพาะ Day 3)
        IdleHUD --> InventoryOverlay : คลิกไอคอนกระเป๋า
        
        CareMiniGame_Feed --> IdleHUD : ครบ 10 ครั้ง / หัก AP
        CareMiniGame_Clean --> IdleHUD : ครบ 10 ครั้ง / หัก AP
        CareMiniGame_Train --> IdleHUD : ครบ 10 ครั้ง / หัก AP
        CareMiniGame_Heal --> IdleHUD : ครบ 10 ครั้ง / หัก AP
        ShopScreen --> IdleHUD : ปิดร้านค้า
        InventoryOverlay --> IdleHUD : ใช้ไอเทม / ปิดกระเป๋า
    }
    
    BaseRoom --> EventResolution : Energy == 0 หรือ กิจกรรมครบ
    
    state EventResolution {
        [*] --> CheckEventDay
        CheckEventDay --> DisasterDay1 : Day 1 (Thunderstorm)
        CheckEventDay --> EncounterDay2 : Day 2 (Toothless Knock Knock)
        CheckEventDay --> MerchantEncounterDay3 : Day 3 (Merchant Offer)
        
        DisasterDay1 --> CalmingQTE : ปลอบประโลมสัตว์เลี้ยง
        EncounterDay2 --> TamingCombat : เลือก Tame (Acid Dodge)
        EncounterDay2 --> ChasePeace : เลือก Chase (หนีไป)
        MerchantEncounterDay3 --> SellEnding : เลือก Yes (ขาย 5,000G)
        MerchantEncounterDay3 --> BossCombat : เลือก No (ปฏิเสธ)
        
        CalmingQTE --> EndPhaseCheck
        TamingCombat --> EndPhaseCheck
        ChasePeace --> EndPhaseCheck
        BossCombat --> EndPhaseCheck
    }
    
    EventResolution --> IncapacitatedScreen : สัตว์เลี้ยง HP == 0
    IncapacitatedScreen --> SummaryReportScreen : จ่ายค่ารักษา 500G (หรือกู้ยืม)
    
    EventResolution --> SummaryReportScreen : ผ่านเหตุการณ์
    SummaryReportScreen --> NightRestScreen : ยืนยันผล
    NightRestScreen --> BaseRoom : เริ่มวันใหม่ (Day 2 หรือ Day 3)
    NightRestScreen --> FinalCreditsScreen : จบวันที่ 3 (Vertical Slice Completed)
```

---

## Controls & Player Agency

| รูปแบบการควบคุม | บริบทการใช้งาน | หน้าที่และผลลัพธ์ |
| --- | --- | --- |
| **Mouse Left Click** | หน้าจอหลัก / HUD / เมนู | เลือกเมนูการดูแล (Feed/Clean/Train/Heal), เปิดกระเป๋า, ซื้อของในร้านค้า, เลือกคำตอบในบทสนทนา |
| **Spacebar (Tap)** | มินิเกม Care QTE | กดยืนยันจังหวะเข็มในวงล้อ QTE (Perfect / Good Zone) |
| **Spacebar (Reflex)** | Dodge QTE (หลบหลีก) | กดทันทีเมื่อเข็มวิ่งเข้าสู่โซนสีทอง (Dodge Zone) เพื่อหลบการโจมตี |
| **Spacebar (Counter)** | Attack / Counter Window | กดเมื่อวงแหวนหดเล็กลงมาบรรจบจุดกึ่งกลาง เพื่อสวนกลับศัตรู |
| **Escape (ESC)** | เมนูและหน้าต่างเสริม | ปิดหน้าต่าง Inventory, ข้ามหน้าต่างไดอะล็อก, หรือเปิดเมนู Pause |

---

## Win, Lose, and Recovery Rules

1. **Daily Victory Condition:**
   - ใช้พลังงานและบริหารสเตตัสสัตว์เลี้ยงให้อยู่รอดจนสิ้นสุดวันโดยที่ค่า Health ไม่แตะระดับ 0
   - ผ่านมินิเกมหรือเหตุการณ์การเผชิญหน้าประจำวัน

2. **Incapacitation State (ภาวะสัตว์เลี้ยงหมดสภาพ):**
   - เกิดขึ้นเมื่อสัตว์เลี้ยงได้รับความเสียหายจากการโดนกรด การโจมตี หรือการอดอาหารจนค่า Health กลายเป็น 0
   - เกมจะไม่จบลงแบบ Game Over ถาวร แต่จะเข้าสู่หน้าจอ **Emergency Revive (หน่วยแพทย์ฉุกเฉิน)**
   - ผู้เล่นต้องชำระค่าธรรมเนียมกู้ชีพ **500 Gold** (หากเงินไม่พอ ระบบจะมีข้อตกลงเงินกู้ฉุกเฉินจากพ่อค้า คิดดอกเบี้ย 20% ต่อวัน)

3. **Campaign Clear Condition:**
   - ผ่านพ้นเหตุการณ์วันที่ 3 (ไม่ว่าจะเลือกขาย Toothless หรือเอาชนะบอส Merchant ได้สำเร็จ)
   - หน้าจอจะแสดงสรุปเกรดรวมทั้ง 3 วัน บันทึกฉายาผู้ดูแล (Caretaker Title) และปลดล็อกสถิติการเล่น
