#nullable enable

namespace BePal.Audio;

/// <summary>
/// Headless null implementation of <see cref="IAudioService"/> that silently ignores playback requests.
/// </summary>
public sealed class NullAudioService : IAudioService
{
    public static readonly NullAudioService Instance = new();

    public float MasterVolume { get; set; } = 1f;
    public bool IsMuted { get; set; }

    public void Play(SoundEffectType type, float volume = 1f, float pitch = 0f, float pan = 0f)
    {
        // No-op for headless testing and hardware-disabled environments.
    }

    public void PlayConfirm() => Play(SoundEffectType.Confirm);
    public void PlaySuccess() => Play(SoundEffectType.Success);
    public void PlayFail() => Play(SoundEffectType.Fail);
    public void PlayTeleport() => Play(SoundEffectType.Teleport);
    public void PlayWarning() => Play(SoundEffectType.Warning);
    public void PlayDodgeSuccess() => Play(SoundEffectType.DodgeSuccess);
    public void PlaySessionComplete() => Play(SoundEffectType.SessionComplete);
    public void PlayBoxOpen() => Play(SoundEffectType.BoxOpen);
    public void PlayTypewriter() => Play(SoundEffectType.Typewriter, 0.35f);
}
