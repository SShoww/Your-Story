using System;
using System.Collections.Generic;
using BePalV2.Audio;
using BePalV2.Gameplay;
using BePalV2.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePalV2.Screens;

public enum CareQtePhase
{
    Selection,
    ActiveQte,
    Completed
}

public sealed class CareQteScreen : IScreen
{
    private readonly ScreenContext _ctx;
    private CareQteEngine _engine = null!;
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;

    private CareQtePhase _phase;
    private CareActionType _activeAction;

    private string? _floatingFeedback;
    private Color _floatingColor;
    private float _feedbackTimer;

    // 1920x1080 Layout Constants
    private const float WheelCenterX = 960f;
    private const float WheelCenterY = 500f;
    private const float WheelRadius = 240f;

    // Selection Phase Data (Staggered & Randomized Spawn)
    private record struct SelectionSlot(CareActionType Action, string Label, float CenterAngle, Color Color, float AppearTime);
    private readonly List<SelectionSlot> _selectionSlots = new();
    private float _selectionElapsed;

    // Active QTE Parameters
    private float _needleAngle;
    private float _needleSpeed = 2.4f;
    private float _targetAngle = 1.5f * MathF.PI; // Top (270 deg)
    private float _perfectWindow = 0.20f;
    private float _goodWindow = 0.45f;
    private float _activeElapsed;

    // Action-specific Gimmick Variables
    // 1. Feed: needle reverses on hit
    // 2. Clean: target zone continuously escapes needle
    // 3. Train: faster needle + smaller constant target zone (no shrinking)
    // 4. Heal: needle blinks intermittently after zone hit
    private bool _healBlinkActive;
    private float _blinkTimer;
    private bool _needleBlinkVisible = true;

    private int _attemptsRemaining = 10;
    private int _successfulHits;
    private int _currentStreak;
    private int _maxStreak;
    private int _totalScore;

    public CareQteScreen(ScreenContext ctx, CareActionType? defaultAction = null)
    {
        _ctx = ctx;
        if (defaultAction.HasValue)
        {
            StartActiveQte(defaultAction.Value);
        }
        else
        {
            StartSelectionPhase();
        }
    }

    private void StartSelectionPhase()
    {
        _phase = CareQtePhase.Selection;
        _selectionElapsed = 0f;
        _needleAngle = 0f;
        _needleSpeed = 2.2f;

        // Preset 4 angles around the wheel, randomized
        float[] presetAngles = { 0.25f * MathF.PI, 0.75f * MathF.PI, 1.25f * MathF.PI, 1.75f * MathF.PI };
        var rng = new Random();
        for (int i = presetAngles.Length - 1; i > 0; i--)
        {
            int swapIdx = rng.Next(i + 1);
            (presetAngles[i], presetAngles[swapIdx]) = (presetAngles[swapIdx], presetAngles[i]);
        }

        var actions = new[]
        {
            (CareActionType.Feed, "FEED", new Color(240, 130, 60)),
            (CareActionType.Clean, "CLEAN", UITheme.AccentCyan),
            (CareActionType.Train, "TRAIN", UITheme.AccentGold),
            (CareActionType.Heal, "HEAL", UITheme.AccentEmerald)
        };

        _selectionSlots.Clear();
        for (int i = 0; i < actions.Length; i++)
        {
            // Staggered appearance: slot 0 at 0s, slot 1 at 0.4s, slot 2 at 0.8s, slot 3 at 1.2s
            float appear = i * 0.40f;
            _selectionSlots.Add(new SelectionSlot(actions[i].Item1, actions[i].Item2, presetAngles[i], actions[i].Item3, appear));
        }
    }

