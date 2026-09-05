## type: gdd-scope

version: 0.1
date: 7/7/2026

---

# [BePal] — Scope & Feature List

## Unique Selling Point (USP)

* **Cute Aesthetics x Hardcore Challenge**: งานภาพสไตล์การ์ตูน 2D แสนน่ารักสดใส แต่แฝงไปด้วยระบบการต่อสู้กับบอสที่ท้าทาย อาศัยจังหวะและการหลบหลีกที่แม่นยำ (น่ารักแต่ไม่อ่อนโยน)
* **Tailored Build Progression**: ระบบอัปเกรดและปรับแต่งตัวละครแบบกึ่ง Roguelite ที่ให้ผู้เล่นสามารถ Reset และปรับเปลี่ยนสายการเล่น (Build) เพื่อแก้ทาง (Counter) รูปแบบการโจมตีของบอสแต่ละตัวได้อย่างอิสระ

## In Scope — สิ่งที่จะทำในภาคการศึกษานี้

| #   | Feature                                                | Priority | รายละเอียด / คำอธิบายเพิ่มเติม                                                                                                    | หมายเหตุ |
| --- | ------------------------------------------------------ | -------- | --------------------------------------------------------------------------------------------------------------------------------- | -------- |
| 1   | **Core Gameplay **                                     | Must     | Wheel skill check Qte for the core gameplay mechanic with four choice of action                                                   |          |
| 2   | **Core Ui**                                            | Must     | Including Menu, pet information, and QTE scene                                                                                    |          |
| 3   | **Pet rule Gameplay design **                          | Should   | Applying some pet rule that impact the gameplay                                                                                   |          |
| 4   | **Narrative **                                         | Should   | to inform the player about the world and background of BEPAL                                                                      |          |
| 5   | **Tutorial and information Ui **                       | Could    | description of each action player choose to inform what happening and how player should considering about it                      |          |
| 6   | **More management mechanics beside the core gameplay** | Could    | Random event that happen in each day or progressing mechanics beside the core game play to attaching player interest              |          |
| 7   | **Audio & UI Elements**                                | Could    | Feedback Sound and element in core gameplay or Ui And background music                                                            |          |
| 8   | **Save/Load System**                                   | Could    | ระบบบันทึกพลังตัวละครและสเตจที่ผ่านลงไฟล์ (เช่น JSON) เพื่อให้เล่นต่อได้                                                          |          |
| 9   | **Narrative & Cutscenes**                              | Could    | บทสนทนาสั้น ๆ หรือภาพนิ่งเล่าเรื่องระหว่างวันหรือการกระทำต่างๆ                                                                    |          |
| 10  | **different weapon arsenal**                           | Could    | ตัวเลือกอาวุธอื่น (เช่น ดาบใหญ่ ปืนคู่ มีด) ที่เปลี่ยนระยะการโจมตีและแอนิเมชันของตัวละคร เพื่อเพิ่มแนวทางในการต่อสู้ให้กับผู้เล่น |          |
|     |                                                        |          |                                                                                                                                   |          |

## Out of Scope - สิ่งที่จะไม่ทำ

* **Advance Animation**: เหตุผลเนื่องจากเวลากำจัดในด้าน Art จึงควรจะลดคุณภาพ/เฟรมของผู้เล่น และ บอส ให้ได้มากที่สุดเพื่อลดภาระงาน
* **Advance aesthetic**: หน้าเมนูส่วนใหญ่จะถูกแทนที่ด้วยกรอบ UI มากกว่า Graphic จะได้ไปโฟกัสกับงานในส่วนอื่น ( พื้นหลังตอนต่อสู้ และ Animation )

## Risks & Assumptions

| ความเสี่ยง                                                       | ผลกระทบ                                                                                                                               | แนวทางการแก้ไข                                                                               |
| -------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------- |
| **ภาระงานที่มากเกินไป**                           | งานอาจจะไม่ได้เป็นอย่างที่ควร และ ทำให้ทีมงานได้รับความเครียดในการทำงาน | ใช้ Asset ซ้ำ และ ลด Mechanic บางอย่าง                                                  |
| **กำหนดเวลาในการทำงานที่กระชั้นชิด** | ทำให้ทีมงานได้รับภาระงานที่มากเกินไปและไม่มีเวลาว่างสำหรับการพักผ่อน     | อาจต้องเร่งแบ่งงานทำในบางส่วนหากมีภาระงานที่มากเกินไป |

---
