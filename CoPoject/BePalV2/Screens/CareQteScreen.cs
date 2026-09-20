using BePalV2.Audio;
using BePalV2.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePalV2.Screens;

public sealed class CareQteScreen : IScreen
{
    private readonly ScreenContext _ctx;
    private readonly CareQteEngine _engine;
    private readonly CareActionType _action;
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;

    private string? _floatingFeedback;
    private Color _floatingColor;
    private float _feedbackTimer;

    private const float WheelCenterX = 640f;
    private const float WheelCenterY = 350f;
    private const float WheelRadius = 150f;

    public CareQteScreen(ScreenContext ctx, CareActionType action)
    {
        _ctx = ctx;
        _action = action;
        _engine = new CareQteEngine(_ctx.Run.ActivePet, action, _ctx.Run.Inventory.EquippedItem);
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_feedbackTimer > 0f)
        {
            _feedbackTimer -= dt;
            if (_feedbackTimer <= 0f) _floatingFeedback = null;
        }

        _engine.Update(dt);

        var kbd = Keyboard.GetState();
        var mouse = Mouse.GetState();

        bool space = kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space);
        bool click = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;

        if ((space || click) && !_engine.IsCompleted)
        {
            var tier = _engine.RecordAttempt();
            ShowFeedback(tier);

            if (_engine.IsCompleted)
            {
                FinishSession();
                return;
            }
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void ShowFeedback(PrecisionTier tier)
    {
        _feedbackTimer = 0.5f;
        switch (tier)
        {
            case PrecisionTier.Perfect:
                _floatingFeedback = "PERFECT! +150";
                _floatingColor = new Color(93, 176, 70);
                _ctx.Audio.PlaySuccess();
                break;
            case PrecisionTier.Good:
                _floatingFeedback = "GOOD! +100";
                _floatingColor = new Color(255, 222, 89);
                _ctx.Audio.PlayConfirm();
                break;
            case PrecisionTier.Miss:
                _floatingFeedback = "MISS!";
                _floatingColor = new Color(231, 25, 31);
                _ctx.Audio.PlayFail();
                break;
        }
    }

    private void FinishSession()
    {
        var run = _ctx.Run;
        var pet = run.ActivePet;
        var equipped = run.Inventory.EquippedItem;
        float expMultiplier = run.Inventory.PermanentTrainExpMultiplier;

        var (grade, _) = _engine.CalculateResults();
        var effectiveTier = grade switch
        {
            CareGrade.S => PrecisionTier.Perfect,
            CareGrade.A or CareGrade.B => PrecisionTier.Good,
            _ => PrecisionTier.Miss
        };

        pet.ExecuteCareAction(_action, effectiveTier, equipped, expMultiplier);

        int apCost = 1;
        if (_action == CareActionType.Heal && !run.Inventory.HasMedicine())
        {
            apCost = 2;
        }
        else if (_action == CareActionType.Heal && run.Inventory.HasMedicine())
        {
            run.Inventory.ConsumeMedicine(out _);
        }
        run.Energy.Spend(apCost);

        run.RecordCareSessionOutcome(_engine);
        _ctx.Audio.PlaySessionComplete();

        if (run.CurrentPhase == DailyPhase.DefenseResolution)
        {
            RouteDefensePhase();
        }
        else
        {
            _ctx.ScreenManager.SetScreen(new BaseHabitatScreen(_ctx));
        }
    }

    private void RouteDefensePhase()
    {
        if (_ctx.Run.DayNumber == 1)
        {
            _ctx.ScreenManager.SetScreen(new CalmingQteScreen(_ctx));
        }
        else if (_ctx.Run.DayNumber == 2)
        {
            _ctx.ScreenManager.SetScreen(new CombatArenaScreen(_ctx, CombatMode.ToothlessTaming));
        }
        else
        {
            _ctx.ScreenManager.SetScreen(new BaseHabitatScreen(_ctx));
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), new Color(22, 22, 26));

        // Slide 17/18 Top Header: Action & "Attemp : 10"
        string actionTarget = _action switch
        {
            CareActionType.Train => "EXP",
            CareActionType.Feed => "STOMACH",
            CareActionType.Clean => "CLEAN",
            CareActionType.Heal => "HEALTH",
            _ => "CARE"
        };
        batch.DrawString(_ctx.Font, $"QTE ACTION: {_action.ToString().ToUpper()} ({actionTarget})", new Vector2(60, 40), Color.White, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0f);

        // Slide 17 concept: "Attemp : 10"
        string attemptStr = $"Attemp : {Math.Max(0, CareQteEngine.TotalAttempts - _engine.CurrentAttemptIndex)} / 10";
        batch.DrawString(_ctx.Font, attemptStr, new Vector2(60, 85), Color.Gold, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);

        // Right Info Box matching Slide 17 / 27
        int rx = 960;
        batch.FillRectangle(new Rectangle(rx, 110, 270, 440), new Color(30, 30, 35));
        batch.DrawRectangle(new Rectangle(rx, 110, 270, 440), new Color(70, 70, 80), 1);

        // Slide 27 concept: "Points : 9999"
        batch.DrawString(_ctx.Font, $"Points : {_engine.GetFinalScore()}", new Vector2(rx + 20, 135), Color.Gold, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        batch.DrawLine(rx + 20, 175, rx + 250, 175, Color.Gray, 1f);

        batch.DrawString(_ctx.Font, $"Streak:      {_engine.CurrentStreak}", new Vector2(rx + 20, 200), Color.White);
        batch.DrawString(_ctx.Font, $"Max Streak:  {_engine.MaxStreak}", new Vector2(rx + 20, 235), Color.White);
        batch.DrawString(_ctx.Font, $"Base Score:  {_engine.TotalScore}", new Vector2(rx + 20, 270), new Color(180, 220, 255));
        batch.DrawString(_ctx.Font, $"Streak Bonus:+{_engine.GetStreakBonus()}", new Vector2(rx + 20, 305), new Color(255, 222, 89));

        batch.DrawString(_ctx.Font, "[ SPACEBAR ]", new Vector2(rx + 20, 430), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, "Hit top green sector", new Vector2(rx + 20, 465), new Color(150, 200, 150));

        // Draw Radial Wheel matching Slide 15
        DrawRadialWheel(batch);

        // Slide 27 / 28 concept: "QTE Energy Progress Bar"
        DrawProgressBar(batch);

        // Floating feedback
        if (!string.IsNullOrEmpty(_floatingFeedback))
        {
            Vector2 fbSize = _ctx.Font.MeasureString(_floatingFeedback);
            Vector2 fbPos = new(WheelCenterX - fbSize.X / 2f, WheelCenterY - 40);
            batch.DrawString(_ctx.Font, _floatingFeedback, fbPos + new Vector2(2, 2), Color.Black, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0f);
            batch.DrawString(_ctx.Font, _floatingFeedback, fbPos, _floatingColor, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0f);
        }
    }

    private void DrawRadialWheel(SpriteBatch batch)
    {
        Vector2 center = new(WheelCenterX, WheelCenterY);

        // Slide 15 4 Action Nodes around the wheel
        DrawActionNode(batch, new Vector2(center.X, center.Y - WheelRadius - 26), "TRAIN", new Color(93, 176, 70), _action == CareActionType.Train);
        DrawActionNode(batch, new Vector2(center.X + WheelRadius + 26, center.Y), "CLEAN", new Color(13, 153, 255), _action == CareActionType.Clean);
        DrawActionNode(batch, new Vector2(center.X - WheelRadius - 26, center.Y), "HEAL", new Color(231, 25, 31), _action == CareActionType.Heal);
        DrawActionNode(batch, new Vector2(center.X, center.Y + WheelRadius + 26), "FEED", new Color(255, 222, 89), _action == CareActionType.Feed);

        // Outer wheel border
        batch.DrawCircle(center, WheelRadius, 64, new Color(55, 55, 65), 6f);
        batch.DrawCircle(center, WheelRadius - 10, 64, new Color(35, 35, 45), 4f);

        // Good zone (yellow)
        DrawArcZone(batch, center, WheelRadius - 8, CareQteEngine.TargetCenterAngle, _engine.GoodWindow, new Color(255, 222, 89, 130), 16f);

        // Perfect zone (green)
        DrawArcZone(batch, center, WheelRadius - 8, CareQteEngine.TargetCenterAngle, _engine.PerfectWindow, new Color(93, 176, 70, 220), 16f);

        // Center hub
        batch.DrawCircle(center, 10f, 16, new Color(40, 40, 50), 20f);
        batch.DrawCircle(center, 20f, 20, Color.White, 2f);

        // Needle
        float nx = center.X + (float)Math.Cos(_engine.CurrentAngle) * (WheelRadius - 4);
        float ny = center.Y + (float)Math.Sin(_engine.CurrentAngle) * (WheelRadius - 4);
        batch.DrawLine(center.X, center.Y, nx, ny, Color.White, 3f);
        batch.DrawCircle(new Vector2(nx, ny), 5f, 12, Color.Cyan, 3f);

        // Arrow
        Vector2 arrowPos = new(center.X, center.Y - WheelRadius - 12);
        batch.DrawString(_ctx.Font, "V", new Vector2(arrowPos.X - 5, arrowPos.Y), Color.Gold, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
    }

    private void DrawActionNode(SpriteBatch batch, Vector2 pos, string label, Color color, bool isActive)
    {
        Rectangle rect = new((int)(pos.X - 35), (int)(pos.Y - 14), 70, 28);
        batch.FillRectangle(rect, isActive ? color : color * 0.4f);
        batch.DrawRectangle(rect, isActive ? Color.White : Color.Gray, isActive ? 2 : 1);
        Vector2 sz = _ctx.Font.MeasureString(label);
        Color txtCol = (color == new Color(255, 222, 89) && isActive) ? Color.Black : Color.White;
        batch.DrawString(_ctx.Font, label, new Vector2(rect.Center.X - sz.X / 2f, rect.Center.Y - sz.Y / 2f), txtCol);
    }

    private void DrawProgressBar(SpriteBatch batch)
    {
        // Slide 27 / 28 concept: "QTE Energy Progress Bar"
        Rectangle barBox = new(180, 580, 920, 40);
        batch.FillRectangle(barBox, new Color(30, 30, 35));
        batch.DrawRectangle(barBox, new Color(70, 70, 80), 1);

        batch.DrawString(_ctx.Font, "PROGRESS BAR:", new Vector2(barBox.X + 16, barBox.Y + 11), Color.White);

        Rectangle fillBg = new(barBox.X + 160, barBox.Y + 10, 560, 20);
        float ratio = Math.Clamp(_engine.DayProgressPercent / 100f, 0f, 1f);
        Rectangle fill = new(barBox.X + 160, barBox.Y + 10, (int)(560 * ratio), 20);

        batch.FillRectangle(fillBg, new Color(45, 45, 50));
        batch.FillRectangle(fill, new Color(93, 176, 70));
        batch.DrawRectangle(fillBg, Color.White, 1);
        batch.DrawString(_ctx.Font, $"{(int)_engine.DayProgressPercent}%", new Vector2(fillBg.Center.X - 15, fillBg.Y + 1), Color.White);

        // AP Indicator
        batch.DrawString(_ctx.Font, $"AP: {_ctx.Run.Energy.CurrentEnergy} / 6", new Vector2(barBox.Right - 140, barBox.Y + 11), Color.Gold);
    }

    private void DrawArcZone(SpriteBatch batch, Vector2 center, float radius, float targetAngle, float halfWindow, Color color, float thickness)
    {
        int segments = 16;
        float start = targetAngle - halfWindow;
        float step = (halfWindow * 2f) / segments;

        for (int i = 0; i < segments; i++)
        {
            float a1 = start + i * step;
            float a2 = start + (i + 1) * step;
            Vector2 p1 = new(center.X + (float)Math.Cos(a1) * radius, center.Y + (float)Math.Sin(a1) * radius);
            Vector2 p2 = new(center.X + (float)Math.Cos(a2) * radius, center.Y + (float)Math.Sin(a2) * radius);
            batch.DrawLine(p1, p2, color, thickness);
        }
    }
}
