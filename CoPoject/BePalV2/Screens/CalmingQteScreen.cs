using BePalV2.Audio;
using BePalV2.Gameplay;
using BePalV2.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePalV2.Screens;

public sealed class CalmingQteScreen : IScreen
{
    private readonly ScreenContext _ctx;
    private readonly Random _random = new();
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;

    private float _needleAngle;
    private const float AngularVelocity = 2.6f;
    private float _targetCenter = 1.5f * (float)Math.PI;
    private const float TargetHalfWidth = 0.35f;

    private int _successCount;
    private const int RequiredSuccesses = 3;
    private int _missCount;
    private const int MaxMisses = 4;

    private string? _feedbackText;
    private Color _feedbackColor;
    private float _feedbackTimer;

    // 1920x1080 Layout Constants
    private const float WheelCenterX = 960f;
    private const float WheelCenterY = 540f;
    private const float WheelRadius = 240f;

    public CalmingQteScreen(ScreenContext ctx)
    {
        _ctx = ctx;
        RandomizeTargetZone();
    }

    private void RandomizeTargetZone()
    {
        _targetCenter = (float)(_random.NextDouble() * Math.PI * 2.0);
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _needleAngle = (_needleAngle + AngularVelocity * dt) % (2f * (float)Math.PI);

        if (_feedbackTimer > 0f)
        {
            _feedbackTimer -= dt;
            if (_feedbackTimer <= 0f)
            {
                _feedbackText = null;
            }
        }

        var kbd = Keyboard.GetState();
        var mouse = Mouse.GetState();

        bool spaceHit = kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space);
        bool mouseHit = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;

        if (spaceHit || mouseHit)
        {
            CheckHit();
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void CheckHit()
    {
        float diff = MathF.Abs(MathHelper.WrapAngle(_needleAngle - _targetCenter));
        if (diff <= TargetHalfWidth)
        {
            _successCount++;
            _feedbackText = "CALMED! +1";
            _feedbackColor = UITheme.AccentEmerald;
            _ctx.Audio.PlaySuccess();
            RandomizeTargetZone();

            if (_successCount >= RequiredSuccesses)
            {
                _ctx.Run.Day1CalmingCompleted = true;
                _ctx.Audio.PlaySessionComplete();
                _ctx.ScreenManager.SetScreen(new DailySummaryScreen(_ctx));
            }
        }
        else
        {
            _missCount++;
            _feedbackText = "MISSED! PET STRESSED";
            _feedbackColor = UITheme.AccentCoral;
            _ctx.Audio.PlayFail();
            RandomizeTargetZone();

            if (_missCount >= MaxMisses)
            {
                _ctx.Run.ActivePet.TakeDamage(25f);
                _ctx.Run.Day1CalmingCompleted = true;
                _ctx.ScreenManager.SetScreen(new DailySummaryScreen(_ctx));
            }
        }
        _feedbackTimer = 0.55f;
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        // Dark turbulent storm background
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), new Color(14, 18, 30));

        // Header Title
        batch.DrawString(_ctx.Font, "EMERGENCY: THUNDERSTORM PANIC!", new Vector2(80, 50), new Color(240, 90, 90), 0f, Vector2.Zero, 1.45f, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, "Thunder rattles the habitat roof. Calm your terrified pet before stress damage occurs!", new Vector2(80, 110), new Color(180, 200, 230));

        // Success counter badge
        string pips = $"Calm Progress: {_successCount} / {RequiredSuccesses}   |   Misses: {_missCount} / {MaxMisses}";
        Rectangle pipsBadge = new(80, 150, 480, 36);
        CleanUI.DrawBadge(batch, _ctx.Font, pipsBadge, pips, new Color(28, 38, 54), UITheme.AccentGold);

        // Wheel
        Vector2 center = new(WheelCenterX, WheelCenterY);
        batch.DrawCircle(center, WheelRadius + 8, 64, new Color(42, 54, 76), 2f);
        batch.DrawCircle(center, WheelRadius, 64, new Color(50, 70, 100), 6f);
        batch.DrawCircle(center, WheelRadius - 16, 64, new Color(42, 54, 76), 2f);

        // Randomized Calming zone (cyan)
        int segments = 24;
        float start = _targetCenter - TargetHalfWidth;
        float step = (TargetHalfWidth * 2f) / segments;
        for (int i = 0; i < segments; i++)
        {
            float a1 = start + i * step;
            float a2 = start + (i + 1) * step;
            Vector2 p1 = new(center.X + (float)Math.Cos(a1) * (WheelRadius - 8), center.Y + (float)Math.Sin(a1) * (WheelRadius - 8));
            Vector2 p2 = new(center.X + (float)Math.Cos(a2) * (WheelRadius - 8), center.Y + (float)Math.Sin(a2) * (WheelRadius - 8));
            batch.DrawLine(p1, p2, UITheme.AccentCyan, 18f);
        }

        // Center hub
        batch.DrawCircle(center, 18f, 24, new Color(24, 32, 48), 18f);
        batch.DrawCircle(center, 24f, 24, Color.White, 2f);

        // Rotating Needle
        float nx = center.X + (float)Math.Cos(_needleAngle) * (WheelRadius - 6);
        float ny = center.Y + (float)Math.Sin(_needleAngle) * (WheelRadius - 6);
        batch.DrawLine(center.X, center.Y, nx, ny, Color.White, 3f);
        batch.DrawCircle(new Vector2(nx, ny), 7f, 16, UITheme.AccentCyan, 2f);

        // Feedback
        if (!string.IsNullOrEmpty(_feedbackText))
        {
            Vector2 size = _ctx.Font.MeasureString(_feedbackText);
            Vector2 pos = new(center.X - (size.X * 1.4f) / 2f, center.Y - 50);
            batch.DrawString(_ctx.Font, _feedbackText, pos, _feedbackColor, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0f);
        }

        string hint = "Press [ SPACEBAR ] when the needle rotates inside the randomized blue Calming Zone";
        Vector2 hSize = _ctx.Font.MeasureString(hint);
        batch.DrawString(_ctx.Font, hint, new Vector2(960 - hSize.X / 2f, 960), Color.White);
    }
}
