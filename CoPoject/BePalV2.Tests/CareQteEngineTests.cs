using BePalV2.Gameplay;
using Xunit;

namespace BePalV2.Tests;

public class CareQteEngineTests
{
    [Fact]
    public void EvaluateAngle_ClassifiesPerfectGoodMissCorrectly()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        var engine = new CareQteEngine(pet, CareActionType.Feed);

        float center = CareQteEngine.TargetCenterAngle; // 1.5 * PI (~4.712)

        // Exact center is Perfect
        Assert.Equal(PrecisionTier.Perfect, engine.EvaluateAngle(center));

        // Center + 0.15 rad is within Perfect window (0.20)
        Assert.Equal(PrecisionTier.Perfect, engine.EvaluateAngle(center + 0.15f));

        // Center + 0.30 rad is outside Perfect (0.20) but within Good (0.45)
        Assert.Equal(PrecisionTier.Good, engine.EvaluateAngle(center + 0.30f));

        // Center + 0.60 rad is outside Good -> Miss
        Assert.Equal(PrecisionTier.Miss, engine.EvaluateAngle(center + 0.60f));
    }

    [Fact]
    public void GrimyPet_IncreasesNeedleSpeedBy15Percent()
    {
        var cleanPet = new PetEntity(PetSpecies.Coco);
        var engineClean = new CareQteEngine(cleanPet, CareActionType.Feed);

        var grimyPet = new PetEntity(PetSpecies.Coco);
        grimyPet.ApplyDisasterPenalties(stomachLoss: 0, cleanLoss: 30); // Clean 70 - 30 = 40 (<50)
        Assert.True(grimyPet.IsGrimy);

        var engineGrimy = new CareQteEngine(grimyPet, CareActionType.Feed);

        Assert.Equal(CareQteEngine.BaseAngularVelocity, engineClean.AngularVelocity);
        Assert.Equal(CareQteEngine.BaseAngularVelocity * 1.15f, engineGrimy.AngularVelocity, precision: 4);
    }

    [Fact]
    public void InfectedPet_ReducesPerfectWindowBy30Percent()
    {
        var infectedPet = new PetEntity(PetSpecies.Coco);
        infectedPet.ApplyDisasterPenalties(stomachLoss: 0, cleanLoss: 55); // Clean 70 - 55 = 15 (<25)
        Assert.True(infectedPet.IsInfected);

        var engine = new CareQteEngine(infectedPet, CareActionType.Feed);
        Assert.Equal(CareQteEngine.BasePerfectWindow * 0.70f, engine.PerfectWindow, precision: 4);
    }

    [Fact]
    public void Sproutlet_TrainAction_ExpandsPerfectWindowBy15Percent()
    {
        var sproutlet = new PetEntity(PetSpecies.Sproutlet);
        var engineTrain = new CareQteEngine(sproutlet, CareActionType.Train);
        var engineFeed = new CareQteEngine(sproutlet, CareActionType.Feed);

        Assert.True(engineTrain.PerfectWindow > engineFeed.PerfectWindow);
        Assert.Equal(CareQteEngine.BasePerfectWindow * 1.15f, engineTrain.PerfectWindow, precision: 4);
    }

    [Fact]
    public void BalletShoes_ExpandsPerfectWindow()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        var shoes = ItemDefinition.BalletShoes;
        var engine = new CareQteEngine(pet, CareActionType.Clean, equipped: shoes);

        Assert.Equal(CareQteEngine.BasePerfectWindow * 1.15f, engine.PerfectWindow, precision: 4);
    }

    [Fact]
    public void TenAttempts_CompletesEngineSession_AndCalculatesGrade()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        var engine = new CareQteEngine(pet, CareActionType.Feed);
        engine.SetAngle(CareQteEngine.TargetCenterAngle);

        // Record 10 attempts manually at target center
        for (int i = 0; i < 10; i++)
        {
            Assert.False(engine.IsCompleted);
            Assert.Equal(i, engine.CurrentAttemptIndex);
            var tier = engine.RecordAttempt();
            Assert.Equal(PrecisionTier.Perfect, tier);
        }

        Assert.True(engine.IsCompleted);
        Assert.Equal(10, engine.CurrentAttemptIndex);
        Assert.Equal(10, engine.MaxStreak);
        Assert.Equal(1500, engine.TotalScore);
        Assert.Equal(250, engine.GetStreakBonus()); // 10 * 25
        Assert.Equal(1750, engine.GetFinalScore()); // 1500 + 250

        var (grade, gold) = engine.CalculateResults();
        Assert.Equal(CareGrade.S, grade);
        Assert.Equal(50, gold);
    }
}