    private void StartActiveQte(CareActionType action)
    {
        _phase = CareQtePhase.ActiveQte;
        _activeAction = action;
        _activeElapsed = 0f;
        _attemptsRemaining = 10;
        _successfulHits = 0;
        _currentStreak = 0;
        _maxStreak = 0;
        _totalScore = 0;
        _healBlinkActive = false;
        _needleBlinkVisible = true;

        var pet = _ctx.Run.ActivePet;
        _engine = new CareQteEngine(pet, action, _ctx.Run.Inventory.EquippedItem);

        _targetAngle = 1.5f * MathF.PI; // Top (270 deg)

        // Tune parameters per action gimmick requested by user
        switch (action)
        {
            case CareActionType.Feed:
                // Normal speed, reverses rotation upon hitting zone
                _needleSpeed = 2.4f;
                _perfectWindow = 0.22f;
                _goodWindow = 0.48f;
                break;

            case CareActionType.Clean:
                // Normal speed, target zone continuously escapes needle
                _needleSpeed = 2.4f;
                _perfectWindow = 0.22f;
                _goodWindow = 0.48f;
                break;

            case CareActionType.Train:
                // Fast needle speed, smaller fixed target zone (does not shrink)
                _needleSpeed = 4.0f;
                _perfectWindow = 0.14f;
                _goodWindow = 0.32f;
                break;

            case CareActionType.Heal:
                // Normal speed, needle blinks intermittently when zone is struck
                _needleSpeed = 2.4f;
                _perfectWindow = 0.22f;
                _goodWindow = 0.48f;
                break;
        }
        if (pet.IsGrimy)
        {
            _needleSpeed *= 1.15f;
        }

        float qteBonus = _ctx.Run.Progression.QteWindowBonus;
        _perfectWindow += qteBonus;
        _goodWindow += qteBonus;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

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

        if (_phase == CareQtePhase.Selection)
        {
            UpdateSelection(dt, spaceHit, mouseHit, kbd);
        }
        else if (_phase == CareQtePhase.ActiveQte)
        {
            UpdateActiveQte(dt, spaceHit, mouseHit);
        }
        else if (_phase == CareQtePhase.Completed)
        {
            if (spaceHit || mouseHit || (kbd.IsKeyDown(Keys.Enter) && !_prevKeyboard.IsKeyDown(Keys.Enter)))
            {
                RouteDefensePhase();
            }
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void UpdateSelection(float dt, bool spaceHit, bool mouseHit, KeyboardState kbd)
    {
        _selectionElapsed += dt;
        _needleAngle = (_needleAngle + _needleSpeed * dt) % (2f * MathF.PI);

        // Numeric Hotkeys 1-4
        if (kbd.IsKeyDown(Keys.D1) && !_prevKeyboard.IsKeyDown(Keys.D1)) { SelectAction(CareActionType.Train); return; }
        if (kbd.IsKeyDown(Keys.D2) && !_prevKeyboard.IsKeyDown(Keys.D2)) { SelectAction(CareActionType.Feed); return; }
        if (kbd.IsKeyDown(Keys.D3) && !_prevKeyboard.IsKeyDown(Keys.D3)) { SelectAction(CareActionType.Clean); return; }
        if (kbd.IsKeyDown(Keys.D4) && !_prevKeyboard.IsKeyDown(Keys.D4)) { SelectAction(CareActionType.Heal); return; }

        if (!spaceHit && !mouseHit) return;

        // Check if needle is inside any currently visible slot
        const float slotHalfSpan = 0.28f;
        SelectionSlot? chosenSlot = null;

        foreach (var slot in _selectionSlots)
        {
            if (_selectionElapsed < slot.AppearTime) continue; // Not spawned yet

            float diff = MathF.Abs(MathHelper.WrapAngle(_needleAngle - slot.CenterAngle));
            if (diff <= slotHalfSpan)
            {
                chosenSlot = slot;
                break;
            }
        }

        if (chosenSlot.HasValue)
        {
            SelectAction(chosenSlot.Value.Action);
        }
    }

    private void SelectAction(CareActionType action)
    {
        var pet = _ctx.Run.ActivePet;
        if (action == CareActionType.Train && !pet.CanTrain())
        {
            _ctx.Audio.PlayWarning();
            _floatingFeedback = "Too hungry to train! (Needs Stomach >= 20)";
            _floatingColor = UITheme.AccentCoral;
            _feedbackTimer = 1.2f;
            return;
        }

        int requiredAp = action == CareActionType.Train ? 2 : 1;
        if (_ctx.Run.Energy.CurrentEnergy < requiredAp)
        {
            _ctx.Audio.PlayWarning();
            _floatingFeedback = $"Not enough AP! (Needs {requiredAp} AP)";
            _floatingColor = UITheme.AccentCoral;
            _feedbackTimer = 1.2f;
            return;
        }

        _ctx.Audio.PlayConfirm();
        StartActiveQte(action);
    }

    private void UpdateActiveQte(float dt, bool spaceHit, bool mouseHit)
    {
        _activeElapsed += dt;

        // Gimmick 1: Clean - target zone continuously rotates away from needle
        if (_activeAction == CareActionType.Clean)
        {
            float escapeSpeed = 0.80f * MathF.Sign(_needleSpeed);
            _targetAngle = MathHelper.WrapAngle(_targetAngle + escapeSpeed * dt);
        }

        // Gimmick 2: Heal - needle blinks intermittently after first hit
        if (_healBlinkActive)
        {
            _blinkTimer += dt;
            if (_blinkTimer >= 0.25f)
            {
                _blinkTimer = 0f;
                _needleBlinkVisible = !_needleBlinkVisible;
            }
        }

        _needleAngle = (_needleAngle + _needleSpeed * dt) % (2f * MathF.PI);
        if (_needleAngle < 0f) _needleAngle += 2f * MathF.PI;

        if ((spaceHit || mouseHit) && _attemptsRemaining > 0)
        {
            ResolveAttempt();
        }
    }

    private void ResolveAttempt()
    {
        _attemptsRemaining--;

        float diff = MathF.Abs(MathHelper.WrapAngle(_needleAngle - _targetAngle));

        bool isPerfect = diff <= _perfectWindow;
        bool isGood = !isPerfect && diff <= _goodWindow;

        var pet = _ctx.Run.ActivePet;

        if (isPerfect || isGood)
        {
            _successfulHits++;
            _currentStreak++;
            _maxStreak = Math.Max(_maxStreak, _currentStreak);
            int scoreGain = isPerfect ? (120 + _currentStreak * 30) : (70 + _currentStreak * 15);
            _totalScore += scoreGain;

            _engine.RecordAttempt(isPerfect ? PrecisionTier.Perfect : PrecisionTier.Good);

            // Gimmick: Feed reverses needle rotation direction immediately on hit!
            if (_activeAction == CareActionType.Feed)
            {
                _needleSpeed = -_needleSpeed;
            }

            // Gimmick: Heal activates blinking needle upon hitting zone!
            if (_activeAction == CareActionType.Heal)
            {
                _healBlinkActive = true;
            }

            // Apply incremental care progress
            switch (_activeAction)
            {
                case CareActionType.Feed:
                    pet.FeedDirect(isPerfect ? 4 : 2);
                    break;
                case CareActionType.Clean:
                    pet.CleanDirect(isPerfect ? 4 : 2);
                    break;
                case CareActionType.Train:
                    float boost = _ctx.Run.Progression.ProgressBoosterMultiplier * _ctx.Run.Inventory.PermanentTrainExpMultiplier;
                    pet.AddProgress(isPerfect ? 10 : 5, boost);
                    break;
                case CareActionType.Heal:
                    pet.Heal(isPerfect ? 3.5f : 2.0f);
                    break;
            }

            _ctx.Audio.PlaySuccess();
            _floatingFeedback = isPerfect ? "PERFECT! +150" : "GOOD! +80";
            _floatingColor = isPerfect ? UITheme.AccentEmerald : UITheme.AccentGold;
            _feedbackTimer = 0.60f;
        }
        else
        {
            _engine.RecordAttempt(PrecisionTier.Miss);
            _currentStreak = 0;
            _ctx.Audio.PlayFail();
            _floatingFeedback = "MISS!";
            _floatingColor = UITheme.AccentCoral;
            _feedbackTimer = 0.60f;
        }

        if (_attemptsRemaining <= 0)
        {
            FinishSession();
        }
    }

    private void FinishSession()
    {
        var run = _ctx.Run;
        int apCost = _activeAction == CareActionType.Train ? 2 : 1;
        run.Energy.Spend(apCost);
        run.Economy.AddPlayerPoints(25);
        run.RecordCareSessionOutcome(_engine);
        if (_activeAction == CareActionType.Train)
        {
            run.ActivePet.FeedDirect(-10);
        }
        else if (_activeAction != CareActionType.Feed)
        {
            run.ActivePet.FeedDirect(-5);
        }

        _ctx.Audio.PlayConfirm();
        _phase = CareQtePhase.Completed;
    }

    private void RouteDefensePhase()
    {
        _ctx.ScreenManager.SetScreen(new BaseHabitatScreen(_ctx));
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        // Deep background
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), UITheme.BgDeep);

        if (_phase == CareQtePhase.Selection)
        {
            DrawSelectionPhase(batch);
        }
        else
        {
            DrawActiveQtePhase(batch);
        }

        DrawBottomProgressBars(batch);

        if (_phase == CareQtePhase.Completed)
        {
            DrawCompletionModal(batch);
        }
    }

