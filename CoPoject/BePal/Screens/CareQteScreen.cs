#nullable enable
using System;
using BePal.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePal.Screens;

/// <summary>
/// Handles the radial Care QTE mini-game with dynamic multi-slot shrinking care zones, rotating Wheel Marker, and Spacebar confirmation.
/// </summary>
public sealed class CareQteScreen : IScreen
{
    public const float Tau = MathF.PI * 2f;

    private record struct CareActionDescriptor(CareAction Action, string Need, Color Color);
    private static readonly CareActionDescriptor[] ActionDescriptors =
    {
        new(CareAction.Feed, "Appetite", new Color(72, 205, 130)),
        new(CareAction.Play, "Recreation", new Color(65, 170, 245)),
        new(CareAction.Pet, "Intimacy", new Color(245, 115, 165)),
        new(CareAction.Observe, "Observation", new Color(180, 125, 245))
    };

    private static readonly CareAction[] AllCareActions =
    {
        CareAction.Feed,
        CareAction.Play,
        CareAction.Pet,
        CareAction.Observe
    };

    private readonly ScreenContext _context;
    private readonly Random _random = new();
    private readonly ShrinkingQteZone _zone;

    private float _qteTime;
    private float _nextTeleportTime;
    private float _teleportFxTimer;
    private float _lastTeleportAngle;
    private int _teleportCount;

    public float Angle => _zone.NeedleAngle;
    public ShrinkingQteZone Zone => _zone;
    public int TeleportCount => _teleportCount;
    public float NextTeleportTime => _nextTeleportTime;
    public float LastTeleportAngle => _lastTeleportAngle;
    public float TeleportFxTimer => _teleportFxTimer;
    public float QteTime => _qteTime;
    public CareQteScreen(ScreenContext context)
    {
        _context = context;
        _zone = new ShrinkingQteZone(initialSpan: MathF.PI / 4f, duration: 3.2f, random: _random);
        ResetQte();
    }

    public void ResetQte()
    {
        _zone.SpawnSlots(AllCareActions);
        _qteTime = 0f;
        _teleportFxTimer = 0f;
        _teleportCount = 0;
        _nextTeleportTime = 0.9f + (float)_random.NextDouble() * 0.5f;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _zone.Update(dt);
        _qteTime += dt;

        PetDefinition pet = PetCatalog.Get(_context.Run.ActivePet);
        if (pet.Pattern.HasTeleportingMarker)
        {
            _teleportFxTimer = MathF.Max(0f, _teleportFxTimer - dt);

            if (_qteTime >= _nextTeleportTime && _teleportCount < 3 && !_zone.IsExpired)
            {
                TriggerTeleport();
            }
        }

        if (_zone.IsExpired)
        {
            HandleMiss("The timing window expired! Lost 1 Health.");
            return;
        }

        if (_context.IsKeyPressed(Keys.Space))
        {
            _context.Audio.PlayConfirm();
            ResolveCare();
        }
    }

    private void TriggerTeleport()
    {
        _lastTeleportAngle = _zone.NeedleAngle;
        _teleportFxTimer = 0.35f;
        _teleportCount++;

        float minOffset = MathF.PI * 0.5f;
        float maxOffset = MathF.PI * 1.15f;
        float offset = minOffset + (float)_random.NextDouble() * (maxOffset - minOffset);
        if (_random.Next(2) == 0) offset = -offset;

        _zone.NeedleAngle = (_zone.NeedleAngle + offset) % ShrinkingQteZone.Tau;
        if (_zone.NeedleAngle < 0f) _zone.NeedleAngle += ShrinkingQteZone.Tau;

        _context.Audio.PlayTeleport();
        _context.TriggerShake(0.20f, 6f);
        _context.SetPetReaction(_context.PetAngry, 0.45f);
        _context.SpawnTag("WARP!", new Color(45, 18, 55), new Color(230, 160, 255));
        _context.Message = "Blinkbun warped the wheel marker!";
        _nextTeleportTime = _qteTime + 1.4f + (float)_random.NextDouble() * 0.5f;
    }

