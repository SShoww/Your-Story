#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using BePal.Gameplay;
using Xunit;

namespace BePal.Tests.Gameplay;

public class ShrinkingQteZoneTests
{
    [Fact]
    public void Needle_RotatesContinuously_WithoutInterruption()
    {
        var zone = new ShrinkingQteZone(speed: 2.0f);
        zone.NeedleAngle = 0f;

        zone.Update(1.0f); // 2.0 rad
        Assert.InRange(zone.NeedleAngle, 1.99f, 2.01f);

        // Update past Tau (~6.283f rad)
        zone.Update(3.0f); // +6.0 rad -> 8.0 rad -> 8.0 % Tau ~ 1.7168 rad
        float expected = 8.0f % ShrinkingQteZone.Tau;
        Assert.InRange(zone.NeedleAngle, expected - 0.01f, expected + 0.01f);
    }

    [Fact]
    public void Zone_ShrinksOverTime_UntilZero()
    {
        float duration = 2.0f;
        float initialSpan = MathF.PI / 3f;
        var zone = new ShrinkingQteZone(duration: duration, initialSpan: initialSpan);

        Assert.Equal(initialSpan, zone.CurrentSpan);
        Assert.False(zone.IsExpired);

        // Advance halfway
        zone.Update(1.0f);
        Assert.InRange(zone.CurrentSpan, (initialSpan / 2f) - 0.01f, (initialSpan / 2f) + 0.01f);
        Assert.False(zone.IsExpired);

        // Advance to full duration
        zone.Update(1.0f);
        Assert.Equal(0f, zone.CurrentSpan);
        Assert.True(zone.IsExpired);

        // Additional update stays at 0
        zone.Update(0.5f);
        Assert.Equal(0f, zone.CurrentSpan);
        Assert.True(zone.IsExpired);
    }

    [Fact]
    public void Zone_IsExpired_BecomesTrueWhenSpanReachesZero()
    {
        var zone = new ShrinkingQteZone(duration: 1.5f);
        Assert.False(zone.IsExpired);

        zone.Update(1.49f);
        Assert.False(zone.IsExpired);

        zone.Update(0.02f);
        Assert.True(zone.IsExpired);
        Assert.Equal(0f, zone.CurrentSpan);
    }

    [Fact]
    public void PresetPositions_HasExactly5DistinctAngles()
    {
        Assert.Equal(5, ShrinkingQteZone.PresetAngles.Length);
        Assert.Equal(5, ShrinkingQteZone.PositionCount);

        for (int i = 0; i < ShrinkingQteZone.PresetAngles.Length; i++)
        {
            for (int j = i + 1; j < ShrinkingQteZone.PresetAngles.Length; j++)
            {
                float diff = MathF.Abs(ShrinkingQteZone.Wrap(ShrinkingQteZone.PresetAngles[i], ShrinkingQteZone.PresetAngles[j]));
                Assert.True(diff > 0.3f, $"Positions {i} and {j} are too close together: diff = {diff}");
            }
        }
    }

    [Fact]
    public void SpawnNewZone_SelectsDifferentPosition()
    {
        var zone = new ShrinkingQteZone();
        zone.SpawnNewZone(forceIndex: 0);
        Assert.Equal(0, zone.CurrentPositionIndex);

        // Over multiple random respawns, it must not repeat the exact same position sequentially
        int current = zone.CurrentPositionIndex;
        for (int i = 0; i < 20; i++)
        {
            zone.SpawnNewZone();
            Assert.NotEqual(current, zone.CurrentPositionIndex);
            current = zone.CurrentPositionIndex;
        }
    }

