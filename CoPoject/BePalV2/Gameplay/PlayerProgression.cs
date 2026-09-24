namespace BePalV2.Gameplay;

public sealed class PlayerProgression
{
    public int Level { get; private set; } = 1;
    public int CurrentExp { get; private set; } = 0;
    public int MaxExp => 10 + (Level - 1) * 10;
    public int SkillPoints { get; private set; } = 0;

    public int QteFocusLevel { get; private set; } = 0;
    public int ProgressBoosterLevel { get; private set; } = 0;
    public int EnergyCapacityLevel { get; private set; } = 0;

    public float QteWindowBonus => QteFocusLevel * 0.15f;
    public float ProgressBoosterMultiplier => 1.0f + (ProgressBoosterLevel * 0.50f);

    public void AddExp(int amount)
    {
        if (amount <= 0) return;
        CurrentExp += amount;
        while (CurrentExp >= MaxExp)
        {
            CurrentExp -= MaxExp;
            Level++;
            SkillPoints++;
        }
    }

    public bool SpendSkillPoint(int amount = 1)
    {
        if (amount <= 0 || SkillPoints < amount) return false;
        SkillPoints -= amount;
        return true;
    }

    public bool UpgradeQteFocus()
    {
        if (!SpendSkillPoint(1)) return false;
        QteFocusLevel++;
        return true;
    }

    public bool UpgradeProgressBooster()
    {
        if (!SpendSkillPoint(1)) return false;
        ProgressBoosterLevel++;
        return true;
    }

    public bool UpgradeEnergyCapacity()
    {
        if (!SpendSkillPoint(1)) return false;
        EnergyCapacityLevel++;
        return true;
    }
}
