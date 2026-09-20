using BePalV2.Audio;
using BePalV2.Gameplay;
using BePalV2.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePalV2.Screens;

public sealed class CombatArenaScreen : IScreen
{
    private readonly ScreenContext _ctx;
    private readonly CombatEngine _combat;
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;

    private string? _combatFeedback;
    private Color _feedbackColor;
    private float _feedbackTimer;

    private bool _showVictoryBanner;
    private bool _showDefeatBanner;
    private readonly Rectangle _victoryBtn = new(480, 480, 320, 50);

    private const float WheelCenterX = 640f;
    private const float WheelCenterY = 360f;
    private const float WheelRadius = 150f;

    public CombatArenaScreen(ScreenContext ctx, CombatMode mode)
    {
        _ctx = ctx;
        if (_ctx.Run == null)
        {
            _ctx.Run = new V2RunState();
        }

        var equipped = _ctx.Run.Inventory.EquippedItem;
        _combat = new CombatEngine(mode, _ctx.Run.ActivePet, _ctx.Run.HasToothlessAlly, equipped);
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_feedbackTimer > 0f)
        {
            _feedbackTimer -= dt;
            if (_feedbackTimer <= 0f)
            {
                _combatFeedback = null;
            }
        }

        var kbd = Keyboard.GetState();
        var mouse = Mouse.GetState();
        Point mPos = mouse.Position;
        bool click = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;

