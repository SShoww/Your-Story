using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace BePalV2.UI;

public static class UITheme
{
    // Backgrounds
    public static readonly Color BgDeep = new(14, 16, 20);
    public static readonly Color BgPanel = new(22, 25, 32);
    public static readonly Color BgPanelHover = new(30, 34, 44);
    public static readonly Color BgCardRecessed = new(18, 20, 26);
    public static readonly Color BgOverlay = new(8, 10, 14, 220);

    // Borders
    public static readonly Color BorderSubtle = new(42, 48, 62);
    public static readonly Color BorderLight = new(68, 76, 98);
    public static readonly Color BorderHighlight = new(180, 192, 215);

    // Accents
    public static readonly Color AccentGold = new(230, 182, 92);
    public static readonly Color AccentCyan = new(74, 180, 216);
    public static readonly Color AccentEmerald = new(76, 188, 136);
    public static readonly Color AccentCoral = new(220, 72, 72);
    public static readonly Color AccentPurple = new(162, 100, 222);

    // Typography
    public static readonly Color TextPrimary = new(245, 248, 252);
    public static readonly Color TextSecondary = new(165, 175, 195);
    public static readonly Color TextMuted = new(105, 115, 132);
}

public static class CleanUI
{
    public static void DrawPanel(SpriteBatch batch, Rectangle rect, Color? bg = null, Color? border = null, int borderWidth = 1, bool shadow = true)
    {
        if (shadow)
        {
            Rectangle shadowRect = new(rect.X + 2, rect.Y + 3, rect.Width, rect.Height);
            batch.FillRectangle(shadowRect, Color.Black * 0.45f);
        }

        Color bgColor = bg ?? UITheme.BgPanel;
        Color borderColor = border ?? UITheme.BorderSubtle;

        batch.FillRectangle(rect, bgColor);
        if (borderWidth > 0)
        {
            batch.DrawRectangle(rect, borderColor, borderWidth);
        }
    }

    public static void DrawButton(
        SpriteBatch batch,
        SpriteFont font,
        Rectangle rect,
        string text,
        bool isHovered,
        Color? accent = null,
        string? hotkey = null,
        bool isPrimary = false)
    {
        Rectangle drawRect = isHovered ? new(rect.X, rect.Y - 1, rect.Width, rect.Height) : rect;

        // Shadow
        batch.FillRectangle(new Rectangle(drawRect.X + 2, drawRect.Y + 3, drawRect.Width, drawRect.Height), Color.Black * 0.4f);

        Color baseBg = isPrimary ? new Color(38, 48, 66) : UITheme.BgPanel;
        Color bg = isHovered ? (isPrimary ? new Color(52, 68, 94) : UITheme.BgPanelHover) : baseBg;

        Color border;
        if (accent.HasValue)
        {
            border = isHovered ? accent.Value : Color.Lerp(accent.Value, UITheme.BorderSubtle, 0.4f);
        }
        else if (isPrimary)
        {
            border = isHovered ? UITheme.AccentGold : UITheme.BorderLight;
        }
        else
        {
            border = isHovered ? UITheme.BorderHighlight : UITheme.BorderSubtle;
        }

        batch.FillRectangle(drawRect, bg);
        batch.DrawRectangle(drawRect, border, isHovered ? 2 : 1);

        // Text rendering
        string fullLabel = string.IsNullOrEmpty(hotkey) ? text : $"{text}  {hotkey}";
        Vector2 size = font.MeasureString(fullLabel);
        Vector2 pos = new(drawRect.Center.X - size.X / 2f, drawRect.Center.Y - size.Y / 2f);

        Color textCol = isHovered ? UITheme.TextPrimary : (isPrimary ? UITheme.AccentGold : UITheme.TextSecondary);
        batch.DrawString(font, fullLabel, pos, textCol);
    }

    public static void DrawProgressBar(
        SpriteBatch batch,
        SpriteFont font,
        Rectangle rect,
        float ratio,
        Color fillColor,
        string? leftText = null,
        string? rightText = null,
        Color? emptyColor = null)
    {
        // Trough
        Color troughColor = emptyColor ?? UITheme.BgCardRecessed;
        batch.FillRectangle(rect, troughColor);
        batch.DrawRectangle(rect, UITheme.BorderSubtle, 1);

        // Fill
        float clamped = Math.Clamp(ratio, 0f, 1f);
        int fillW = (int)Math.Round((rect.Width - 4) * clamped);
        if (fillW > 0)
        {
            Rectangle fillRect = new(rect.X + 2, rect.Y + 2, fillW, rect.Height - 4);
            batch.FillRectangle(fillRect, fillColor);
        }

        // Labels
        if (!string.IsNullOrEmpty(leftText))
        {
            Vector2 lPos = new(rect.X + 6, rect.Center.Y - font.LineSpacing / 2f + 1);
            batch.DrawString(font, leftText, lPos, UITheme.TextPrimary);
        }

        if (!string.IsNullOrEmpty(rightText))
        {
            Vector2 rSize = font.MeasureString(rightText);
            Vector2 rPos = new(rect.Right - rSize.X - 6, rect.Center.Y - font.LineSpacing / 2f + 1);
            batch.DrawString(font, rightText, rPos, UITheme.TextSecondary);
        }
    }