    private void DrawSelectionPhase(SpriteBatch batch)
    {
        // Header
        batch.DrawString(_ctx.Font, "CARE PROTOCOL - SELECT PROCEDURE", new Vector2(80, 48), UITheme.TextPrimary, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0f);

        Rectangle phaseBadge = new(80, 100, 240, 36);
        CleanUI.DrawBadge(batch, _ctx.Font, phaseBadge, "SELECTION PHASE", new Color(34, 42, 58), UITheme.AccentCyan);

        string guide = "Target slots are engaging. Press [ SPACEBAR ] or Click when the needle touches a procedure.";
        batch.DrawString(_ctx.Font, guide, new Vector2(80, 150), UITheme.TextSecondary, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);

        Vector2 center = new(WheelCenterX, WheelCenterY);
        DrawCenterPetPortrait(batch, center);

        // Concentric track circles
        batch.DrawCircle(center, WheelRadius + 12, 64, UITheme.BorderSubtle, 1f);
        batch.DrawCircle(center, WheelRadius, 64, UITheme.BorderLight, 3f);
        batch.DrawCircle(center, WheelRadius - 20, 64, UITheme.BorderSubtle, 1f);

        const float slotHalfSpan = 0.28f;
        for (int i = 0; i < _selectionSlots.Count; i++)
        {
            var slot = _selectionSlots[i];
            if (_selectionElapsed < slot.AppearTime) continue;

            float alpha = Math.Clamp((_selectionElapsed - slot.AppearTime) / 0.3f, 0f, 1f);
            Color arcCol = slot.Color * alpha;

            DrawArcZone(batch, center, WheelRadius - 10, slot.CenterAngle, slotHalfSpan, arcCol, 18f);

            Vector2 nodePos = new(center.X + MathF.Cos(slot.CenterAngle) * (WheelRadius + 48),
                                  center.Y + MathF.Sin(slot.CenterAngle) * (WheelRadius + 48));
            Rectangle nodeRect = new((int)nodePos.X - 56, (int)nodePos.Y - 20, 112, 40);
            CleanUI.DrawBadge(batch, _ctx.Font, nodeRect, $"[{i + 1}] {slot.Label}", arcCol * 0.25f, arcCol);
        }

        // Needle
        DrawNeedle(batch, center, _needleAngle, UITheme.TextPrimary);
    }

