#nullable enable
using System;
using System.Collections.Generic;
using BePal.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePal.Screens;

/// <summary>
/// Encapsulates shared rendering assets, domain state, input state, and UI helpers across screens.
/// </summary>
public sealed class ScreenContext
{
    private readonly Random _random = new();

    public ScreenManager Manager { get; internal set; } = null!;
    public PrototypeRun Run { get; set; } = new();

    public SpriteFont Font { get; }
    public Texture2D Pixel { get; }
    public Texture2D Circle { get; }
    public Texture2D PetIdle { get; }
    public Texture2D PetHappy { get; }
    public Texture2D PetAngry { get; }

    public Texture2D PetImage { get; set; }
    public float ReactionTime { get; set; }
    public float ShakeTime { get; set; }
    public float ShakeAmount { get; set; } = 8f;
    public string Message { get; set; } = "Welcome home.";

    public List<FloatingTag> Tags { get; } = new();

    public KeyboardState Keyboard { get; internal set; }
    public KeyboardState PreviousKeyboard { get; internal set; }
    public MouseState Mouse { get; internal set; }
    public MouseState PreviousMouse { get; internal set; }

    public Action ExitGame { get; }
    public Action<string> SaveScreenshotAction { get; }

    public ScreenContext(
        SpriteFont font,
        Texture2D pixel,
        Texture2D circle,
        Texture2D petIdle,
        Texture2D petHappy,
        Texture2D petAngry,
        Action exitGame,
        Action<string> saveScreenshotAction)
    {
        Font = font;
        Pixel = pixel;
        Circle = circle;
        PetIdle = petIdle;
        PetHappy = petHappy;
        PetAngry = petAngry;
        PetImage = petIdle;
        ExitGame = exitGame;
        SaveScreenshotAction = saveScreenshotAction;
    }

    public void SetPetReaction(Texture2D texture, float duration)
    {
        PetImage = texture;
        ReactionTime = duration;
    }

    public void ResetPetReaction()
    {
        PetImage = PetIdle;
        ReactionTime = 0;
    }

    public void TriggerShake(float time = 0.22f, float amount = 8f)
    {
        ShakeTime = time;
        ShakeAmount = amount;
    }

    public void SpawnTag(string text, Color bg, Color fg, Vector2? pos = null)
    {
        float rot = ((float)_random.NextDouble() - 0.5f) * 0.18f;
        Tags.Add(new FloatingTag
        {
            Text = text,
            Position = pos ?? new Vector2(640, 235),
            BgColor = bg,
            TextColor = fg,
            Lifetime = 0.85f,
            MaxLifetime = 0.85f,
            Rotation = rot
        });
    }

    public void ClearTags() => Tags.Clear();

    public bool IsKeyPressed(Keys key) => Keyboard.IsKeyDown(key) && PreviousKeyboard.IsKeyUp(key);

    public bool IsButtonClicked(Rectangle bounds) =>
        Mouse.LeftButton == ButtonState.Pressed &&
        PreviousMouse.LeftButton == ButtonState.Released &&
        bounds.Contains(Mouse.Position);

    public static Rectangle ButtonRect(int y) => new(490, y, 300, 60);

    public void DrawText(SpriteBatch batch, string text, Vector2 pos, Color color) =>
        batch.DrawString(Font, text, pos, color);

    public void DrawCenterText(SpriteBatch batch, string text, Vector2 pos, float scale, Color color) =>
        batch.DrawString(Font, text, pos - Font.MeasureString(text) * scale / 2f, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

    public void DrawButton(SpriteBatch batch, Rectangle bounds, string label, bool enabled)
    {
        Color bg = enabled ? new Color(90, 116, 168) : new Color(71, 74, 85);
        Color border = enabled ? new Color(255, 203, 139) : Color.Gray;
        Color textCol = enabled ? Color.White : Color.LightGray;

        batch.Draw(Pixel, bounds, bg);
        batch.DrawRectangle(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height), border, 2f);
        DrawCenterText(batch, label, bounds.Center.ToVector2(), 0.8f, textCol);
    }

