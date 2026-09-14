#nullable enable
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BePal.Screens;

/// <summary>
/// Geometric Venetian-blinds diagonal stripe wipe scene transition.
/// Sweeps alternating horizontal bars across the screen with an angled leading edge
/// (bottom-leading) to fully occlude the outgoing screen, then clears with an angled
/// trailing edge (top-leading) to reveal the incoming screen.
/// </summary>
public sealed class StripeWipeTransition
{
    public float Duration { get; set; } = 0.45f;
    public float Elapsed { get; private set; }
    public float Progress => Math.Clamp(Duration > 0f ? Elapsed / Duration : 1f, 0f, 1f);

    public int StripeHeight { get; set; } = 14;
    public int GapHeight { get; set; } = 14;
    public int Period => StripeHeight + GapHeight;
    public float Slant { get; set; } = 540f;

    public Color StripeColor { get; set; } = Color.White;
    public Color BackgroundColor { get; set; } = new Color(14, 16, 24);

    public bool IsActive => Elapsed < Duration;
    public bool IsCovered => Progress >= 0.5f;

    public IScreen? FromScreen { get; }
    public IScreen? ToScreen { get; }

    private readonly Action? _onMidpoint;
    private readonly Action? _onComplete;
    private bool _midpointFired;
    private bool _completeFired;

    public StripeWipeTransition(
        IScreen? fromScreen = null,
        IScreen? toScreen = null,
        float duration = 0.45f,
        Action? onMidpoint = null,
        Action? onComplete = null)
    {
        FromScreen = fromScreen;
        ToScreen = toScreen;
        Duration = duration;
        _onMidpoint = onMidpoint;
        _onComplete = onComplete;
    }

    public void Update(float dt)
    {
        if (!IsActive && Progress >= 1f) return;

        Elapsed += dt;

        if (!_midpointFired && Progress >= 0.5f)
        {
            _midpointFired = true;
            _onMidpoint?.Invoke();
        }

        if (!_completeFired && Progress >= 1f)
        {
            _completeFired = true;
            _onComplete?.Invoke();
        }
    }

    /// <summary>
    /// Computes horizontal bounds [xTrail, xLead] for a stripe located at normalized vertical position normY (0.0 at top, 1.0 at bottom).
    /// </summary>
    public (int xTrail, int xLead) CalculateStripeBounds(float normY, int screenWidth)
    {
        float t = Progress;

        float leadT;
        float trailT;

        if (t < 0.5f)
        {
            leadT = t / 0.5f;
            trailT = 0f;
        }
        else
        {
            leadT = 1f;
            trailT = (t - 0.5f) / 0.5f;
        }

        // Apply smoothstep easing to make start and stop feel organic
        leadT = Ease(leadT);
        trailT = Ease(trailT);

        float leadBaseX = -Slant + leadT * (screenWidth + 2f * Slant);
        float trailBaseX = -Slant + trailT * (screenWidth + 2f * Slant);

        // Phase 1 (Enter): bottom leads => normY * Slant
        float leadX = leadBaseX + normY * Slant;

        // Phase 2 (Exit): top leads => (1.0 - normY) * Slant
        float trailX = trailBaseX + (1f - normY) * Slant;

        int clampedLead = Math.Clamp((int)MathF.Round(leadX), 0, screenWidth);
        int clampedTrail = Math.Clamp((int)MathF.Round(trailX), 0, screenWidth);

        return (clampedTrail, clampedLead);
    }

    private static float Ease(float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return t * t * (3f - 2f * t); // Smoothstep
    }

    public void Draw(SpriteBatch batch, Texture2D pixel, int screenWidth, int screenHeight)
    {
        if (!IsActive && Progress >= 1f) return;

        int period = Period;
        int stripeHeight = StripeHeight;
        int numStripes = (screenHeight + period - 1) / period;

        for (int i = 0; i < numStripes; i++)
        {
            int y1 = i * period;
            float yMid = y1 + period / 2f;
            float normY = Math.Clamp(yMid / (float)screenHeight, 0f, 1f);

            var (xTrail, xLead) = CalculateStripeBounds(normY, screenWidth);

            int width = xLead - xTrail;
            if (width <= 0) continue;

            // Draw dark background backing for this period slice
            int sliceHeight = Math.Min(screenHeight - y1, period);
            batch.Draw(pixel, new Rectangle(xTrail, y1, width, sliceHeight), BackgroundColor);

            // Draw white stripe on upper portion of slice
            int stripeH = Math.Min(screenHeight - y1, stripeHeight);
            batch.Draw(pixel, new Rectangle(xTrail, y1, width, stripeH), StripeColor);
        }
    }
}
