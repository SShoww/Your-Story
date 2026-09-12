---
type: README
version: 0.2
date:
  - 09/01/2026
---
# [BePal] — Documentation Index
## 👥คนในทีม

| รหัส | ชื่อ | ชื่อเล่น | บทบาทหน้าที่ |
| --- | --- | --- | --- |
| 682110141 | วศิน ศรีวรกุล | โชว์ | Lead Programmer |
| 682110128 | ปีย์ตะวัน แห่งหาญ | ซุง | Game Designer |
| 682110137 | ภูมิพัฒน์ ตามวงค์ | ภูมิ | Flex (leans towards Design) / Audio Support |
| 682110119 | ธัญญรัตน์ ติ๊บหน่อ | เดียร์ | 2D Art & UI Lead |

| ไฟล์                                          | เนื้อหา                          | สถานะ |
| --------------------------------------------- | -------------------------------- | ----- |
| [00-concept.md](BEPAL/Docs/GDD/00-concept.md) | Game concept, core loop, scope   | ✅     |
| [01-core-loop.md](01-core-loop.md)            | Core loop & daily gameplay flow  | ✅     |
| [02-scope-features.md](02-scope-features.md)  | Scope ของ Feature ในเกม          | ✅     |
| [03-mechanics.md](03-mechanics.md)            | Mechanic flow & QTE formulas     | ✅     |
| [04-class-diagram.md](04-class-diagram.md)    | OOP Diagram & Architecture       | ✅     |
| [05-asset-list.md](05-asset-list.md)          | Asset list + asset pipeline flow | ✅     |


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
| `spr_` | Sprite / Texture | `spr_pet_idle.png`      |
| `sfx_` | Sound Effect     | `sfx_qte_success.wav`   |
| `bgm_` | Background Music | `bgm_shelter_cozy.mp3`  |
| `fnt_` | Font             | `fnt_ui_cozy.ttf`       |
| `dat_` | Data / Config    | `dat_pets.json`         |

## 📁 ใครดูแลส่วนไหน

| คนในทีม | รับผิดชอบ | โฟลเดอร์ staging / Source |
| --- | --- | --- |
| เดียร์ | 2D Art, Sprites & UI Graphics | `docs/02_Assets/_candidates/sprites/` |
| ภูมิ | Flex Design, Narrative & Audio (SFX/BGM) | `docs/02_Assets/_candidates/sfx/`, `music/` |
| ซุง | Game Design & Mechanics | `docs/02_Assets/_candidates/fonts/`, `data/` |
| โชว์ | Lead Gameplay Code & Architecture | `CoPoject/BePal/` |
