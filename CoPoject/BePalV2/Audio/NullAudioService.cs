namespace BePalV2.Audio;

public sealed class NullAudioService : IAudioService
{
    public float MasterVolume { get; set; } = 1.0f;
    public bool IsMuted { get; set; } = false;

    public void Play(SoundEffectType type, float volume = 1f, float pitch = 0f, float pan = 0f)
    {
    }
}
