#nullable enable
using System;
using System.Collections.Generic;

namespace BePal.Gameplay;

public sealed class QteSlot
{
    public int PositionIndex { get; set; }
    public CareAction? Action { get; set; }
    public float CenterAngle => ShrinkingQteZone.PresetAngles[PositionIndex];

    public QteSlot(int positionIndex, CareAction? action = null)
    {
        PositionIndex = positionIndex;
        Action = action;
    }
}

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
    public List<QteSlot> Slots { get; } = new();
    public int CurrentPositionIndex => Slots.Count > 0 ? Slots[0].PositionIndex : 0;
    public float CenterAngle => Slots.Count > 0 ? Slots[0].CenterAngle : PresetAngles[0];
    public float InitialSpan { get; set; }
    public float CurrentSpan { get; private set; }
    public float Duration { get; set; }
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
        Slots.Add(new QteSlot(0));
    }

    public void Update(float dt)
    {
        NeedleAngle = (NeedleAngle + NeedleSpeed * dt) % Tau;
        if (NeedleAngle < 0f) NeedleAngle += Tau;

        ElapsedTime = MathF.Min(Duration, ElapsedTime + dt);
        CurrentSpan = MathF.Max(0f, InitialSpan * (1f - ElapsedTime / Duration));
    }

    public QteSlot? GetHoveredSlot()
    {
        if (CurrentSpan <= 0f) return null;
        for (int i = 0; i < Slots.Count; i++)
        {
            float diff = MathF.Abs(Wrap(NeedleAngle, Slots[i].CenterAngle));
            if (diff <= CurrentSpan / 2f)
            {
                return Slots[i];
            }
        }
        return null;
    }

    public bool IsNeedleInsideZone() => GetHoveredSlot() != null;

    public void SpawnNewZone(int? forceIndex = null, float? newDuration = null)
    {
        if (newDuration.HasValue) Duration = newDuration.Value;

        int newIndex;
        if (forceIndex.HasValue)
        {
            newIndex = ((forceIndex.Value % PositionCount) + PositionCount) % PositionCount;
        }
        else
        {
            int currentIdx = Slots.Count > 0 ? Slots[0].PositionIndex : 0;
            int offset = _random.Next(1, PositionCount);
            newIndex = (currentIdx + offset) % PositionCount;
        }

        ElapsedTime = 0f;
        CurrentSpan = InitialSpan;
        Slots.Clear();
        Slots.Add(new QteSlot(newIndex));
    }

    public void SpawnSlots(CareAction[] actions)
    {
        ElapsedTime = 0f;
        CurrentSpan = InitialSpan;
        Slots.Clear();

        int[] positions = { 0, 1, 2, 3, 4 };
        for (int i = positions.Length - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (positions[i], positions[j]) = (positions[j], positions[i]);
        }

        int count = Math.Min(actions.Length, PositionCount);
        for (int i = 0; i < count; i++)
        {
            Slots.Add(new QteSlot(positions[i], actions[i]));
        }
    }

    public static float Wrap(float a, float b)
    {
        float diff = (a - b + MathF.PI) % Tau;
        return (diff < 0 ? diff + Tau : diff) - MathF.PI;
    }
}
