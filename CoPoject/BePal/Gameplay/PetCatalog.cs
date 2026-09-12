#nullable enable
using System.Collections.Generic;

namespace BePal.Gameplay;

public static class PetCatalog
{
    public static IReadOnlyDictionary<PetKind, PetDefinition> Pets { get; } = new Dictionary<PetKind, PetDefinition>
    {
        [PetKind.Baseline] = new PetDefinition
        {
            Kind = PetKind.Baseline,
            Name = "Mossling",
            HazardLevel = 1,
            HarmType = HarmType.Physical,
            Pattern = new ActionPattern
            {
                PreferredAction = CareAction.Feed,
                PreferredSequence = new[] { CareAction.Feed },
                HasDodgeAttack = false,
                AttackThreshold = 0,
                HasTeleportingMarker = false,
                Description = "Steady wheel."
            }
        },
        [PetKind.Attacker] = new PetDefinition
        {
            Kind = PetKind.Attacker,
            Name = "Nibbleclaw",
            HazardLevel = 2,
            HarmType = HarmType.Physical,
            Pattern = new ActionPattern
            {
                PreferredAction = CareAction.Play,
                PreferredSequence = new[] { CareAction.Play },
                HasDodgeAttack = true,
                AttackThreshold = 2,
                HasTeleportingMarker = false,
                Description = "Attacks after two successes."
            }
        },
        [PetKind.Trickster] = new PetDefinition
        {
            Kind = PetKind.Trickster,
            Name = "Blinkbun",
            HazardLevel = 3,
            HarmType = HarmType.Mental,
            Pattern = new ActionPattern
            {
                PreferredAction = CareAction.Pet,
                PreferredSequence = new[] { CareAction.Pet },
                HasDodgeAttack = false,
                AttackThreshold = 0,
                HasTeleportingMarker = true,
                Description = "Marker teleports once."
            }
        }
    };

    public static PetDefinition Get(PetKind kind) => Pets[kind];
}
