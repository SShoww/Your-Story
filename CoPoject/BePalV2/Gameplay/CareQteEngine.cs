namespace BePalV2.Gameplay;

public enum CareGrade
{
    S,
    A,
    B,
    C,
    F
}

public sealed class CareQteEngine
{
    public const float BaseAngularVelocity = 2.4f;
    public const float TargetCenterAngle = 1.5f * (float)Math.PI; // 270 deg (top)
    public const float BasePerfectWindow = 0.20f;
    public const float BaseGoodWindow = 0.45f;
    public const int TotalAttempts = 10;

    public CareActionType CurrentAction { get; }
    public PetEntity Pet { get; }
    public ItemDefinition? EquippedItem { get; }

    public float CurrentAngle { get; private set; }
    public float AngularVelocity { get; }
    public float PerfectWindow { get; }
    public float GoodWindow { get; }

    // Action-specific dynamic behaviors (Slides 15-20)
    public float RotationDirection { get; private set; } = 1.0f; // Feed: reverse rotation
    private float _directionTimer;

    public float TargetOffsetAngle { get; private set; } = 0f; // Clean: target zone moves away
    public float CurrentTargetCenterAngle => (TargetCenterAngle + TargetOffsetAngle) % (float)(2.0 * Math.PI);

    public bool IsTargetVisible { get; private set; } = true; // Heal: target zone flickers
    private float _blinkTimer;

    public int CurrentAttemptIndex { get; private set; } = 0; // 0 to 10
    public bool IsCompleted => CurrentAttemptIndex >= TotalAttempts;

    public int CurrentStreak { get; private set; }
    public int MaxStreak { get; private set; }
    public int TotalScore { get; private set; }
    public float DayProgressPercent { get; private set; }

    private readonly List<PrecisionTier> _history = new();
    public IReadOnlyList<PrecisionTier> History => _history;

    public CareQteEngine(PetEntity pet, CareActionType action, ItemDefinition? equipped = null)
    {
        Pet = pet;
        CurrentAction = action;
        EquippedItem = equipped;

        float speedMultiplier = pet.IsGrimy ? 1.15f : 1.0f;
        // Slide 18: Train (EXP) -> "เล็กและหมุนเร็ว" (Faster needle)
        if (action == CareActionType.Train)
        {
            speedMultiplier *= 1.2f;
        }
        AngularVelocity = BaseAngularVelocity * speedMultiplier;

        float perfectBonusMultiplier = 1.0f;
        if (pet.IsInfected)
        {
            perfectBonusMultiplier -= 0.30f;
        }
        if (equipped?.PerfectZoneBonus > 0f)
        {
            perfectBonusMultiplier += equipped.PerfectZoneBonus;
        }

        if (action == CareActionType.Train)
        {
            if (pet.Species == PetSpecies.Sproutlet)
            {
                // Sproutlet Agile Reflex trait: +15% wider window on Train
                perfectBonusMultiplier *= 1.15f;
            }
            else
            {
                perfectBonusMultiplier *= 0.85f; // Standard Train narrow window
            }
        }

        PerfectWindow = BasePerfectWindow * Math.Max(0.1f, perfectBonusMultiplier);
        GoodWindow = BaseGoodWindow;
    }

    public void SetAngle(float angle)
    {
        CurrentAngle = angle;
    }

    public void Update(float dt)
    {
        if (IsCompleted) return;

        // Slide 17: Feed -> Needle reverses rotation direction
        if (CurrentAction == CareActionType.Feed)
        {
            _directionTimer += dt;
            if (_directionTimer >= 1.6f)
            {
                _directionTimer = 0f;
                RotationDirection *= -1.0f;
            }
        }

        // Slide 19: Clean -> Target zone shifts and evades needle
        if (CurrentAction == CareActionType.Clean)
        {
            TargetOffsetAngle = (TargetOffsetAngle + 0.35f * dt) % (float)(2.0 * Math.PI);
        }

        // Slide 20: Heal -> Target zone blinks/flickers
        if (CurrentAction == CareActionType.Heal)
        {
            _blinkTimer += dt;
            if (_blinkTimer >= 0.35f)
            {
                _blinkTimer = 0f;
                IsTargetVisible = !IsTargetVisible;
            }
        }

        float tau = (float)(2.0 * Math.PI);
        CurrentAngle = (CurrentAngle + AngularVelocity * RotationDirection * dt) % tau;
        if (CurrentAngle < 0f) CurrentAngle += tau;
    }

    public PrecisionTier EvaluateAtCurrentAngle()
    {
        return EvaluateAngle(CurrentAngle);
    }

    public PrecisionTier EvaluateAngle(float angle)
    {
        float diff = Math.Abs(angle - CurrentTargetCenterAngle);
        float tau = (float)(2.0 * Math.PI);
        diff = Math.Min(diff, tau - diff);

        if (diff <= PerfectWindow) return PrecisionTier.Perfect;
        if (diff <= GoodWindow) return PrecisionTier.Good;
        return PrecisionTier.Miss;
    }

    public PrecisionTier RecordAttempt(PrecisionTier? overrideTier = null)
    {
        if (IsCompleted) return PrecisionTier.Miss;

        var tier = overrideTier ?? EvaluateAtCurrentAngle();
        _history.Add(tier);
        CurrentAttemptIndex++;

        switch (tier)
        {
            case PrecisionTier.Perfect:
                TotalScore += 150;
                CurrentStreak++;
                MaxStreak = Math.Max(MaxStreak, CurrentStreak);
                DayProgressPercent = Math.Min(100f, DayProgressPercent + 10f);
                break;

            case PrecisionTier.Good:
                TotalScore += 100;
                CurrentStreak++;
                MaxStreak = Math.Max(MaxStreak, CurrentStreak);
                DayProgressPercent = Math.Min(100f, DayProgressPercent + 10f);
                break;

            case PrecisionTier.Miss:
                CurrentStreak = 0;
                DayProgressPercent = Math.Min(100f, DayProgressPercent + 5f);
                break;
        }

        return tier;
    }

    public int GetStreakBonus() => MaxStreak * 25;

    public int GetFinalScore() => TotalScore + GetStreakBonus();

    public (CareGrade grade, int goldReward) CalculateResults()
    {
        int score = GetFinalScore();
        if (score >= 1200) return (CareGrade.S, 50);
        if (score >= 900) return (CareGrade.A, 35);
        if (score >= 600) return (CareGrade.B, 20);
        if (score >= 300) return (CareGrade.C, 10);
        return (CareGrade.F, 0);
    }
}