        if (_showVictoryBanner)
        {
            if ((kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space)) ||
                (kbd.IsKeyDown(Keys.Enter) && !_prevKeyboard.IsKeyDown(Keys.Enter)) ||
                (click && _victoryBtn.Contains(mPos)))
            {
                AdvanceAfterVictory();
            }
            _prevKeyboard = kbd;
            _prevMouse = mouse;
            return;
        }

        if (_showDefeatBanner)
        {
            if ((kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space)) ||
                (kbd.IsKeyDown(Keys.Enter) && !_prevKeyboard.IsKeyDown(Keys.Enter)) ||
                (click && _victoryBtn.Contains(mPos)))
            {
                AdvanceAfterDefeat();
            }
            _prevKeyboard = kbd;
            _prevMouse = mouse;
            return;
        }

        _combat.Update(dt);

        ProcessCombatInput(kbd, mouse);

        // Check Victory / Defeat conditions
        if (_combat.IsCombatWon && !_showVictoryBanner)
        {
            TriggerVictory();
        }
        else if (_combat.IsPlayerDefeated && !_showDefeatBanner)
        {
            TriggerDefeat();
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void ProcessCombatInput(KeyboardState kbd, MouseState mouse)
    {
        bool spaceHit = kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space);
        bool mouseHit = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;

        if (spaceHit || mouseHit)
        {
            if (_combat.IsCounterWindowOpen)
            {
                // Counter attack
                bool countered = _combat.AttemptCounter();
                if (countered)
                {
                    _combatFeedback = "CRITICAL COUNTER HIT!";
                    _feedbackColor = UITheme.AccentEmerald;
                    _feedbackTimer = 0.6f;
                    _ctx.Audio.PlaySuccess();
                }
                else
                {
                    _combatFeedback = "COUNTER MISSED!";
                    _feedbackColor = UITheme.AccentCoral;
                    _feedbackTimer = 0.5f;
                    _ctx.Audio.PlayFail();
                }
            }
            else
            {
                // Dodge
                bool dodged = _combat.AttemptDodge();
                if (dodged)
                {
                    _combatFeedback = "EVASION SUCCESSFUL!";
                    _feedbackColor = UITheme.AccentGold;
                    _feedbackTimer = 0.5f;
                    _ctx.Audio.PlayConfirm();
                }
                else
                {
                    _combatFeedback = "DODGE MISSED!";
                    _feedbackColor = UITheme.AccentCoral;
                    _feedbackTimer = 0.5f;
                    _ctx.Audio.PlayFail();
                }
            }
        }
    }

    private void TriggerVictory()
    {
        _showVictoryBanner = true;
        _ctx.Audio.PlaySuccess();

        if (_combat.Mode == CombatMode.ToothlessTaming)
        {
            _ctx.Run.ResolveDay2Encounter(chooseTame: true, tameSuccess: true);
        }
        else if (_combat.Mode == CombatMode.MerchantBoss)
        {
            _ctx.Run.CompleteMerchantBossFight(bossDefeated: true);
        }
    }

    private void AdvanceAfterVictory()
    {
        _ctx.Audio.PlayConfirm();
        if (_combat.Mode == CombatMode.MerchantBoss)
        {
            _ctx.ScreenManager.SetScreen(new EndingScreen(_ctx, StoryEnding.EndingB_Protector));
        }
        else
        {
            _ctx.ScreenManager.SetScreen(new DailySummaryScreen(_ctx));
        }
    }

    private void TriggerDefeat()
    {
        _showDefeatBanner = true;
        _ctx.Audio.PlayFail();
    }

    private void AdvanceAfterDefeat()
    {
        _ctx.Audio.PlayConfirm();

        if (_combat.Mode == CombatMode.ToothlessTaming)
        {
            _ctx.Run.ResolveDay2Encounter(chooseTame: true, tameSuccess: false);
            _ctx.ScreenManager.SetScreen(new DailySummaryScreen(_ctx));
        }
        else if (_combat.Mode == CombatMode.MerchantBoss)
        {
            _ctx.Run.CompleteMerchantBossFight(bossDefeated: false);
            _ctx.ScreenManager.SetScreen(new EndingScreen(_ctx, StoryEnding.EndingBad_Foreclosure));
        }
        else
        {
            _ctx.ScreenManager.SetScreen(new BaseHabitatScreen(_ctx));
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        // Dark atmosphere background
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), UITheme.BgDeep);

        // 1. Top HUD Bar
        DrawTopHUD(batch);

        // 2. Radial Combat Wheel Layout
        DrawCombatWheel(batch);

        // 3. Counter Window Ring
        if (_combat.IsCounterWindowOpen)
        {
            float shrinkRatio = Math.Clamp(_combat.CounterWindowTimer / CombatEngine.CounterWindowDuration, 0f, 1f);
            float ringRadius = 40f + shrinkRatio * 80f;
            batch.DrawCircle(new Vector2(WheelCenterX, WheelCenterY), ringRadius, 32, UITheme.AccentEmerald, 2f);
            batch.DrawString(_ctx.Font, "!! ATTACK / COUNTER !!", new Vector2(WheelCenterX - 95, WheelCenterY - 18), UITheme.AccentEmerald, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
        }

        // 4. Feedback Text
        if (!string.IsNullOrEmpty(_combatFeedback))
        {
            Vector2 fbSize = _ctx.Font.MeasureString(_combatFeedback);
            Vector2 fbPos = new(WheelCenterX - (fbSize.X * 1.3f) / 2f, WheelCenterY - 195);
            batch.DrawString(_ctx.Font, _combatFeedback, fbPos + new Vector2(2, 2), Color.Black * 0.7f, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
            batch.DrawString(_ctx.Font, _combatFeedback, fbPos, _feedbackColor, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        }

        // 5. Bottom Instructions Bar
        DrawBottomBar(batch);

        // 6. Victory or Defeat Modals
        if (_showVictoryBanner) DrawVictoryBanner(batch);
        else if (_showDefeatBanner) DrawDefeatBanner(batch);
    }

    private void DrawTopHUD(SpriteBatch batch)
    {
        Rectangle topBar = new(0, 0, _ctx.ScreenWidth, 54);
        CleanUI.DrawPanel(batch, topBar, UITheme.BgPanel, UITheme.BorderSubtle, borderWidth: 1, shadow: true);

        // Gold Count Pill
        Rectangle goldPill = new(24, 12, 130, 30);
        CleanUI.DrawPanel(batch, goldPill, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);
        batch.FillRectangle(new Rectangle(goldPill.X + 8, goldPill.Y + 7, 16, 16), UITheme.AccentGold);
        batch.DrawString(_ctx.Font, $"{_ctx.Run.Economy.Gold} G", new Vector2(goldPill.X + 32, goldPill.Y + 5), UITheme.AccentGold);

        // Day Number Pill
        Rectangle dayPill = new(580, 12, 120, 30);
        CleanUI.DrawBadge(batch, _ctx.Font, dayPill, $"DAY {_ctx.Run.DayNumber}", new Color(34, 42, 58), UITheme.AccentGold);

        // Player LV. 1 Badge
        Rectangle lvlPill = new(_ctx.ScreenWidth - 160, 12, 120, 30);
        CleanUI.DrawBadge(batch, _ctx.Font, lvlPill, "PLAYER LV. 1", new Color(28, 44, 38), UITheme.AccentEmerald);
    }

    private void DrawCombatWheel(SpriteBatch batch)
    {
        Vector2 center = new(WheelCenterX, WheelCenterY);

        // Top-Right: DODGE Node
        Vector2 dodgePos = new(center.X + 130, center.Y - 130);
        Rectangle dodgeRect = new((int)dodgePos.X - 45, (int)dodgePos.Y - 15, 90, 30);
        CleanUI.DrawBadge(batch, _ctx.Font, dodgeRect, "DODGE", UITheme.AccentGold * 0.25f, UITheme.AccentGold);

        // Middle-Left: ACID Node
        Vector2 acidPos = new(center.X - 200, center.Y - 15);
        Rectangle acidRect = new((int)acidPos.X, (int)acidPos.Y, 80, 30);
        CleanUI.DrawBadge(batch, _ctx.Font, acidRect, "ACID", UITheme.AccentPurple * 0.25f, UITheme.AccentPurple);

        // Bottom-Right: ATTACK Node
        Vector2 attackPos = new(center.X + 130, center.Y + 120);
        Rectangle attackRect = new((int)attackPos.X - 45, (int)attackPos.Y - 15, 90, 30);
        CleanUI.DrawBadge(batch, _ctx.Font, attackRect, "ATTACK", UITheme.AccentEmerald * 0.25f, UITheme.AccentEmerald);

        // Bottom-Left: HEALTH Gauge
        Vector2 hpPos = new(center.X - 220, center.Y + 110);
        Rectangle hpBarBg = new((int)hpPos.X, (int)hpPos.Y + 14, 150, 20);
        CleanUI.DrawProgressBar(batch, _ctx.Font, hpBarBg, _combat.ActivePet.Health / 100f, UITheme.AccentCoral, leftText: "HP", rightText: $"{(int)_combat.ActivePet.Health}");

        // Boss / Opponent Gauge at Top-Center
        Rectangle oppRect = new((int)center.X - 160, (int)center.Y - 190, 320, 26);
        if (_combat.Mode == CombatMode.ToothlessTaming)
        {
            CleanUI.DrawProgressBar(batch, _ctx.Font, oppRect, _combat.TameGauge / 100f, UITheme.AccentEmerald, leftText: "TOOTHLESS TAME", rightText: $"{(int)_combat.TameGauge}%");
        }
        else
        {
            float hitRatio = _combat.BossHitsRemaining / 5f;
            CleanUI.DrawProgressBar(batch, _ctx.Font, oppRect, hitRatio, UITheme.AccentGold, leftText: "BOSS INTEGRITY", rightText: $"{_combat.BossHitsRemaining} HITS LEFT");
        }

        // Concentric tracks
        batch.DrawCircle(center, WheelRadius + 8, 64, UITheme.BorderSubtle, 1f);
        batch.DrawCircle(center, WheelRadius, 64, UITheme.BorderLight, 3f);
        batch.DrawCircle(center, WheelRadius - 16, 64, UITheme.BorderSubtle, 1f);

        // Dodge Zone arc
        int segments = 24;
        float start = _combat.DodgeZoneCenter - _combat.DodgeZoneHalfWidth;
        float step = (_combat.DodgeZoneHalfWidth * 2f) / segments;
        Color zoneColor = (_combat.Mode == CombatMode.MerchantBoss && _combat.BossPhase == 3)
            ? UITheme.AccentPurple
            : UITheme.AccentGold;

        for (int i = 0; i < segments; i++)
        {
            float a1 = start + i * step;
            float a2 = start + (i + 1) * step;
            Vector2 p1 = new(center.X + (float)Math.Cos(a1) * (WheelRadius - 6), center.Y + (float)Math.Sin(a1) * (WheelRadius - 6));
            Vector2 p2 = new(center.X + (float)Math.Cos(a2) * (WheelRadius - 6), center.Y + (float)Math.Sin(a2) * (WheelRadius - 6));
            batch.DrawLine(p1, p2, zoneColor, 14f);
        }

        // Center hub
        batch.DrawCircle(center, 12f, 24, UITheme.BgPanel, 14f);
        batch.DrawCircle(center, 18f, 24, UITheme.BorderHighlight, 2f);

        // Laser Needle
        float nx = center.X + (float)Math.Cos(_combat.NeedleAngle) * (WheelRadius - 4);
        float ny = center.Y + (float)Math.Sin(_combat.NeedleAngle) * (WheelRadius - 4);
        batch.DrawLine(center.X, center.Y, nx, ny, UITheme.TextPrimary, 2f);
        batch.DrawCircle(new Vector2(nx, ny), 5f, 16, zoneColor, 2f);
        batch.FillRectangle(new Rectangle((int)nx - 2, (int)ny - 2, 4, 4), zoneColor);
    }

    private void DrawBottomBar(SpriteBatch batch)
    {
        Rectangle btmBar = new(180, 590, 920, 48);
        CleanUI.DrawPanel(batch, btmBar, UITheme.BgPanel, UITheme.BorderSubtle, borderWidth: 1, shadow: true);

        string hint = _combat.IsCounterWindowOpen
            ? "COUNTER WINDOW OPEN: Press [ SPACEBAR ] or Click to strike!"
            : "EVADE ATTACK: Press [ SPACEBAR ] or Click inside the golden zone (shrinking window)!";

        Vector2 hSize = _ctx.Font.MeasureString(hint);
        Color hintCol = _combat.IsCounterWindowOpen ? UITheme.AccentEmerald : UITheme.AccentGold;
        batch.DrawString(_ctx.Font, hint, new Vector2(btmBar.Center.X - hSize.X / 2f, btmBar.Center.Y - hSize.Y / 2f), hintCol);
    }

    private void DrawVictoryBanner(SpriteBatch batch)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.8f);

        Rectangle modal = new(340, 150, 600, 420);
        CleanUI.DrawPanel(batch, modal, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(modal.X, modal.Y, modal.Width, 3), UITheme.AccentEmerald);

        Point mPos = Mouse.GetState().Position;

        if (_combat.Mode == CombatMode.ToothlessTaming)
        {
            string winTitle = "NEW COMPANION TAMED";
            Vector2 wSize = _ctx.Font.MeasureString(winTitle);
            batch.DrawString(_ctx.Font, winTitle, new Vector2(modal.Center.X - (wSize.X * 1.3f) / 2f, modal.Y + 36), UITheme.AccentEmerald, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

            batch.DrawLine(modal.X + 40, modal.Y + 74, modal.Right - 40, modal.Y + 74, UITheme.BorderSubtle, 1f);

            string note = "Toothless was successfully pacified and tamed!\nThe specimen has been admitted to your shelter companions.";
            batch.DrawString(_ctx.Font, note, new Vector2(modal.X + 50, modal.Y + 130), UITheme.TextPrimary);
        }
        else
        {
            string winTitle = "MERCHANT REPELLED (+9,999 G)";
            Vector2 wSize = _ctx.Font.MeasureString(winTitle);
            batch.DrawString(_ctx.Font, winTitle, new Vector2(modal.Center.X - (wSize.X * 1.3f) / 2f, modal.Y + 36), UITheme.AccentGold, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

            batch.DrawLine(modal.X + 40, modal.Y + 74, modal.Right - 40, modal.Y + 74, UITheme.BorderSubtle, 1f);

            string note = "You successfully defeated the predatory Merchant!\nHe fled into the outer ruins, leaving his treasury sack (+9999 G)!";
            batch.DrawString(_ctx.Font, note, new Vector2(modal.X + 50, modal.Y + 130), UITheme.TextPrimary);
        }

        CleanUI.DrawButton(batch, _ctx.Font, _victoryBtn, "CONTINUE OPERATION", _victoryBtn.Contains(mPos), accent: UITheme.AccentEmerald, hotkey: "[ SPACE ]", isPrimary: true);
    }

    private void DrawDefeatBanner(SpriteBatch batch)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.8f);

        Rectangle modal = new(340, 150, 600, 420);
        CleanUI.DrawPanel(batch, modal, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(modal.X, modal.Y, modal.Width, 3), UITheme.AccentCoral);

        string title = "COMBAT INCAPACITATION";
        Vector2 tSize = _ctx.Font.MeasureString(title);
        batch.DrawString(_ctx.Font, title, new Vector2(modal.Center.X - (tSize.X * 1.3f) / 2f, modal.Y + 36), UITheme.AccentCoral, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

        batch.DrawLine(modal.X + 40, modal.Y + 74, modal.Right - 40, modal.Y + 74, UITheme.BorderSubtle, 1f);

        string note = _combat.ActivePet.Health <= 0
            ? "Your active specimen fainted during the encounter!\nEmergency medical protocol or retreat required."
            : "Containment breach failed: specimen sustained severe trauma.";
        batch.DrawString(_ctx.Font, note, new Vector2(modal.X + 50, modal.Y + 130), UITheme.TextPrimary);

        Point mPos = Mouse.GetState().Position;
        CleanUI.DrawButton(batch, _ctx.Font, _victoryBtn, "CONTINUE OPERATION", _victoryBtn.Contains(mPos), accent: UITheme.AccentCoral, hotkey: "[ SPACE ]", isPrimary: true);
    }
}
