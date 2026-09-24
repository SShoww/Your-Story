namespace BePalV2.Gameplay;

public sealed class PetEntity
{
    public PetSpecies Species { get; }
    public string Name { get; } = "";

    public float MaxHealth { get; private set; }
    public float Health { get; private set; }
    public int Stomach { get; private set; }
    public int Clean { get; private set; }

    public int Level { get; private set; } = 1;
    public int CurrentProgress { get; private set; } = 0;
    public int MaxProgress => 100 + (Level - 1) * 50;

    public int CurrentExp => CurrentProgress;
    public int MaxExp => MaxProgress;

    public float CounterDamageBonus { get; private set; } = 0f;

    public bool HasAcidBurn { get; set; }

    public bool IsStarving => Stomach <= 0;
    public bool IsGrimy => Clean < 50;
    public bool IsInfected => Clean < 25;
    public bool IsIncapacitated => Health <= 0;

    public PetEntity(PetSpecies species)
    {
        Species = species;
        switch (species)
        {
            case PetSpecies.Coco:
                Name = "Coco (Mossling)";
                MaxHealth = 100f;
                Health = 100f;
                Stomach = 80;
                Clean = 70;
                break;
            case PetSpecies.Sproutlet:
                Name = "Sproutlet";
                MaxHealth = 90f;
                Health = 90f;
                Stomach = 70;
                Clean = 80;
                break;
            case PetSpecies.Gloomtail:
                Name = "Gloomtail";
                MaxHealth = 110f;
                Health = 110f;
                Stomach = 60;
                Clean = 60;
                break;
            case PetSpecies.Toothless:
                Name = "Toothless";
                MaxHealth = 100f;
                Health = 60f;
                Stomach = 50;
                Clean = 60;
                break;
        }
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;
        Health = Math.Clamp(Health - amount, 0f, MaxHealth);
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || IsIncapacitated) return;
        Health = Math.Clamp(Health + amount, 0f, MaxHealth);
    }

    public void Revive(float restoredHealth = 1f)
    {
        Health = Math.Clamp(restoredHealth, 1f, MaxHealth);
        HasAcidBurn = false;
    }

    public void FeedDirect(int amount)
    {
        Stomach = Math.Clamp(Stomach + amount, 0, 100);
    }

    public void CleanDirect(int amount)
    {
        Clean = Math.Clamp(Clean + amount, 0, 100);
    }

    public void AddProgress(int pp, float multiplier = 1.0f)
    {
        int adjusted = (int)Math.Round(pp * multiplier, MidpointRounding.AwayFromZero);
        if (adjusted <= 0) return;
        CurrentProgress += adjusted;

        while (CurrentProgress >= MaxProgress && Level < 10)
        {
            CurrentProgress -= MaxProgress;
            Level++;
            if (Level % 2 == 0)
            {
                MaxHealth += 10f;
                Health = Math.Clamp(Health + 10f, 0f, MaxHealth);
            }
            else
            {
                CounterDamageBonus += 0.10f;
            }
        }
    }

    public void AddExp(int exp, float multiplier = 1.0f)
    {
        AddProgress(exp, multiplier);
    }

    public bool CanTrain()
    {
        return Stomach >= 20 && !IsStarving && !IsIncapacitated;
    }

    public void ExecuteCareAction(CareActionType action, PrecisionTier tier, ItemDefinition? equipped = null, float expMultiplier = 1.0f)
    {
        if (IsIncapacitated) return;

        int extraBurn = equipped?.ActionStomachBurnExtra ?? 0;

        // Apply metabolic burn for non-feed actions
        if (action != CareActionType.Feed)
        {
            int burn = (action == CareActionType.Train ? 10 : 5) + extraBurn;
            Stomach = Math.Max(0, Stomach - burn);
        }

        // Apply starving action penalty
        if (IsStarving)
        {
            TakeDamage(2f);
        }

        switch (action)
        {
            case CareActionType.Feed:
                if (tier == PrecisionTier.Perfect)
                {
                    Stomach = Math.Min(100, Stomach + 40);
                    AddExp(30, expMultiplier);
                }
                else if (tier == PrecisionTier.Good)
                {
                    Stomach = Math.Min(100, Stomach + 25);
                    AddExp(15, expMultiplier);
                }
                break;

            case CareActionType.Clean:
                if (tier == PrecisionTier.Perfect)
                {
                    Clean = Math.Min(100, Clean + 40);
                    AddExp(30, expMultiplier);
                }
                else if (tier == PrecisionTier.Good)
                {
                    Clean = Math.Min(100, Clean + 25);
                    AddExp(15, expMultiplier);
                }
                break;

            case CareActionType.Train:
                if (tier == PrecisionTier.Perfect)
                {
                    AddExp(70, expMultiplier);
                }
                else if (tier == PrecisionTier.Good)
                {
                    AddExp(40, expMultiplier);
                }
                else
                {
                    AddExp(10, expMultiplier);
                }
                break;

            case CareActionType.Heal:
                if (tier == PrecisionTier.Perfect)
                {
                    Heal(35f);
                    AddExp(25, expMultiplier);
                }
                else if (tier == PrecisionTier.Good)
                {
                    Heal(20f);
                    AddExp(15, expMultiplier);
                }
                break;
        }
    }

    public (int stomachLost, int cleanLost, float overnightHpDamage, float photosynthHeal) ApplyDailyDecay(ItemDefinition? equipped = null)
    {
        int stomachLoss = 20;
        float cleanDecayFactor = 1.0f - (equipped?.CleanDecayReduction ?? 0f);
        int cleanLoss = (int)Math.Round(15f * cleanDecayFactor, MidpointRounding.AwayFromZero);

        Stomach = Math.Max(0, Stomach - stomachLoss);
        Clean = Math.Max(0, Clean - cleanLoss);

        float damage = 0f;
        if (IsStarving) damage += 10f;
        if (IsGrimy) damage += 5f;
        if (IsInfected) damage += 15f;

        if (damage > 0f)
        {
            TakeDamage(damage);
        }

        float photoHeal = 0f;
        if (Species == PetSpecies.Coco && Clean > 80 && !IsIncapacitated)
        {
            photoHeal = 5f;
            Heal(photoHeal);
        }

        return (stomachLoss, cleanLoss, damage, photoHeal);
    }

    public void ApplyDisasterPenalties(int stomachLoss, int cleanLoss)
    {
        Stomach = Math.Max(0, Stomach - stomachLoss);
        Clean = Math.Max(0, Clean - cleanLoss);
    }
}