    private void HandleMiss(string message)
    {
        _context.Audio.PlayFail();
        _context.TriggerShake(0.24f, 9f);
        _context.SetPetReaction(_context.PetAngry, 0.65f);
        _context.SpawnTag("MISSED! -1 HP", new Color(45, 12, 18), new Color(255, 80, 80));

        bool forcedRetreat = _context.Run.TakeDamage();
        if (forcedRetreat)
        {
            _context.Manager.HandleForcedRetreat();
        }
        else
        {
            _context.Message = message;
            ResetQte();
        }
    }

    private void HandleRejection(CareAction action)
    {
        _context.Audio.PlayFail();
        _context.TriggerShake(0.24f, 9f);
        _context.SetPetReaction(_context.PetAngry, 0.65f);
        string actionName = action.ToString().ToUpperInvariant();
        _context.SpawnTag($"REJECTED: {actionName}! -1 HP", new Color(45, 12, 18), new Color(255, 80, 80));

        bool forcedRetreat = _context.Run.TakeDamage();
        if (forcedRetreat)
        {
            _context.Manager.HandleForcedRetreat();
        }
        else
        {
            _context.Message = $"Disliked {action}! Lost 1 Health.";
            ResetQte();
        }
    }

    public CareAction? GetHoveredAction()
    {
        return _zone.GetHoveredSlot()?.Action;
    }

