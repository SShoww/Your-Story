using BePalV2.Audio;
using BePalV2.Gameplay;
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

    private const float WheelCenterX = 640f;
    private const float WheelCenterY = 360f;
    private const float WheelRadius = 150f;

    public CombatArenaScreen(ScreenContext ctx, CombatMode mode)
    {
        _ctx = ctx;
        var run = ctx.Run;
        _combat = new CombatEngine(
            mode,
            run.ActivePet,
            hasToothlessAlly: run.HasToothlessAlly,
            equipped: run.Inventory.EquippedItem,
            consumableDodgeBonus: run.Inventory.ActiveDodgeBonus,
            initialShieldHits: run.Inventory.ActiveShieldHits
        );
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_feedbackTimer > 0f)
        {
            _feedbackTimer -= dt;
            if (_feedbackTimer <= 0f) _combatFeedback = null;
        }

        _combat.Update(dt);

        if (_combat.IsCombatWon)
        {
            HandleVictory();
            return;
        }
        if (_combat.IsPlayerDefeated)
        {
            HandleDefeat();
            return;
        }

        var kbd = Keyboard.GetState();
        var mouse = Mouse.GetState();
        bool space = kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space);
        bool click = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;

        if (space || click)
        {
            ProcessCombatInput();
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void ProcessCombatInput()
    {
        if (_combat.IsCounterWindowOpen)
        {
            bool countered = _combat.AttemptCounter();
            if (countered)
            {
                _combatFeedback = _combat.Mode == CombatMode.ToothlessTaming ? "ATTACK / TAME STRIKE! +25%" : "COUNTER ATTACK! -80 HP";
                _feedbackColor = new Color(93, 176, 70);
                _ctx.Audio.PlayCounterSuccess();
                _feedbackTimer = 0.5f;
            }
            return;
        }

        if (_combat.Mode == CombatMode.MerchantBoss && _combat.BossPhase == 3)
        {
            bool parried = _combat.AttemptParry();
            if (parried)
            {
                _combatFeedback = "PARRY ATTACK! -150 HP";
                _feedbackColor = new Color(200, 100, 255);
                _ctx.Audio.PlayParrySuccess();
                _feedbackTimer = 0.5f;
                return;
            }
        }

        bool dodged = _combat.AttemptDodge();
        if (dodged)
        {
            _combatFeedback = "DODGED! PRESS SPACE TO ATTACK!";
            _feedbackColor = new Color(255, 222, 89);
            _ctx.Audio.PlayDodgeSuccess();
        }
        else
        {
            _combatFeedback = "ACID HIT TAKEN!";
            _feedbackColor = new Color(231, 25, 31);
            _ctx.Audio.PlayFail();
        }
        _feedbackTimer = 0.5f;
    }

    private void HandleVictory()
    {
        _ctx.Audio.PlaySuccess();
        if (_combat.Mode == CombatMode.ToothlessTaming)
        {
            _ctx.Run.ResolveDay2Encounter(chooseTame: true, tameSuccess: true);
            _ctx.ScreenManager.SetScreen(new DailySummaryScreen(_ctx));
        }
        else
        {
            _ctx.Run.CompleteMerchantBossFight(bossDefeated: true);
            _ctx.ScreenManager.SetScreen(new EndingScreen(_ctx, StoryEnding.EndingB_Protector));
        }
    }

    private void HandleDefeat()
    {
        _ctx.Audio.PlayFail();
        _ctx.Run.ResolveEmergencyRevive();
        if (_combat.Mode == CombatMode.MerchantBoss)
        {
            _ctx.ScreenManager.SetScreen(new EndingScreen(_ctx, StoryEnding.EndingBad_Foreclosure));
        }
        else
        {
            _ctx.ScreenManager.SetScreen(new DailySummaryScreen(_ctx));
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), new Color(24, 22, 28));

        // Slide 38 concept: Top Row "Acid" | "Health" | "Toothless / Merchant Fight"
        batch.DrawString(_ctx.Font, "ACID", new Vector2(60, 30), new Color(180, 80, 240), 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, $"HEALTH: {(int)_combat.PlayerHp} / 100", new Vector2(400, 30), new Color(231, 25, 31), 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

        string opponentLabel = _combat.Mode == CombatMode.ToothlessTaming ? "NEW PET: TOOTHLESS" : $"MERCHANT FIGHT (PHASE {_combat.BossPhase} / 3)";
        batch.DrawString(_ctx.Font, opponentLabel, new Vector2(850, 30), Color.Gold, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

        // Health bars
        Rectangle pBarBg = new(400, 65, 300, 18);
        Rectangle pBarFill = new(400, 65, (int)(300 * Math.Clamp(_combat.PlayerHp / 100f, 0f, 1f)), 18);
        batch.FillRectangle(pBarBg, new Color(45, 45, 50));
        batch.FillRectangle(pBarFill, new Color(231, 25, 31));
        batch.DrawRectangle(pBarBg, Color.White, 1);

        Rectangle oBarBg = new(850, 65, 360, 18);
        float oRatio = _combat.Mode == CombatMode.ToothlessTaming ? (_combat.TameGauge / 100f) : (_combat.BossHp / 1000f);
        Color oColor = _combat.Mode == CombatMode.ToothlessTaming ? new Color(93, 176, 70) : new Color(231, 25, 31);
        Rectangle oBarFill = new(850, 65, (int)(360 * Math.Clamp(oRatio, 0f, 1f)), 18);
        batch.FillRectangle(oBarBg, new Color(45, 45, 50));
        batch.FillRectangle(oBarFill, oColor);
        batch.DrawRectangle(oBarBg, Color.White, 1);

        string oStatus = _combat.Mode == CombatMode.ToothlessTaming ? $"Tame Gauge: {(int)_combat.TameGauge}%" : $"Boss HP: {(int)_combat.BossHp}";
        batch.DrawString(_ctx.Font, oStatus, new Vector2(850, 90), Color.White);

        // Slide 38 Radial Wheel with "Dodge" and "Attack"
        DrawCombatWheel(batch);

        // Counter Window Ring
        if (_combat.IsCounterWindowOpen)
        {
            float shrinkRatio = Math.Clamp(_combat.CounterWindowTimer / CombatEngine.CounterWindowDuration, 0f, 1f);
            float ringRadius = 40f + shrinkRatio * 80f;
            batch.DrawCircle(new Vector2(WheelCenterX, WheelCenterY), ringRadius, 32, new Color(93, 176, 70), 3f);
            batch.DrawString(_ctx.Font, "!! ATTACK / COUNTER !!", new Vector2(WheelCenterX - 95, WheelCenterY - 18), new Color(93, 176, 70), 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
        }

        // Feedback Text
        if (!string.IsNullOrEmpty(_combatFeedback))
        {
            Vector2 fbSize = _ctx.Font.MeasureString(_combatFeedback);
            Vector2 fbPos = new(WheelCenterX - fbSize.X / 2f, WheelCenterY - 180);
            batch.DrawString(_ctx.Font, _combatFeedback, fbPos, _feedbackColor, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        }

        // Slide 38 Bottom Progress Bar
        Rectangle btmBar = new(180, 580, 920, 40);
        batch.FillRectangle(btmBar, new Color(30, 30, 35));
        batch.DrawRectangle(btmBar, new Color(70, 70, 80), 1);

        string hint = _combat.IsCounterWindowOpen
            ? "ATTACK OPPORTUNITY! Press [ SPACEBAR ] to strike!"
            : "DODGE INCOMING ATTACK! Press [ SPACEBAR ] in the golden zone!";
        Vector2 hSize = _ctx.Font.MeasureString(hint);
        batch.DrawString(_ctx.Font, hint, new Vector2(btmBar.Center.X - hSize.X / 2f, btmBar.Center.Y - hSize.Y / 2f), Color.White);
    }

    private void DrawCombatWheel(SpriteBatch batch)
    {
        Vector2 center = new(WheelCenterX, WheelCenterY);

        // Labels matching Slide 38: "Dodge" and "Attack"
        batch.DrawString(_ctx.Font, "DODGE", new Vector2(center.X - WheelRadius - 65, center.Y - 12), new Color(255, 222, 89));
        batch.DrawString(_ctx.Font, "ATTACK", new Vector2(center.X + WheelRadius + 15, center.Y - 12), new Color(93, 176, 70));

        batch.DrawCircle(center, WheelRadius, 64, new Color(60, 45, 65), 6f);
        batch.DrawCircle(center, WheelRadius - 10, 64, new Color(40, 30, 45), 4f);

        // Dodge Zone arc
        int segments = 16;
        float start = _combat.DodgeZoneCenter - _combat.DodgeZoneHalfWidth;
        float step = (_combat.DodgeZoneHalfWidth * 2f) / segments;
        Color zoneColor = (_combat.Mode == CombatMode.MerchantBoss && _combat.BossPhase == 3)
            ? new Color(180, 80, 240)
            : new Color(255, 222, 89);

        for (int i = 0; i < segments; i++)
        {
            float a1 = start + i * step;
            float a2 = start + (i + 1) * step;
            Vector2 p1 = new(center.X + (float)Math.Cos(a1) * (WheelRadius - 6), center.Y + (float)Math.Sin(a1) * (WheelRadius - 6));
            Vector2 p2 = new(center.X + (float)Math.Cos(a2) * (WheelRadius - 6), center.Y + (float)Math.Sin(a2) * (WheelRadius - 6));
            batch.DrawLine(p1, p2, zoneColor, 16f);
        }

        // Center hub
        batch.DrawCircle(center, 10f, 16, new Color(45, 30, 45), 20f);
        batch.DrawCircle(center, 20f, 20, Color.White, 2f);

        // Needle
        float nx = center.X + (float)Math.Cos(_combat.NeedleAngle) * (WheelRadius - 4);
        float ny = center.Y + (float)Math.Sin(_combat.NeedleAngle) * (WheelRadius - 4);
        batch.DrawLine(center.X, center.Y, nx, ny, Color.White, 3f);
        batch.DrawCircle(new Vector2(nx, ny), 5f, 12, zoneColor, 3f);
    }
}
