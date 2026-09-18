#nullable enable
using System;
using System.Collections.Generic;

namespace BePal.Gameplay;

public sealed class QteSlot
{
    public int PositionIndex { get; set; }
    public CareAction? Action { get; set; }
    public float CenterAngle => ShrinkingQteZone.PresetAngles[PositionIndex];
    public float AppearTime { get; set; }
    public float Duration { get; set; }
    public float InitialSpan { get; set; }
    public float CurrentSpan { get; private set; }

    public QteSlot(
        int positionIndex,
        CareAction? action = null,
        float appearTime = 0f,
        float duration = ShrinkingQteZone.DefaultSlotDuration,
        float initialSpan = ShrinkingQteZone.DefaultInitialSpan)
    {
        PositionIndex = positionIndex;
        Action = action;
        AppearTime = appearTime;
        Duration = duration;
        InitialSpan = initialSpan;
        CurrentSpan = appearTime == 0f ? initialSpan : 0f;
    }

    public void Update(float totalElapsed)
    {
        if (totalElapsed < AppearTime)
        {
            CurrentSpan = 0f;
        }
        else
        {
            float activeElapsed = totalElapsed - AppearTime;
            if (activeElapsed >= Duration)
            {
                CurrentSpan = 0f;
            }
            else
            {
                CurrentSpan = MathF.Max(0f, InitialSpan * (1f - activeElapsed / Duration));
            }
        }
    }

    public bool IsFinished(float totalElapsed) => totalElapsed >= AppearTime + Duration;
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
    public const float DefaultStaggerInterval = 1.0f;
    public const float DefaultSlotDuration = 2.5f;

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
    public float CurrentSpan => Slots.Count > 0 ? Slots[0].CurrentSpan : 0f;
    public float Duration { get; set; }
    public float ElapsedTime { get; private set; }
    public bool IsExpired => ElapsedTime >= Duration || (Slots.Count > 0 && Slots.TrueForAll(s => s.IsFinished(ElapsedTime)));

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
        Slots.Add(new QteSlot(0, null, 0f, duration, initialSpan));
    }

    public void Update(float dt)
    {
        NeedleAngle = (NeedleAngle + NeedleSpeed * dt) % Tau;
        if (NeedleAngle < 0f) NeedleAngle += Tau;

        ElapsedTime = MathF.Min(Duration, ElapsedTime + dt);
        for (int i = 0; i < Slots.Count; i++)
        {
            Slots[i].Update(ElapsedTime);
        }
    }

    public QteSlot? GetHoveredSlot()
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            var slot = Slots[i];
            if (slot.CurrentSpan <= 0f) continue;
            float diff = MathF.Abs(Wrap(NeedleAngle, slot.CenterAngle));
            if (diff <= slot.CurrentSpan / 2f)
            {
                return slot;
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
        Slots.Clear();
        Slots.Add(new QteSlot(newIndex, null, 0f, Duration, InitialSpan));
    }

    public void SpawnSlots(CareAction[] actions, float staggerInterval = DefaultStaggerInterval, float slotDuration = DefaultSlotDuration)
    {
        ElapsedTime = 0f;
        Slots.Clear();

        int[] positions = { 0, 1, 2, 3, 4 };
        for (int i = positions.Length - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (positions[i], positions[j]) = (positions[j], positions[i]);
        }

        CareAction[] shuffledActions = (CareAction[])actions.Clone();
        for (int i = shuffledActions.Length - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (shuffledActions[i], shuffledActions[j]) = (shuffledActions[j], shuffledActions[i]);
        }

        int count = Math.Min(shuffledActions.Length, PositionCount);
        Duration = count > 0 ? (count - 1) * staggerInterval + slotDuration : DefaultDuration;

        for (int i = 0; i < count; i++)
        {
            float appearTime = i * staggerInterval;
            Slots.Add(new QteSlot(positions[i], shuffledActions[i], appearTime, slotDuration, InitialSpan));
        }
    }

    public static float Wrap(float a, float b)
    {
        float diff = (a - b + MathF.PI) % Tau;
        return (diff < 0 ? diff + Tau : diff) - MathF.PI;
    }
}
