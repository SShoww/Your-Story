namespace BePalV2.Audio;

public interface IAudioService
{
    float MasterVolume { get; set; }
    bool IsMuted { get; set; }

    void Play(SoundEffectType type, float volume = 1f, float pitch = 0f, float pan = 0f);

    void PlayConfirm() => Play(SoundEffectType.Confirm);
    void PlaySuccess() => Play(SoundEffectType.Success);
    void PlayFail() => Play(SoundEffectType.Fail);
    void PlayWarning() => Play(SoundEffectType.Warning);
    void PlayDodgeSuccess() => Play(SoundEffectType.DodgeSuccess);
    void PlayCounterSuccess() => Play(SoundEffectType.CounterSuccess);
    void PlayParrySuccess() => Play(SoundEffectType.ParrySuccess);
    void PlaySessionComplete() => Play(SoundEffectType.SessionComplete);
    void PlayBoxOpen() => Play(SoundEffectType.BoxOpen);
    void PlayTypewriter() => Play(SoundEffectType.Typewriter, 0.35f);
    void PlayCoins() => Play(SoundEffectType.Coins);
}