    [Fact]
    public void SpawnNewZone_ResetsSpanAndDurationWithoutResettingNeedle()
    {
        var zone = new ShrinkingQteZone(duration: 2.0f);
        zone.NeedleAngle = 1.234f;
        zone.Update(1.5f); // Span shrinks, elapsed increases

        Assert.True(zone.ElapsedTime > 0f);
        Assert.True(zone.CurrentSpan < zone.InitialSpan);

        float needleBefore = zone.NeedleAngle;
        zone.SpawnNewZone(forceIndex: 2);

        Assert.Equal(2, zone.CurrentPositionIndex);
        Assert.Equal(0f, zone.ElapsedTime);
        Assert.Equal(zone.InitialSpan, zone.CurrentSpan);
        Assert.Equal(needleBefore, zone.NeedleAngle);
    }

    [Fact]
    public void IsNeedleInsideZone_DetectsHitOnlyWithinCurrentSpan()
    {
        // Position 0 is Top (1.5 * PI)
        float center = ShrinkingQteZone.PresetAngles[0];
        float initialSpan = MathF.PI / 3f; // 60 deg, half span is 30 deg (~0.5236 rad)
        var zone = new ShrinkingQteZone(initialSpan: initialSpan, duration: 2.0f);
        zone.SpawnNewZone(forceIndex: 0);

        // Center hit
        zone.NeedleAngle = center;
        Assert.True(zone.IsNeedleInsideZone());

        // Near edge within half-span (0.2 rad < 0.5236 rad)
        zone.NeedleAngle = center + 0.2f;
        Assert.True(zone.IsNeedleInsideZone());

        // Outside span (0.6 rad > 0.5236 rad)
        zone.NeedleAngle = center + 0.6f;
        Assert.False(zone.IsNeedleInsideZone());

        // When span shrinks to half (half-span becomes ~0.2618 rad)
        zone.Update(1.0f); // 50% shrunk
        zone.NeedleAngle = center + 0.3f; // 0.3 > 0.2618 -> now outside!
        Assert.False(zone.IsNeedleInsideZone());

        zone.NeedleAngle = center + 0.1f; // 0.1 < 0.2618 -> still inside
        Assert.True(zone.IsNeedleInsideZone());

        // When fully expired
        zone.Update(1.0f);
        zone.NeedleAngle = center;
        Assert.False(zone.IsNeedleInsideZone());
    }

    [Fact]
    public void Wrap_HandlesAnglesAcrossZeroBoundary()
    {
        // Angle 0.05 vs 2 * PI - 0.05 (~6.233 rad)
        float a = 0.05f;
        float b = ShrinkingQteZone.Tau - 0.05f;
        float diff = ShrinkingQteZone.Wrap(a, b);
        Assert.InRange(MathF.Abs(diff), 0.099f, 0.101f);
    }

    [Fact]
    public void Zone_WhenCustomDurationSet_ExpiresAtConfiguredTime()
    {
        var zone = new ShrinkingQteZone(duration: 4.0f);
        Assert.Equal(4.0f, zone.Duration);

        zone.Update(3.9f);
        Assert.False(zone.IsExpired);

        zone.Update(0.2f);
        Assert.True(zone.IsExpired);

        // Respawn with a new duration override (e.g. 1.5s)
        zone.SpawnNewZone(newDuration: 1.5f);
        Assert.Equal(1.5f, zone.Duration);
        Assert.False(zone.IsExpired);

        zone.Update(1.5f);
        Assert.True(zone.IsExpired);
    }

    [Fact]
    public void Zone_SequentialCycles_NeverStopsNeedleProgression()
    {
        var zone = new ShrinkingQteZone(speed: 1.0f, duration: 1.0f);
        zone.NeedleAngle = 0f;

        for (int cycle = 0; cycle < 5; cycle++)
        {
            float angleBefore = zone.NeedleAngle;
            zone.Update(0.5f);
            Assert.True(zone.NeedleAngle > angleBefore || (angleBefore > 5.5f && zone.NeedleAngle < 1.0f));

            zone.SpawnNewZone();
            // Span resets, needle continues
            Assert.Equal(zone.InitialSpan, zone.CurrentSpan);
            Assert.Equal(0f, zone.ElapsedTime);
        }
    }

