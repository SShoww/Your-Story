# BePal — Session Handoff Document

**Date:** 2026-09-24  
**Target Branch:** `Develop`  
**Current Status:** Production architecture **BePalV2 (`CoPoject/BePalV2`)** implementing canonical New GDD (v2.0): 3-day high-density loop, 6 AP energy budget, starter pets (Coco, Sproutlet, Gloomtail), 10-attempt Care QTE with dynamic gimmicks, Toothless combat taming, 3-phase Merchant boss battle, CleanUI borderless theme, and 56 passing unit tests in `BePalV2.Tests`.

---

## 1. Summary of Completed Work

### 1.1. Core Production Architecture (`BePalV2`)
- **Primary Production Game:** `CoPoject/BePalV2` (MonoGame DesktopGL + CleanUI borderless theme + `MonoGame.Extended`).
- **Domain Decoupling:** Pure C# domain in `CoPoject/BePalV2/Gameplay/` with zero MonoGame dependencies.
- **Unit Test Suite:** 56 passing xUnit unit tests in `CoPoject/BePalV2.Tests/` verifying state machines, care QTE engine, combat engine, energy accounts, and economy.
- **Automated Visual Regression QA:** `--screenshot` harness captures 10 canonical PNG frames in `screenshots/v2/`.

### 1.2. Legacy Prototype (`BePal` V1)
- Preserved under `CoPoject/BePal/` and `CoPoject/BePal.Tests/` (83 tests) as historical regression reference.
- **Rule for AI Agents:** Do NOT modify `CoPoject/BePal` unless explicitly directed. All new features and fixes target `CoPoject/BePalV2`.

---

## 2. Codebase Health & Verification Commands

- **Build Solution:**
  ```bash
  dotnet build CoPoject/CoPoject.slnx
  ```
- **Run V2 Production Tests (Fast, ~0.3s):**
  ```bash
  dotnet test CoPoject/BePalV2.Tests
  ```
- **Run All Tests (V1 + V2, 139 tests):**
  ```bash
  dotnet test CoPoject/CoPoject.slnx
  ```
- **Run V2 Screenshot Harness:**
  ```bash
  dotnet run --project CoPoject/BePalV2 -- --screenshot
  ```

---

## 3. Gitflow Reminders for Next Agent

- **Always branch off `Develop`** using `feature/<topic>`.
- **Do not commit directly to `main` or `Develop`**.
- Merge back into `Develop` via Pull Request or local non-fast-forward merge (`--no-ff`).
- Maintain 0 build warnings, 100% pass rate on `dotnet test CoPoject/BePalV2.Tests`, and clean screenshot harness before completing feature merges.
