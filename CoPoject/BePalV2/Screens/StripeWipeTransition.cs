using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BePalV2.Screens;

public sealed class StripeWipeTransition
{
    public float Duration { get; set; } = 0.85f;
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
        float duration = 0.85f,
        Action? onMidpoint = null,
        Action? onComplete = null)
    {
        FromScreen = fromScreen;
        ToScreen = toScreen;
        Duration = Math.Max(0.01f, duration);
        _onMidpoint = onMidpoint;
        _onComplete = onComplete;
    }

    public void Update(float dt)
    {
        if (!IsActive) return;

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

    public (int xTrail, int xLead) CalculateStripeBounds(float normY, int screenWidth)
    {
        float p = Progress;
        float slantOffset = normY * Slant;

        if (p < 0.5f)
        {
            float subT = Ease(p * 2f);
            float leadPos = subT * (screenWidth + Slant);
            int xLead = (int)MathF.Ceiling(leadPos - Slant + slantOffset);
            xLead = Math.Clamp(xLead, 0, screenWidth);
            return (0, xLead);
        }
        else
        {
            float subT = Ease((p - 0.5f) * 2f);
            float trailPos = subT * (screenWidth + Slant);
            int xTrail = (int)MathF.Floor(trailPos - slantOffset);
            xTrail = Math.Clamp(xTrail, 0, screenWidth);
            return (xTrail, screenWidth);
        }
    }

    private static float Ease(float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return t * t * (3f - 2f * t);
    }

    public void Draw(SpriteBatch batch, Texture2D pixel, int screenWidth, int screenHeight)
    {
        if (!IsActive) return;

        float p = Progress;
        float bgAlpha = p < 0.5f
            ? Ease(p * 2f)
            : 1f - Ease((p - 0.5f) * 2f);
        Color bg = BackgroundColor * bgAlpha;

        if (bg.A > 0)
        {
            batch.Draw(pixel, new Rectangle(0, 0, screenWidth, screenHeight), bg);
        }

        int numStripes = (screenHeight + Period - 1) / Period;
        for (int i = 0; i < numStripes; i++)
        {
            int y = i * Period;
            int height = Math.Min(StripeHeight, screenHeight - y);
            float normY = y / (float)screenHeight;

            var (xTrail, xLead) = CalculateStripeBounds(normY, screenWidth);
            int width = xLead - xTrail;
            if (width > 0)
            {
                batch.Draw(pixel, new Rectangle(xTrail, y, width, height), StripeColor);
            }
        }
    }
}