    [Fact]
    public void SpawnSlots_DistributesActionsAcrossDistinctPositions()
    {
        var zone = new ShrinkingQteZone();
        CareAction[] actions = { CareAction.Feed, CareAction.Play, CareAction.Pet, CareAction.Observe };
        zone.SpawnSlots(actions);

        Assert.Equal(4, zone.Slots.Count);
        var positions = zone.Slots.Select(s => s.PositionIndex).ToList();
        Assert.Equal(4, positions.Distinct().Count());
        Assert.All(positions, pos => Assert.InRange(pos, 0, 4));

        var slotActions = zone.Slots.Select(s => s.Action).ToList();
        Assert.Equal(4, slotActions.Distinct().Count());
        foreach (var action in actions)
        {
            Assert.Contains(action, slotActions);
        }
    }

    [Fact]
    public void GetHoveredSlot_ReturnsMatchingAction_WhenNeedleInsideActiveSlot()
    {
        var zone = new ShrinkingQteZone();
        CareAction[] actions = { CareAction.Feed, CareAction.Play, CareAction.Pet, CareAction.Observe };
        zone.SpawnSlots(actions);

        // Advance to each slot's active window and test hit
        float lastTime = 0f;
        foreach (var slot in zone.Slots)
        {
            float dt = slot.AppearTime - lastTime;
            if (dt > 0f)
            {
                zone.Update(dt);
                lastTime = slot.AppearTime;
            }

            zone.NeedleAngle = slot.CenterAngle;
            var hovered = zone.GetHoveredSlot();
            Assert.NotNull(hovered);
            Assert.Equal(slot.Action, hovered.Action);
            Assert.Equal(slot.PositionIndex, hovered.PositionIndex);
            Assert.True(zone.IsNeedleInsideZone());
        }
    }

    [Fact]
    public void GetHoveredSlot_ReturnsNull_WhenNeedleInDeadZoneOrEmptyPosition()
    {
        var zone = new ShrinkingQteZone(initialSpan: MathF.PI / 4f);
        CareAction[] actions = { CareAction.Feed, CareAction.Play, CareAction.Pet, CareAction.Observe };
        zone.SpawnSlots(actions);

        // Find the 5th unused preset position
        var occupied = zone.Slots.Select(s => s.PositionIndex).ToHashSet();
        int emptyPos = Enumerable.Range(0, 5).First(p => !occupied.Contains(p));

        zone.NeedleAngle = ShrinkingQteZone.PresetAngles[emptyPos];
        Assert.Null(zone.GetHoveredSlot());
        Assert.False(zone.IsNeedleInsideZone());

        // Shrink the zone and test dead space between slots
        zone.Update(1.5f);
        zone.NeedleAngle = (ShrinkingQteZone.PresetAngles[emptyPos] + 0.1f) % ShrinkingQteZone.Tau;
        if (zone.GetHoveredSlot() == null)
        {
            Assert.False(zone.IsNeedleInsideZone());
        }
    }

    [Fact]
    public void SpawnSlots_StaggersAppearTimes_AtOneSecondIntervals()
    {
        var zone = new ShrinkingQteZone();
        CareAction[] actions = { CareAction.Feed, CareAction.Play, CareAction.Pet, CareAction.Observe };
        zone.SpawnSlots(actions);

        Assert.Equal(4, zone.Slots.Count);
        Assert.Equal(0.0f, zone.Slots[0].AppearTime);
        Assert.Equal(1.0f, zone.Slots[1].AppearTime);
        Assert.Equal(2.0f, zone.Slots[2].AppearTime);
        Assert.Equal(3.0f, zone.Slots[3].AppearTime);

        // Total duration: (4 - 1) * 1.0s + 4.5s = 7.5s
        Assert.Equal(7.5f, zone.Duration);
    }

