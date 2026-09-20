using BePalV2.Gameplay;
using Xunit;

namespace BePalV2.Tests;

public class V2RunStateTests
{
    [Fact]
    public void InitialState_IsDayOne_MorningEvent()
    {
        var run = new V2RunState(PetSpecies.Coco);
        Assert.Equal(1, run.DayNumber);
        Assert.Equal(DailyPhase.MorningEvent, run.CurrentPhase);
        Assert.Equal(6, run.Energy.CurrentEnergy);
        Assert.Equal(150, run.Economy.Gold);
        Assert.False(run.HasToothlessAlly);
        Assert.Equal(StoryEnding.None, run.Ending);
    }

    [Fact]
    public void CareSession_DrainsEnergy_AndAPExhaustionTransitionsToDefense()
    {
        var run = new V2RunState(PetSpecies.Coco);
        run.AdvanceFromMorningToCare();
        Assert.Equal(DailyPhase.CareAction, run.CurrentPhase);

        var engine = new CareQteEngine(run.ActivePet, CareActionType.Feed);
        engine.SetAngle(CareQteEngine.TargetCenterAngle);
        for (int i = 0; i < 10; i++) engine.RecordAttempt();

        run.Energy.Spend(6); // Exhaust all AP
        run.RecordCareSessionOutcome(engine);

        Assert.Equal(DailyPhase.DefenseResolution, run.CurrentPhase);
        Assert.True(run.Economy.Gold > 150); // Received reward
    }

    [Fact]
    public void Day1Calming_RestoresHealthAndAdvancesToSummary()
    {
        var run = new V2RunState(PetSpecies.Coco);
        run.ActivePet.TakeDamage(20f);

        run.CompleteDay1Calming(success: true);
        Assert.True(run.Day1CalmingCompleted);
        Assert.Equal(DailyPhase.ProgressionSummary, run.CurrentPhase);
        Assert.Equal(95f, run.ActivePet.Health); // 80 + 15
    }

    [Fact]
    public void Day2ToothlessTamed_UnlocksToothlessAlly()
    {
        var run = new V2RunState(PetSpecies.Coco);
        run.ResolveDay2Encounter(chooseTame: true, tameSuccess: true);

        Assert.True(run.Day2EncounterResolved);
        Assert.True(run.HasToothlessAlly);
        Assert.NotNull(run.ToothlessPet);
        Assert.Equal(PetSpecies.Toothless, run.ToothlessPet.Species);
    }

    [Fact]
    public void Day3MerchantBuyout_TriggersEndingA()
    {
        var run = new V2RunState(PetSpecies.Coco);
        run.UnlockToothless();
        Assert.True(run.HasToothlessAlly);

        run.AcceptMerchantBuyout();
        Assert.Equal(StoryEnding.EndingA_Betrayal, run.Ending);
        Assert.Equal(5150, run.Economy.Gold); // 150 + 5000
        Assert.False(run.HasToothlessAlly); // Toothless sold
        Assert.True(run.IsRunComplete);
    }

    [Fact]
    public void Day3BossVictory_TriggersEndingB()
    {
        var run = new V2RunState(PetSpecies.Coco);
        run.CompleteMerchantBossFight(bossDefeated: true);

        Assert.True(run.Day3BossDefeated);
        Assert.Equal(StoryEnding.EndingB_Protector, run.Ending);
        Assert.True(run.IsRunComplete);
    }

    [Fact]
    public void EmergencyRevive_UsesGoldOrIssuesLoan()
    {
        var run = new V2RunState(PetSpecies.Coco);
        run.ActivePet.TakeDamage(100f);
        Assert.True(run.ActivePet.IsIncapacitated);

        // Gold is 150 (< 500), so loan is issued
        run.ResolveEmergencyRevive();
        Assert.False(run.ActivePet.IsIncapacitated);
        Assert.Equal(50f, run.ActivePet.Health);
        Assert.Equal(500, run.Economy.Debt);
    }

    [Fact]
    public void Day3Foreclosure_TriggersBadEnding_WhenDebtUnpaid()
    {
        var run = new V2RunState(PetSpecies.Coco);
        // Advance to Day 3 with debt
        run.Economy.IssueEmergencyLoan(500);
        run.Economy.SpendGold(run.Economy.Gold); // empty wallet

        // Fast forward to Day 3 end
        run.ApplyEndOfDayProgression(); // Day 1 decay & interest
        run.AdvanceToNextDay();          // Day 2
        run.ApplyEndOfDayProgression(); // Day 2 decay & interest
        run.AdvanceToNextDay();          // Day 3
        Assert.Equal(3, run.DayNumber);

        run.ApplyEndOfDayProgression(); // Day 3 end check
        Assert.Equal(StoryEnding.EndingBad_Foreclosure, run.Ending);
    }
}