    public static void DrawKeycap(
        SpriteBatch batch,
        SpriteFont font,
        Rectangle rect,
        string label,
        bool isPressed = false,
        bool isAccent = false)
    {
        Rectangle drawRect = isPressed ? new(rect.X, rect.Y + 1, rect.Width, rect.Height) : rect;

        // Bottom 3D bevel line
        batch.FillRectangle(new Rectangle(drawRect.X, drawRect.Bottom, drawRect.Width, 3), new Color(10, 12, 16));

        Color bg = isPressed ? new Color(20, 24, 30) : (isAccent ? new Color(34, 42, 56) : new Color(26, 30, 38));
        Color border = isAccent ? UITheme.AccentGold : UITheme.BorderLight;

        batch.FillRectangle(drawRect, bg);
        batch.DrawRectangle(drawRect, border, 1);

        Vector2 size = font.MeasureString(label);
        Vector2 pos = new(drawRect.Center.X - size.X / 2f, drawRect.Center.Y - size.Y / 2f);
        Color textCol = isAccent ? UITheme.AccentGold : UITheme.TextPrimary;
        batch.DrawString(font, label, pos, textCol);
    }

    public static void DrawBadge(
        SpriteBatch batch,
        SpriteFont font,
        Rectangle rect,
        string text,
        Color bg,
        Color textCol)
    {
        batch.FillRectangle(rect, bg);
        batch.DrawRectangle(rect, bg * 1.3f, 1);
        Vector2 size = font.MeasureString(text);
        Vector2 pos = new(rect.Center.X - size.X / 2f, rect.Center.Y - size.Y / 2f);
        batch.DrawString(font, text, pos, textCol);
    }

    public static void DrawModalBackdrop(SpriteBatch batch, int screenWidth, int screenHeight, float alpha = 0.75f)
    {
        batch.FillRectangle(new Rectangle(0, 0, screenWidth, screenHeight), Color.Black * alpha);
    }

    public static void DrawWindowHeader(
        SpriteBatch batch,
        SpriteFont font,
        int screenWidth,
        Point mousePos,
        bool click,
        Action onToggleFullscreen,
        Action onClose)
    {
        // Minimalist top-right borderless controls: [ [] ] maximize/fullscreen, [ X ] close
        Rectangle maxRect = new(screenWidth - 68, 6, 26, 22);
        Rectangle closeRect = new(screenWidth - 34, 6, 26, 22);

        bool maxHover = maxRect.Contains(mousePos);
        bool closeHover = closeRect.Contains(mousePos);

        // Maximize/Fullscreen button
        batch.FillRectangle(maxRect, maxHover ? UITheme.BgPanelHover : new Color(20, 24, 30, 180));
        batch.DrawRectangle(maxRect, maxHover ? UITheme.AccentCyan : UITheme.BorderSubtle, 1);
        Vector2 maxSize = font.MeasureString("o");
        batch.DrawString(font, "o", new Vector2(maxRect.Center.X - maxSize.X / 2f, maxRect.Center.Y - maxSize.Y / 2f - 1), maxHover ? UITheme.AccentCyan : UITheme.TextSecondary);

        // Close button
        batch.FillRectangle(closeRect, closeHover ? new Color(140, 36, 36) : new Color(20, 24, 30, 180));
        batch.DrawRectangle(closeRect, closeHover ? UITheme.AccentCoral : UITheme.BorderSubtle, 1);
        Vector2 closeSize = font.MeasureString("x");
        batch.DrawString(font, "x", new Vector2(closeRect.Center.X - closeSize.X / 2f, closeRect.Center.Y - closeSize.Y / 2f - 1), closeHover ? Color.White : UITheme.TextSecondary);

        if (click)
        {
            if (maxHover) onToggleFullscreen?.Invoke();
            if (closeHover) onClose?.Invoke();
        }
    }
}
