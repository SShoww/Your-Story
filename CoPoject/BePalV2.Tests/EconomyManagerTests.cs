using BePalV2.Gameplay;
using Xunit;

namespace BePalV2.Tests;

public class EconomyManagerTests
{
    [Fact]
    public void EconomyManager_StartsAt150Gold_NoDebt()
    {
        var eco = new EconomyManager();
        Assert.Equal(150, eco.Gold);
        Assert.Equal(0, eco.Debt);
        Assert.False(eco.HasDebt);
    }

    [Fact]
    public void DailySubsidy_Adds100Gold()
    {
        var eco = new EconomyManager();
        eco.ReceiveDailySubsidy();
        Assert.Equal(250, eco.Gold);
    }

    [Fact]
    public void SpendGold_EnforcesBalanceCheck()
    {
        var eco = new EconomyManager();
        Assert.True(eco.CanAfford(50));
        Assert.True(eco.SpendGold(50));
        Assert.Equal(100, eco.Gold);

        Assert.False(eco.CanAfford(200));
        Assert.False(eco.SpendGold(200));
        Assert.Equal(100, eco.Gold);
    }

    [Fact]
    public void EmergencyLoan_IncreasesGoldAndDebt_AndCompoundsInterest()
    {
        var eco = new EconomyManager();
        eco.IssueEmergencyLoan(500);

        Assert.Equal(650, eco.Gold); // 150 + 500
        Assert.Equal(500, eco.Debt);
        Assert.True(eco.HasDebt);

        // Day 1 interest: 500 * 0.20 = 100 -> Debt 600
        int interest1 = eco.CompoundDailyInterest(0.20f);
        Assert.Equal(100, interest1);
        Assert.Equal(600, eco.Debt);

        // Day 2 interest: 600 * 0.20 = 120 -> Debt 720
        int interest2 = eco.CompoundDailyInterest(0.20f);
        Assert.Equal(120, interest2);
        Assert.Equal(720, eco.Debt);
    }

    [Fact]
    public void RepayDebt_DecreasesBothGoldAndDebt()
    {
        var eco = new EconomyManager();
        eco.IssueEmergencyLoan(500);

        bool success = eco.RepayDebt(200);
        Assert.True(success);
        Assert.Equal(450, eco.Gold);
        Assert.Equal(300, eco.Debt);
    }
}
