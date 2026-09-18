#nullable enable
using System;
using System.Linq;
using BePal.Gameplay;
using BePal.Screens;
using Microsoft.Xna.Framework;
using Xunit;

namespace BePal.Tests.Screens;

public class CareQteScreenTests
{
    [Fact]
    public void Timeout_DoesNotInflictDamage_AndDoesNotSpawnMissTag_AndResetsInQte()
    {
        // Arrange
        var run = new PrototypeRun();
        Assert.Equal(3, run.Health);

        var context = ScreenContext.CreateTestContext(run);
        var manager = new ScreenManager(context);
        var screen = new CareQteScreen(context);
        manager.SetScreen(screen, useTransition: false);

        // Act: Advance time past multi-slot zone duration (5.5s)
        screen.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(6.0f)));

        // Assert: No damage taken, no miss tag spawned, stayed on CareQteScreen, zone reset
        Assert.Equal(3, run.Health);
        Assert.DoesNotContain(context.Tags, t => t.Text.Contains("MISSED"));
        Assert.IsType<CareQteScreen>(manager.CurrentScreen);
        Assert.False(screen.Zone.IsExpired);
    }

    [Fact]
    public void DeadZonePress_InflictsOneDamage_AndStaysInQte()
    {
        // Arrange
        var run = new PrototypeRun();
        var context = ScreenContext.CreateTestContext(run);
        var manager = new ScreenManager(context);
        var screen = new CareQteScreen(context);
        manager.SetScreen(screen, useTransition: false);

        // Dead zone angle (0.9*PI is midway between 0.7*PI and 1.1*PI, guaranteeing empty space)
        screen.Zone.NeedleAngle = 0.9f * MathF.PI;
        Assert.Null(screen.Zone.GetHoveredSlot());

        // Act
        screen.ResolveCare();

        // Assert
        Assert.Equal(2, run.Health);
        Assert.Equal(0, run.Satisfaction);
        Assert.Contains(context.Tags, t => t.Text == "MISSED! -1 HP");
        Assert.IsType<CareQteScreen>(manager.CurrentScreen);
    }

    [Fact]
    public void Rejection_InflictsOneDamage_AndStaysInQte()
    {
        // Arrange: Mossling prefers Feed. Find a slot with a non-preferred action.
        var run = new PrototypeRun();
        var context = ScreenContext.CreateTestContext(run);
        var manager = new ScreenManager(context);
        var screen = new CareQteScreen(context);
        manager.SetScreen(screen, useTransition: false);

        PetDefinition pet = PetCatalog.Get(run.ActivePet);
        var nonPreferredSlot = screen.Zone.Slots.First(s => s.Action != pet.Pattern.PreferredAction);

        // Advance time so the slot has appeared and has span
        screen.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(nonPreferredSlot.AppearTime + 0.1f)));
        screen.Zone.NeedleAngle = nonPreferredSlot.CenterAngle;

        var hovered = screen.Zone.GetHoveredSlot();
        Assert.NotNull(hovered);
        Assert.NotEqual(pet.Pattern.PreferredAction, hovered.Action);

        // Act
        screen.ResolveCare();

        // Assert
        Assert.Equal(2, run.Health);
        Assert.Contains(context.Tags, t => t.Text.StartsWith("REJECTED:"));
        Assert.IsType<CareQteScreen>(manager.CurrentScreen);
    }

    [Fact]
    public void FatalMiss_TriggersForcedRetreat()
    {
        // Arrange: Player down to 1 HP
        var run = new PrototypeRun();
        run.TakeDamage();
        run.TakeDamage();
        Assert.Equal(1, run.Health);
        Assert.Equal(1, run.DayNumber);

        var context = ScreenContext.CreateTestContext(run);
        var manager = new ScreenManager(context);
        var screen = new CareQteScreen(context);
        manager.SetScreen(screen, useTransition: false);

        // Act: Dead zone hit reduces HP from 1 to 0
        screen.Zone.NeedleAngle = 0.9f * MathF.PI;
        screen.ResolveCare();

        // Assert: Forced retreat triggered
        Assert.Equal(1, run.ForcedRetreats);
        Assert.Equal(2, run.DayNumber);
        Assert.Equal(3, run.Health);
        Assert.NotNull(manager.ActiveTransition);
        manager.CompleteTransition();
        Assert.IsType<DoorstepScreen>(manager.CurrentScreen);
    }

    [Fact]
    public void Blinkbun_TeleportsMultipleTimes_WithSignificantDisplacement()
    {
        // Arrange: Advance to Day 3 for Trickster (Blinkbun)
        var run = new PrototypeRun();
        run.EndDay(); // Day 2
        run.EndDay(); // Day 3
        Assert.Equal(PetKind.Trickster, run.ActivePet);

        var context = ScreenContext.CreateTestContext(run);
        var manager = new ScreenManager(context);
        var screen = new CareQteScreen(context);
        manager.SetScreen(screen, useTransition: false);

        // Act: Step through time up to 3.0s in 0.05s increments
        int observedWarps = 0;
        float totalTime = 0f;
        while (totalTime < 3.0f)
        {
            int prevWarpCount = screen.TeleportCount;

            screen.Update(new GameTime(TimeSpan.FromSeconds(totalTime), TimeSpan.FromSeconds(0.05f)));
            totalTime += 0.05f;

            if (screen.TeleportCount > prevWarpCount)
            {
                observedWarps++;
                // Verify displacement was at least PI/2 (accounting for circular wrap)
                float diff = MathF.Abs(screen.Angle - screen.LastTeleportAngle);
                if (diff > MathF.PI) diff = ShrinkingQteZone.Tau - diff;
                Assert.True(diff >= (MathF.PI * 0.5f) - 0.05f, $"Expected angular leap >= PI/2 rad, got {diff}");
            }
        }

        // Assert: Teleported at least once or twice during 3s
        Assert.True(observedWarps >= 1);
        Assert.True(screen.TeleportCount >= 1);
    }

    [Fact]
    public void BaselinePet_NeverTeleports()
    {
        // Arrange: Day 1 Mossling (Baseline)
        var run = new PrototypeRun();
        Assert.Equal(PetKind.Baseline, run.ActivePet);

        var context = ScreenContext.CreateTestContext(run);
        var manager = new ScreenManager(context);
        var screen = new CareQteScreen(context);
        manager.SetScreen(screen, useTransition: false);

        // Act: Step through 3 seconds
        float totalTime = 0f;
        while (totalTime < 3.0f)
        {
            screen.Update(new GameTime(TimeSpan.FromSeconds(totalTime), TimeSpan.FromSeconds(0.05f)));
            totalTime += 0.05f;
        }

        // Assert: 0 teleports
        Assert.Equal(0, screen.TeleportCount);
    }

    [Fact]
    public void Teleport_TelegraphBeginsHalfSecondPrior_WithDestinationCalculated()
    {
        var run = new PrototypeRun();
        run.EndDay();
        run.EndDay();
        var context = ScreenContext.CreateTestContext(run);
        var screen = new CareQteScreen(context);

        // Step up to just before telegraph window
        float beforeTelegraph = screen.NextTeleportTime - CareQteScreen.TelegraphDuration - 0.05f;
        if (beforeTelegraph > 0f)
        {
            screen.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(beforeTelegraph)));
            Assert.False(screen.IsTelegraphing);
        }

        // Step into telegraph window
        screen.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.10f)));

        // Assert: IsTelegraphing is true and destination is locked
        Assert.True(screen.IsTelegraphing);
        Assert.True(screen.TeleportTargetAngle >= 0f && screen.TeleportTargetAngle < ShrinkingQteZone.Tau);
    }

    [Fact]
    public void Teleport_LandingPositionIsUpstreamOfActiveSlot_WithSufficientRunway()
    {
        var run = new PrototypeRun();
        run.EndDay();
        run.EndDay();
        var context = ScreenContext.CreateTestContext(run);
        var screen = new CareQteScreen(context);

        // Step until first teleport occurs
        while (screen.TeleportCount == 0 && screen.QteTime < 2.5f)
        {
            screen.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.02f)));
        }

        Assert.Equal(1, screen.TeleportCount);
        float needleAngle = screen.Angle;

        // Landing angle should be upstream of an active slot (diff = slotCenter - needle > 0)
        bool foundUpstreamSlot = false;
        foreach (var slot in screen.Zone.Slots)
        {
            if (slot.CurrentSpan > 0f)
            {
                float diff = ShrinkingQteZone.Wrap(slot.CenterAngle, needleAngle);
                if (diff >= 0.5f && diff <= 2.2f)
                {
                    foundUpstreamSlot = true;
                    break;
                }
            }
        }
        Assert.True(foundUpstreamSlot, "Expected landing angle to be upstream of an active slot with clear runway");
    }

    [Fact]
    public void Teleport_AntiCheapGuard_DelaysWarpWhenInsidePreferredSlot()
    {
        var run = new PrototypeRun();
        run.EndDay();
        run.EndDay();
        var context = ScreenContext.CreateTestContext(run);
        var screen = new CareQteScreen(context);

        // Step up to just before telegraph window
        float justBeforeTelegraph = screen.NextTeleportTime - CareQteScreen.TelegraphDuration - 0.05f;
        if (justBeforeTelegraph > 0f)
        {
            screen.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(justBeforeTelegraph)));
        }

        // Position needle directly inside preferred slot
        var pet = PetCatalog.Get(run.ActivePet);
        var prefSlot = screen.Zone.Slots.First(s => s.Action == pet.Pattern.PreferredAction);
        prefSlot.AppearTime = 0f;
        screen.Zone.NeedleAngle = prefSlot.CenterAngle;
        float originalTeleportTime = screen.NextTeleportTime;

        // Step 0.10s into telegraph window
        screen.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.10f)));

        // Anti-cheap guard should have delayed _nextTeleportTime
        Assert.True(screen.NextTeleportTime >= originalTeleportTime + 0.7f,
            $"Expected teleport time to be delayed beyond {originalTeleportTime + 0.7f}, was {screen.NextTeleportTime}");
    }

    [Fact]
    public void Teleport_PostWarpGracePeriod_AbsorbsDeadZonePress()
    {
        var run = new PrototypeRun();
        run.EndDay();
        run.EndDay();
        var context = ScreenContext.CreateTestContext(run);
        var screen = new CareQteScreen(context);

        // Step until teleport occurs
        while (screen.TeleportCount == 0 && screen.QteTime < 2.5f)
        {
            screen.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.02f)));
        }

        Assert.Equal(1, screen.TeleportCount);
        Assert.True(screen.TeleportGraceTimer > 0f);

        // Ensure needle is in dead zone (null hovered action)
        if (screen.GetHoveredAction() != null)
        {
            screen.Zone.NeedleAngle = (screen.Zone.NeedleAngle + MathF.PI * 0.5f) % ShrinkingQteZone.Tau;
        }

        int hpBefore = run.Health;
        screen.ResolveCare();

        // Grace period absorbed the hit without taking damage
        Assert.Equal(hpBefore, run.Health);
        Assert.Contains(context.Tags, t => t.Text == "WARP DEFLECTED!");
    }

    [Fact]
    public void Teleport_MaxTwoWarpsPerCycle()
    {
        var run = new PrototypeRun();
        run.EndDay();
        run.EndDay();
        var context = ScreenContext.CreateTestContext(run);
        var screen = new CareQteScreen(context);

        // Step through 5.0s (less than zone expiration 5.5s)
        float total = 0f;
        while (total < 5.0f && !screen.Zone.IsExpired)
        {
            screen.Update(new GameTime(TimeSpan.FromSeconds(total), TimeSpan.FromSeconds(0.05f)));
            total += 0.05f;
        }

        Assert.True(screen.TeleportCount <= CareQteScreen.MaxTeleportsPerCycle);
        Assert.Equal(2, screen.TeleportCount);
    }

    [Fact]
    public void DodgeFailure_NonFatal_ReturnsToCareQteScreen()
    {
        // Arrange
        var run = new PrototypeRun();
        var context = ScreenContext.CreateTestContext(run);
        var manager = new ScreenManager(context);
        var dodgeScreen = new DodgeQteScreen(context);
        manager.SetScreen(dodgeScreen, useTransition: false);

        // Act: Expire dodge zone
        dodgeScreen.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(3.0f)));

        // Assert: Player loses 1 HP and returns to CareQteScreen (NOT PanoramicRoomScreen)
        Assert.Equal(2, run.Health);
        Assert.IsType<CareQteScreen>(manager.CurrentScreen);
        Assert.Contains(context.Tags, t => t.Text == "DODGE TIMEOUT! -1 HP");
    }

    [Fact]
    public void DodgeFailure_Fatal_TriggersForcedRetreat()
    {
        // Arrange: Player down to 1 HP
        var run = new PrototypeRun();
        run.TakeDamage();
        run.TakeDamage();
        Assert.Equal(1, run.Health);

        var context = ScreenContext.CreateTestContext(run);
        var manager = new ScreenManager(context);
        var dodgeScreen = new DodgeQteScreen(context);
        manager.SetScreen(dodgeScreen, useTransition: false);

        // Act: Expire dodge zone to inflict fatal damage
        dodgeScreen.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(3.0f)));

        // Assert: Forced retreat triggered, day advanced, doorstep shown
        Assert.Equal(1, run.ForcedRetreats);
        Assert.Equal(2, run.DayNumber);
        Assert.Equal(3, run.Health);
        Assert.NotNull(manager.ActiveTransition);
        manager.CompleteTransition();
        Assert.IsType<DoorstepScreen>(manager.CurrentScreen);
    }
}