    private void DrawActiveQtePhase(SpriteBatch batch)
    {
        string title = _activeAction switch
        {
            CareActionType.Feed => "FEEDING SESSION - NUTRITIONAL INTAKE",
            CareActionType.Clean => "DECONTAMINATION - STERILIZATION PROTOCOL",
            CareActionType.Train => "TRAINING DRILL - REFLEX CONDITIONING",
            _ => "MEDICAL TREATMENT - CELLULAR REPAIR"
        };

        Color accent = _activeAction switch
        {
            CareActionType.Feed => new Color(240, 130, 60),
            CareActionType.Clean => UITheme.AccentCyan,
            CareActionType.Train => UITheme.AccentGold,
            _ => UITheme.AccentEmerald
        };

        batch.DrawString(_ctx.Font, title, new Vector2(80, 48), UITheme.TextPrimary, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0f);

        Rectangle attBadge = new(80, 100, 240, 36);
        CleanUI.DrawBadge(batch, _ctx.Font, attBadge, $"ATTEMPTS: {_attemptsRemaining} / 10", new Color(34, 42, 58), accent);

        Rectangle streakBadge = new(340, 100, 180, 36);
        CleanUI.DrawBadge(batch, _ctx.Font, streakBadge, $"STREAK: {_currentStreak}", new Color(34, 42, 58), UITheme.AccentGold);

        string gimmickGuide = _activeAction switch
        {
            CareActionType.Feed => "NEEDLE REVERSAL: Striking the target zone immediately reverses needle rotation!",
            CareActionType.Clean => "ESCAPING TARGET: The target zone continuously rotates and flees from the needle!",
            CareActionType.Train => "HIGH SPEED DRILL: Fast needle rotation with a compact, fixed target zone!",
            _ => "PHANTOM NEEDLE: Striking the target zone causes the needle to blink and flicker!"
        };

        batch.DrawString(_ctx.Font, gimmickGuide, new Vector2(80, 150), UITheme.TextSecondary, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);

        Vector2 center = new(WheelCenterX, WheelCenterY);
        DrawCenterPetPortrait(batch, center);

        // Concentric track circles
        batch.DrawCircle(center, WheelRadius + 12, 64, UITheme.BorderSubtle, 1f);
        batch.DrawCircle(center, WheelRadius, 64, UITheme.BorderLight, 3f);
        batch.DrawCircle(center, WheelRadius - 20, 64, UITheme.BorderSubtle, 1f);

        // Target Zones (Good and Perfect)
        DrawArcZone(batch, center, WheelRadius - 10, _targetAngle, _goodWindow, accent * 0.45f, 18f);
        DrawArcZone(batch, center, WheelRadius - 10, _targetAngle, _perfectWindow, accent, 20f);

        // Target Indicator Badge on perimeter
        Vector2 targetPos = new(center.X + MathF.Cos(_targetAngle) * (WheelRadius + 44),
                                center.Y + MathF.Sin(_targetAngle) * (WheelRadius + 44));
        Rectangle targetRect = new((int)targetPos.X - 50, (int)targetPos.Y - 20, 100, 40);
        CleanUI.DrawBadge(batch, _ctx.Font, targetRect, "ZONE", accent * 0.25f, accent);

        // Needle (Respects Heal blinking mechanic)
        if (!_healBlinkActive || _needleBlinkVisible)
        {
            DrawNeedle(batch, center, _needleAngle, UITheme.TextPrimary);
        }

        // Floating feedback
        if (!string.IsNullOrEmpty(_floatingFeedback))
        {
            Vector2 fbSize = _ctx.Font.MeasureString(_floatingFeedback) * 1.3f;
            batch.DrawString(_ctx.Font, _floatingFeedback, new Vector2(center.X - fbSize.X / 2f, center.Y + 120), _floatingColor, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        }
    }

    private void DrawCenterPetPortrait(SpriteBatch batch, Vector2 center)
    {
        Rectangle centralFrame = new((int)center.X - 90, (int)center.Y - 110, 180, 220);
        CleanUI.DrawPanel(batch, centralFrame, new Color(14, 18, 26), UITheme.BorderSubtle, borderWidth: 1, shadow: true);

        if (_ctx.PetIdleTex != null)
        {
            int sprH = 170;
            int sprW = (int)(sprH * (1298f / 1731f)); // 127px (3:4 ratio)
            Rectangle petRect = new((int)center.X - sprW / 2, (int)center.Y - sprH / 2, sprW, sprH);
            batch.Draw(_ctx.PetIdleTex, petRect, Color.White);
        }
    }

    private static void DrawNeedle(SpriteBatch batch, Vector2 center, float angle, Color color)
    {
        batch.DrawCircle(center, 24f, 32, UITheme.BorderHighlight, 2f);

        float nx = center.X + MathF.Cos(angle) * (WheelRadius - 4);
        float ny = center.Y + MathF.Sin(angle) * (WheelRadius - 4);
        batch.DrawLine(center.X, center.Y, nx, ny, color, 3f);

        batch.DrawCircle(new Vector2(nx, ny), 7f, 16, UITheme.AccentCyan, 2f);
        batch.FillRectangle(new Rectangle((int)nx - 3, (int)ny - 3, 6, 6), UITheme.AccentCyan);
    }

    private void DrawCompletionModal(SpriteBatch batch)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.80f);

