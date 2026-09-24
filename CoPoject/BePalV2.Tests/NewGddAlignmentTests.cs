using BePalV2.Gameplay;
using Xunit;

namespace BePalV2.Tests;

public class NewGddAlignmentTests
{
    [Fact]
    public void PlayerProgression_LevelUpCurve_AndSkillPointGain()
    {
        var prog = new PlayerProgression();
        Assert.Equal(1, prog.Level);
        Assert.Equal(0, prog.CurrentExp);
        Assert.Equal(10, prog.MaxExp); // 10 + (1-1)*10 = 10
        Assert.Equal(0, prog.SkillPoints);

        // Add 15 EXP: should level up to Lv 2, 5 EXP overflow, 1 Skill Point
        prog.AddExp(15);
        Assert.Equal(2, prog.Level);
        Assert.Equal(5, prog.CurrentExp);
        Assert.Equal(20, prog.MaxExp); // 10 + (2-1)*10 = 20
        Assert.Equal(1, prog.SkillPoints);

        // Add 25 EXP: (5 + 25 = 30 >= 20) -> Lv 3, 10 EXP overflow, 2 Skill Points total
        prog.AddExp(25);
        Assert.Equal(3, prog.Level);
        Assert.Equal(10, prog.CurrentExp);
        Assert.Equal(30, prog.MaxExp); // 10 + (3-1)*10 = 30
        Assert.Equal(2, prog.SkillPoints);
    }

    [Fact]
    public void PlayerProgression_Upgrades_ConsumeSkillPoints_AndApplyBonuses()
    {
        var prog = new PlayerProgression();
        prog.AddExp(30); // Gives Lv 2 (1 SP) then towards Lv 3
        Assert.True(prog.SkillPoints >= 1);

        int initialSp = prog.SkillPoints;
        bool upgradedQte = prog.UpgradeQteFocus();
        Assert.True(upgradedQte);
        Assert.Equal(1, prog.QteFocusLevel);
        Assert.Equal(0.15f, prog.QteWindowBonus, precision: 3);
        Assert.Equal(initialSp - 1, prog.SkillPoints);

        // Without skill points, upgrade fails
        while (prog.SkillPoints > 0) prog.SpendSkillPoint(1);
        Assert.False(prog.UpgradeProgressBooster());
        Assert.Equal(0, prog.ProgressBoosterLevel);
        Assert.Equal(1.0f, prog.ProgressBoosterMultiplier);
    }

    [Fact]
    public void EndlessEventManager_AssignsScriptedAndEndlessEvents()
    {
        var mgr = new EndlessEventManager();
        Assert.Equal(DailyEventType.SafeDayTutorial, mgr.DetermineMorningEvent(1));
        Assert.Equal(DailyEventType.ToothlessEncounter, mgr.DetermineMorningEvent(2));
        Assert.Equal(DailyEventType.MerchantEncounter, mgr.DetermineMorningEvent(3));

        // Day 4+: Deterministic seed test
        var seededMgr = new EndlessEventManager(seed: 42);
        var endlessEvt = seededMgr.DetermineMorningEvent(4);
        Assert.True(endlessEvt is DailyEventType.Thunderstorm or DailyEventType.WildIncursion or DailyEventType.TravelingMerchant);

        string briefing = mgr.GetEventBriefing(DailyEventType.SafeDayTutorial, 1);
        Assert.Contains("orientation", briefing, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PetProgression_ScaleFormulas_AndLevelRewards()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        Assert.Equal(1, pet.Level);
        Assert.Equal(100, pet.MaxProgress); // 100 + (1-1)*50 = 100
        Assert.Equal(0f, pet.CounterDamageBonus);

        // Level 1 -> 2 (requires 100 PP): Max HP +10
        pet.AddProgress(100);
        Assert.Equal(2, pet.Level);
        Assert.Equal(150, pet.MaxProgress); // 100 + (2-1)*50 = 150
        Assert.Equal(110f, pet.MaxHealth);

        // Level 2 -> 3 (requires 150 PP): Counter Damage +10%
        pet.AddProgress(150);
        Assert.Equal(3, pet.Level);
        Assert.Equal(200, pet.MaxProgress); // 100 + (3-1)*50 = 200
        Assert.Equal(0.10f, pet.CounterDamageBonus, precision: 3);
    }

    [Fact]
    public void CombatEngine_AppliesGrimyPenalty_AndCounterBonus()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        pet.CleanDirect(-60); // Clean 70 - 60 = 10 < 50 (Grimy)
        Assert.True(pet.IsGrimy);

        var engine = new CombatEngine(CombatMode.MerchantBoss, pet);
        while (!engine.IsNeedleInDodgeZone()) engine.Update(0.05f);
        engine.AttemptDodge();

        float bossHpBefore = engine.BossHp;
        engine.AttemptCounter();
        float damageDealt = bossHpBefore - engine.BossHp;

        // Base 80 dmg * 0.75 (Grimy) = 60 dmg
        Assert.Equal(60f, damageDealt, precision: 1);
    }

    [Fact]
    public void V2RunState_SupportsEndlessModePastDayThree()
    {
        var run = new V2RunState();
        Assert.Equal(1, run.DayNumber);
        Assert.False(run.IsEndlessMode);

        // Advance to Day 2
        run.AdvanceToNextDay();
        Assert.Equal(2, run.DayNumber);

        // Advance to Day 3
        run.AdvanceToNextDay();
        Assert.Equal(3, run.DayNumber);

        // Simulate beating boss and moving to Day 4 Endless
        run.CompleteMerchantBossFight(true);
        run.AdvanceToNextDay();
        Assert.Equal(4, run.DayNumber);
        Assert.True(run.IsEndlessMode);
    }
}
