namespace BePalV2.Gameplay;

public sealed class V2RunState
{
    public int DayNumber { get; private set; } = 1;
    public const int MaxDays = 3;

    public DailyPhase CurrentPhase { get; private set; } = DailyPhase.MorningEvent;
    public PetEntity ActivePet { get; private set; }
    public PetEntity? ToothlessPet { get; private set; }
    public bool HasToothlessAlly => ToothlessPet != null;

    public EnergyAccount Energy { get; } = new();
    public EconomyManager Economy { get; } = new();
    public InventoryService Inventory { get; } = new();

    public StoryEnding Ending { get; private set; } = StoryEnding.None;

    public void SetEnding(StoryEnding ending)
    {
        Ending = ending;
    }
    public bool IsRunComplete => Ending != StoryEnding.None || DayNumber > MaxDays;

    public int TotalCareSessionsCompleted { get; private set; }
    public int TotalPerfectAttempts { get; private set; }
    public int TotalGoodAttempts { get; private set; }
    public int TotalMissAttempts { get; private set; }

    // Last session results for Daily Summary
    public CareGrade LastCareGrade { get; set; } = CareGrade.B;
    public int LastGoldReward { get; set; } = 20;
    public int LastStomachLost { get; private set; }
    public int LastCleanLost { get; private set; }
    public float LastOvernightDamage { get; private set; }
    public float LastPhotosynthesisHeal { get; private set; }

    // Disaster & Encounter state
    public bool Day1CalmingCompleted { get; set; }
    public bool Day2EncounterResolved { get; set; }
    public bool Day2ToothlessTamed { get; set; }
    public bool Day3BossDefeated { get; set; }

    public V2RunState(PetSpecies starterSpecies = PetSpecies.Coco)
    {
        ActivePet = new PetEntity(starterSpecies);
    }

    public void SwitchActivePet(PetEntity pet)
    {
        ActivePet = pet;
    }

    public void UnlockToothless()
    {
        ToothlessPet = new PetEntity(PetSpecies.Toothless);
        Day2ToothlessTamed = true;
    }

    public void SetPhase(DailyPhase phase)
    {
        CurrentPhase = phase;
    }

    public void AdvanceFromMorningToCare()
    {
        if (CurrentPhase == DailyPhase.MorningEvent)
        {
            CurrentPhase = DailyPhase.CareAction;
        }
    }

    public void RecordCareSessionOutcome(CareQteEngine engine)
    {
        var (grade, reward) = engine.CalculateResults();
        LastCareGrade = grade;
        LastGoldReward = reward;
        Economy.AddGold(reward);
        TotalCareSessionsCompleted++;

        foreach (var tier in engine.History)
        {
            if (tier == PrecisionTier.Perfect) TotalPerfectAttempts++;
            else if (tier == PrecisionTier.Good) TotalGoodAttempts++;
            else TotalMissAttempts++;
        }

        // Check if AP is exhausted
        if (Energy.CurrentEnergy <= 0)
        {
            CurrentPhase = DailyPhase.DefenseResolution;
        }
    }

    public void ResolveEmergencyRevive()
    {
        const int fee = 500;
        if (Economy.CanAfford(fee))
        {
            Economy.SpendGold(fee);
        }
        else
        {
            Economy.IssueEmergencyLoan(fee);
        }
        ActivePet.Revive(50f);
        ToothlessPet?.Revive(50f);
    }

    public void CompleteDay1Calming(bool success)
    {
        Day1CalmingCompleted = true;
        if (success)
        {
            ActivePet.Heal(15f);
        }
        else
        {
            ActivePet.ApplyDisasterPenalties(stomachLoss: 10, cleanLoss: 25);
        }
        CurrentPhase = DailyPhase.ProgressionSummary;
    }

    public void ResolveDay2Encounter(bool chooseTame, bool tameSuccess)
    {
        Day2EncounterResolved = true;
        if (chooseTame && tameSuccess)
        {
            UnlockToothless();
        }
        CurrentPhase = DailyPhase.ProgressionSummary;
    }

    public void AcceptMerchantBuyout()
    {
        Economy.AddGold(5000);
        ToothlessPet = null;
        Ending = StoryEnding.EndingA_Betrayal;
    }

    public void CompleteMerchantBossFight(bool bossDefeated)
    {
        if (bossDefeated)
        {
            Day3BossDefeated = true;
            Economy.AddGold(500); // Boss bounty
            Ending = StoryEnding.EndingB_Protector;
        }
        CurrentPhase = DailyPhase.ProgressionSummary;
    }

    public void ApplyEndOfDayProgression()
    {
        var (sLost, cLost, dmg, heal) = ActivePet.ApplyDailyDecay(Inventory.EquippedItem);
        LastStomachLost = sLost;
        LastCleanLost = cLost;
        LastOvernightDamage = dmg;
        LastPhotosynthesisHeal = heal;

        ToothlessPet?.ApplyDailyDecay();

        // Debt compound interest
        Economy.CompoundDailyInterest(0.20f);

        // Check Day 3 Endings
        if (DayNumber >= MaxDays)
        {
            if (Ending == StoryEnding.None)
            {
                if (Economy.HasDebt && Economy.Gold < Economy.Debt)
                {
                    Ending = StoryEnding.EndingBad_Foreclosure;
                }
                else
                {
                    Ending = StoryEnding.EndingB_Protector;
                }
            }
        }
    }

    public void AdvanceToNextDay()
    {
        if (DayNumber >= MaxDays || Ending != StoryEnding.None)
        {
            return;
        }

        DayNumber++;
        Energy.Replenish();
        Economy.ReceiveDailySubsidy();
        Inventory.ResetDailyCombatBuffs();
        CurrentPhase = DailyPhase.MorningEvent;
    }
}