        Rectangle modal = new(640, 280, 640, 420);
        CleanUI.DrawPanel(batch, modal, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 2, shadow: true);
        batch.FillRectangle(new Rectangle(modal.X, modal.Y, modal.Width, 4), UITheme.AccentEmerald);

        var (grade, gold) = _engine.CalculateResults();

        batch.DrawString(_ctx.Font, "CARE SESSION COMPLETE", new Vector2(modal.X + 40, modal.Y + 36), UITheme.AccentGold, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

        batch.DrawString(_ctx.Font, $"Care Grade: {grade}", new Vector2(modal.X + 40, modal.Y + 100), UITheme.AccentEmerald, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, $"Hits: {_successfulHits} / 10   |   Max Streak: {_maxStreak}", new Vector2(modal.X + 40, modal.Y + 160), UITheme.TextPrimary, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, $"Performance Bonus: +{gold} G   |   Points: +25 PTS", new Vector2(modal.X + 40, modal.Y + 210), UITheme.AccentCyan, 0f, Vector2.Zero, 1.05f, SpriteEffects.None, 0f);

        Rectangle contBtn = new(modal.Center.X - 160, modal.Bottom - 80, 320, 48);
        Point mPos = Mouse.GetState().Position;
        CleanUI.DrawButton(batch, _ctx.Font, contBtn, "CONTINUE", contBtn.Contains(mPos), accent: UITheme.AccentEmerald, isPrimary: true, hotkey: "[ SPACE / ENTER ]");
    }

