using BePalV2.Audio;
using BePalV2.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePalV2.Screens;

public sealed class CalmingQteScreen : IScreen
{
    private readonly ScreenContext _ctx;
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;

    private float _needleAngle;
    private const float AngularVelocity = 2.4f;
    private const float TargetCenter = 1.5f * (float)Math.PI;
    private const float TargetHalfWidth = 0.35f;

    private int _successCount;
    private const int RequiredSuccesses = 3;
    private int _missCount;
    private const int MaxMisses = 4;

    private string? _feedbackText;
    private Color _feedbackColor;
    private float _feedbackTimer;

    private const float WheelCenterX = 640f;
    private const float WheelCenterY = 360f;
    private const float WheelRadius = 150f;

    public CalmingQteScreen(ScreenContext ctx)
    {
        _ctx = ctx;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_feedbackTimer > 0f)
        {
            _feedbackTimer -= dt;
            if (_feedbackTimer <= 0f) _feedbackText = null;
        }

        _needleAngle = (_needleAngle + AngularVelocity * dt) % (float)(2.0 * Math.PI);

        var kbd = Keyboard.GetState();
        var mouse = Mouse.GetState();
        bool space = kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space);
        bool click = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;

        if (space || click)
        {
            CheckHit();
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void CheckHit()
    {
        float diff = Math.Abs(_needleAngle - TargetCenter);
        float tau = (float)(2.0 * Math.PI);
        diff = Math.Min(diff, tau - diff);

        if (diff <= TargetHalfWidth)
        {
            _successCount++;
            _feedbackText = $"CALMING HIT! ({_successCount} / {RequiredSuccesses})";
            _feedbackColor = new Color(100, 220, 255);
            _ctx.Audio.PlaySuccess();

            if (_successCount >= RequiredSuccesses)
            {
                _ctx.Run.CompleteDay1Calming(success: true);
                _ctx.ScreenManager.SetScreen(new DailySummaryScreen(_ctx));
            }
        }
        else
        {
            _missCount++;
            _feedbackText = "PANIC ACCELERATION!";
            _feedbackColor = new Color(240, 70, 70);
            _ctx.Audio.PlayFail();

            if (_missCount >= MaxMisses)
            {
                _ctx.Run.CompleteDay1Calming(success: false);
                _ctx.ScreenManager.SetScreen(new DailySummaryScreen(_ctx));
            }
        }
        _feedbackTimer = 0.6f;
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        // Dark turbulent storm background
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), new Color(14, 18, 30));

        // Thunderstorm header
        batch.DrawString(_ctx.Font, "EMERGENCY: THUNDERSTORM PANIC!", new Vector2(60, 40), new Color(240, 90, 90), 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, "Thunder rattles the habitat roof. Calm your terrified pet before damage occurs!", new Vector2(60, 85), new Color(180, 200, 230));

        // Success counter
        string pips = $"Calm Progress: {_successCount} / {RequiredSuccesses}   |   Misses: {_missCount} / {MaxMisses}";
        batch.DrawString(_ctx.Font, pips, new Vector2(60, 120), Color.Gold);

        // Wheel
        Vector2 center = new(WheelCenterX, WheelCenterY);
        batch.DrawCircle(center, WheelRadius, 64, new Color(50, 70, 100), 5f);

        // Calming zone (cyan)
        int segments = 16;
        float start = TargetCenter - TargetHalfWidth;
        float step = (TargetHalfWidth * 2f) / segments;
        for (int i = 0; i < segments; i++)
        {
            float a1 = start + i * step;
            float a2 = start + (i + 1) * step;
            Vector2 p1 = new(center.X + (float)Math.Cos(a1) * (WheelRadius - 6), center.Y + (float)Math.Sin(a1) * (WheelRadius - 6));
            Vector2 p2 = new(center.X + (float)Math.Cos(a2) * (WheelRadius - 6), center.Y + (float)Math.Sin(a2) * (WheelRadius - 6));
            batch.DrawLine(p1, p2, new Color(80, 200, 255), 14f);
        }

        // Needle
        float nx = center.X + (float)Math.Cos(_needleAngle) * (WheelRadius - 4);
        float ny = center.Y + (float)Math.Sin(_needleAngle) * (WheelRadius - 4);
        batch.DrawLine(center.X, center.Y, nx, ny, Color.White, 3f);
        batch.DrawCircle(center, 10f, 16, new Color(30, 40, 60), 16f);
        batch.DrawCircle(center, 18f, 20, Color.White, 2f);

        // Feedback
        if (!string.IsNullOrEmpty(_feedbackText))
        {
            Vector2 size = _ctx.Font.MeasureString(_feedbackText);
            Vector2 pos = new(center.X - size.X / 2f, center.Y - 40);
            batch.DrawString(_ctx.Font, _feedbackText, pos, _feedbackColor, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        }

        string hint = "Press [ SPACEBAR ] when the needle aligns inside the blue Calming Zone";
        Vector2 hSize = _ctx.Font.MeasureString(hint);
        batch.DrawString(_ctx.Font, hint, new Vector2(640 - hSize.X / 2f, 620), Color.White);
    }
}
