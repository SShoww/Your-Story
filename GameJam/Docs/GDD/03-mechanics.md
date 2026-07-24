---
type: gdd-mechanics
version: 0.1
date: 14/7/2026
---
## State Diagram - ระบบ Text-Based & เลือกตอบ (Choice-driven Narrative)

```mermaid
stateDiagram-v2
    Start --> Choose_weapon
    Choose_weapon--> Fight
    Fight --> Reward
    Reward --> Next_Room
    Next_Room --> Fight
```

## State Diagram - Player Movement & Combat State Diagram

```mermaid
stateDiagram-v2
    [*] --> Idle
    Idle --> Moving : เริ่มเดิน (wasd)
    Moving --> Idle : หยุดเดิน
    Idle --> Attack : ตี Click Left/Right
    Moving --> Dash : Space key
    Idle --> Dash : Space key
    Jump --> Idle : ตกสู่พื้น
    [*] --> Hurt 
    Hurt --> Dead : โดนตีตาย
    Dead --> Summary : ไปหน้า
```

## Rules

| State   | เข้าเงื่อนไข                                                                  | ออกเงื่อนไข                 | Note                                          |
| ------- | ----------------------------------------------------------------------------------------- | -------------------------------------- | --------------------------------------------- |
| Idle    | เริ่มเกม / หยุดเคลื่อนที่                                           | กด input ใดๆ                      | Animation loop                                |
| Move    | กดปุ่มทิศทาง                                                                  | ปล่อยปุ่ม / กระโดด      | Speed = [ค่า]                              |
| Dash    | กด Space ขณะอยู่นื่งหรือเดินเพื่อพุ่งหลบการโจมตี | ถึงจุดสูงสุด               | Dash speed= [ค่า]<br />Iframe = True/false |
| Attack  | กดปุ่มตี                                                                          | โดนตี / หยุดตี              | Damage = [ค่า]                             |
| Hurt    | เมื่อโดนตี                                                                      | เดินหนีออกมา / ตาย      | Hurt_Damge = [ค่า]                         |
| Summary | เมื่อเลือดหมด                                                                | เมื่ออ่าน Summary เสร็จ | Story Summary                                |