    public void DrawHUD(SpriteBatch batch)
    {
        // Health Badge Top-Left (Death Spiral style)
        RectangleF hpRect = new(36, 20, 260, 70);
        batch.FillRectangle(hpRect, new Color(22, 25, 38));
        batch.DrawRectangle(hpRect, new Color(255, 100, 110), 2f);
        string hearts = "";
        for (int i = 1; i <= 3; i++) hearts += i <= Run.Health ? "[X] " : "[ ] ";
        DrawCenterText(batch, $"HEALTH {Run.Health}.0", new Vector2(hpRect.Center.X, hpRect.Y + 20f), 0.85f, Color.White);
        DrawCenterText(batch, $"LIVES: {hearts.Trim()}", new Vector2(hpRect.Center.X, hpRect.Y + 48f), 0.72f, new Color(255, 140, 140));

        // Pet Info Top-Right
        RectangleF petRect = new(860, 20, 384, 70);
        batch.FillRectangle(petRect, new Color(22, 25, 38));
        batch.DrawRectangle(petRect, new Color(255, 203, 139), 2f);
        PetDefinition pet = PetCatalog.Get(Run.ActivePet);
        string hazard = pet.Kind switch
        {
            PetKind.Baseline => $"Hazard Lv {pet.HazardLevel} ({pet.HarmType})",
            PetKind.Attacker => $"Hazard Lv {pet.HazardLevel} (Aggressive)",
            _ => $"Hazard Lv {pet.HazardLevel} (Teleporting)"
        };
        DrawCenterText(batch, $"DAY {Run.DayNumber} / 5", new Vector2(petRect.Center.X, petRect.Y + 20f), 0.85f, Color.White);
        DrawCenterText(batch, $"{pet.Name.ToUpperInvariant()} - {hazard}", new Vector2(petRect.Center.X, petRect.Y + 48f), 0.68f, new Color(255, 203, 139));
    }

    public void DrawFloatingTags(SpriteBatch batch)
    {
        for (int i = 0; i < Tags.Count; i++)
        {
            FloatingTag tag = Tags[i];
            float alpha = MathF.Min(1f, tag.Lifetime / 0.22f);
            Vector2 size = Font.MeasureString(tag.Text) * 0.80f;
            RectangleF r = new(tag.Position.X - size.X / 2f - 16f, tag.Position.Y - size.Y / 2f - 8f, size.X + 32f, size.Y + 16f);

            batch.FillRectangle(r, tag.BgColor * alpha);
            batch.DrawRectangle(r, tag.TextColor * alpha, 2f);
            DrawCenterText(batch, tag.Text, tag.Position, 0.78f, tag.TextColor * alpha);
        }
    }

    public void DrawScrapBadge(SpriteBatch batch, Vector2 center, string title, string subtitle, Color accentColor, Color textColor, bool highlight)
    {
        Vector2 titleSize = Font.MeasureString(title) * 0.72f;
        Vector2 subSize = Font.MeasureString(subtitle) * 0.52f;
        float w = MathF.Max(titleSize.X, subSize.X) + 24f;
        float h = 46f;
        RectangleF rect = new(center.X - w / 2f, center.Y - h / 2f, w, h);

        Color bg = highlight ? new Color(48, 56, 80) : new Color(24, 27, 40);
        batch.FillRectangle(rect, bg);
        batch.DrawRectangle(rect, highlight ? Color.White : accentColor, highlight ? 3f : 2f);

        DrawCenterText(batch, title, new Vector2(center.X, center.Y - 8f), 0.70f, highlight ? Color.White : accentColor);
        DrawCenterText(batch, subtitle, new Vector2(center.X, center.Y + 11f), 0.50f, Color.LightGray);
    }
}
