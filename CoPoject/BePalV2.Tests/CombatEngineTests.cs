using BePalV2.Gameplay;
using Xunit;

namespace BePalV2.Tests;

public class CombatEngineTests
{
    [Fact]
    public void ToothlessTaming_SuccessfulDodgeAndCounter_FillsTameGauge()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        var engine = new CombatEngine(CombatMode.ToothlessTaming, pet);

        Assert.Equal(0f, engine.TameGauge);
        Assert.False(engine.IsCombatWon);

        // 4 successful dodge + counter strikes should reach 100% (25% each)
        for (int i = 0; i < 4; i++)
        {
            // Align needle into dodge zone
            while (!engine.IsNeedleInDodgeZone())
            {
                engine.Update(0.05f);
            }

            bool dodged = engine.AttemptDodge();
            Assert.True(dodged);
            Assert.True(engine.IsCounterWindowOpen);

            bool countered = engine.AttemptCounter();
            Assert.True(countered);
            Assert.False(engine.IsCounterWindowOpen);
        }

        Assert.Equal(100f, engine.TameGauge);
        Assert.True(engine.IsCombatWon);
    }

    [Fact]
    public void MerchantBoss_PhaseProgression_AndSynergies()
    {
        var coco = new PetEntity(PetSpecies.Coco);
        var cocoEngine = new CombatEngine(CombatMode.MerchantBoss, coco);
        // Coco synergy: speed 3.0 * 0.8 = 2.4
        Assert.Equal(2.4f, cocoEngine.AngularVelocity, precision: 3);

        var sproutlet = new PetEntity(PetSpecies.Sproutlet);
        var sproutEngine = new CombatEngine(CombatMode.MerchantBoss, sproutlet);
        // Sproutlet synergy: dodge half width 0.30 * 1.25 = 0.375
        Assert.Equal(0.375f, sproutEngine.DodgeZoneHalfWidth, precision: 3);

        var gloomtail = new PetEntity(PetSpecies.Gloomtail);
        var gloomEngine = new CombatEngine(CombatMode.MerchantBoss, gloomtail);
        // Ensure needle is OUTSIDE dodge zone to trigger miss
        while (gloomEngine.IsNeedleInDodgeZone())
        {
            gloomEngine.Update(0.1f);
        }
        gloomEngine.AttemptDodge();
        // Merchant boss attack is 20 dmg, Gloomtail takes 50% = 10 dmg -> PlayerHp = 90
        Assert.Equal(90f, gloomEngine.PlayerHp);
    }

    [Fact]
    public void CloudyGlasses_AbsorbsTwoHits()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        var engine = new CombatEngine(CombatMode.MerchantBoss, pet, initialShieldHits: 2);

        // Force miss 1
        while (engine.IsNeedleInDodgeZone()) engine.Update(0.05f);
        engine.AttemptDodge();
        Assert.Equal(100f, engine.PlayerHp); // Shield absorbed!
        Assert.Equal(1, engine.ShieldHitsRemaining);

        // Force miss 2
        while (engine.IsNeedleInDodgeZone()) engine.Update(0.05f);
        engine.AttemptDodge();
        Assert.Equal(100f, engine.PlayerHp); // Shield absorbed!
        Assert.Equal(0, engine.ShieldHitsRemaining);

        // Force miss 3
        while (engine.IsNeedleInDodgeZone()) engine.Update(0.05f);
        engine.AttemptDodge();
        Assert.Equal(80f, engine.PlayerHp); // Took full 20 damage!
    }

    [Fact]
    public void MerchantBoss_Phase1ThreeDodges_TriggersPetCounterDamage()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        var engine = new CombatEngine(CombatMode.MerchantBoss, pet);
        Assert.Equal(1000f, engine.BossHp);
        Assert.Equal(1, engine.BossPhase);

        for (int i = 0; i < 3; i++)
        {
            while (!engine.IsNeedleInDodgeZone()) engine.Update(0.05f);
            engine.AttemptDodge();
        }

        // 3 consecutive dodges in phase 1 dealt 100 dmg
        Assert.Equal(900f, engine.BossHp);
    }

    [Fact]
    public void ToothlessAlly_DoublesCounterDamage()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        var engineWithout = new CombatEngine(CombatMode.MerchantBoss, pet, hasToothlessAlly: false);
        var engineWith = new CombatEngine(CombatMode.MerchantBoss, pet, hasToothlessAlly: true);

        // Trigger counter opportunity
        while (!engineWithout.IsNeedleInDodgeZone()) engineWithout.Update(0.05f);
        engineWithout.AttemptDodge();
        engineWithout.AttemptCounter(); // 80 dmg
        Assert.Equal(920f, engineWithout.BossHp);

        while (!engineWith.IsNeedleInDodgeZone()) engineWith.Update(0.05f);
        engineWith.AttemptDodge();
        engineWith.AttemptCounter(); // 80 * 2 = 160 dmg
        Assert.Equal(840f, engineWith.BossHp);
    }

    [Fact]
    public void MerchantBoss_MissedDodge_StealsGold()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        var engine = new CombatEngine(CombatMode.MerchantBoss, pet);

        // Force miss
        while (engine.IsNeedleInDodgeZone()) engine.Update(0.05f);
        bool dodged = engine.AttemptDodge();

        Assert.False(dodged);
        // Slide 54: "ถ้ากดไม่ทัน จะโดนขโมยเงิน"
        Assert.Equal(200, engine.GoldStolenFromPlayer);
    }

    [Fact]
    public void ToothlessTaming_MissedDodge_DamagesPet()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        var engine = new CombatEngine(CombatMode.ToothlessTaming, pet);
        float startHp = pet.Health;

        // Force miss
        while (engine.IsNeedleInDodgeZone()) engine.Update(0.05f);
        bool dodged = engine.AttemptDodge();

        Assert.False(dodged);
        // Slide 38: "ถ้ากดไม่ทัน สัตว์เราจะโดนโจมตี"
        Assert.Equal(startHp - 25f, pet.Health);
    }

    [Fact]
    public void BossHitsGauge_TracksRemainingHitsCorrectly()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        var engine = new CombatEngine(CombatMode.MerchantBoss, pet);

        Assert.Equal(5, engine.BossHitsRemaining);

        // Deal 250 damage (each hit is 200 hp)
        while (!engine.IsNeedleInDodgeZone()) engine.Update(0.05f);
        engine.AttemptDodge();
        engine.AttemptCounter(); // 80 dmg
        Assert.Equal(920f, engine.BossHp);
        Assert.Equal(5, engine.BossHitsRemaining); // ceil(920 / 200) = 5
    }

    [Fact]
    public void CanPetFight_ChecksCleanAndHealthRequirements()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        // Default Coco: HP 100, Clean 70
        Assert.True(CombatEngine.CanPetFight(pet, out string? refusalReason));
        Assert.Null(refusalReason);
        // Pet too filthy (Clean < 50)
        pet.CleanDirect(-25); // 70 - 25 = 45
        Assert.False(CombatEngine.CanPetFight(pet, out refusalReason));
        Assert.Contains("too filthy", refusalReason);

        // Restore clean, but pet incapacitated (HP = 0)
        pet.CleanDirect(50);
        pet.TakeDamage(100f);
        Assert.False(CombatEngine.CanPetFight(pet, out refusalReason));
        Assert.Contains("incapacitated", refusalReason);
    }

    [Fact]
    public void ChapterBoss_ForcedDefeatMechanic_EndsInPlayerDefeat()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        var engine = new CombatEngine(CombatMode.ChapterBoss, pet);

        Assert.Equal(2000f, engine.BossHp);
        Assert.Equal(3.6f * 0.8f, engine.AngularVelocity, precision: 3); // Coco synergy
        Assert.False(engine.IsCombatWon);
        Assert.False(engine.IsFinished);

        // Chapter boss misses deal lethal 35 damage each
        for (int i = 0; i < 3; i++)
        {
            while (engine.IsNeedleInDodgeZone()) engine.Update(0.05f);
            engine.AttemptDodge();
        }

        // 3 misses = 105 dmg dealt to player -> PlayerHp hits 0
        Assert.Equal(0f, engine.PlayerHp);
        Assert.True(engine.IsPlayerDefeated);
        Assert.False(engine.IsCombatWon);
        Assert.True(engine.IsFinished);
    }
}
