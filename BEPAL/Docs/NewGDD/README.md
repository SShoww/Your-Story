---
type: README
version: 2.0
date:
  - 2026-09-20
---

# [BePal] — Documentation Index (v2.0)

## 👥คนในทีม

| รหัส | ชื่อ | ชื่อเล่น | บทบาทหน้าที่ |
| --- | --- | --- | --- |
| 682110141 | วศิน ศรีวรกุล | โชว์ | Lead Programmer |
| 682110128 | ปีย์ตะวัน แห่งหาญ | ซุง | Flex |
| 682110137 | ภูมิพัฒน์ ตามวงค์ | ภูมิ | Game Designer & UI Lead |
| 682110119 | ธัญญรัตน์ ติ๊บหน่อ | เดียร์ | 2D Art |

| ไฟล์ | เนื้อหา | สถานะ |
| --- | --- | --- |
| [00-concept.md](00-concept.md) | Game concept, core pillars, starter pets, and narrative arc | ✅ |
| [01-core-loop.md](01-core-loop.md) | 4-phase daily loop, scene breakdown, and controls mapping | ✅ |
| [02-scope-features.md](02-scope-features.md) | Scope, MoSCoW feature priorities, and risk mitigation | ✅ |
| [03-mechanics.md](03-mechanics.md) | Pet stats, 6 AP energy, 10-attempt QTE, encounters, and boss battle | ✅ |
| [04-class-diagram.md](04-class-diagram.md) | OOP class diagram, MonoGame architecture, and JSON save schema | ✅ |
| [05-asset-list.md](05-asset-list.md) | Master 2D art, UI badges, audio SFX/BGM, and font checklist | ✅ |

---

## 🏷️ Naming Convention

**Asset:** ดูตารางเต็มใน [05-asset-list.md](05-asset-list.md)

| Prefix | ประเภท |
| --- | --- |
| `spr_` | Sprite / Texture |
| `sfx_` | Sound Effect |
| `bgm_` | Background Music |
| `fnt_` | Font |
| `dat_` | Data / Config |

**เอกสาร:** ไฟล์ใน `BEPAL/Docs/NewGDD/` เรียงลำดับด้วย prefix ตัวเลข 2 หลัก (`00-`, `01-`, ..., `05-`) สอดคล้องกับโครงสร้างมาตรฐานของโปรเจกต์อย่างเคร่งครัด

---

## Asset Naming Convention

| Prefix | ประเภท | ตัวอย่าง |
| --- | --- | --- |
| `spr_` | Sprite / Texture | `spr_coco_idle.png`, `spr_merchant_boss.png` |
| `sfx_` | Sound Effect | `sfx_qte_perfect.wav`, `sfx_acid_spit.wav` |
| `bgm_` | Background Music | `bgm_base_shelter.mp3`, `bgm_merchant_boss.mp3` |
| `fnt_` | Font | `fnt_title_horror.ttf`, `fnt_ui_cozy.ttf` |
| `dat_` | Data / Config | `dat_pet_species.json`, `dat_shop_items.json` |

---

## 📁 ใครดูแลส่วนไหน

| คนในทีม | รับผิดชอบ | โฟลเดอร์ staging / Source |
| --- | --- | --- |
| เดียร์ | 2D Art, Sprites (Pets & Boss), Environment Backgrounds | `docs/02_Assets/_candidates/sprites/` |
| ภูมิ | Game Design, Numbers Balance, Narrative Scripts & UI Lead | `docs/02_Assets/_candidates/data/`, `ui/` |
| ซุง | Flex Support, Audio Production (SFX/BGM) & QA Playtesting | `docs/02_Assets/_candidates/sfx/`, `music/` |
| โชว์ | Lead Gameplay Code, Architecture & Automated QA | `CoPoject/BePalV2/`, `CoPoject/BePalV2.Tests/` |

---

## Executive Summary & Comparison Matrix: GDD v1 vs GDD v2

เอกสารชุด **GDD v2.0** สังเคราะห์จากชุดข้อมูลนำเสนอ 64 สไลด์ โดยยกระดับจากเอกสารต้นแบบเดิม (GDD v1) อย่างเป็นรูปธรรม:

