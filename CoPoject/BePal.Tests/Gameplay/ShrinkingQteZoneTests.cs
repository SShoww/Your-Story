#nullable enable
using System;
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
}
