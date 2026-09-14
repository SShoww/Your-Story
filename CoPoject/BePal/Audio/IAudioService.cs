#nullable enable

namespace BePal.Audio;

/// <summary>
/// Service interface governing sound effect and audio feedback playback.
/// </summary>
public interface IAudioService
{
    /// <summary>Global master volume between 0.0 (silent) and 1.0 (full).</summary>
    float MasterVolume { get; set; }

    /// <summary>Whether all audio output is currently muted.</summary>
    bool IsMuted { get; set; }

    /// <summary>
    /// Plays the requested sound effect with optional volume scaling, pitch shift, and stereo panning.
    /// </summary>
    void Play(SoundEffectType type, float volume = 1f, float pitch = 0f, float pan = 0f);

    void PlayConfirm() => Play(SoundEffectType.Confirm);
    void PlaySuccess() => Play(SoundEffectType.Success);
    void PlayFail() => Play(SoundEffectType.Fail);
    void PlayTeleport() => Play(SoundEffectType.Teleport);
    void PlayWarning() => Play(SoundEffectType.Warning);
    void PlayDodgeSuccess() => Play(SoundEffectType.DodgeSuccess);
    void PlaySessionComplete() => Play(SoundEffectType.SessionComplete);
    void PlayBoxOpen() => Play(SoundEffectType.BoxOpen);
    void PlayTypewriter() => Play(SoundEffectType.Typewriter, 0.35f);
}
