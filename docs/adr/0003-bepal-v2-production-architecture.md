# 3. BePalV2 Production Architecture (New GDD v2.0)

Date: 2026-09-24

## Status

Accepted (Supersedes ADR 0002 for V2 Production)

## Context

Following playtesting of the V1 Samsara 4-wall prototype, the team transitioned to the canonical **New GDD (v2.0)** design:
1. Shifted from a 5-day loose loop to a high-density 3-day narrative loop with 6 AP energy budget.
2. Evolved from 3 HP player life to 4-tier pet virtual pet stats: Health (0-100), Stomach (0-100), Clean (0-100), EXP/Level.
3. Upgraded Care QTE to a 10-attempt session with dynamic gimmicks (reverse rotation, escaping zone, shrinking sectors, blinking needle) and S/A/B/C/F grade calculation.
4. Added real-time combat & taming arena (`CombatArenaScreen`) with Clean < 50 combat refusal, Toothless taming, and 3-phase Merchant boss battle.
5. Modernized UI with borderless CleanUI theme and 8-slot inventory grid.

## Decision

1. **BePalV2 Primary Production Project**: Establish `CoPoject/BePalV2` as the canonical production game and `CoPoject/BePalV2.Tests` as the primary test suite.
2. **Base Habitat Screen**: Adopt a unified widescreen base shelter hub (`BaseHabitatScreen`) integrating interactive stations (Care hotkeys, Door, Upgrade Station, Doctor Clinic, Survival Desk, Inventory overlay, Merchant Shop modal) rather than 4-wall panoramic spinning.
3. **Pure C# Domain Isolation**: Maintain strict zero-dependency separation in `CoPoject/BePalV2/Gameplay/` (`V2RunState`, `PetEntity`, `CareQteEngine`, `CombatEngine`, `EnergyAccount`, `EconomyManager`, `InventoryService`).
4. **CleanUI Design System**: All screens leverage `CleanUI.cs` design tokens and `MonoGame.Extended` drawing primitives.
5. **Legacy Preservation**: Maintain `CoPoject/BePal` (V1) solely as a legacy regression reference without active modifications.

## Consequences

- Direct traceability to `BEPAL/Docs/NewGDD/` and `BEPAL/Docs/NewAgile/`.
- Fast, deterministic unit tests (56 tests in <0.3s).
- Unambiguous target for AI agents and human developers.
