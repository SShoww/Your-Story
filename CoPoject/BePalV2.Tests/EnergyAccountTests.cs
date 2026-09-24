using BePalV2.Gameplay;
using Xunit;

namespace BePalV2.Tests;

public class EnergyAccountTests
{
    [Fact]
    public void EnergyAccount_StartsAtSixEnergy()
    {
        var energy = new EnergyAccount();
        Assert.Equal(6, energy.CurrentEnergy);
        Assert.Equal(6, energy.MaxEnergy);
    }

    [Fact]
    public void Spend_DecreasesCurrentEnergy_WhenAffordable()
    {
        var energy = new EnergyAccount();
        Assert.True(energy.CanSpend(2));
        Assert.True(energy.Spend(2));
        Assert.Equal(4, energy.CurrentEnergy);
    }

    [Fact]
    public void Spend_ReturnsFalse_WhenCostExceedsEnergy()
    {
        var energy = new EnergyAccount();
        energy.Spend(5);
        Assert.Equal(1, energy.CurrentEnergy);
        Assert.False(energy.CanSpend(2));
        Assert.False(energy.Spend(2));
        Assert.Equal(1, energy.CurrentEnergy);
    }

    [Fact]
    public void Replenish_RestoresEnergyToMax()
    {
        var energy = new EnergyAccount();
        energy.Spend(6);
        Assert.Equal(0, energy.CurrentEnergy);
        energy.Replenish();
        Assert.Equal(6, energy.CurrentEnergy);
    }

    [Fact]
    public void AddBonus_AddsExtraEnergy()
    {
        var energy = new EnergyAccount();
        energy.AddBonus(2);
        Assert.Equal(8, energy.CurrentEnergy);
    }
    [Fact]
    public void UpgradeMaxEnergy_IncreasesMaxAndCurrentEnergy_AndPersistsThroughReplenish()
    {
        var energy = new EnergyAccount();
        energy.UpgradeMaxEnergy(2);
        Assert.Equal(8, energy.MaxEnergy);
        Assert.Equal(8, energy.CurrentEnergy);

        energy.Spend(8);
        Assert.Equal(0, energy.CurrentEnergy);

        energy.Replenish();
        Assert.Equal(8, energy.CurrentEnergy);
    }
}