| มิติการออกแบบ | Legacy GDD (v1) | New GDD (v2) | ประโยชน์และผลลัพธ์ที่ยกระดับ |
| --- | --- | --- | --- |
| **ขอบเขตการเล่น (Scope)** | ลูป 5 วันแบบหลวมๆ วนรับสัตว์แปลกหน้าประตู | **3-Day High-Density Vertical Slice** ที่มีโครงเรื่องเข้มข้น มีจุดเริ่มต้น จุดวิกฤต และไคลแมกซ์ชัดเจน | Pacing กระชับ สนุก ตื่นเต้น เหมาะแก่การนำเสนอและทดสอบ Alpha/Demo |
| **ระบบสเตตัสสัตว์เลี้ยง** | มีเพียง Health 3 แต้ม และ Satisfaction Bar 3 แต้ม | **สเตตัสเสมือนจริง 4 มิติ:** Health (0–100), Stomach (0–100), Clean (0–100), และ Level/EXP | มอบความรู้สึกของ Virtual Pet สมจริง ผูกพันและต้องใส่ใจดูแลรอบด้าน |
| **ทรัพยากรและการบริหาร** | เล่นได้เรื่อยๆ ไม่จำกัดครั้งจนกว่าเลือดจะหมด | **Discrete Energy Budget (6 AP/วัน)** จัดสรรการกระทำอย่างมีกลยุทธ์ | เกิดการวางแผน Resource Management ที่ท้าทาย ทุกแอ็กชันมีความหมาย |
| **ระบบมินิเกม Care QTE** | หมุนเข็มสุ่มเลือก 1 ใน 4 ช่อง (Feed, Play, Pet, Observe) | **10-Attempt Session** แยกตามหมวดหมู่ (Feed, Clean, Train, Heal) พร้อมคำนวณ Perfect/Good/Miss | ควบคุมจังหวะได้แม่นยำ ท้าทายฝีมือผู้เล่น และมีระบบสะสมคะแนนสตรีค |
| **สูตรเวลา (Day Progress)** | เวลาไม่เดินหน้าตามผลลัพธ์ | **Day Progress Scaling:** สำเร็จ $+10\%$, พลาด $+15\%$ (เวลาเร่งเร็วขึ้นเมื่อพลาด) | เพิ่มความกดดันทางอารมณ์และสะท้อนความตื่นตระหนกได้อย่างแยบยล |
| **ระบบการต่อสู้ (Combat)** | มีเพียง Dodge QTE รับการโจมตีแบบตั้งรับ | **ระบบต่อสู้เต็มรูปแบบ:** มี Boss HP, Telegraphs, Dodge Zones, และ Counter-Attack | มีความตื่นเต้นแบบเกมแอ็กชัน สามารถสยบสัตว์และต่อสู้ป้องกันบ้านได้ |
| **เนื้อเรื่องและทางเลือก** | รับกล่องเปิดดูสัตว์ ไม่มีการตัดสินใจเชิงจริยธรรม | **Moral Dilemmas:** ทางเลือกขับไล่หรือสยบ Toothless, ทางเลือกขายสัตว์เลี้ยง 5,000G หรือสู้บอส | เพิ่มคุณค่าการเล่นซ้ำ (Replayability) และสร้างผลกระทบทางอารมณ์ |
| **ระบบเศรษฐกิจและไอเทม** | ไม่มีระบบเงินและร้านค้า | **ระบบเศรษฐกิจ Gold สมบูรณ์แบบ:** เงินสนับสนุน, ค่ารักษา, ร้านค้า 5 ชนิด, กระเป๋า 8 ช่อง และอุปกรณ์สวมใส่ | เพิ่มความลึกในการวางแผนการเงินและความหลากหลายของบิลด์สัตว์เลี้ยง |

---

## Guidelines for Developers & Designers

1. **สำหรับ Game Designer & UI Lead (ภูมิ):**
   - ใช้ค่าตัวเลขและสูตรคำนวณใน `03-mechanics.md` เป็นฐานในการ Balance ตัวเลข และออกแบบโครงสร้าง UI/HUD/Dialogue
   - ติดตามบทสนทนาและทางเลือกเนื้อเรื่องใน `01-core-loop.md` และ `03-mechanics.md`
2. **สำหรับ Lead Programmer (โชว์):**
   - ศึกษาสถาปัตยกรรมและแผนภาพคลาสใน `04-class-diagram.md`
   - พัฒนาโมเดลในโฟลเดอร์ `CoPoject/BePalV2/Gameplay/` ให้เป็น Pure C# เพื่อให้ผ่านชุดทดสอบใน `CoPoject/BePalV2.Tests/`
3. **สำหรับ Flex (ซุง):**
   - จัดหาและตรวจสอบไฟล์เสียงตามรายการใน `05-asset-list.md`
   - ดำเนินการทดสอบ Playtesting QA ระบบ และดูแลไฟล์ฟอนต์/Config สนับสนุนทีม
4. **สำหรับ 2D Artist (เดียร์):**
   - ยึดสัดส่วนความละเอียดและรายการภาพตามที่ระบุใน `05-asset-list.md`
   - ออกแบบชุดสไปรต์สัตว์เลี้ยงให้มีอารมณ์ Idle, Happy, Angry, Attack/Teleport และ Hurt เพื่อรองรับ Status Effects
