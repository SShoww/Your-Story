#nullable enable

namespace BePal.Audio;

/// <summary>
/// Canonical sound effect identifiers defined in GDD-05.
/// </summary>
public enum SoundEffectType
{
    /// <summary>Spacebar click confirmation on wheel QTE.</summary>
    Confirm,

    /// <summary>Correct care action match (+1 Satisfaction).</summary>
    Success,

    /// <summary>Action mismatch or dead-zone hit (-1 HP).</summary>
    Fail,

    /// <summary>Blinkbun needle erratic warp sound.</summary>
    Teleport,

    /// <summary>Pet attack alert siren / Dodge warning.</summary>
    Warning,

    /// <summary>Successful evasive maneuver in Dodge Zone.</summary>
    DodgeSuccess,

    /// <summary>Satisfaction reached 3, pet session completed.</summary>
    SessionComplete,

    /// <summary>Delivery crate unboxing lid opening.</summary>
    BoxOpen,

    /// <summary>Typewriter text ticking sound.</summary>
    Typewriter
}
