# Handoff — BePal: Canva whiteboard → Figma port

_Date: 2026-09-27 · Owner: SSHOWWEE_

## Goal
Port all content from the Canva whiteboard **"Your Story 2"** (https://canva.link/9gmznn1klt9hnn7, design ID `DAHRxmocJ6Q`) into the Figma file **Bepal** (https://www.figma.com/design/akrHzNmTSxOSfPKBU0V5kk/Bepal, page "White Board", page id `0:1`), rearranged freely but with no information lost.

## Status: Done (first full pass)
All text is rebuilt as editable Figma text (font: Kanit) in 8 top-level auto-layout frames, stacked vertically at x=0:

| Frame | Node ID | Contents |
|---|---|---|
| 01 · Game Overview — BePal | `5:2` | What is BePal, Game Pillar, Core Theme & Mood, Setting & World-Building, Character & Lore, Lore & Concept for art |
| 02 · Brainstorm — Team Ideas | `5:59` | One column per member: Zunk (Pitawan Hanghan), P0 (POOMIPAT TAMWONG), Dear_N, SShowwee — with "เพิ่มโดย" credits |
| 03 · Concept Design | `7:2` | Assignment brief, Theme/Concept/Idea, Mood Board, Competitor review (Cozy Grove / Spiritfarer / Strange Horticulture), 2D art-style guide |
| 04 · Early Prototype (v1) | `8:2` | v1 flow, Game rule, Day 1 scene notes, PET ROOM UI, INFO card template + Shewwee (cat) example |
| 05 · Game Loop (temp) | `8:85` | Core loop as steps, branches from Habitat Base, original loop diagram, "Not use" flowchart |
| 06 · Game Flow — Scenes 1–13 | `9:2` | Flow map + one card per scene: mockup, on-screen UI labels, programmer tasks, Art tasks |
| 07 · Game Flow (Event) — Scenes 14–20 | `10:2` | Event flow map + scene cards 14–20 and the unnumbered Upgrade branch |
| 08 · Original Canva Board — Reference | `10:170` | Full-board overview image (node `6:2`, 4096 px wide) |

Images (27) are raster crops from a 25000 px Canva PNG export, placed as fills on rectangles named `IMG:<key>` (S1–S20, QTEEX, BRAIN, MOOD, PROTO, RULE, LOOP, NOTUSE).

## Known issues / decisions
- **Scene 20** is titled "หน้า Upgrade" on the board but its mockup is the **Merchant** event — kept the title, added a note on the card.
- **Scene 19** programmer note is placeholder text on the board ("่า้า้่า") — copied as-is and flagged.
- **Unnumbered "Scene — หน้า Upgrade"** (3rd Event branch) duplicates Scene 12's text — kept and noted.
- Mood board / prototype / loop / "Not use" images are **low-res** (the whiteboard export is huge, those areas are small). Their text is transcribed in the cards anyway.
- **Game Loop crop** cuts off "Bro i" of the "Bro i forgot doctor / Sry" note (note is repeated in the card caption).
- Mockups are **flat images**, not editable Figma layers.
- Board typos were preserved intentionally (e.g. "Stomuch", "Attemp", "ลูกษร").

## How it was done (to repeat or extend)
1. Read whiteboard text via the Browser pane (Canva editor page text).
2. Exported the board with the Canva connector (`export-design`, PNG, width 25000). The cloud sandbox can't download `export-download.canva.com` (proxy blocked), so the image was opened in the Browser pane instead.
3. Built layouts with Figma `use_figma` (Plugin API, auto-layout cards).
4. Got upload URLs from Figma `upload_assets` (with `nodeIds` targeting the `IMG:` rectangles), then cropped each region in the browser with an OffscreenCanvas and POSTed it as `multipart/form-data` straight to the Figma submit URL (CORS allowed).

Export/upload URLs are short-lived; re-export and request new upload URLs if repeating.

## Suggested next steps
- Rebuild key mockups (Habitat Base, Care pet, QTE wheels) as editable Figma layers/components.
- Re-crop the Game Loop image to include the full side note.
- Add prototype links between scene cards to make the flow clickable.
- Split the board into Figma pages (Concept / Game Flow / Events) if it gets crowded.
