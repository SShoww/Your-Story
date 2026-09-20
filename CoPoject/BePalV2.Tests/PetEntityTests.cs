using BePalV2.Gameplay;
using Xunit;

namespace BePalV2.Tests;

public class PetEntityTests
{
    [Fact]
    public void StarterPets_HaveCorrectInitialStats()
    {
        var coco = new PetEntity(PetSpecies.Coco);
        Assert.Equal(100f, coco.Health);
        Assert.Equal(100f, coco.MaxHealth);
        Assert.Equal(80, coco.Stomach);
        Assert.Equal(70, coco.Clean);
        Assert.False(coco.IsGrimy);
        Assert.False(coco.IsInfected);
        Assert.False(coco.IsStarving);

        var sproutlet = new PetEntity(PetSpecies.Sproutlet);
        Assert.Equal(90f, sproutlet.Health);
        Assert.Equal(70, sproutlet.Stomach);
        Assert.Equal(80, sproutlet.Clean);

        var gloomtail = new PetEntity(PetSpecies.Gloomtail);
        Assert.Equal(110f, gloomtail.Health);
        Assert.Equal(60, gloomtail.Stomach);
        Assert.Equal(60, gloomtail.Clean);
    }

    [Fact]
    public void NaturalDecay_ReducesStomachAndClean()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        var (sLost, cLost, dmg, heal) = pet.ApplyDailyDecay();

        Assert.Equal(20, sLost);
        Assert.Equal(15, cLost);
        Assert.Equal(60, pet.Stomach);
        Assert.Equal(55, pet.Clean);
        Assert.Equal(0f, dmg);
    }

    [Fact]
    public void FadedRibbon_ReducesCleanDecayBy30Percent()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        var ribbon = ItemDefinition.FadedRibbon;

        var (_, cLost, _, _) = pet.ApplyDailyDecay(ribbon);

        // 15 * 0.70 = 10.5 -> rounded to 11
        Assert.Equal(11, cLost);
        Assert.Equal(59, pet.Clean);
    }

    [Fact]
    public void SicknessStates_TriggerOvernightDamage()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        // Force starving and infected
        pet.ApplyDisasterPenalties(stomachLoss: 80, cleanLoss: 60);
        Assert.True(pet.IsStarving);
        Assert.True(pet.IsInfected);
        Assert.True(pet.IsGrimy);

        // Overnight damage should be 10 (starving) + 5 (grimy) + 15 (infected) = 30
        var (_, _, dmg, _) = pet.ApplyDailyDecay();
        Assert.Equal(30f, dmg);
        Assert.Equal(70f, pet.Health);
    }

    [Fact]
    public void Coco_Photosynthesis_HealsWhenCleanAbove80()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        pet.TakeDamage(20f);
        // Clean is 70, boost Clean to 90
        pet.ExecuteCareAction(CareActionType.Clean, PrecisionTier.Perfect);
        Assert.True(pet.Clean > 80);

        var (_, _, _, photoHeal) = pet.ApplyDailyDecay();
        Assert.Equal(5f, photoHeal);
        Assert.Equal(85f, pet.Health);
    }

    [Fact]
    public void MetabolicBurn_SubtractsStomachOnNonFeedActions()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        int initialStomach = pet.Stomach; // 80

        pet.ExecuteCareAction(CareActionType.Clean, PrecisionTier.Perfect);
        Assert.Equal(initialStomach - 5, pet.Stomach);

        pet.ExecuteCareAction(CareActionType.Train, PrecisionTier.Perfect);
        Assert.Equal(initialStomach - 15, pet.Stomach); // 10 more
    }

    [Fact]
    public void BalletShoes_AddsExtraStomachBurn()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        var shoes = ItemDefinition.BalletShoes;

        pet.ExecuteCareAction(CareActionType.Clean, PrecisionTier.Good, equipped: shoes);
        // 5 base + 5 extra = 10 burn
        Assert.Equal(70, pet.Stomach);
    }

    [Fact]
    public void StarvingPet_TakesDamageOnAction_AndBlocksTrain()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        pet.ApplyDisasterPenalties(stomachLoss: 80, cleanLoss: 0);
        Assert.True(pet.IsStarving);
        Assert.False(pet.CanTrain());

        float hpBefore = pet.Health;
        pet.ExecuteCareAction(CareActionType.Clean, PrecisionTier.Perfect);
        Assert.Equal(hpBefore - 2f, pet.Health);
    }

    [Fact]
    public void ExperienceAndLevelUp_IncreasesMaxHealth()
    {
        var pet = new PetEntity(PetSpecies.Sproutlet);
        Assert.Equal(1, pet.Level);
        Assert.Equal(90f, pet.MaxHealth);

        pet.AddExp(120);
        Assert.Equal(2, pet.Level);
        Assert.Equal(100f, pet.MaxHealth);
        Assert.Equal(20, pet.CurrentExp);
    }

    [Fact]
    public void Revive_RestoresHealthAndClearsAcidBurn()
    {
        var pet = new PetEntity(PetSpecies.Toothless);
        pet.TakeDamage(100f);
        pet.HasAcidBurn = true;
        Assert.True(pet.IsIncapacitated);

        pet.Revive(50f);
        Assert.False(pet.IsIncapacitated);
        Assert.Equal(50f, pet.Health);
        Assert.False(pet.HasAcidBurn);
    }
}
