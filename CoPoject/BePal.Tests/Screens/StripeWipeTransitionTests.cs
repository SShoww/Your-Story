#nullable enable
using System;
using BePal.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xunit;

namespace BePal.Tests.Screens;

public class StripeWipeTransitionTests
{
    private class DummyScreen : IScreen
    {
        public int UpdateCount { get; private set; }
        public int DrawCount { get; private set; }

        public void Update(GameTime gameTime) => UpdateCount++;
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) => DrawCount++;
    }

    [Fact]
    public void Constructor_DefaultDuration_IsCinematicZeroPointEightFiveSeconds()
    {
        var transition = new StripeWipeTransition();

        Assert.Equal(0.85f, transition.Duration);
        Assert.Equal(0f, transition.Progress);
        Assert.True(transition.IsActive);
    }

    [Fact]
    public void Constructor_InitialState_ProgressIsZeroAndActive()
    {
        var transition = new StripeWipeTransition(duration: 0.5f);

        Assert.Equal(0f, transition.Progress);
        Assert.Equal(0f, transition.Elapsed);
        Assert.True(transition.IsActive);
        Assert.False(transition.IsCovered);
    }

    [Fact]
    public void Update_AdvancesProgress_Proportionally()
    {
        var transition = new StripeWipeTransition(duration: 0.5f);

        transition.Update(0.25f);

        Assert.Equal(0.5f, transition.Progress);
        Assert.True(transition.IsCovered);
        Assert.True(transition.IsActive);
    }

    [Fact]
    public void Update_WhenReachingMidpoint_FiresMidpointCallbackOnce()
    {
        int midpointCalls = 0;
        var transition = new StripeWipeTransition(
            duration: 0.4f,
            onMidpoint: () => midpointCalls++);

        transition.Update(0.1f);
        Assert.Equal(0, midpointCalls);

        transition.Update(0.15f); // Total: 0.25s >= 0.20s (midpoint)
        Assert.Equal(1, midpointCalls);
        Assert.True(transition.IsCovered);

        transition.Update(0.1f);
        Assert.Equal(1, midpointCalls); // Should not fire again
    }

    [Fact]
    public void Update_WhenReachingDuration_FiresCompleteCallback()
    {
        int completeCalls = 0;
        var transition = new StripeWipeTransition(
            duration: 0.4f,
            onComplete: () => completeCalls++);

        transition.Update(0.2f);
        Assert.Equal(0, completeCalls);

        transition.Update(0.25f); // Total 0.45s >= 0.4s
        Assert.Equal(1, completeCalls);
        Assert.False(transition.IsActive);
    }

    [Fact]
    public void CalculateStripeBounds_AtProgressZero_LeadingEdgeAtScreenLeft()
    {
        var transition = new StripeWipeTransition(duration: 1.0f);
        int screenWidth = 1280;

        var topBounds = transition.CalculateStripeBounds(0f, screenWidth);
        var bottomBounds = transition.CalculateStripeBounds(1f, screenWidth);

        Assert.Equal(0, topBounds.xLead);
        Assert.Equal(0, topBounds.xTrail);
        Assert.Equal(0, bottomBounds.xTrail);
        Assert.True(bottomBounds.xLead >= 0);
    }

    [Fact]
    public void CalculateStripeBounds_AtMidpoint_FullCoverageAcrossAllRows()
    {
        var transition = new StripeWipeTransition(duration: 1.0f);
        transition.Update(0.5f); // Exact midpoint

        int screenWidth = 1280;

        for (float normY = 0f; normY <= 1.0f; normY += 0.1f)
        {
            var bounds = transition.CalculateStripeBounds(normY, screenWidth);
            Assert.Equal(0, bounds.xTrail);
            Assert.Equal(screenWidth, bounds.xLead);
        }
    }

    [Fact]
    public void CalculateStripeBounds_AtProgressOne_FullClearAcrossAllRows()
    {
        var transition = new StripeWipeTransition(duration: 1.0f);
        transition.Update(1.0f); // Complete

        int screenWidth = 1280;

        for (float normY = 0f; normY <= 1.0f; normY += 0.1f)
        {
            var bounds = transition.CalculateStripeBounds(normY, screenWidth);
            Assert.Equal(screenWidth, bounds.xTrail);
            Assert.Equal(screenWidth, bounds.xLead);
        }
    }

    [Fact]
    public void CalculateStripeBounds_PhaseOne_BottomLeadsTop()
    {
        var transition = new StripeWipeTransition(duration: 1.0f);
        transition.Update(0.25f); // Quarterway through Phase 1

        int screenWidth = 1280;
        var topBounds = transition.CalculateStripeBounds(0f, screenWidth);
        var bottomBounds = transition.CalculateStripeBounds(1f, screenWidth);

        Assert.True(bottomBounds.xLead >= topBounds.xLead,
            $"Bottom lead ({bottomBounds.xLead}) should be >= top lead ({topBounds.xLead})");
    }

    [Fact]
    public void CalculateStripeBounds_PhaseTwo_TopLeadsBottom()
    {
        var transition = new StripeWipeTransition(duration: 1.0f);
        transition.Update(0.75f); // Quarterway through Phase 2

        int screenWidth = 1280;
        var topBounds = transition.CalculateStripeBounds(0f, screenWidth);
        var bottomBounds = transition.CalculateStripeBounds(1f, screenWidth);

        Assert.True(topBounds.xTrail >= bottomBounds.xTrail,
            $"Top trail ({topBounds.xTrail}) should be >= bottom trail ({bottomBounds.xTrail})");
    }

    [Fact]
    public void Properties_CanCustomizeStripeHeightGapAndSlant()
    {
        var transition = new StripeWipeTransition(duration: 0.6f)
        {
            StripeHeight = 20,
            GapHeight = 10,
            Slant = 600f
        };

        Assert.Equal(20, transition.StripeHeight);
        Assert.Equal(10, transition.GapHeight);
        Assert.Equal(30, transition.Period);
        Assert.Equal(600f, transition.Slant);
    }

    [Fact]
    public void FromAndToScreen_AreRetained()
    {
        var from = new DummyScreen();
        var to = new DummyScreen();
        var transition = new StripeWipeTransition(from, to, 0.5f);

        Assert.Same(from, transition.FromScreen);
        Assert.Same(to, transition.ToScreen);
    }

    [Fact]
    public void Update_AfterCompletion_DoesNotReTriggerCallbacks()
    {
        int midCount = 0;
        int compCount = 0;
        var transition = new StripeWipeTransition(
            duration: 0.2f,
            onMidpoint: () => midCount++,
            onComplete: () => compCount++);

        transition.Update(0.25f); // Finish in one step
        Assert.Equal(1, midCount);
        Assert.Equal(1, compCount);

        transition.Update(0.1f); // Further updates
        Assert.Equal(1, midCount);
        Assert.Equal(1, compCount);
    }
}