    public void ResolveCare()
    {
        PetDefinition activePet = PetCatalog.Get(_context.Run.ActivePet);
        QteSlot? hovered = _zone.GetHoveredSlot();

        if (hovered == null)
        {
            HandleMiss("Hit the dead zone! Lost 1 Health.");
            return;
        }

        if (hovered.Action == activePet.Pattern.PreferredAction)
        {
            _context.Run.RecordCareSuccess();
            _context.SetPetReaction(_context.PetHappy, 0.75f);
            _context.SpawnTag("PERFECT! +1 SATISFACTION", new Color(16, 48, 28), new Color(110, 245, 150));

            if (activePet.Pattern.HasDodgeAttack && _context.Run.Satisfaction == activePet.Pattern.AttackThreshold)
            {
                _context.Audio.PlaySuccess();
                _context.Manager.BeginDodge();
            }
            else if (_context.Run.Satisfaction >= 3)
            {
                _context.Run.CompleteSession();
                _context.Audio.PlaySessionComplete();
                _context.SetPetReaction(_context.PetHappy, 1.2f);
                _context.SpawnTag("SESSION COMPLETE!", new Color(25, 45, 60), new Color(255, 230, 130));
                _context.Manager.ShowHome("Session complete! Satisfaction is full.");
            }
            else
            {
                _context.Audio.PlaySuccess();
                ResetQte();
                _context.Message = "Correct action! Keep going.";
            }
        }
        else
        {
            HandleRejection(hovered.Action!.Value);
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Vector2 c = new(640, 360);
        float trackRadius = 215f;

        // 1. Central Creature Display with Vignette Backdrop
        spriteBatch.FillRectangle(new RectangleF(c.X - 110, c.Y - 145, 220, 290), new Color(14, 16, 24, 210));
        if (_context.PetImage != null)
        {
            spriteBatch.Draw(_context.PetImage, new Rectangle((int)c.X - 90, (int)c.Y - 125, 180, 250), Color.White);
        }
        spriteBatch.DrawRectangle(new RectangleF(c.X - 95, c.Y - 130, 190, 260), new Color(42, 50, 72), 2f);

        // 2. Circular Ring Track (Death Spiral style)
        Color trackBg = new(34, 40, 58);
        Color trackBorder = new(22, 26, 38);
        spriteBatch.DrawCircle(c, trackRadius, 64, trackBg, 18f);
        spriteBatch.DrawCircle(c, trackRadius - 10f, 64, trackBorder, 2f);
        spriteBatch.DrawCircle(c, trackRadius + 10f, 64, trackBorder, 2f);

        // 5 Position Notches
        for (int i = 0; i < ShrinkingQteZone.PresetAngles.Length; i++)
        {
            float notchAngle = ShrinkingQteZone.PresetAngles[i];
            Vector2 p1 = c + Dir(notchAngle) * (trackRadius - 8f);
            Vector2 p2 = c + Dir(notchAngle) * (trackRadius + 8f);
            spriteBatch.DrawLine(p1, p2, new Color(55, 65, 88), 2f);
        }

        // 3. Dynamic Shrinking Care Zone Arcs & Scrap Badges for All Active Slots
        QteSlot? hoveredSlot = _zone.GetHoveredSlot();

        foreach (var slot in _zone.Slots)
        {
            if (!slot.Action.HasValue || slot.CurrentSpan <= 0f) continue;

            CareActionDescriptor desc = GetDescriptor(slot.Action.Value);
            float centerA = slot.CenterAngle;
            bool isHovered = (hoveredSlot == slot);
            Color baseCol = desc.Color;
            Color arcCol = isHovered ? Color.Lerp(baseCol, Color.White, 0.45f) : baseCol;

            spriteBatch.DrawArc(c, trackRadius, centerA - slot.CurrentSpan / 2f, slot.CurrentSpan, 32, arcCol, isHovered ? 24f : 18f);

            Vector2 badgePos = c + Dir(centerA) * (trackRadius + 58f);
            _context.DrawScrapBadge(spriteBatch, badgePos, desc.Action.ToString().ToUpperInvariant(), desc.Need, arcCol, Color.White, isHovered);
        }

        PetDefinition activePet = PetCatalog.Get(_context.Run.ActivePet);

        // Ghost Trail Afterimage
        if (_teleportFxTimer > 0f)
        {
            float alpha = Math.Clamp(_teleportFxTimer / 0.35f, 0f, 1f);
            Vector2 ghostInner = c + Dir(_lastTeleportAngle) * (trackRadius - 22f);
            Vector2 ghostOuter = c + Dir(_lastTeleportAngle) * (trackRadius + 24f);
            spriteBatch.DrawLine(ghostInner, ghostOuter, new Color(190, 80, 255) * alpha, 6f);
            spriteBatch.DrawCircle(ghostOuter, 3f, 16, new Color(230, 160, 255) * alpha, 2f);

            Vector2 warpFrom = c + Dir(_lastTeleportAngle) * trackRadius;
            Vector2 warpTo = c + Dir(_zone.NeedleAngle) * trackRadius;
            spriteBatch.DrawLine(warpFrom, warpTo, new Color(220, 150, 255) * (alpha * 0.5f), 2f);
        }

        // 4. Rotating Needle Marker (with telegraph pulse)
        bool isTelegraphing = activePet.Pattern.HasTeleportingMarker &&
                              _teleportCount < 3 &&
                              _nextTeleportTime - _qteTime <= 0.25f &&
                              _nextTeleportTime > _qteTime;

        Color needleColor = Color.White;
        Color pipColor = Color.Gold;

        if (isTelegraphing)
        {
            float pulse = (MathF.Sin(_qteTime * 25f) + 1f) * 0.5f;
            needleColor = Color.Lerp(Color.White, new Color(230, 120, 255), pulse);
            pipColor = new Color(255, 110, 230);
        }

        Vector2 innerPt = c + Dir(_zone.NeedleAngle) * (trackRadius - 22f);
        Vector2 outerPt = c + Dir(_zone.NeedleAngle) * (trackRadius + 24f);
        spriteBatch.DrawLine(innerPt, outerPt, needleColor, 8f);
        spriteBatch.DrawCircle(outerPt, 4f, 16, pipColor, 2f);
        // 5. HUD & Status
        _context.DrawHUD(spriteBatch);

        Vector2 promptPos = new(640, 610);
        string pips = "SATISFACTION: ";
        for (int i = 1; i <= 3; i++) pips += i <= _context.Run.Satisfaction ? "[#] " : "[.] ";
        pips += $"({_context.Run.Satisfaction} / 3)";
        _context.DrawCenterText(spriteBatch, pips, promptPos, 0.95f, new Color(110, 240, 160));
        _context.DrawCenterText(spriteBatch, _context.Message, new Vector2(640, 655), 0.72f, new Color(255, 225, 165));

        // 6. Floating Tags
        _context.DrawFloatingTags(spriteBatch);
    }

    private static CareActionDescriptor GetDescriptor(CareAction action)
    {
        foreach (var desc in ActionDescriptors)
        {
            if (desc.Action == action) return desc;
        }
        return ActionDescriptors[0];
    }

    private static Vector2 Dir(float a) => new(MathF.Cos(a), MathF.Sin(a));
}
