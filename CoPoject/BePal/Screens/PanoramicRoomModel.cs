#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using BePal.Gameplay;
using Microsoft.Xna.Framework;

namespace BePal.Screens;

/// <summary>
/// Represents an interactive item in the 4-wall panoramic shelter room.
/// </summary>
public sealed record InspectableItem(
    string Id,
    string Name,
    Rectangle Bounds,
    int WallIndex,
    string Header,
    string[] DescriptionLines);

/// <summary>
/// Domain model managing panoramic room wall rotation, inspectable items, and behavior cues.
/// </summary>
public class PanoramicRoomModel
{
    public const int TotalWalls = 4;

    public int CurrentWallIndex { get; private set; }

    private static readonly InspectableItem[] AllItems =
    {
        // Wall 0: Pet Zone
        new("pet", "Active Abnormal Pet", new Rectangle(530, 165, 220, 290), 0, "PET INTERACTION",
            new[] { "The creature rests quietly in its habitat.", "Click to interact or begin care." }),
        new("cradle", "Moss Bed Cradle", new Rectangle(460, 420, 360, 80), 0, "HABITAT CRADLE",
            new[] { "A reinforced pet bed lined with damp peat moss and soil.", "Designed to minimize stress for anomalous organisms." }),

        // Wall 1: Prep & Pantry
        new("pantry", "Pantry Shelf", new Rectangle(300, 160, 260, 260), 1, "PANTRY SHELVES",
            new[] { "Glass canisters filled with moist dark soil and organic root cuttings.", "A subtle herbal aroma drifts from the shelves. Mossling seems drawn to earthy feed." }),
        new("water", "Fresh Water Station", new Rectangle(620, 260, 180, 180), 1, "WATER BASIN",
            new[] { "A heavy ceramic basin filled with clean, filtered water.", "Adequate hydration is critical for specimen stability." }),
        new("disposal", "Disposal Receptacle", new Rectangle(860, 320, 160, 220), 1, "DISPOSAL BIN",
            new[] { "Discarded synthetic nutrient pellets, chewed and thoroughly rejected.", "Whoever is staying here refuses processed laboratory kibble." }),

        // Wall 2: Study Desk
        new("log", "Survival Log Desk", new Rectangle(320, 240, 280, 200), 2, "RESEARCH NOTEBOOK",
            new[] { "A weathered research journal with handwritten behavioral observations.", "Click to open the Survival Log." }),
        new("board", "Notice Board", new Rectangle(680, 120, 320, 220), 2, "BULLETIN BOARD",
            new[] { "CONFIDENTIAL: Daycare Subject Protocol.", "1. Maintain specimen physical and mental stability.", "2. Log all behavioral anomalies before dusk." }),

        // Wall 3: Front Door & Entrance
        new("door", "Front Entrance Door", new Rectangle(500, 120, 280, 420), 3, "HEAVY OAK DOOR",
            new[] { "A reinforced oak door with heavy steel deadbolts.", "Locked securely from the inside. Deliveries only arrive at sunrise." }),
        new("window", "Porch Window", new Rectangle(220, 160, 200, 240), 3, "SHELTER WINDOW",
            new[] { "Heavy fog blankets the gravel path outside.", "No courier or vehicle is in sight. The outside world remains eerily still." }),
        new("clock", "Shift Clock & Calendar", new Rectangle(860, 180, 200, 160), 3, "FACILITY CALENDAR",
            new[] { "Research Daycare Shift Monitor.", "Maintain continuous observation until daily requirements are fulfilled." })
    };

    public void RotateRight()
    {
        CurrentWallIndex = (CurrentWallIndex + 1) % TotalWalls;
    }

    public void RotateLeft()
    {
        CurrentWallIndex = (CurrentWallIndex - 1 + TotalWalls) % TotalWalls;
    }

    public void SetWall(int wallIndex)
    {
        CurrentWallIndex = Math.Clamp(wallIndex, 0, TotalWalls - 1);
    }

    public string GetWallTitle(int wallIndex) => wallIndex switch
    {
        0 => "WALL 1: PET ZONE - MOSS CRADLE",
        1 => "WALL 2: PREP & PANTRY - FEEDING STATION",
        2 => "WALL 3: STUDY DESK - RESEARCH LOGS",
        3 => "WALL 4: FRONT DOOR & SHIFT CONTROL",
        _ => "SHELTER INTERIOR"
    };

    public IReadOnlyList<InspectableItem> GetItemsForWall(int wallIndex) =>
        AllItems.Where(item => item.WallIndex == wallIndex).ToList();

    public static string GetPetBehaviorCue(PetKind pet) => pet switch
    {
        PetKind.Baseline => "Behavior Cue: The Mossling's belly grumbles softly with an appetite for moist feed.",
        PetKind.Attacker => "Behavior Cue: Nibbleclaw paces restlessly, claws twitching with recreation energy.",
        PetKind.Trickster => "Behavior Cue: Blinkbun trembles gently, eyes dilated seeking soothing intimacy.",
        _ => "The creature appears calm."
    };
}
