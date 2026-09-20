using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace BePalV2.Audio;

public sealed class AudioManager : IAudioService, IDisposable
{
    private readonly Dictionary<SoundEffectType, SoundEffect> _effects = new();
    private bool _hasAudioHardware = true;
    private bool _disposed;

    public float MasterVolume { get; set; } = 0.85f;
    public bool IsMuted { get; set; }

    public AudioManager(ContentManager? content = null)
    {
        InitializeEffects(content);
    }

    private void InitializeEffects(ContentManager? content)
    {
        foreach (SoundEffectType type in Enum.GetValues<SoundEffectType>())
        {
            try
            {
                var effect = TryCreateSynthetic(type);
                if (effect != null)
                {
                    _effects[type] = effect;
                }
            }
            catch
            {
                _hasAudioHardware = false;
                break;
            }
        }
    }

    private SoundEffect? TryCreateSynthetic(SoundEffectType type)
    {
        try
        {
            const int sampleRate = 22050;
            byte[] data = type switch
            {
                SoundEffectType.Confirm => GenerateTone(0.08f, t => (float)Math.Sin(2 * Math.PI * 880 * t), sampleRate),
                SoundEffectType.Success => GenerateTone(0.18f, t => (float)Math.Sin(2 * Math.PI * (523 + 200 * t) * t), sampleRate),
                SoundEffectType.Fail => GenerateTone(0.25f, t => (float)Math.Sin(2 * Math.PI * (220 - 60 * t) * t), sampleRate),
                SoundEffectType.Warning => GenerateTone(0.20f, t => (float)Math.Sin(2 * Math.PI * 660 * t), sampleRate),
                SoundEffectType.DodgeSuccess => GenerateTone(0.14f, t => (float)Math.Sin(2 * Math.PI * (600 + 400 * t) * t), sampleRate),
                SoundEffectType.CounterSuccess => GenerateTone(0.20f, t => (float)Math.Sin(2 * Math.PI * 990 * t), sampleRate),
                SoundEffectType.ParrySuccess => GenerateTone(0.25f, t => (float)Math.Sin(2 * Math.PI * 1200 * t), sampleRate),
                SoundEffectType.SessionComplete => GenerateTone(0.35f, t => (float)Math.Sin(2 * Math.PI * 784 * t), sampleRate),
                SoundEffectType.BoxOpen => GenerateTone(0.12f, t => (float)(new Random((int)(t * 10000)).NextDouble() * 2.0 - 1.0), sampleRate),
                SoundEffectType.Typewriter => GenerateTone(0.03f, t => (float)Math.Sin(2 * Math.PI * 1400 * t), sampleRate),
                SoundEffectType.Coins => GenerateTone(0.15f, t => (float)Math.Sin(2 * Math.PI * 1046 * t), sampleRate),
                _ => GenerateTone(0.05f, t => (float)Math.Sin(2 * Math.PI * 440 * t), sampleRate)
            };

            return new SoundEffect(data, sampleRate, AudioChannels.Mono);
        }
        catch
        {
            return null;
        }
    }

    private static byte[] GenerateTone(float duration, Func<float, float> waveFunc, int sampleRate)
    {
        int samples = (int)(duration * sampleRate);
        byte[] buffer = new byte[samples * 2];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = 1f - (t / duration); // linear fade out
            float sample = Math.Clamp(waveFunc(t) * envelope, -1f, 1f);
            short pcm = (short)(sample * short.MaxValue * 0.45f);
            buffer[i * 2] = (byte)(pcm & 0xFF);
            buffer[i * 2 + 1] = (byte)((pcm >> 8) & 0xFF);
        }
        return buffer;
    }

    public void Play(SoundEffectType type, float volume = 1f, float pitch = 0f, float pan = 0f)
    {
        if (IsMuted || !_hasAudioHardware || _effects.Count == 0) return;

        try
        {
            if (_effects.TryGetValue(type, out var effect))
            {
                float finalVol = Math.Clamp(volume * MasterVolume, 0f, 1f);
                effect.Play(finalVol, Math.Clamp(pitch, -1f, 1f), Math.Clamp(pan, -1f, 1f));
            }
        }
        catch
        {
            // Tolerate audio failure
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        foreach (var effect in _effects.Values)
        {
            effect.Dispose();
        }
        _effects.Clear();
    }
}
