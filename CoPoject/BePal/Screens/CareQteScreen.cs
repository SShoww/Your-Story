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
    public const float TelegraphDuration = 0.60f;
    public const float GracePeriodDuration = 0.20f;
    public const int MaxTeleportsPerCycle = 2;

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
    private readonly Random _random;
    private readonly ShrinkingQteZone _zone;

    private float _qteTime;
    private float _nextTeleportTime;
    private float _teleportTargetAngle;
    private bool _hasPreparedDestination;
    private float _teleportGraceTimer;
    private float _teleportFxTimer;
    private float _lastTeleportAngle;
    private int _teleportCount;

    public float Angle => _zone.NeedleAngle;
    public ShrinkingQteZone Zone => _zone;
    public int TeleportCount => _teleportCount;
    public float NextTeleportTime => _nextTeleportTime;
    public float LastTeleportAngle => _lastTeleportAngle;
    public float TeleportFxTimer => _teleportFxTimer;
    public float TeleportGraceTimer => _teleportGraceTimer;
    public float TeleportTargetAngle => _teleportTargetAngle;
    public float QteTime => _qteTime;
    public bool IsTelegraphing => PetCatalog.Get(_context.Run.ActivePet).Pattern.HasTeleportingMarker &&
                                  _teleportCount < MaxTeleportsPerCycle &&
                                  _qteTime >= _nextTeleportTime - TelegraphDuration &&
                                  _qteTime < _nextTeleportTime;

    public CareQteScreen(ScreenContext context, Random? random = null)
    {
        _context = context;
        _random = random ?? new Random();
        _zone = new ShrinkingQteZone(initialSpan: MathF.PI / 4f, duration: 3.2f, random: _random);
        ResetQte();
    }

    public void ResetQte()
    {
        _zone.SpawnSlots(AllCareActions);
        _qteTime = 0f;
        _teleportFxTimer = 0f;
        _teleportGraceTimer = 0f;
        _teleportCount = 0;
        _hasPreparedDestination = false;
        _teleportTargetAngle = 0f;
        _nextTeleportTime = 1.3f + (float)_random.NextDouble() * 0.2f;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _zone.Update(dt);
        _qteTime += dt;
        _teleportGraceTimer = MathF.Max(0f, _teleportGraceTimer - dt);
        _teleportFxTimer = MathF.Max(0f, _teleportFxTimer - dt);

        PetDefinition pet = PetCatalog.Get(_context.Run.ActivePet);
        if (pet.Pattern.HasTeleportingMarker)
        {
            if (_teleportCount < MaxTeleportsPerCycle && !_zone.IsExpired)
            {
                if (_qteTime >= _nextTeleportTime - TelegraphDuration)
                {
                    if (!_hasPreparedDestination)
                    {
                        if (IsNeedleInOrApproachingPreferredSlot(pet.Pattern.PreferredAction))
                        {
                            _nextTeleportTime += 0.8f;
                        }
                        else
                        {
                            float timeUntilWarp = MathF.Max(0f, _nextTeleportTime - _qteTime);
                            float needleAtWarp = (_zone.NeedleAngle + _zone.NeedleSpeed * timeUntilWarp) % ShrinkingQteZone.Tau;
                            if (needleAtWarp < 0f) needleAtWarp += ShrinkingQteZone.Tau;

                            _teleportTargetAngle = CalculateFairLandingAngle(needleAtWarp, timeUntilWarp);
                            _hasPreparedDestination = true;
                        }
                    }
                }

                if (_qteTime >= _nextTeleportTime)
                {
                    if (IsNeedleInOrApproachingPreferredSlot(pet.Pattern.PreferredAction))
                    {
                        _nextTeleportTime = _qteTime + 0.8f;
                        _hasPreparedDestination = false;
                    }
                    else
                    {
                        TriggerTeleport();
                    }
                }
            }
        }

        if (_zone.IsExpired)
        {
            ResetQte();
            _context.Message = "The timing window expired. A new opportunity appeared.";
            return;
        }

        if (_context.IsKeyPressed(Keys.Space))
        {
            _context.Audio.PlayConfirm();
            ResolveCare();
        }
    }

    public bool IsNeedleInOrApproachingPreferredSlot(CareAction preferredAction)
    {
        foreach (var slot in _zone.Slots)
        {
            if (slot.Action == preferredAction && slot.CurrentSpan > 0f)
            {
                float dist = ShrinkingQteZone.Wrap(slot.CenterAngle, _zone.NeedleAngle);
                float halfSpan = slot.CurrentSpan / 2f;
                if (dist >= -halfSpan && dist <= halfSpan + 0.25f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public float CalculateFairLandingAngle(float? referenceNeedleAngle = null, float? timeUntilWarp = null)
    {
        float needleRef = referenceNeedleAngle ?? _zone.NeedleAngle;
        float warpTime = _zone.ElapsedTime + (timeUntilWarp ?? 0f);
        PetDefinition activePet = PetCatalog.Get(_context.Run.ActivePet);

        float runway = 1.1f + (float)_random.NextDouble() * 0.4f;
        float minDisplacement = MathF.PI * 0.5f + 0.10f;

        bool IsEligible(QteSlot s) =>
            !s.IsFinished(warpTime) && (warpTime >= s.AppearTime || s.CurrentSpan > 0f);
        // 1. Preferred action slot if eligible and provides >= minDisplacement
        foreach (var slot in _zone.Slots)
        {
            if (slot.Action == activePet.Pattern.PreferredAction && IsEligible(slot))
            {
                float candidate = (slot.CenterAngle - runway) % ShrinkingQteZone.Tau;
                if (candidate < 0f) candidate += ShrinkingQteZone.Tau;
                if (MathF.Abs(ShrinkingQteZone.Wrap(candidate, needleRef)) >= minDisplacement)
                {
                    return candidate;
                }
            }
        }

        // 2. Any eligible slot that provides >= minDisplacement
        foreach (var slot in _zone.Slots)
        {
            if (IsEligible(slot))
            {
                float candidate = (slot.CenterAngle - runway) % ShrinkingQteZone.Tau;
                if (candidate < 0f) candidate += ShrinkingQteZone.Tau;
                if (MathF.Abs(ShrinkingQteZone.Wrap(candidate, needleRef)) >= minDisplacement)
                {
                    return candidate;
                }
            }
        }

        // 3. Fallback among any eligible slots maximizing displacement
        float maxDisp = -1f;
        float bestCandidate = 0f;
        bool foundEligible = false;

        foreach (var slot in _zone.Slots)
        {
            if (IsEligible(slot))
            {
                float candidate = (slot.CenterAngle - runway) % ShrinkingQteZone.Tau;
                if (candidate < 0f) candidate += ShrinkingQteZone.Tau;
                float disp = MathF.Abs(ShrinkingQteZone.Wrap(candidate, needleRef));
                if (disp > maxDisp)
                {
                    maxDisp = disp;
                    bestCandidate = candidate;
                    foundEligible = true;
                }
            }
        }

        if (foundEligible && maxDisp >= MathF.PI * 0.5f - 0.05f)
        {
            return bestCandidate;
        }

        // 4. Ultimate fallback: opposite needleRef (180 deg, PI displacement)
        float fallback = (needleRef + MathF.PI) % ShrinkingQteZone.Tau;
        if (fallback < 0f) fallback += ShrinkingQteZone.Tau;
        return fallback;
    }

    private void TriggerTeleport()
    {
        _lastTeleportAngle = _zone.NeedleAngle;
        _zone.NeedleAngle = _teleportTargetAngle;
        _teleportFxTimer = 0.35f;
        _teleportGraceTimer = GracePeriodDuration;
        _teleportCount++;
        _hasPreparedDestination = false;

        _context.Audio.PlayTeleport();
        _context.TriggerShake(0.18f, 5f);
        _context.SetPetReaction(_context.PetAngry, 0.45f);
        _context.SpawnTag("WARP!", new Color(45, 18, 55), new Color(230, 160, 255));
        _context.Message = "Blinkbun warped the wheel marker!";

        if (_teleportCount < MaxTeleportsPerCycle)
        {
            _nextTeleportTime = 3.4f + (float)_random.NextDouble() * 0.4f;
        }
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
            if (_teleportGraceTimer > 0f)
            {
                _context.SpawnTag("WARP DEFLECTED!", new Color(45, 25, 55), new Color(230, 160, 255));
                _context.Message = "The marker warped as you pressed! Deflected safely.";
                return;
            }
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

        // 4. Rotating Needle Marker (with telegraph pulse and ghost destination preview)
        Color needleColor = Color.White;
        Color pipColor = Color.Gold;

        if (IsTelegraphing)
        {
            float progress = Math.Clamp(1f - (_nextTeleportTime - _qteTime) / TelegraphDuration, 0f, 1f);
            float pulseRate = MathHelper.Lerp(15f, 35f, progress);
            float pulse = (MathF.Sin(_qteTime * pulseRate) + 1f) * 0.5f;
            needleColor = Color.Lerp(Color.White, new Color(235, 130, 255), pulse);
            pipColor = Color.Lerp(Color.Gold, new Color(255, 100, 230), pulse);

            // Ghost destination preview marker
            float ghostAlpha = MathHelper.Lerp(0.35f, 0.85f, progress);
            Vector2 ghostIn = c + Dir(_teleportTargetAngle) * (trackRadius - 20f);
            Vector2 ghostOut = c + Dir(_teleportTargetAngle) * (trackRadius + 22f);
            spriteBatch.DrawLine(ghostIn, ghostOut, new Color(200, 100, 255) * ghostAlpha, 5f);
            spriteBatch.DrawCircle(ghostOut, 5f, 16, new Color(255, 150, 255) * ghostAlpha, 2f);

            // Dynamic warp trajectory line from needle to target
            spriteBatch.DrawLine(c + Dir(_zone.NeedleAngle) * trackRadius, c + Dir(_teleportTargetAngle) * trackRadius, new Color(220, 130, 255) * (ghostAlpha * 0.4f), 2f);
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
