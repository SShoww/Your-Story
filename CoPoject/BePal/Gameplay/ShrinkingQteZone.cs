#nullable enable
using System;

namespace BePal.Gameplay;

/// <summary>
/// Domain model for dynamic shrinking radial QTE zones with continuous needle rotation and 5-position cycling.
/// </summary>
public sealed class ShrinkingQteZone
{
    public const float Tau = MathF.PI * 2f;
    public const int PositionCount = 5;
    public const float DefaultSpeed = 2.2f;
    public const float DefaultInitialSpan = MathF.PI / 3f;
    public const float DefaultDuration = 2.8f;

    public static readonly float[] PresetAngles =
    {
        1.5f * MathF.PI, // Position 0: Top (270°)
        1.9f * MathF.PI, // Position 1: Top-Right (342°)
        0.3f * MathF.PI, // Position 2: Bottom-Right (54°)
        0.7f * MathF.PI, // Position 3: Bottom-Left (126°)
        1.1f * MathF.PI  // Position 4: Top-Left (198°)
    };

    private readonly Random _random;

    public float NeedleAngle { get; set; }
    public float NeedleSpeed { get; set; }
    public int CurrentPositionIndex { get; private set; }
    public float CenterAngle => PresetAngles[CurrentPositionIndex];
    public float InitialSpan { get; private set; }
    public float CurrentSpan { get; private set; }
    public float Duration { get; private set; }
    public float ElapsedTime { get; private set; }
    public bool IsExpired => ElapsedTime >= Duration || CurrentSpan <= 0f;

    public ShrinkingQteZone(
        float speed = DefaultSpeed,
        float initialSpan = DefaultInitialSpan,
        float duration = DefaultDuration,
        Random? random = null)
    {
        NeedleSpeed = speed;
        InitialSpan = initialSpan;
        Duration = duration;
        _random = random ?? new Random();
        CurrentSpan = initialSpan;
        CurrentPositionIndex = 0;
    }

    public void Update(float dt)
    {
        NeedleAngle = (NeedleAngle + NeedleSpeed * dt) % Tau;
        if (NeedleAngle < 0f) NeedleAngle += Tau;

        ElapsedTime = MathF.Min(Duration, ElapsedTime + dt);
        CurrentSpan = MathF.Max(0f, InitialSpan * (1f - ElapsedTime / Duration));
    }

    public bool IsNeedleInsideZone()
    {
        if (CurrentSpan <= 0f) return false;
        float diff = MathF.Abs(Wrap(NeedleAngle, CenterAngle));
        return diff <= CurrentSpan / 2f;
    }

    public void SpawnNewZone(int? forceIndex = null, float? newDuration = null)
    {
        if (newDuration.HasValue) Duration = newDuration.Value;

        if (forceIndex.HasValue)
        {
            CurrentPositionIndex = ((forceIndex.Value % PositionCount) + PositionCount) % PositionCount;
        }
        else
        {
            int offset = _random.Next(1, PositionCount);
            CurrentPositionIndex = (CurrentPositionIndex + offset) % PositionCount;
        }

        ElapsedTime = 0f;
        CurrentSpan = InitialSpan;
    }

    public static float Wrap(float a, float b)
    {
        float diff = (a - b + MathF.PI) % Tau;
        return (diff < 0 ? diff + Tau : diff) - MathF.PI;
    }
}