    [Fact]
    public void Slot_IsInactiveBeforeAppearTime_AndActiveAfter()
    {
        var zone = new ShrinkingQteZone();
        CareAction[] actions = { CareAction.Feed, CareAction.Play, CareAction.Pet, CareAction.Observe };
        zone.SpawnSlots(actions);

        // At t = 0.5s: Slot 0 is active, Slot 1 has not appeared
        zone.Update(0.5f);
        Assert.True(zone.Slots[0].CurrentSpan > 0f);
        Assert.Equal(0f, zone.Slots[1].CurrentSpan);

        // At t = 1.0s: Slot 1 has now appeared at full span
        zone.Update(0.5f);
        Assert.Equal(zone.InitialSpan, zone.Slots[1].CurrentSpan);
    }

    [Fact]
    public void Slot_ShrinksIndependently_OverConfiguredDuration()
    {
        var zone = new ShrinkingQteZone(initialSpan: 1.0f);
        CareAction[] actions = { CareAction.Feed, CareAction.Play, CareAction.Pet, CareAction.Observe };
        zone.SpawnSlots(actions, staggerInterval: 1.0f, slotDuration: 2.0f);

        // Slot 0 appears at 0s, expires at 2s
        // Slot 1 appears at 1s, expires at 3s
        zone.Update(1.0f); // t = 1.0s
        Assert.Equal(0.5f, zone.Slots[0].CurrentSpan, 2); // halfway
        Assert.Equal(1.0f, zone.Slots[1].CurrentSpan, 2); // just appeared

        zone.Update(1.0f); // t = 2.0s
        Assert.Equal(0f, zone.Slots[0].CurrentSpan); // expired
        Assert.True(zone.Slots[0].IsFinished(2.0f));
        Assert.Equal(0.5f, zone.Slots[1].CurrentSpan, 2); // halfway
    }

    [Fact]
    public void GetHoveredSlot_ReturnsNull_BeforeSlotAppears()
    {
        var zone = new ShrinkingQteZone();
        CareAction[] actions = { CareAction.Feed, CareAction.Play, CareAction.Pet, CareAction.Observe };
        zone.SpawnSlots(actions);

        // At t = 0.5s, Slot 1 has appearTime = 1.0s
        zone.Update(0.5f);
        zone.NeedleAngle = zone.Slots[1].CenterAngle;

        // Even though needle points exactly at Slot 1's position, Slot 1 is not active yet
        Assert.Null(zone.GetHoveredSlot());
        Assert.False(zone.IsNeedleInsideZone());
    }

    [Fact]
    public void Zone_ExpiresOnlyAfterAllStaggeredSlotsFinish()
    {
        var zone = new ShrinkingQteZone();
        CareAction[] actions = { CareAction.Feed, CareAction.Play, CareAction.Pet, CareAction.Observe };
        zone.SpawnSlots(actions, staggerInterval: 1.0f, slotDuration: 2.5f);
        // Total duration = 3 * 1.0 + 2.5 = 5.5s

        zone.Update(2.5f); // Slot 0 expires, but Slots 1-3 still active
        Assert.False(zone.IsExpired);

        zone.Update(2.0f); // t = 4.5s: Slot 2 expires, Slot 3 still has 1.0s
        Assert.False(zone.IsExpired);

        zone.Update(1.0f); // t = 5.5s: All slots finished
        Assert.True(zone.IsExpired);
    }

    [Fact]
    public void SpawnSlots_MultipleCalls_ProduceVariedPositions()
    {
        var zone = new ShrinkingQteZone();
        CareAction[] actions = { CareAction.Feed, CareAction.Play, CareAction.Pet, CareAction.Observe };

        // Over 20 spawns, action Feed should not stay fixed to the same position index every time
        var feedPositions = new HashSet<int>();
        for (int i = 0; i < 20; i++)
        {
            zone.SpawnSlots(actions);
            var feedSlot = zone.Slots.First(s => s.Action == CareAction.Feed);
            feedPositions.Add(feedSlot.PositionIndex);
        }

        Assert.True(feedPositions.Count > 1, "Feed position should vary across multiple SpawnSlots calls.");
    }
}
