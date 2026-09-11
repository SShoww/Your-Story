# 1. Screen Management and Care QTE Architecture

Date: 2026-09-11

## Status

Accepted

## Context

The BePal game design requires a 5-day simulation cycle where a player cares for abnormal pets in a research shelter/daycare. The game includes:
1. Navigating between multiple views (Main Menu, Help, Main Pet Room, Care QTE, Dodge QTE, Survival Log overlay, and Run Summary).
2. A 4-action Care QTE wheel (Feed, Play, Pet, Observe) with a rotating marker and spacebar confirmation.
3. Abnormal pet mechanics including attack triggers (Dodge QTE with a gold safe zone) and erratic marker movement (Teleporting Marker).
4. Health (3 HP) tracking with Forced Retreat upon reaching 0 HP.
5. A Survival Log unlocking detailed Action Patterns after 3 completed sessions.

Previously, prototype logic and rendering were co-located in a monolithic `Game1.cs`, and design documentation contained generic platformer templates.

## Decision

1. **Screen Decomposition**: Decompose user interfaces into focused screen classes implementing an `IScreen` lifecycle (`Update` and `Draw`). `Game1` retains responsibility only for framework setup, graphics device configuration, and top-level screen transitions.
2. **Domain Model Extraction**: Separate game state (`PrototypeRun`), pet definitions (`PetDefinition`), actions (`CareAction`: Feed, Play, Pet, Observe), and action patterns (`ActionPattern`) into pure C# classes under `Gameplay/` decoupled from MonoGame render loops.
3. **Deterministic Testing**: Support unit testing in a future `BePal.Tests` project by keeping all state transitions, damage calculations, and session rules testable without a graphics device.
4. **Alignment with Canva Design**: Establish the core design pillars "Cozy yet Dangerous", "Consequential Interaction", and 4-tier Pet Favor (+2, +1, +0, Rejection) across all design specifications.

## Consequences

- Improved maintainability and testability 6 months out.
- High cohesion and adherence to repository guidelines in `AGENTS.md` and `CONTEXT.md`.
- Easy onboarding for team members (Show, Zunk, Poom, Dear) to contribute gameplay features, art, and audio independently.
