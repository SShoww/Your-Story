namespace BePalV2.Gameplay;

public sealed class EnergyAccount
{
    public const int DefaultMaxEnergy = 6;
    public int MaxEnergy { get; private set; } = DefaultMaxEnergy;
    public int CurrentEnergy { get; private set; } = DefaultMaxEnergy;

    public bool CanSpend(int cost) => cost > 0 && CurrentEnergy >= cost;

    public bool Spend(int cost)
    {
        if (!CanSpend(cost)) return false;
        CurrentEnergy -= cost;
        return true;
    }

    public void Replenish()
    {
        CurrentEnergy = MaxEnergy;
    }

    public void AddBonus(int amount)
    {
        if (amount > 0)
        {
            CurrentEnergy += amount;
        }
    }
}
