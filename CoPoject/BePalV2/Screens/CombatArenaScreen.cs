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
    private readonly Rectangle _victoryBtn = new(720, 720, 480, 56);

    // 1920x1080 Layout Constants
    private const float WheelCenterX = 960f;
    private const float WheelCenterY = 520f;
    private const float WheelRadius = 220f;

    // Pet Skill Zones (dynamically determined by active combatant)
    private readonly string _skillName;
    private readonly Color _skillColor;
    private const float SkillCenterAngle = 1.25f * MathF.PI; // Top-Left (225°)
    private const float SkillHalfWindow = 0.30f;

    public CombatArenaScreen(ScreenContext ctx, CombatMode mode)
    {
        _ctx = ctx;
        if (_ctx.Run == null)
        {
            _ctx.Run = new V2RunState();
        }

        var equipped = _ctx.Run.Inventory.EquippedItem;
        _combat = new CombatEngine(mode, _ctx.Run.ActivePet, _ctx.Run.HasToothlessAlly, equipped);

        // Dynamically configure skill zone based on player's chosen pet
        (_skillName, _skillColor) = GetPetSkillInfo(_combat.ActivePet.Species);
    }

    private static (string name, Color color) GetPetSkillInfo(PetSpecies species) => species switch
    {
        PetSpecies.Coco => ("PHOTOSYNTHESIS", UITheme.AccentEmerald),
        PetSpecies.Gloomtail => ("SHADOW BARRIER", UITheme.AccentPurple),
        PetSpecies.Sproutlet => ("AGILE REFLEX", UITheme.AccentCoral),
        _ => ("ACID SPRAY", UITheme.AccentCyan)
    };

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

        // Banner modals take priority
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

        // Active Combat Loop
        _combat.Update(dt);

        ProcessCombatInput(kbd, mouse);

        // Check victory / defeat triggers
        if (_combat.IsCombatWon)
        {
            TriggerVictory();
        }
        else if (_combat.IsPlayerDefeated)
        {
            TriggerDefeat();
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void ProcessCombatInput(KeyboardState kbd, MouseState mouse)
    {
        bool spacePressed = kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space);
        bool mouseClicked = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;

        if (spacePressed || mouseClicked)
        {
            // 1. Counter / Attack Window Strike
            if (_combat.IsCounterWindowOpen)
            {
                bool counterSuccess = _combat.AttemptCounter();
                if (counterSuccess)
                {
                    _ctx.Audio.PlaySuccess();
                    _combatFeedback = "CRITICAL COUNTER STRIKE!";
                    _feedbackColor = UITheme.AccentEmerald;
                    _feedbackTimer = 0.6f;
                    _ctx.Run.Economy.AddPlayerPoints(20);
                    return;
                }
            }

            // 2. Dodge Zone Evaluation (Primary defense action)
            if (_combat.IsNeedleInDodgeZone())
            {
                bool dodgeSuccess = _combat.AttemptDodge();
                if (dodgeSuccess)
                {
                    _ctx.Audio.PlayConfirm();
                    _combatFeedback = "CLEAN EVASION!";
                    _feedbackColor = UITheme.AccentGold;
                    _ctx.Run.Economy.AddPlayerPoints(15);
                }
                _feedbackTimer = 0.6f;
                return;
            }

            // 3. Pet Skill Zone (Triggered when striking the pet's unique skill sector)
            float skillDiff = MathF.Abs(MathHelper.WrapAngle(_combat.NeedleAngle - SkillCenterAngle));
            if (skillDiff <= SkillHalfWindow)
            {
                _ctx.Audio.PlaySuccess();
                _combatFeedback = $"{_skillName} TRIGGERED!";
                _feedbackColor = _skillColor;
                _feedbackTimer = 0.6f;
                _combat.ActivePet.Heal(15f);
                if (_combat.Mode == CombatMode.ToothlessTaming)
                {
                    _combat.AttemptDodge();
                }
                _ctx.Run.Economy.AddPlayerPoints(30);
                return;
            }

            // 4. Missed outside both zones -> Sustains damage
            _combat.AttemptDodge();
            _ctx.Audio.PlayFail();
            _combatFeedback = "DODGE MISSED! DAMAGE SUSTAINED";
            _feedbackColor = UITheme.AccentCoral;
            _feedbackTimer = 0.6f;
        }
    }

    private void TriggerVictory()
    {
        _showVictoryBanner = true;
        _ctx.Audio.PlaySessionComplete();

        if (_combat.Mode == CombatMode.ToothlessTaming)
        {
            _ctx.Run.ResolveDay2Encounter(chooseTame: true, tameSuccess: true);
            _ctx.Run.Economy.AddPlayerPoints(100);
        }
        else
        {
            _ctx.Run.Economy.AddGold(9999);
            _ctx.Run.Economy.AddPlayerPoints(200);
            _ctx.Run.Day3BossDefeated = true;
        }
    }

    private void AdvanceAfterVictory()
    {
        if (_combat.Mode == CombatMode.ToothlessTaming)
        {
            _ctx.ScreenManager.SetScreen(new BaseHabitatScreen(_ctx));
        }
        else
        {
            _ctx.ScreenManager.SetScreen(new EndingScreen(_ctx, StoryEnding.EndingB_Protector));
        }
    }

    private void TriggerDefeat()
    {
        _showDefeatBanner = true;
        _ctx.Audio.PlayFail();
    }

    private void AdvanceAfterDefeat()
    {
        if (_combat.Mode == CombatMode.ChapterBoss)
        {
            _ctx.Run.SetEnding(StoryEnding.Ending_VerticalSliceForcedDefeat);
            _ctx.ScreenManager.SetScreen(new EndingScreen(_ctx, StoryEnding.Ending_VerticalSliceForcedDefeat));
        }
        else if (_combat.Mode == CombatMode.MerchantBoss)
        {
            _ctx.ScreenManager.SetScreen(new EndingScreen(_ctx, StoryEnding.EndingA_Betrayal));
        }
        else
        {
            _ctx.Run.ResolveDay2Encounter(chooseTame: false, tameSuccess: false);
            _ctx.ScreenManager.SetScreen(new BaseHabitatScreen(_ctx));
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), UITheme.BgDeep);

        // 1. Top HUD Bar
        DrawTopHUD(batch);

        // 2. Combatants Display (Player Pet on Left, Opponent on Right)
        DrawCombatants(batch);

        // 3. Radial Combat Wheel Layout
        DrawCombatWheel(batch);

        // 4. Counter Window Ring Prompt
        if (_combat.IsCounterWindowOpen)
        {
            float shrinkRatio = Math.Clamp(_combat.CounterWindowTimer / 1.0f, 0f, 1f);
            float ringRadius = 50f + shrinkRatio * 110f;
            batch.DrawCircle(new Vector2(WheelCenterX, WheelCenterY), ringRadius, 32, UITheme.AccentEmerald, 3f);
            batch.DrawString(_ctx.Font, "!! ATTACK / COUNTER !!", new Vector2(WheelCenterX - 110, WheelCenterY - 24), UITheme.AccentEmerald, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        }

        // 5. Floating Feedback Text
        if (!string.IsNullOrEmpty(_combatFeedback))
        {
            Vector2 fbSize = _ctx.Font.MeasureString(_combatFeedback);
            Vector2 fbPos = new(WheelCenterX - (fbSize.X * 1.4f) / 2f, WheelCenterY - 260);
            batch.DrawString(_ctx.Font, _combatFeedback, fbPos + new Vector2(2, 2), Color.Black * 0.7f, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0f);
            batch.DrawString(_ctx.Font, _combatFeedback, fbPos, _feedbackColor, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0f);
        }

        // 6. Bottom Instructions Bar
        DrawBottomBar(batch);

        // 7. Victory or Defeat Modals
        if (_showVictoryBanner) DrawVictoryBanner(batch);
        else if (_showDefeatBanner) DrawDefeatBanner(batch);
    }

    private void DrawTopHUD(SpriteBatch batch)
    {
        Rectangle topBar = new(0, 0, _ctx.ScreenWidth, 70);
        CleanUI.DrawPanel(batch, topBar, UITheme.BgPanel, UITheme.BorderSubtle, borderWidth: 1, shadow: true);

        // Gold Pill
        Rectangle goldPill = new(40, 15, 170, 40);
        CleanUI.DrawPanel(batch, goldPill, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);
        batch.FillRectangle(new Rectangle(goldPill.X + 10, goldPill.Y + 10, 20, 20), UITheme.AccentGold);
        batch.DrawString(_ctx.Font, $"{_ctx.Run.Economy.Gold} G", new Vector2(goldPill.X + 40, goldPill.Y + 8), UITheme.AccentGold);

        // Research Points Pill
        Rectangle ptsPill = new(230, 15, 190, 40);
        CleanUI.DrawPanel(batch, ptsPill, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);
        batch.FillRectangle(new Rectangle(ptsPill.X + 10, ptsPill.Y + 10, 20, 20), UITheme.AccentCyan);
        batch.DrawString(_ctx.Font, $"{_ctx.Run.Economy.PlayerPoints} PTS", new Vector2(ptsPill.X + 40, ptsPill.Y + 8), UITheme.AccentCyan);

        // Day Number Pill (matches canonical encounter timeline)
        int combatDay = _combat.Mode switch
        {
            CombatMode.ToothlessTaming => 2,
            CombatMode.MerchantBoss => 3,
            CombatMode.ChapterBoss => 3,
            _ => _ctx.Run.DayNumber
        };
        Rectangle dayPill = new(900, 15, 160, 40);
        CleanUI.DrawBadge(batch, _ctx.Font, dayPill, $"DAY {combatDay}", new Color(34, 42, 58), UITheme.AccentGold);
        // Player Level Pill (Shifted safely away from window controls)
        Rectangle lvlPill = new(_ctx.ScreenWidth - 320, 15, 180, 40);
        CleanUI.DrawBadge(batch, _ctx.Font, lvlPill, "PLAYER LV. 1", new Color(28, 44, 38), UITheme.AccentEmerald);
    }

    private void DrawCombatants(SpriteBatch batch)
    {
        var playerPet = _combat.ActivePet;

        // === LEFT SIDE: Player's Pet ===
        Rectangle leftCard = new(80, 180, 380, 680);
        CleanUI.DrawPanel(batch, leftCard, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(leftCard.X, leftCard.Y, leftCard.Width, 4), UITheme.AccentEmerald);

        batch.DrawString(_ctx.Font, "OUR COMBATANT", new Vector2(leftCard.X + 24, leftCard.Y + 18), UITheme.AccentEmerald, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);

        // Player Pet Portrait Box
        Rectangle leftBox = new(leftCard.Center.X - 90, leftCard.Y + 60, 180, 220);
        CleanUI.DrawPanel(batch, leftBox, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);

        if (_ctx.PetIdleTex != null)
        {
            int sprH = 190;
            int sprW = (int)(sprH * (1298f / 1731f)); // 142px
            Rectangle sprRect = new(leftBox.Center.X - sprW / 2, leftBox.Center.Y - sprH / 2, sprW, sprH);
            batch.Draw(_ctx.PetIdleTex, sprRect, Color.White);
        }

        // Pet Name & Level
        batch.DrawString(_ctx.Font, playerPet.Name, new Vector2(leftCard.X + 24, leftCard.Y + 300), UITheme.TextPrimary, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, $"LV. {playerPet.Level} Companion", new Vector2(leftCard.X + 24, leftCard.Y + 340), UITheme.TextSecondary);

        // Player Pet HEALTH: Text is placed ABOVE the bar so it is NEVER covered!
        string hpLabel = $"HEALTH: {(int)playerPet.Health} / {(int)playerPet.MaxHealth} HP";
        batch.DrawString(_ctx.Font, hpLabel, new Vector2(leftCard.X + 24, leftCard.Y + 380), UITheme.AccentCoral, 0f, Vector2.Zero, 1.05f, SpriteEffects.None, 0f);

        Rectangle hpBarBg = new(leftCard.X + 24, leftCard.Y + 415, leftCard.Width - 48, 26);
        CleanUI.DrawProgressBar(batch, _ctx.Font, hpBarBg, playerPet.Health / playerPet.MaxHealth, UITheme.AccentCoral, leftText: null, rightText: null);

        // Skill info badge
        Rectangle skillPill = new(leftCard.X + 24, leftCard.Y + 465, leftCard.Width - 48, 40);
        CleanUI.DrawBadge(batch, _ctx.Font, skillPill, $"SKILL: {_skillName}", _skillColor * 0.25f, _skillColor);

        // Cleanliness readiness check note
        batch.DrawString(_ctx.Font, $"Cleanliness: {playerPet.Clean}% (Combat Ready)", new Vector2(leftCard.X + 24, leftCard.Y + 520), UITheme.TextMuted, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0f);


        // === RIGHT SIDE: Opponent Specimen ===
        Rectangle rightCard = new(1460, 180, 380, 680);
        Color oppAccent = _combat.Mode == CombatMode.ToothlessTaming ? UITheme.AccentCyan : UITheme.AccentCoral;
        CleanUI.DrawPanel(batch, rightCard, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(rightCard.X, rightCard.Y, rightCard.Width, 4), oppAccent);

        string oppCategory = _combat.Mode == CombatMode.ToothlessTaming ? "WILD ENCOUNTER" : "HOSTILE OPPONENT";
        batch.DrawString(_ctx.Font, oppCategory, new Vector2(rightCard.X + 24, rightCard.Y + 18), oppAccent, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);

        // Opponent Portrait Box
        Rectangle rightBox = new(rightCard.Center.X - 90, rightCard.Y + 60, 180, 220);
        CleanUI.DrawPanel(batch, rightBox, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);

        string oppIcon = _combat.Mode == CombatMode.ToothlessTaming ? "[ TOOTHLESS ]" : (_combat.Mode == CombatMode.MerchantBoss ? "[ MERCHANT ]" : "[ CHAPTER BOSS ]");
        Vector2 oSz = _ctx.Font.MeasureString(oppIcon);
        batch.DrawString(_ctx.Font, oppIcon, new Vector2(rightBox.Center.X - oSz.X / 2f, rightBox.Center.Y - oSz.Y / 2f), oppAccent);

        // Opponent Name
        string oppName = _combat.Mode == CombatMode.ToothlessTaming
            ? "Toothless"
            : (_combat.Mode == CombatMode.MerchantBoss ? "Traveling Merchant" : "Void Behemoth");
        batch.DrawString(_ctx.Font, oppName, new Vector2(rightCard.X + 24, rightCard.Y + 300), UITheme.TextPrimary, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

        string oppSub = _combat.Mode == CombatMode.ToothlessTaming
            ? "Acid-Dripping Stray (Tameable)"
            : (_combat.Mode == CombatMode.MerchantBoss ? "Illegal Specimen Collector" : "Chapter 1 Climax Threat");
        batch.DrawString(_ctx.Font, oppSub, new Vector2(rightCard.X + 24, rightCard.Y + 340), UITheme.TextSecondary);

        // Opponent Gauge: scaled nicely so it never runs off the edge
        string oppGaugeLabel = _combat.Mode == CombatMode.ToothlessTaming
            ? $"TAME PROGRESS: {(int)_combat.TameGauge}% / 100%"
            : (_combat.Mode == CombatMode.ChapterBoss ? "CHAPTER BOSS: IMMENSE THREAT" : $"BOSS INTEGRITY: {_combat.BossHitsRemaining} HITS REMAINING");
        batch.DrawString(_ctx.Font, oppGaugeLabel, new Vector2(rightCard.X + 24, rightCard.Y + 382), oppAccent, 0f, Vector2.Zero, 0.88f, SpriteEffects.None, 0f);
        Rectangle oppBarBg = new(rightCard.X + 24, rightCard.Y + 415, rightCard.Width - 48, 26);
        float oppRatio = _combat.Mode == CombatMode.ToothlessTaming
            ? (_combat.TameGauge / 100f)
            : (_combat.Mode == CombatMode.ChapterBoss ? Math.Clamp(_combat.BossHp / 2000f, 0f, 1f) : (_combat.BossHitsRemaining / 5f));
        CleanUI.DrawProgressBar(batch, _ctx.Font, oppBarBg, oppRatio, oppAccent, leftText: null, rightText: null);

        // Target Action guide: scale 0.78f ensures text never clips outside the card
        string actionGuide = _combat.Mode == CombatMode.ToothlessTaming
            ? "Pacify by evading acid & striking counter windows!"
            : "Break through defenses to force enemy retreat!";
        batch.DrawString(_ctx.Font, actionGuide, new Vector2(rightCard.X + 24, rightCard.Y + 470), UITheme.TextMuted, 0f, Vector2.Zero, 0.78f, SpriteEffects.None, 0f);
    }

    private void DrawCombatWheel(SpriteBatch batch)
    {
        Vector2 center = new(WheelCenterX, WheelCenterY);

        // Concentric track circles
        batch.DrawCircle(center, WheelRadius + 10, 64, UITheme.BorderSubtle, 1f);
        batch.DrawCircle(center, WheelRadius, 64, UITheme.BorderLight, 4f);
        batch.DrawCircle(center, WheelRadius - 20, 64, UITheme.BorderSubtle, 1f);

        // 1. ATTACK Zone (Top-Right / Strike Window, Emerald)
        float attackCenter = 1.75f * MathF.PI; // 315°
        float attackSpan = 0.35f;
        DrawArcZone(batch, center, WheelRadius - 10, attackCenter, attackSpan, UITheme.AccentEmerald, 18f);

        Vector2 atkPos = new(center.X + MathF.Cos(attackCenter) * (WheelRadius + 45),
                             center.Y + MathF.Sin(attackCenter) * (WheelRadius + 45));
        Rectangle atkRect = new((int)atkPos.X - 55, (int)atkPos.Y - 18, 110, 36);
        CleanUI.DrawBadge(batch, _ctx.Font, atkRect, "ATTACK", UITheme.AccentEmerald * 0.25f, UITheme.AccentEmerald);

        // 2. DODGE Zone (Bottom / Evasion Window, Gold)
        float dodgeCenter = 0.5f * MathF.PI; // 90° (Bottom)
        float dodgeSpan = _combat.DodgeZoneHalfWidth;
        DrawArcZone(batch, center, WheelRadius - 10, dodgeCenter, dodgeSpan, UITheme.AccentGold, 18f);

        Vector2 ddgPos = new(center.X + MathF.Cos(dodgeCenter) * (WheelRadius + 45),
                             center.Y + MathF.Sin(dodgeCenter) * (WheelRadius + 45));
        Rectangle ddgRect = new((int)ddgPos.X - 55, (int)ddgPos.Y - 18, 110, 36);
        CleanUI.DrawBadge(batch, _ctx.Font, ddgRect, "DODGE", UITheme.AccentGold * 0.25f, UITheme.AccentGold);

        // 3. Dynamic Pet SKILL Zone (Top-Left / Skill Trigger, Pet-Specific Color)
        DrawArcZone(batch, center, WheelRadius - 10, SkillCenterAngle, SkillHalfWindow, _skillColor, 18f);

        Vector2 sklPos = new(center.X + MathF.Cos(SkillCenterAngle) * (WheelRadius + 45),
                             center.Y + MathF.Sin(SkillCenterAngle) * (WheelRadius + 45));
        Rectangle sklRect = new((int)sklPos.X - 65, (int)sklPos.Y - 18, 130, 36);
        CleanUI.DrawBadge(batch, _ctx.Font, sklRect, _skillName, _skillColor * 0.25f, _skillColor);

        // Center hub ring
        batch.DrawCircle(center, 24f, 32, UITheme.BgPanel, 20f);
        batch.DrawCircle(center, 28f, 32, UITheme.BorderHighlight, 2f);

        // Rotating Needle
        float nx = center.X + MathF.Cos(_combat.NeedleAngle) * (WheelRadius - 4);
        float ny = center.Y + MathF.Sin(_combat.NeedleAngle) * (WheelRadius - 4);
        batch.DrawLine(center.X, center.Y, nx, ny, UITheme.TextPrimary, 3f);
        batch.DrawCircle(new Vector2(nx, ny), 7f, 16, UITheme.AccentGold, 2f);
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

    private void DrawBottomBar(SpriteBatch batch)
    {
        Rectangle btmBar = new(460, 930, 1000, 56);
        CleanUI.DrawPanel(batch, btmBar, UITheme.BgPanel, UITheme.BorderSubtle, borderWidth: 1, shadow: true);

        string hint = _combat.IsCounterWindowOpen
            ? "COUNTER WINDOW OPEN: Press [ SPACEBAR ] to Strike Boss Weakness!"
            : "Press [ SPACEBAR ] to strike in ATTACK zone, evade in DODGE zone, or trigger PET SKILL!";

        Vector2 hSize = _ctx.Font.MeasureString(hint);
        Color hintCol = _combat.IsCounterWindowOpen ? UITheme.AccentEmerald : UITheme.AccentGold;
        batch.DrawString(_ctx.Font, hint, new Vector2(btmBar.Center.X - hSize.X / 2f, btmBar.Center.Y - hSize.Y / 2f), hintCol);
    }

    private void DrawVictoryBanner(SpriteBatch batch)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.85f);

        Rectangle modal = new(560, 240, 800, 600);
        CleanUI.DrawPanel(batch, modal, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(modal.X, modal.Y, modal.Width, 4), UITheme.AccentEmerald);

        Point mPos = Mouse.GetState().Position;

        if (_combat.Mode == CombatMode.ToothlessTaming)
        {
            string winTitle = "TOOTHLESS PACIFIED & TAMED!";
            Vector2 wSize = _ctx.Font.MeasureString(winTitle);
            batch.DrawString(_ctx.Font, winTitle, new Vector2(modal.Center.X - (wSize.X * 1.35f) / 2f, modal.Y + 48), UITheme.AccentEmerald, 0f, Vector2.Zero, 1.35f, SpriteEffects.None, 0f);
            batch.DrawLine(modal.X + 50, modal.Y + 100, modal.Right - 50, modal.Y + 100, UITheme.BorderSubtle, 1f);

            string note = "Toothless was successfully calmed and tamed!\nThe specimen has been admitted as your secondary shelter companion.\n\n+100 Research Points Awarded!";
            batch.DrawString(_ctx.Font, note, new Vector2(modal.X + 60, modal.Y + 160), UITheme.TextPrimary);
        }
        else
        {
            string winTitle = "MERCHANT DRIVEN OFF!";
            Vector2 wSize = _ctx.Font.MeasureString(winTitle);
            batch.DrawString(_ctx.Font, winTitle, new Vector2(modal.Center.X - (wSize.X * 1.35f) / 2f, modal.Y + 48), UITheme.AccentGold, 0f, Vector2.Zero, 1.35f, SpriteEffects.None, 0f);
            batch.DrawLine(modal.X + 50, modal.Y + 100, modal.Right - 50, modal.Y + 100, UITheme.BorderSubtle, 1f);

            string note = "You successfully repelled the predatory Traveling Collector!\nHe retreated into the ruins, dropping his contraband sack (+9999 G, +200 PTS)!";
            batch.DrawString(_ctx.Font, note, new Vector2(modal.X + 60, modal.Y + 160), UITheme.TextPrimary);
        }

        CleanUI.DrawButton(batch, _ctx.Font, _victoryBtn, "CONTINUE OPERATION", _victoryBtn.Contains(mPos), accent: UITheme.AccentEmerald, hotkey: "[ SPACE ]", isPrimary: true);
    }

    private void DrawDefeatBanner(SpriteBatch batch)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.85f);

        Rectangle modal = new(560, 240, 800, 600);
        CleanUI.DrawPanel(batch, modal, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(modal.X, modal.Y, modal.Width, 4), UITheme.AccentCoral);

        string title = _combat.Mode == CombatMode.ChapterBoss
            ? "CHAPTER BOSS INCURSION: FORCED RETREAT"
            : "COMBAT PROTOCOL BREACHED";

        Vector2 tSize = _ctx.Font.MeasureString(title);
        batch.DrawString(_ctx.Font, title, new Vector2(modal.Center.X - (tSize.X * 1.3f) / 2f, modal.Y + 48), UITheme.AccentCoral, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        batch.DrawLine(modal.X + 50, modal.Y + 100, modal.Right - 50, modal.Y + 100, UITheme.BorderSubtle, 1f);

        string note = _combat.Mode == CombatMode.ChapterBoss
            ? "The Chapter Boss overwhelmed the sanctuary with cataclysmic force!\nYour companions shielded you as you retreated into the inner research vault.\n\nVertical Slice Complete!"
            : "Specimen sustained excessive combat trauma.\nYou retreated to safety inside the shelter.";
        batch.DrawString(_ctx.Font, note, new Vector2(modal.X + 60, modal.Y + 160), UITheme.TextPrimary);

        Point mPos = Mouse.GetState().Position;
        CleanUI.DrawButton(batch, _ctx.Font, _victoryBtn, "CONTINUE OPERATION", _victoryBtn.Contains(mPos), accent: UITheme.AccentCoral, hotkey: "[ SPACE ]", isPrimary: true);
    }
}
