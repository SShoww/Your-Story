---
type: gdd-mechanics
version: 0.1
date: 14/7/2026
---
## State Diagram - ระบบ Text-Based & เลือกตอบ (Choice-driven Narrative)

```mermaid
stateDiagram-v2
    [*] --> Reading
    Reading --> Chooses_Choice : กด Left-Click เพื่อเลือก
    Chooses_Choice --> Event_Result : อ่านผลลัพธ์
    Event_Result --> Reading : อ่านเรื่องราว
    Event_Result --> Figth_Scene : ได้ฉาก GamePlay ต่อสู้
    Figth_Scene --> Event_Result : ผลลัพธ์การต่อสู้ 
```

## State Diagram - Player Movement & Combat State Diagram

```mermaid
stateDiagram-v2
    [*] --> Idle
    Idle --> Moving : เริ่มเดิน (wasd)
    Moving --> Idle : หยุดเดิน
    Idle --> Attack : ตี Click Left/Right
    Moving --> Jump : กระโดด Space key
    Jump --> Idle : ตกสู่พื้น
    [*] --> Hurt 
    Hurt --> Dead : โดนตีตาย
    Dead --> Summary : ไปหน้า
```

## Rules

| State   | เข้าเงื่อนไข                        | ออกเงื่อนไข                 | Note                  |
| ------- | ----------------------------------------------- | -------------------------------------- | --------------------- |
| Idle    | เริ่มเกม / หยุดเคลื่อนที่ | กด input ใดๆ                      | Animation loop        |
| Move    | กดปุ่มทิศทาง                        | ปล่อยปุ่ม / กระโดด      | Speed = [ค่า]      |
| Jump    | กด Space ขณะอยู่พื้น               | ถึงจุดสูงสุด               | Gravity = [ค่า]    |
| Attack  | กดปุ่มตี                                | โดนตี / หยุดตี              | Damage = [ค่า]     |
| Hurt    | เมื่อโดนตี                            | เดินหนีออกมา / ตาย      | Hurt_Damge = [ค่า] |
| Summary | เมื่อเลือดหมด                      | เมื่ออ่าน Summary เสร็จ | Story Summary        |
