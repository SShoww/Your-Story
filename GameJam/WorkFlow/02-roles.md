---
type: jam-roles
version: 0.1
date: [21/07/26]
team: [Your Story]
---
# Team Roles & Pipeline Ownership — [Your Story]

## แบ่งความรับผิดชอบตาม Pipeline (4 คน)

| คน | ชื่อ             | Module ที่รับผิดชอบ         | Phase ที่ต้องเสร็จ |
| ---- | -------------------- | --------------------------------------- | ------------------------------ |
| 1    | [ปีย์ตะวัน] | `GameStateManager` + `InputManager` | Phase 1 (Hour 6–14)           |
| 2    | [วศิน]           | Core mechanic `Update()` logic        | Phase 2–3 (Hour 6–24)        |
| 3    | [ธัญญรัตน์] | Render/`SpriteBatch` + Collision      | Phase 2–3 (Hour 6–24)        |
| 4    | [ภูมิพัฒน์] | Content pipeline (MGCB) + Audio/UI      | Phase 3–4 (Hour 6–24)        |

> คนละไฟล์/module = ชนกันน้อยที่สุด ปรับ module ตามตาราง [01-pipeline-checklist.md](01-pipeline-checklist.md) ให้ตรงกับเกมจริงของทีม

## File Ownership Rule

> ไฟล์ที่ต้องแตะร่วมกันบ่อย เช่น `Game1.cs`, ไฟล์ scene หลัก — ตกลงล่วงหน้าว่าใครแก้ก่อน-หลัง เพื่อลด merge conflict

| ไฟล์/พื้นที่ที่เสี่ยงชนกัน | เจ้าของหลัก | กติกาการแก้ไข                                                   |
| --------------------------------------------------- | ---------------------- | ---------------------------------------------------------------------------- |
| `Game1.cs`                                        | [วศิน]             | ["คนอื่นแจ้งใน chat ก่อนแก้ แล้ว pull ก่อน push"] |
| asset.cs                                            | [ธัญญรัตน์]   | [เพิ่มแล้วแจ้งด้วย]                                         |
| Sound.cs                                            | [ภูมิพัฒน์]   | [เพิ่มแล้วแจ้งด้วย]                                         |

## เมื่อคนใดคนหนึ่งเสร็จงานตัวเองก่อน

- ซุง idea finish -> ไปช่วยเดียหา asset
- โช feature finish -> ไป deploy itch.oi
- ภู idea finish -> ไปช่วยหา sound and BGM
- เดีย asset finish -> ไปช่วย โชจัด asset
