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
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;

    private string? _floatingFeedback;
    private Color _floatingColor;
    private float _feedbackTimer;

    // 1920x1080 Layout Constants
    private const float WheelCenterX = 960f;
    private const float WheelCenterY = 480f;
    private const float WheelRadius = 240f;

    // Death Spiral Shrinking Zone Parameters (Version 1 Style)
    private float _needleAngle;
    private float _needleSpeed = 2.4f;
    private float _elapsedTime;
    private const float CycleDuration = 4.0f;
    private const float InitialSpan = MathF.PI / 3.2f; // ~56 degrees

    // 4 Sectors (Death Spiral Radial Slots)
    private record struct CareSector(CareActionType Action, string Label, float CenterAngle, Color Color);
    private static readonly CareSector[] Sectors =
    {
        new(CareActionType.Train, "TRAIN", 1.25f * MathF.PI, UITheme.AccentGold),    // Top-Left (225°)
        new(CareActionType.Heal, "HEAL", 1.75f * MathF.PI, UITheme.AccentEmerald),   // Top-Right (315°)
        new(CareActionType.Feed, "FEED", 0.25f * MathF.PI, new Color(240, 130, 60)), // Bottom-Right (45°)
        new(CareActionType.Clean, "CLEAN", 0.75f * MathF.PI, UITheme.AccentCyan)     // Bottom-Left (135°)
    };

    private int _attemptsRemaining = 10;
    private int _successfulHits;
    private int _currentStreak;
    private int _maxStreak;
    private int _totalScore;

    public CareQteScreen(ScreenContext ctx, CareActionType defaultAction = CareActionType.Train)
    {
        _ctx = ctx;
        _engine = new CareQteEngine(ctx.Run.ActivePet, defaultAction, ctx.Run.Inventory.EquippedItem);
        var pet = ctx.Run.ActivePet;
        if (pet.IsGrimy) _needleSpeed *= 1.15f;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _elapsedTime += dt;
        _needleAngle = (_needleAngle + _needleSpeed * dt) % (2f * MathF.PI);

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

        if ((spaceHit || mouseHit) && _attemptsRemaining > 0)
        {
            ResolveAttempt();
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private float GetCurrentSpan()
    {
        // Death Spiral: zone shrinks over each cycle duration
        float cycleProgress = (_elapsedTime % CycleDuration) / CycleDuration;
        return MathF.Max(0.12f, InitialSpan * (1f - cycleProgress * 0.75f));
    }

    private void ResolveAttempt()
    {
        _attemptsRemaining--;
        float currentSpan = GetCurrentSpan();
        CareSector? hitSector = null;

        foreach (var sector in Sectors)
        {
            float diff = MathF.Abs(MathHelper.WrapAngle(_needleAngle - sector.CenterAngle));
            if (diff <= currentSpan / 2f)
            {
                hitSector = sector;
                break;
            }
        }

        if (hitSector.HasValue)
        {
            var sector = hitSector.Value;
            _successfulHits++;
            _currentStreak++;
            _maxStreak = Math.Max(_maxStreak, _currentStreak);
            _totalScore += 100 + (_currentStreak * 25);

            // Apply Care Action
            var pet = _ctx.Run.ActivePet;
            _engine.RecordAttempt(PrecisionTier.Perfect);
            switch (sector.Action)
            {
                case CareActionType.Train:
                    pet.AddExp(25, _ctx.Run.Inventory.PermanentTrainExpMultiplier);
                    break;
                case CareActionType.Feed:
                    pet.FeedDirect(20);
                    break;
                case CareActionType.Clean:
                    pet.CleanDirect(20);
                    break;
                case CareActionType.Heal:
                    pet.Heal(20f);
                    break;
            }

            // Award Player Points for successful action
            _ctx.Run.Economy.AddPlayerPoints(25);

            _ctx.Audio.PlaySuccess();
            _floatingFeedback = $"{sector.Label} HIT! +100 PTS";
            _floatingColor = sector.Color;
            _feedbackTimer = 0.55f;
        }
        else
        {
            _engine.RecordAttempt(PrecisionTier.Miss);
            _currentStreak = 0;
            _ctx.Audio.PlayFail();
            _floatingFeedback = "MISS!";
            _floatingColor = UITheme.AccentCoral;
            _feedbackTimer = 0.55f;
        }

        if (_attemptsRemaining <= 0)
        {
            FinishSession();
        }
    }

    private void FinishSession()
    {
        var run = _ctx.Run;
        run.Energy.Spend(1);
        run.RecordCareSessionOutcome(_engine);

        _ctx.Audio.PlayConfirm();
        RouteDefensePhase();
    }

    private void RouteDefensePhase()
    {
        var run = _ctx.Run;
        if (run.DayNumber == 1 && run.CurrentPhase == DailyPhase.CareAction && run.Energy.CurrentEnergy == 0)
        {
            run.SetPhase(DailyPhase.DefenseResolution);
            _ctx.ScreenManager.SetScreen(new CalmingQteScreen(_ctx));
        }
        else
        {
            _ctx.ScreenManager.SetScreen(new BaseHabitatScreen(_ctx));
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        // Deep background
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), UITheme.BgDeep);

        // Header Title
        batch.DrawString(_ctx.Font, "RADIAL CARE PROTOCOL - DEATH SPIRAL", new Vector2(80, 48), UITheme.TextPrimary, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0f);

        // Attempts Remaining Badge
        Rectangle attBadge = new(80, 100, 220, 36);
        CleanUI.DrawBadge(batch, _ctx.Font, attBadge, $"ATTEMPTS: {_attemptsRemaining} / 10", new Color(34, 42, 58), UITheme.AccentGold);

        // Subtitle Guide
        batch.DrawString(_ctx.Font, "Press [ SPACEBAR ] when the needle rotates into any Care Sector", new Vector2(80, 150), UITheme.TextSecondary, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);

        // Right Score & Player Points Panel
        int rx = 1500;
        Rectangle scorePanel = new(rx, 160, 340, 560);
        CleanUI.DrawPanel(batch, scorePanel, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(scorePanel.X, scorePanel.Y, scorePanel.Width, 4), UITheme.AccentGold);

        batch.DrawString(_ctx.Font, "RESEARCH LOG", new Vector2(rx + 24, 185), UITheme.AccentGold, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
        batch.DrawLine(rx + 24, 225, rx + 316, 225, UITheme.BorderSubtle, 1f);

        batch.DrawString(_ctx.Font, $"Care Score:   {_totalScore}", new Vector2(rx + 24, 250), UITheme.TextPrimary);
        batch.DrawString(_ctx.Font, $"Hits / Total: {_successfulHits} / {10 - _attemptsRemaining}", new Vector2(rx + 24, 295), UITheme.AccentEmerald);
        batch.DrawString(_ctx.Font, $"Streak:       {_currentStreak}", new Vector2(rx + 24, 340), UITheme.TextSecondary);
        batch.DrawString(_ctx.Font, $"Max Streak:   {_maxStreak}", new Vector2(rx + 24, 385), UITheme.AccentGold);
        batch.DrawString(_ctx.Font, $"Points Earned:+{_successfulHits * 25} PTS", new Vector2(rx + 24, 430), UITheme.AccentCyan);

        Rectangle keycapBox = new(rx + 40, 540, 260, 56);
        CleanUI.DrawKeycap(batch, _ctx.Font, keycapBox, "SPACEBAR", isPressed: false, isAccent: true);

        Vector2 subPrompt = _ctx.Font.MeasureString("Hit space inside sector");
        float subScale = 0.85f;
        Vector2 subPos = new(scorePanel.Center.X - (subPrompt.X * subScale) / 2f, 616);
        batch.DrawString(_ctx.Font, "Hit space inside sector", subPos, UITheme.TextMuted, 0f, Vector2.Zero, subScale, SpriteEffects.None, 0f);

        // Draw Radial Wheel with Central Pet
        DrawRadialWheel(batch);

        // Bottom Progress Bars: EXP Bar and AP / Points HUD
        DrawBottomProgressBars(batch);

        // Floating feedback
        if (!string.IsNullOrEmpty(_floatingFeedback))
        {
            Vector2 fbSize = _ctx.Font.MeasureString(_floatingFeedback);
            Vector2 fbPos = new(WheelCenterX - (fbSize.X * 1.4f) / 2f, WheelCenterY - 40);
            batch.DrawString(_ctx.Font, _floatingFeedback, fbPos + new Vector2(2, 2), Color.Black * 0.7f, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0f);
            batch.DrawString(_ctx.Font, _floatingFeedback, fbPos, _floatingColor, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0f);
        }
    }

    private void DrawRadialWheel(SpriteBatch batch)
    {
        Vector2 center = new(WheelCenterX, WheelCenterY);

        // 1. Central Creature Display (Version 1 Style)
        Rectangle centralFrame = new((int)center.X - 90, (int)center.Y - 110, 180, 220);
        CleanUI.DrawPanel(batch, centralFrame, new Color(14, 18, 26), UITheme.BorderSubtle, borderWidth: 1, shadow: true);

        if (_ctx.PetIdleTex != null)
        {
            int sprH = 170;
            int sprW = (int)(sprH * (1298f / 1731f)); // 127px (3:4 ratio)
            Rectangle petRect = new((int)center.X - sprW / 2, (int)center.Y - sprH / 2, sprW, sprH);
            batch.Draw(_ctx.PetIdleTex, petRect, Color.White);
        }

        // Concentric track circles
        batch.DrawCircle(center, WheelRadius + 12, 64, UITheme.BorderSubtle, 1f);
        batch.DrawCircle(center, WheelRadius, 64, UITheme.BorderLight, 3f);
        batch.DrawCircle(center, WheelRadius - 20, 64, UITheme.BorderSubtle, 1f);

        // 2. The 4 Shrinking Care Sectors (Death Spiral)
        float currentSpan = GetCurrentSpan();
        float halfSpan = currentSpan / 2f;

        foreach (var sector in Sectors)
        {
            // Arc Zone for the sector
            DrawArcZone(batch, center, WheelRadius - 10, sector.CenterAngle, halfSpan, sector.Color, 18f);

            // Action Node Badge on the perimeter
            Vector2 nodePos = new(center.X + MathF.Cos(sector.CenterAngle) * (WheelRadius + 44),
                                  center.Y + MathF.Sin(sector.CenterAngle) * (WheelRadius + 44));
            Rectangle nodeRect = new((int)nodePos.X - 52, (int)nodePos.Y - 20, 104, 40);
            CleanUI.DrawBadge(batch, _ctx.Font, nodeRect, sector.Label, sector.Color * 0.25f, sector.Color);
        }

        // Center hub ring
        batch.DrawCircle(center, 24f, 32, UITheme.BorderHighlight, 2f);

        // 3. Rotating Needle
        float nx = center.X + MathF.Cos(_needleAngle) * (WheelRadius - 4);
        float ny = center.Y + MathF.Sin(_needleAngle) * (WheelRadius - 4);
        batch.DrawLine(center.X, center.Y, nx, ny, UITheme.TextPrimary, 3f);

        // Needle tip pip
        batch.DrawCircle(new Vector2(nx, ny), 7f, 16, UITheme.AccentCyan, 2f);
        batch.FillRectangle(new Rectangle((int)nx - 3, (int)ny - 3, 6, 6), UITheme.AccentCyan);
    }

    private void DrawBottomProgressBars(SpriteBatch batch)
    {
        // Container bar box at bottom
        Rectangle barBox = new(360, 890, 1200, 64);
        CleanUI.DrawPanel(batch, barBox, UITheme.BgPanel, UITheme.BorderSubtle, borderWidth: 1, shadow: true);

        var pet = _ctx.Run.ActivePet;

        // "EXP: LV. 1" Left Badge (Properly proportioned and non-clipping)
        Rectangle lvlBadge = new(barBox.X + 20, barBox.Y + 12, 160, 40);
        CleanUI.DrawBadge(batch, _ctx.Font, lvlBadge, $"EXP: LV. {pet.Level}", new Color(34, 46, 38), UITheme.AccentEmerald);

        // EXP Progress Bar
        Rectangle fillBg = new(barBox.X + 200, barBox.Y + 16, 680, 32);
        float ratio = Math.Clamp((float)pet.CurrentExp / Math.Max(1, pet.MaxExp), 0f, 1f);
        CleanUI.DrawProgressBar(batch, _ctx.Font, fillBg, ratio, UITheme.AccentEmerald, leftText: null, rightText: $"{pet.CurrentExp} / {pet.MaxExp} EXP");

        // AP Indicator Badge
        Rectangle apPill = new(barBox.Right - 300, barBox.Y + 14, 135, 36);
        CleanUI.DrawBadge(batch, _ctx.Font, apPill, $"AP: {_ctx.Run.Energy.CurrentEnergy}/{_ctx.Run.Energy.MaxEnergy}", new Color(28, 36, 48), UITheme.AccentCyan);

        // Player Points Badge
        Rectangle ptsPill = new(barBox.Right - 150, barBox.Y + 14, 130, 36);
        CleanUI.DrawBadge(batch, _ctx.Font, ptsPill, $"{_ctx.Run.Economy.PlayerPoints} PTS", new Color(42, 38, 24), UITheme.AccentGold);
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
            Vector2 p1 = new(center.X + MathF.Cos(a1) * radius, center.Y + MathF.Sin(a1) * radius);
            Vector2 p2 = new(center.X + MathF.Cos(a2) * radius, center.Y + MathF.Sin(a2) * radius);
            batch.DrawLine(p1, p2, color, thickness);
        }
    }
}
