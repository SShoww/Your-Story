using BePalV2.Gameplay;
using Xunit;

namespace BePalV2.Tests;

public class BaseHabitatTests
{
    [Fact]
    public void DoctorRevive_RevivesFaintedPetTo1HP_WhenPlayerHasGold()
    {
        var run = new V2RunState(PetSpecies.Coco);
        var pet = run.ActivePet;
        pet.TakeDamage(100f);
        Assert.True(pet.IsIncapacitated);
        Assert.Equal(0f, pet.Health);

        run.Economy.AddGold(1000);
        int initialGold = run.Economy.Gold;

        // Player pays 500 G
        bool paid = run.Economy.SpendGold(500);
        Assert.True(paid);
        pet.Revive(1f);

        Assert.Equal(1f, pet.Health);
        Assert.False(pet.IsIncapacitated);
        Assert.Equal(initialGold - 500, run.Economy.Gold);
    }

    [Fact]
    public void DoctorRevive_IssuesEmergencyLoan_WhenPlayerLacksGold()
    {
        var run = new V2RunState(PetSpecies.Coco);
        var pet = run.ActivePet;
        pet.TakeDamage(100f);

        // Spend down gold
        run.Economy.SpendGold(run.Economy.Gold);
        Assert.Equal(0, run.Economy.Gold);

        // Emergency loan of 500 G issued
        run.Economy.IssueEmergencyLoan(500);
        Assert.Equal(500, run.Economy.Gold);
        Assert.Equal(500, run.Economy.Debt);

        // Deduct 500 G for treatment
        run.Economy.SpendGold(500);
        pet.Revive(1f);

        Assert.Equal(1f, pet.Health);
        Assert.Equal(0, run.Economy.Gold);
        Assert.Equal(500, run.Economy.Debt);
    }

    [Fact]
    public void CanonicalFiveItems_ApplyCorrectRestorationValues()
    {
        var pet = new PetEntity(PetSpecies.Coco);
        // Reduce stats
        pet.TakeDamage(60f);
        pet.ApplyDisasterPenalties(stomachLoss: 50, cleanLoss: 50);

        Assert.Equal(40f, pet.Health);
        Assert.Equal(30, pet.Stomach);
        Assert.Equal(20, pet.Clean);

        // 1. Food Potion (+35 Stomach)
        pet.FeedDirect(ItemDefinition.FoodPotion.StomachRestore);
        Assert.Equal(65, pet.Stomach);

        // 2. Sanitizer (+40 Clean)
        pet.CleanDirect(ItemDefinition.CleanSanitizer.CleanRestore);
        Assert.Equal(60, pet.Clean);

        // 3. Health Medicine (+50 HP)
        pet.Heal(ItemDefinition.HealthMedicine.HealthRestore);
        Assert.Equal(90f, pet.Health);

        // 4. EXP Booster (+50 EXP)
        int initialExp = pet.CurrentExp;
        pet.AddExp(ItemDefinition.ExpBooster.ExpGain);
        Assert.Equal(initialExp + 50, pet.CurrentExp);

        // 5. Energy Tonic (+2 AP)
        var energy = new EnergyAccount();
        energy.Spend(4); // 6 - 4 = 2
        energy.AddBonus(ItemDefinition.EnergyTonic.EnergyRestore);
        Assert.Equal(4, energy.CurrentEnergy);
    }

    [Fact]
    public void StarterSpecimens_MatchCanonicalSlideStats()
    {
        // Slide 9: Coco (Red)
        var coco = new PetEntity(PetSpecies.Coco);
        Assert.Equal(100f, coco.MaxHealth);
        Assert.Equal(80, coco.Stomach);
        Assert.Equal(70, coco.Clean);

        // Slide 9: Gloomtail (Log)
        var gloomtail = new PetEntity(PetSpecies.Gloomtail);
        Assert.Equal(110f, gloomtail.MaxHealth);
        Assert.Equal(60, gloomtail.Stomach);
        Assert.Equal(60, gloomtail.Clean);

        // Slide 9: Sproutlet (Green)
        var sproutlet = new PetEntity(PetSpecies.Sproutlet);
        Assert.Equal(90f, sproutlet.MaxHealth);
        Assert.Equal(70, sproutlet.Stomach);
        Assert.Equal(80, sproutlet.Clean);
    }
}
