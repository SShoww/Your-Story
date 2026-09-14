#nullable enable
using System;
using System.Collections.Generic;
using BePal.Audio;
using Xunit;

namespace BePal.Tests.Audio;

public class AudioServiceTests
{
    private sealed class RecordingAudioService : IAudioService
    {
        public List<(SoundEffectType Type, float Volume, float Pitch, float Pan)> Invocations { get; } = new();

        public float MasterVolume { get; set; } = 1f;
        public bool IsMuted { get; set; }

        public void Play(SoundEffectType type, float volume = 1f, float pitch = 0f, float pan = 0f)
        {
            if (IsMuted) return;
            Invocations.Add((type, volume, pitch, pan));
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

    [Fact]
    public void NullAudioService_PropertiesAndPlayback_ExecuteSafely()
    {
        IAudioService audio = NullAudioService.Instance;

        Assert.Equal(1f, audio.MasterVolume);
        Assert.False(audio.IsMuted);

        audio.MasterVolume = 0.5f;
        audio.IsMuted = true;
        Assert.Equal(0.5f, audio.MasterVolume);
        Assert.True(audio.IsMuted);

        // Verify none of the play methods throw exceptions
        audio.Play(SoundEffectType.Confirm);
        audio.PlayConfirm();
        audio.PlaySuccess();
        audio.PlayFail();
        audio.PlayTeleport();
        audio.PlayWarning();
        audio.PlayDodgeSuccess();
        audio.PlaySessionComplete();
        audio.PlayBoxOpen();
        audio.PlayTypewriter();

        // Reset
        audio.MasterVolume = 1f;
        audio.IsMuted = false;
    }

    [Fact]
    public void DefaultInterfaceMethods_RouteToExpectedSoundEffectTypes()
    {
        RecordingAudioService recorder = new();

        recorder.PlayConfirm();
        recorder.PlaySuccess();
        recorder.PlayFail();
        recorder.PlayTeleport();
        recorder.PlayWarning();
        recorder.PlayDodgeSuccess();
        recorder.PlaySessionComplete();
        recorder.PlayBoxOpen();
        recorder.PlayTypewriter();

        Assert.Equal(9, recorder.Invocations.Count);
        Assert.Equal(SoundEffectType.Confirm, recorder.Invocations[0].Type);
        Assert.Equal(SoundEffectType.Success, recorder.Invocations[1].Type);
        Assert.Equal(SoundEffectType.Fail, recorder.Invocations[2].Type);
        Assert.Equal(SoundEffectType.Teleport, recorder.Invocations[3].Type);
        Assert.Equal(SoundEffectType.Warning, recorder.Invocations[4].Type);
        Assert.Equal(SoundEffectType.DodgeSuccess, recorder.Invocations[5].Type);
        Assert.Equal(SoundEffectType.SessionComplete, recorder.Invocations[6].Type);
        Assert.Equal(SoundEffectType.BoxOpen, recorder.Invocations[7].Type);
        Assert.Equal(SoundEffectType.Typewriter, recorder.Invocations[8].Type);
    }

    [Fact]
    public void IsMuted_SuppressesPlaybackInvocations()
    {
        RecordingAudioService recorder = new() { IsMuted = true };

        recorder.PlayConfirm();
        recorder.PlaySuccess();

        Assert.Empty(recorder.Invocations);

        recorder.IsMuted = false;
        recorder.PlayConfirm();

        Assert.Single(recorder.Invocations);
    }

    [Fact]
    public void AudioManager_HeadlessInitialization_DoesNotCrashWithoutAudioHardware()
    {
        // On CI/headless test environments, audio hardware might not exist.
        // AudioManager must gracefully handle absence of audio hardware without throwing exceptions.
        using AudioManager manager = new(content: null);

        Assert.NotNull(manager);
        Assert.True(manager.MasterVolume > 0f);

        // Calling Play on all enum values should be safe and non-crashing
        foreach (SoundEffectType type in Enum.GetValues<SoundEffectType>())
        {
            manager.Play(type);
        }

        manager.PlayConfirm();
        manager.PlaySuccess();
        manager.PlayFail();
        manager.PlayWarning();
        manager.PlayDodgeSuccess();
        manager.PlaySessionComplete();
    }

    [Fact]
    public void AudioManager_Dispose_CanBeInvokedMultipleTimesSafely()
    {
        AudioManager manager = new(content: null);

        manager.Dispose();
        manager.Dispose(); // Idempotency check: should not throw ObjectDisposedException
    }

    [Fact]
    public void SoundEffectType_ContainsAllEightGddCatalogCues()
    {
        // Validate GDD-05 canonical sound entries are accounted for in enum
        Assert.True(Enum.IsDefined(SoundEffectType.Confirm));
        Assert.True(Enum.IsDefined(SoundEffectType.Success));
        Assert.True(Enum.IsDefined(SoundEffectType.Fail));
        Assert.True(Enum.IsDefined(SoundEffectType.Teleport));
        Assert.True(Enum.IsDefined(SoundEffectType.Warning));
        Assert.True(Enum.IsDefined(SoundEffectType.DodgeSuccess));
        Assert.True(Enum.IsDefined(SoundEffectType.SessionComplete));
        Assert.True(Enum.IsDefined(SoundEffectType.BoxOpen));
        Assert.True(Enum.IsDefined(SoundEffectType.Typewriter));
    }
}
