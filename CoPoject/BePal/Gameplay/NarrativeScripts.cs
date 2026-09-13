#nullable enable
using System.Collections.Generic;

namespace BePal.Gameplay;

/// <summary>
/// Provides narrative story scripts for Day 1 Prologue and daily morning Doorstep deliveries.
/// Written in English first per project specifications.
/// </summary>
public static class NarrativeScripts
{
    public static IReadOnlyList<string> GetPrologueLines() => new[]
    {
        "The dawn chill clings to the windowpanes of your newly established abnormal creature daycare.",
        "Ordinary shelters care for stray dogs and abandoned cats. Your facility holds a classified license for the unregistered.",
        "*DING-DONG* ... The sharp electric door chime echoes through the barren corridor.",
        "By the time you reach the porch, the courier vehicle has vanished down the mist-shrouded road.",
        "Sitting on your doormat is a heavy pine crate wrapped with yellow industrial hazard seal tape.",
        "A manifest slip is taped to the lid: 'Subject: MOSSLING (Hazard Lv 1: Baseline harm). Prefers damp soil feed.'",
        "You pry the wooden slats open... Curled inside moist peat moss, two obsidian eyes blink up at you.",
        "Welcome to the daycare, Mossling. Your first day begins."
    };

    public static IReadOnlyList<string> GetDoorstepLines(int dayNumber, PetKind pet) => pet switch
    {
        PetKind.Baseline => new[]
        {
            $"Day {dayNumber} begins. The morning air is crisp and smelling faintly of petrichor.",
            "Another crate has arrived on your doorstep from the unknown research sponsor.",
            "Mossling sits patiently inside, its mossy coat damp and calm.",
            "Let us bring the specimen into the habitat and start the morning routine."
        },
        PetKind.Attacker => new[]
        {
            $"Day {dayNumber} begins with harsh scraping noises resonating from the porch.",
            "A heavy reinforced steel crate sits outside, scored with deep claw gouges.",
            "Subject Manifest: 'NIBBLECLAW (Hazard Lv 2: Aggressive). High recreation drive. Attacks if frustrated.'",
            "Sharp amber eyes peer through the iron slats. Keep your reflexes sharp today!"
        },
        PetKind.Trickster => new[]
        {
            $"Day {dayNumber} begins. The atmosphere hums with static electricity and the smell of ozone.",
            "The crate on the doorstep seems to subtly vibrate, its edges phasing in and out of focus.",
            "Subject Manifest: 'BLINKBUN (Hazard Lv 3: Trickster). Spatial instability detected. Demands gentle intimacy.'",
            "Be patient and watchful. This creature bends the rules of space itself."
        },
        _ => new[]
        {
            $"Day {dayNumber} begins. A fresh containment crate awaits on your doorstep.",
            "Bring the specimen into the shelter to begin today's care routine."
        }
    };
}