    private void DrawBottomProgressBars(SpriteBatch batch)
    {
        Rectangle barBox = new(360, 910, 1200, 64);
        CleanUI.DrawPanel(batch, barBox, UITheme.BgPanel, UITheme.BorderSubtle, borderWidth: 1, shadow: true);

        var pet = _ctx.Run.ActivePet;

        Rectangle lvlBadge = new(barBox.X + 20, barBox.Y + 12, 160, 40);
        CleanUI.DrawBadge(batch, _ctx.Font, lvlBadge, $"EXP: LV. {pet.Level}", new Color(34, 46, 38), UITheme.AccentEmerald);

        Rectangle fillBg = new(barBox.X + 200, barBox.Y + 16, 680, 32);
        float ratio = Math.Clamp((float)pet.CurrentExp / Math.Max(1, pet.MaxExp), 0f, 1f);
        CleanUI.DrawProgressBar(batch, _ctx.Font, fillBg, ratio, UITheme.AccentEmerald, leftText: null, rightText: $"{pet.CurrentExp} / {pet.MaxExp} EXP");

        Rectangle apPill = new(barBox.Right - 300, barBox.Y + 14, 135, 36);
        CleanUI.DrawBadge(batch, _ctx.Font, apPill, $"AP: {_ctx.Run.Energy.CurrentEnergy}/{_ctx.Run.Energy.MaxEnergy}", new Color(28, 36, 48), UITheme.AccentCyan);

        Rectangle ptsPill = new(barBox.Right - 150, barBox.Y + 14, 130, 36);
        CleanUI.DrawBadge(batch, _ctx.Font, ptsPill, $"{_ctx.Run.Economy.PlayerPoints} PTS", new Color(42, 38, 24), UITheme.AccentGold);
    }

    private static void DrawArcZone(SpriteBatch batch, Vector2 center, float radius, float targetAngle, float halfWindow, Color color, float thickness)
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
