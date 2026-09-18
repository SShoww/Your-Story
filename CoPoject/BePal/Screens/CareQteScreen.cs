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
    private float _teleportAt;
    private bool _teleported;

    public float Angle => _zone.NeedleAngle;
    public ShrinkingQteZone Zone => _zone;

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
        _teleported = false;
        _teleportAt = 0.45f + (float)_random.NextDouble() * 0.85f;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _zone.Update(dt);
        _qteTime += dt;

        PetDefinition pet = PetCatalog.Get(_context.Run.ActivePet);
        if (pet.Pattern.HasTeleportingMarker && !_teleported && _qteTime >= _teleportAt)
        {
            _zone.NeedleAngle = (float)_random.NextDouble() * ShrinkingQteZone.Tau;
            _teleported = true;
            _context.Audio.PlayTeleport();
            _context.TriggerShake(0.18f, 5f);
            _context.SpawnTag("MARKER TELEPORTED!", new Color(45, 25, 55), new Color(230, 160, 255));
            _context.Message = "The marker teleported!";
        }

        if (_zone.IsExpired)
        {
            _context.Audio.PlayFail();
            _context.SpawnTag("MISSED! TRY AGAIN", new Color(45, 35, 20), new Color(255, 200, 100));
            _context.Message = "The timing window expired! A new opportunity appeared.";
            _zone.SpawnSlots(AllCareActions);
            _qteTime = 0f;
            _teleported = false;
            _teleportAt = 0.45f + (float)_random.NextDouble() * 0.85f;
            return;
        }

        if (_context.IsKeyPressed(Keys.Space))
        {
            _context.Audio.PlayConfirm();
            ResolveCare();
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
            _context.Audio.PlayFail();
            _context.TriggerShake(0.24f, 9f);
            _context.SetPetReaction(_context.PetAngry, 0.65f);
            _context.SpawnTag("MISSED! -1 HP", new Color(45, 12, 18), new Color(255, 80, 80));
            _context.Manager.Fail("Hit the dead zone! Lost 1 Health.");
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
                _zone.SpawnSlots(AllCareActions);
                _qteTime = 0f;
                _teleported = false;
                _teleportAt = 0.45f + (float)_random.NextDouble() * 0.85f;
                _context.Message = "Correct action! Keep going.";
            }
        }
        else
        {
            _context.Audio.PlayFail();
            _context.TriggerShake(0.24f, 9f);
            _context.SetPetReaction(_context.PetAngry, 0.65f);
            string actionName = hovered.Action?.ToString().ToUpperInvariant() ?? "UNKNOWN";
            _context.SpawnTag($"REJECTED: {actionName}! -1 HP", new Color(45, 12, 18), new Color(255, 80, 80));
            _context.Manager.Fail($"Disliked {hovered.Action}! Lost 1 Health.");
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Vector2 c = new(640, 360);
        float trackRadius = 215f;

        // 1. Central Creature Display with Vignette Backdrop
        spriteBatch.FillRectangle(new RectangleF(c.X - 110, c.Y - 145, 220, 290), new Color(14, 16, 24, 210));
        spriteBatch.Draw(_context.PetImage, new Rectangle((int)c.X - 90, (int)c.Y - 125, 180, 250), Color.White);
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
        float span = _zone.CurrentSpan;

        foreach (var slot in _zone.Slots)
        {
            if (!slot.Action.HasValue) continue;

            CareActionDescriptor desc = GetDescriptor(slot.Action.Value);
            float centerA = slot.CenterAngle;
            bool isHovered = (hoveredSlot == slot);
            Color baseCol = desc.Color;
            Color arcCol = isHovered ? Color.Lerp(baseCol, Color.White, 0.45f) : baseCol;

            if (span > 0f)
            {
                spriteBatch.DrawArc(c, trackRadius, centerA - span / 2f, span, 32, arcCol, isHovered ? 24f : 18f);
            }

            Vector2 badgePos = c + Dir(centerA) * (trackRadius + 58f);
            _context.DrawScrapBadge(spriteBatch, badgePos, desc.Action.ToString().ToUpperInvariant(), desc.Need, arcCol, Color.White, isHovered);
        }

        // 4. Rotating Needle Marker
        Vector2 innerPt = c + Dir(_zone.NeedleAngle) * (trackRadius - 22f);
        Vector2 outerPt = c + Dir(_zone.NeedleAngle) * (trackRadius + 24f);
        spriteBatch.DrawLine(innerPt, outerPt, Color.White, 8f);
        spriteBatch.DrawCircle(outerPt, 4f, 16, Color.Gold, 2f);

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
