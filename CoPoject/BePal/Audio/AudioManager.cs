#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace BePal.Audio;

/// <summary>
/// MonoGame audio manager that plays sound effects with procedural fallback synthesis
/// and hardware-absence tolerance.
/// </summary>
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
            SoundEffect? effect = TryLoadFromContent(content, type);
            effect ??= TryCreateSynthetic(type);

            if (effect != null)
            {
                _effects[type] = effect;
            }
        }
    }

    private static SoundEffect? TryLoadFromContent(ContentManager? content, SoundEffectType type)
    {
        if (content == null) return null;

        string assetName = type switch
        {
            SoundEffectType.Confirm => "sfx/sfx_qte_confirm",
            SoundEffectType.Success => "sfx/sfx_qte_success",
            SoundEffectType.Fail => "sfx/sfx_qte_fail",
            SoundEffectType.Teleport => "sfx/sfx_qte_teleport",
            SoundEffectType.Warning => "sfx/sfx_dodge_warning",
            SoundEffectType.DodgeSuccess => "sfx/sfx_dodge_success",
            SoundEffectType.SessionComplete => "sfx/sfx_session_complete",
            SoundEffectType.BoxOpen => "sfx/sfx_box_open",
            SoundEffectType.Typewriter => "sfx/sfx_typewriter_click",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(assetName)) return null;

        try
        {
            return content.Load<SoundEffect>(assetName);
        }
        catch
        {
            // Content asset not yet loaded or not compiled; fallback to synthetic tone.
            return null;
        }
    }

    private SoundEffect? TryCreateSynthetic(SoundEffectType type)
    {
        if (!_hasAudioHardware) return null;

        try
        {
            const int sampleRate = 22050;
            byte[] pcm = type switch
            {
                SoundEffectType.Confirm => GenerateTone(0.08f, t => MathF.Sin(2 * MathF.PI * 750 * t), sampleRate),
                SoundEffectType.Success => GenerateTone(0.22f, t => MathF.Sin(2 * MathF.PI * (523.25f + 130f * (t / 0.22f)) * t), sampleRate),
                SoundEffectType.Fail => GenerateTone(0.25f, t => (MathF.Sin(2 * MathF.PI * 130 * t) > 0 ? 0.7f : -0.7f), sampleRate),
                SoundEffectType.Teleport => GenerateTone(0.20f, t => MathF.Sin(2 * MathF.PI * (300f + 500f * MathF.Sin(MathF.PI * t / 0.20f)) * t), sampleRate),
                SoundEffectType.Warning => GenerateTone(0.30f, t => MathF.Sin(2 * MathF.PI * (680f + 80f * (MathF.Sin(25 * t) > 0 ? 1 : -1)) * t), sampleRate),
                SoundEffectType.DodgeSuccess => GenerateTone(0.20f, t => MathF.Sin(2 * MathF.PI * (880f + 200f * (t / 0.20f)) * t), sampleRate),
                SoundEffectType.SessionComplete => GenerateTone(0.40f, t =>
                {
                    float freq = t < 0.12f ? 523.25f : t < 0.24f ? 659.25f : 783.99f;
                    return MathF.Sin(2 * MathF.PI * freq * t);
                }, sampleRate),
                SoundEffectType.BoxOpen => GenerateTone(0.18f, t => (MathF.Sin(2 * MathF.PI * 180 * t) * (1f - t / 0.18f)), sampleRate),
                SoundEffectType.Typewriter => GenerateTone(0.03f, t => (MathF.Sin(2 * MathF.PI * 1200 * t) * (1f - t / 0.03f)), sampleRate),
                _ => Array.Empty<byte>()
            };

            if (pcm.Length == 0) return null;
            return new SoundEffect(pcm, sampleRate, AudioChannels.Mono);
        }
        catch
        {
            // Audio subsystem unavailable or running in a headless test harness.
            _hasAudioHardware = false;
            return null;
        }
    }

    private static byte[] GenerateTone(float duration, Func<float, float> waveFunc, int sampleRate)
    {
        int numSamples = (int)(duration * sampleRate);
        byte[] buffer = new byte[numSamples * 2];
        for (int i = 0; i < numSamples; i++)
        {
            float t = (float)i / sampleRate;
            float rawVal = waveFunc(t);
            float clamped = Math.Clamp(rawVal, -1f, 1f);

            // Envelope: 5% attack ramp, 25% release decay to prevent audio pops
            float envelope = 1f;
            float attackSamples = numSamples * 0.05f;
            float releaseSamples = numSamples * 0.25f;

            if (i < attackSamples)
            {
                envelope = i / attackSamples;
            }
            else if (i > numSamples - releaseSamples)
            {
                envelope = (numSamples - i) / releaseSamples;
            }

            short sample = (short)(clamped * envelope * 22000f);
            buffer[i * 2] = (byte)(sample & 0xFF);
            buffer[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
        }
        return buffer;
    }

    public void Play(SoundEffectType type, float volume = 1f, float pitch = 0f, float pan = 0f)
    {
        if (IsMuted || !_hasAudioHardware) return;

        float effectiveVolume = Math.Clamp(volume * MasterVolume, 0f, 1f);
        if (effectiveVolume <= 0f) return;

        if (_effects.TryGetValue(type, out SoundEffect? effect) && effect != null)
        {
            try
            {
                effect.Play(effectiveVolume, Math.Clamp(pitch, -1f, 1f), Math.Clamp(pan, -1f, 1f));
            }
            catch
            {
                // Graceful fallback if device is detached mid-run
            }
        }
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

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        foreach (SoundEffect effect in _effects.Values)
        {
            try
            {
                effect.Dispose();
            }
            catch
            {
                // Ignored during shutdown
            }
        }
        _effects.Clear();
    }
}
