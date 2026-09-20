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
        if (pet.Species == PetSpecies.Sproutlet && action == CareActionType.Train)
        {
            perfectBonusMultiplier += 0.15f;
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
        CurrentAngle = (CurrentAngle + AngularVelocity * dt) % (float)(2.0 * Math.PI);
        if (CurrentAngle < 0f) CurrentAngle += (float)(2.0 * Math.PI);
    }

    public PrecisionTier EvaluateAtCurrentAngle()
    {
        return EvaluateAngle(CurrentAngle);
    }

    public PrecisionTier EvaluateAngle(float angle)
    {
        float diff = Math.Abs(angle - TargetCenterAngle);
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
                DayProgressPercent = Math.Min(100f, DayProgressPercent + 15f);
                break;
        }

        return tier;
    }

    public int GetStreakBonus() => MaxStreak * 25;

    public int GetFinalScore() => TotalScore + GetStreakBonus();

    public (CareGrade grade, int goldReward) CalculateResults()
    {
        int finalScore = GetFinalScore();
        if (finalScore >= 1400) return (CareGrade.S, 50);
        if (finalScore >= 1100) return (CareGrade.A, 35);
        if (finalScore >= 800)  return (CareGrade.B, 20);
        if (finalScore >= 500)  return (CareGrade.C, 10);
        return (CareGrade.F, 0);
    }
}
