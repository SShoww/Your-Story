---
type: README
version: 0.2
date:
  - 09/01/2026
---
# [BePal] — Documentation Index
## 👥คนในทีม

| รหัส      | ชื่อ               | ชื่อเล่น |
| --------- | ------------------ | -------- |
| 682110119 | ธัญญรัตน์ ติ๊บหน่อ | เดียร์   |
| 682110128 | ปีย์ตะวัน แห่งหาญ  | ซุง      |
| 682110137 | ภูมิพัฒน์ ตามวงค์  | ภูมิ     |
| 682110141 | วศิน ศรีวรกุล      | โชว์     |

## 📖 เอกสารในโปรเจกต์นี้

| ไฟล์                                          | เนื้อหา                          | สถานะ |
| --------------------------------------------- | -------------------------------- | ----- |
| [00-concept.md](BEPAL/Docs/GDD/00-concept.md) | Game concept, core loop, scope   | ✅     |
| [01-core-loop.md](01-core-loop.md)            | Class diagram เบื้องต้น          | ✅     |
| [02-scope-features.md](02-scope-features.md)  | Scope ของ Feature ในเกม          | ✅     |
| [03-mechanics.md](03-mechanics.md)            | Mechanic flow (state diagram)    | ✅     |
| [04-class-diagram.md](04-class-diagram.md)    | OOP Diagram                      | 🛑    |
| [05-asset-list.md](05-asset-list.md)          | Asset list + asset pipeline flow | 🛑    |


## 🏷️ Naming Convention

**Asset:** ดูตารางเต็มใน [00-concept.md](BEPAL/Docs/GDD/00-concept.md#asset-naming-convention)

| Prefix   | ประเภท     |
| -------- | ---------------- |
| `spr_` | Sprite / Texture |
| `sfx_` | Sound Effect     |
| `bgm_` | Background Music |
| `fnt_` | Font             |
| `dat_` | Data / Config    |

**เอกสาร:** ไฟล์ใน `docs/01_GDD/` เรียงลำดับด้วย prefix ตัวเลข 2 หลัก (`00-`, `01-`, ...) ตามลำดับที่สร้างขึ้นในแต่ละ Lab — ห้ามสลับเลขไฟล์ที่มีอยู่แล้ว เพิ่มไฟล์ใหม่ให้ต่อเลขถัดไป

## Asset Naming Convention

| Prefix   | ประเภท     | ตัวอย่าง        |
| -------- | ---------------- | ----------------------- |
| `spr_` | Sprite / Texture | `spr_player_idle.png` |
| `sfx_` | Sound Effect     | `sfx_jump.wav`        |
| `bgm_` | Background Music | `bgm_stage_01.mp3`    |
| `fnt_` | Font             | `fnt_ui_main.ttf`     |
| `dat_` | Data / Config    | `dat_enemies.json`    |

## 📁 ใครดูแลส่วนไหน

| คนในทีม       | รับผิดชอบ           | โฟลเดอร์ staging                      |
| ------------- | ------------------- | ------------------------------------- |
| เดียร์ & โชว์ | Sprites / Textures  | `docs/02_Assets/_candidates/sprites/` |
| โชว์          | Sound Effects (SFX) | `docs/02_Assets/_candidates/sfx/`     |
| ภู            | Music / BGM         | `docs/02_Assets/_candidates/music/`   |
| ซุง           | Fonts + Data        | `docs/02_Assets/_candidates/fonts/`   |
