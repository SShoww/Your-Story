#nullable enable
using System;
using System.Collections.Generic;
using BePal.Audio;
using BePal.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePal.Screens;

/// <summary>
/// Encapsulates shared rendering assets, domain state, input state, audio, and UI helpers across screens.
/// </summary>
public sealed class ScreenContext
{
    private readonly Random _random = new();

    public ScreenManager Manager { get; internal set; } = null!;
    public PrototypeRun Run { get; set; } = new();
    public IAudioService Audio { get; set; }

    public SpriteFont Font { get; }
    public Texture2D Pixel { get; }
    public Texture2D Circle { get; }
    public Texture2D PetIdle { get; }
    public Texture2D PetHappy { get; }
    public Texture2D PetAngry { get; }
    public Texture2D? PetImage { get; set; }
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
        Action<string> saveScreenshotAction,
        IAudioService? audio = null)
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
        Audio = audio ?? NullAudioService.Instance;
    }

    public static ScreenContext CreateTestContext(PrototypeRun? run = null, IAudioService? audio = null)
    {
        return new ScreenContext(
            font: null!,
            pixel: null!,
            circle: null!,
            petIdle: null!,
            petHappy: null!,
            petAngry: null!,
            exitGame: () => { },
            saveScreenshotAction: _ => { },
            audio: audio)
        {
            Run = run ?? new PrototypeRun()
        };
    }

    public void SetPetReaction(Texture2D? texture, float duration)
    {
        PetImage = texture;
        ReactionTime = duration;
    }

    public void ResetPetReaction()
    {
        PetImage = PetIdle;
        ReactionTime = 0f;
    }

    public void TriggerShake(float duration, float amount = 8f)
    {
        ShakeTime = duration;
        ShakeAmount = amount;
    }

    public void SpawnTag(string text, Color bg, Color textClr)
    {
        Vector2 pos = new(640f + (float)(_random.NextDouble() * 30 - 15), 320f + (float)(_random.NextDouble() * 20 - 10));
        Tags.Add(new FloatingTag
        {
            Text = text,
            Position = pos,
            BgColor = bg,
            TextColor = textClr
        });
    }

    public bool IsKeyPressed(Keys key) => Keyboard.IsKeyDown(key) && PreviousKeyboard.IsKeyUp(key);

    public bool IsButtonClicked(Rectangle rect)
    {
        Point p = Mouse.Position;
        return rect.Contains(p) &&
               Mouse.LeftButton == ButtonState.Released &&
               PreviousMouse.LeftButton == ButtonState.Pressed;
    }

    public static Rectangle ButtonRect(int y) => new(490, y, 300, 58);

    public void DrawButton(SpriteBatch batch, Rectangle rect, string text, bool enabled = true)
    {
        bool hover = enabled && rect.Contains(Mouse.Position);
        Color fill = !enabled ? new Color(42, 45, 58) : hover ? new Color(85, 95, 125) : new Color(60, 68, 92);
        Color border = !enabled ? new Color(70, 75, 92) : hover ? Color.White : new Color(170, 185, 220);

        batch.FillRectangle(rect, fill);
        batch.DrawRectangle(rect, border, 2f);
        DrawCenterText(batch, text, new Vector2(rect.Center.X, rect.Center.Y), 0.88f, enabled ? Color.White : Color.Gray);
    }

    public void DrawText(SpriteBatch batch, string text, Vector2 pos, Color clr, float scale = 0.8f)
    {
        batch.DrawString(Font, text, pos, clr, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }

    public void DrawCenterText(SpriteBatch batch, string text, Vector2 center, float scale, Color clr)
    {
        Vector2 size = Font.MeasureString(text) * scale;
        Vector2 pos = new(center.X - size.X / 2f, center.Y - size.Y / 2f);
        batch.DrawString(Font, text, pos, clr, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }

    public void DrawHUD(SpriteBatch batch)
    {
        PetDefinition pet = PetCatalog.Get(Run.ActivePet);
        string hazard = pet.HazardLevel switch
        {
            1 => "Lv 1 (Safe)",
            2 => "Lv 2 (Aggressive)",
            3 => "Lv 3 (Trickster)",
            _ => "Unknown"
        };

        // Left Panel: Health status
        RectangleF healthRect = new(40, 24, 250, 72);
        batch.FillRectangle(healthRect, new Color(12, 14, 20, 220));
        batch.DrawRectangle(healthRect, new Color(185, 75, 90), 2f);

        DrawText(batch, $"HEALTH: {Run.Health} / 3", new Vector2(56, 36), new Color(255, 120, 130), 0.85f);
        string hearts = new string('O', Math.Clamp(Run.Health, 0, 3)).PadRight(3, '-');
        DrawText(batch, $"LIVES: [ {hearts} ]", new Vector2(56, 62), Color.LightGray, 0.72f);

        // Right Panel: Day & Pet Status
        RectangleF petRect = new(990, 24, 250, 72);
        batch.FillRectangle(petRect, new Color(12, 14, 20, 220));
        batch.DrawRectangle(petRect, new Color(75, 120, 185), 2f);

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
