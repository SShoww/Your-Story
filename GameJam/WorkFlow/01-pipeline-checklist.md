---
type: jam-pipeline
version: 0.1
date: [21/7/2026]
team: [Your Story]
---
# Pipeline Function Checklist — [Necro Life]

> เกมทุกแนวต้องมี pipeline ขั้นต่ำนี้ก่อนถึงจะเรียกว่า "เล่นได้" — เติมชื่อผู้รับผิดชอบและติ๊กสถานะระหว่างลงมือทำจริง เพิ่มแถวได้ถ้าเกมของทีมต้องการ module อื่น

## Must-Have (ต้องมีก่อน Hour 24 — Playable Build)

| Module (Unity Engine & Game Design)         | หน้าที่                                                                                                                                                | สถานะ     | ผู้รับผิดชอบ |
| :------------------------------------------ | :------------------------------------------------------------------------------------------------------------------------------------------------------------ | :------------- | :----------------------- |
| **Scene & State Manager**             | สลับ Scene (Text Story / 2D Platformer / Pause / GameOver) ผ่าน `SceneManager`                                                                      | 🔲 Not Started | [SShowwee]               |
| **Unity Input System**                | รับ Input รวมศูนย์ (UI Navigation สำหรับ Text, Keyboard/Gamepad สำหรับ 2D Action)                                                      | 🔲 Not Started | [SShowwee]               |
| **Story & Choice Data Manager**       | ระบบอ่าน Text/JSON/ScriptableObject เพื่อแสดงผลเหตุการณ์และตัวเลือก (Core Mechanic เกมหลัก)                     | 🔲 Not Started | [SShowwee]               |
| **2D Physics & Character Controller** | `Update()` / `FixedUpdate()` ควบคุมตัวละคร 2D และจัดการ `Rigidbody2D` / `Collider2D` สำหรับตรวจชนและ Platforming | 🔲 Not Started | [SShowwee]               |
| **UI Canvas & Cinemachine**           | Render ผ่าน `Canvas` (UI Text, Buttons) และใช้ `Cinemachine` คุมกล้อง 2D (รองรับ Responsive Resolution)                           | 🔲 Not Started | [SShowwee]               |
| **Condition & Event Trigger**         | ตรวจสอบ Win/Lose (HP หมดใน 2D, หรือเลือก Choice จบเกม)                                                                              | 🔲 Not Started | [SShowwee]               |
| **Unity Asset Management**            | โหลด Asset ผ่าน Inspector Reference,`Resources.Load`, หรือ Addressables                                                                         | 🔲 Not Started | [SShowwee]               |

## Nice-to-Have (ทำถ้าเหลือเวลา — Hour 24–34)

| Module                                      | หน้าที่                                                                                       | สถานะ     | ผู้รับผิดชอบ |
| :------------------------------------------ | :--------------------------------------------------------------------------------------------------- | :------------- | :----------------------- |
| **AudioManager (SFX พื้นฐาน)** | เล่นเสียงผ่าน `AudioSource` (เสียงกดปุ่ม, เอฟเฟกต์ต่อสู้ 2D) | 🔲 Not Started | [ชื่อ]               |
| **UI / HUD (Dynamic)**                | แสดงแถบ HP, ไอเทม หรือสถานะตัวละครระหว่างเล่น Text และ 2D  | 🔲 Not Started | [ชื่อ]               |
| **BGM Manager**                       | สลับเพลงประกอบระหว่างฉากเล่าเรื่องกับฉากแอ็กชัน       | 🔲 Not Started | [ชื่อ]               |

## Cut-List (ตัดทิ้งก่อนถ้าเวลาไม่พอ — ห้ามเริ่มก่อน Must-Have เสร็จ)

* ❌ **Save/Load** (PlayerPrefs / JSON)
* ❌ **Settings menu** (ปรับเสียง / ควบคุม)
* ❌ **Leaderboard**
* ❌ [feature อื่นที่ทีมตกลงตัดก่อน]

> เมื่อถึง **Feature Freeze (Hour 34)** ทุก module ในตารางนี้ต้องมีสถานะ ✅ Done หรือถูกย้ายไป Cut-List อย่างชัดเจน — ห้ามค้างเป็น 🔲/🟡
