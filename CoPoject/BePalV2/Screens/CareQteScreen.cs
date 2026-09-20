using BePalV2.Audio;
using BePalV2.Gameplay;
using BePalV2.UI;
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
        _engine = new CareQteEngine(ctx.Run.ActivePet, action, ctx.Run.Inventory.EquippedItem);
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _engine.Update(dt);

        if (_feedbackTimer > 0f)
        {
            _feedbackTimer -= dt;
            if (_feedbackTimer <= 0f)
            {
                _floatingFeedback = null;
            }
        }

        var kbd = Keyboard.GetState();
        var mouse = Mouse.GetState();

        bool spaceHit = kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space);
        bool mouseHit = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;
        if ((spaceHit || mouseHit) && !_engine.IsCompleted)
        {
            var tier = _engine.RecordAttempt();
            ShowFeedback(tier);

            if (_engine.IsCompleted)
            {
                FinishSession();
            }
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void ShowFeedback(PrecisionTier tier)
    {
        _feedbackTimer = 0.55f;
        switch (tier)
        {
            case PrecisionTier.Perfect:
                _floatingFeedback = "PERFECT! +150";
                _floatingColor = UITheme.AccentEmerald;
                _ctx.Audio.PlaySuccess();
                break;
            case PrecisionTier.Good:
                _floatingFeedback = "GOOD! +100";
                _floatingColor = UITheme.AccentGold;
                _ctx.Audio.PlayConfirm();
                break;
            case PrecisionTier.Miss:
                _floatingFeedback = "MISS!";
                _floatingColor = UITheme.AccentCoral;
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
        // Dark deep background
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), UITheme.BgDeep);

        // Top Header
        string actionTarget = _action switch
        {
            CareActionType.Train => "EXPERIENCE",
            CareActionType.Feed => "STOMACH",
            CareActionType.Clean => "CLEANLINESS",
            CareActionType.Heal => "HEALTH",
            _ => "CARE"
        };
        batch.DrawString(_ctx.Font, $"CARE PROTOCOL: {_action.ToString().ToUpper()} ({actionTarget})", new Vector2(60, 32), UITheme.TextPrimary, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

        // Attempt Badge
        int attemptsLeft = Math.Max(0, CareQteEngine.TotalAttempts - _engine.CurrentAttemptIndex);
        Rectangle attBadge = new(60, 72, 160, 28);
        CleanUI.DrawBadge(batch, _ctx.Font, attBadge, $"ATTEMPTS: {attemptsLeft} / 10", new Color(34, 42, 58), UITheme.AccentGold);

        // Action mechanic guide text
        string mechanicNote = _action switch
        {
            CareActionType.Train => "Train: Accelerated needle speed with narrow calibration window",
            CareActionType.Feed => "Feed: Erratic rotation dynamically inverting needle vector",
            CareActionType.Clean => "Clean: Evasive target zone actively shifting around perimeter",
            CareActionType.Heal => "Heal: Fluctuating sensory signal intermittently flickering target",
            _ => ""
        };
        batch.DrawString(_ctx.Font, mechanicNote, new Vector2(60, 114), UITheme.TextSecondary);

        // Right Research Score Panel
        int rx = 960;
        Rectangle scorePanel = new(rx, 110, 270, 440);
        CleanUI.DrawPanel(batch, scorePanel, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(scorePanel.X, scorePanel.Y, scorePanel.Width, 3), UITheme.AccentGold);

        batch.DrawString(_ctx.Font, "EVALUATION LOG", new Vector2(rx + 20, 130), UITheme.AccentGold, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);
        batch.DrawLine(rx + 20, 162, rx + 250, 162, UITheme.BorderSubtle, 1f);

        batch.DrawString(_ctx.Font, $"Score:       {_engine.GetFinalScore()}", new Vector2(rx + 20, 180), UITheme.TextPrimary);
        batch.DrawString(_ctx.Font, $"Current Streak:{_engine.CurrentStreak}", new Vector2(rx + 20, 215), UITheme.TextSecondary);
        batch.DrawString(_ctx.Font, $"Max Streak:  {_engine.MaxStreak}", new Vector2(rx + 20, 250), UITheme.TextSecondary);
        batch.DrawString(_ctx.Font, $"Base Points: {_engine.TotalScore}", new Vector2(rx + 20, 285), UITheme.AccentCyan);
        batch.DrawString(_ctx.Font, $"Streak Bonus:+{_engine.GetStreakBonus()}", new Vector2(rx + 20, 320), UITheme.AccentGold);

        Rectangle keycapBox = new(rx + 30, 430, 210, 44);
        CleanUI.DrawKeycap(batch, _ctx.Font, keycapBox, "SPACEBAR", isPressed: false, isAccent: true);

        Vector2 subPrompt = _ctx.Font.MeasureString("Align needle inside target");
        batch.DrawString(_ctx.Font, "Align needle inside target", new Vector2(scorePanel.Center.X - subPrompt.X / 2f, 486), UITheme.TextMuted);

        // Draw Radial Wheel
        DrawRadialWheel(batch);

        // Bottom Stat Progress Bar
        DrawStatProgressBar(batch);

        // Floating feedback
        if (!string.IsNullOrEmpty(_floatingFeedback))
        {
            Vector2 fbSize = _ctx.Font.MeasureString(_floatingFeedback);
            Vector2 fbPos = new(WheelCenterX - (fbSize.X * 1.3f) / 2f, WheelCenterY - 40);
            batch.DrawString(_ctx.Font, _floatingFeedback, fbPos + new Vector2(2, 2), Color.Black * 0.7f, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
            batch.DrawString(_ctx.Font, _floatingFeedback, fbPos, _floatingColor, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        }
    }

    private void DrawRadialWheel(SpriteBatch batch)
    {
        Vector2 center = new(WheelCenterX, WheelCenterY);

        // 4 Fixed Action Nodes around the wheel
        DrawActionNode(batch, new Vector2(center.X - 120, center.Y - 120), "TRAIN", UITheme.AccentGold, _action == CareActionType.Train);
        DrawActionNode(batch, new Vector2(center.X + 120, center.Y - 120), "HEAL", UITheme.AccentEmerald, _action == CareActionType.Heal);
        DrawActionNode(batch, new Vector2(center.X - 120, center.Y + 120), "CLEAN", UITheme.AccentCyan, _action == CareActionType.Clean);
        DrawActionNode(batch, new Vector2(center.X + 120, center.Y + 120), "FEED", new Color(230, 130, 60), _action == CareActionType.Feed);

        // Concentric track circles
        batch.DrawCircle(center, WheelRadius + 8, 64, UITheme.BorderSubtle, 1f);
        batch.DrawCircle(center, WheelRadius, 64, UITheme.BorderLight, 3f);
        batch.DrawCircle(center, WheelRadius - 16, 64, UITheme.BorderSubtle, 1f);

        // Good & Perfect zones
        if (_engine.IsTargetVisible)
        {
            float targetAngle = _engine.CurrentTargetCenterAngle;
            // Good zone (warm amber)
            DrawArcZone(batch, center, WheelRadius - 8, targetAngle, _engine.GoodWindow, UITheme.AccentGold * 0.65f, 14f);
            // Perfect zone (emerald)
            DrawArcZone(batch, center, WheelRadius - 8, targetAngle, _engine.PerfectWindow, UITheme.AccentEmerald, 14f);
        }

        // Center hub
        batch.DrawCircle(center, 12f, 24, UITheme.BgPanel, 14f);
        batch.DrawCircle(center, 18f, 24, UITheme.BorderHighlight, 2f);

        // Laser Needle
        float nx = center.X + (float)Math.Cos(_engine.CurrentAngle) * (WheelRadius - 4);
        float ny = center.Y + (float)Math.Sin(_engine.CurrentAngle) * (WheelRadius - 4);
        batch.DrawLine(center.X, center.Y, nx, ny, UITheme.TextPrimary, 2f);
        // Needle tip pip
        batch.DrawCircle(new Vector2(nx, ny), 5f, 16, UITheme.AccentCyan, 2f);
        batch.FillRectangle(new Rectangle((int)nx - 2, (int)ny - 2, 4, 4), UITheme.AccentCyan);
    }

    private void DrawActionNode(SpriteBatch batch, Vector2 pos, string label, Color color, bool isActive)
    {
        Rectangle rect = new((int)(pos.X - 38), (int)(pos.Y - 14), 76, 28);
        Color bg = isActive ? color * 0.35f : UITheme.BgCardRecessed;
        Color border = isActive ? color : UITheme.BorderSubtle;

        CleanUI.DrawPanel(batch, rect, bg, border, borderWidth: isActive ? 2 : 1, shadow: false);

        Vector2 sz = _ctx.Font.MeasureString(label);
        Color textCol = isActive ? UITheme.TextPrimary : UITheme.TextMuted;
        batch.DrawString(_ctx.Font, label, new Vector2(rect.Center.X - sz.X / 2f, rect.Center.Y - sz.Y / 2f), textCol);
    }

    private void DrawStatProgressBar(SpriteBatch batch)
    {
        Rectangle barBox = new(180, 590, 920, 48);
        CleanUI.DrawPanel(batch, barBox, UITheme.BgPanel, UITheme.BorderSubtle, borderWidth: 1, shadow: true);

        var pet = _ctx.Run.ActivePet;
        string statLabel = _action switch
        {
            CareActionType.Train => $"EXP: LV. {pet.Level}",
            CareActionType.Feed => $"STOMACH: {pet.Stomach}%",
            CareActionType.Clean => $"CLEAN: {pet.Clean}%",
            CareActionType.Heal => $"HEALTH: {pet.Health} / 100",
            _ => "STAT"
        };
        batch.DrawString(_ctx.Font, statLabel, new Vector2(barBox.X + 20, barBox.Y + 14), UITheme.AccentGold);

        Rectangle fillBg = new(barBox.X + 220, barBox.Y + 12, 510, 24);
        float ratio = Math.Clamp(_engine.DayProgressPercent / 100f, 0f, 1f);

        Color fillColor = _action switch
        {
            CareActionType.Train => UITheme.AccentGold,
            CareActionType.Feed => new Color(230, 130, 60),
            CareActionType.Clean => UITheme.AccentCyan,
            CareActionType.Heal => UITheme.AccentEmerald,
            _ => UITheme.AccentCyan
        };

        CleanUI.DrawProgressBar(batch, _ctx.Font, fillBg, ratio, fillColor, leftText: null, rightText: $"{(int)_engine.DayProgressPercent}%");

        // AP Indicator badge
        Rectangle apPill = new(barBox.Right - 150, barBox.Y + 12, 130, 24);
        CleanUI.DrawBadge(batch, _ctx.Font, apPill, $"AP: {_ctx.Run.Energy.CurrentEnergy} / 6", new Color(28, 36, 48), UITheme.AccentCyan);
    }

    private void DrawArcZone(SpriteBatch batch, Vector2 center, float radius, float targetAngle, float halfWindow, Color color, float thickness)
    {
        int segments = 24;
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
