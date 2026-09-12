#nullable enable
using System;

namespace BePal.Gameplay;

public sealed class ActionPattern
{
    public CareAction[] PreferredSequence { get; init; } = Array.Empty<CareAction>();
    public CareAction PreferredAction { get; init; }
    public bool HasDodgeAttack { get; init; }
    public int AttackThreshold { get; init; }
    public bool HasTeleportingMarker { get; init; }
    public string Description { get; init; } = string.Empty;
}
