#nullable enable
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePal.Screens;

/// <summary>
/// Handles the high-stakes reactive Dodge QTE mini-game with an evasive Dodge Zone.
/// </summary>
public sealed class DodgeQteScreen : IScreen
{
    public const float Tau = MathF.PI * 2;

    private readonly ScreenContext _context;
    private float _angle;

    public float Angle => _angle;

    public DodgeQteScreen(ScreenContext context)
    {
        _context = context;
        ResetQte();
    }

    public void ResetQte()
    {
        _angle = 0;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _angle = (_angle + 2.2f * dt) % Tau;

        if (_context.IsKeyPressed(Keys.Space))
        {
            ResolveDodge();
        }
    }

    public bool CheckDodgeSuccess()
    {
        float dodgeCenter = MathF.PI * 1.5f;
        float diff = MathF.Abs(Wrap(_angle, dodgeCenter));
        return diff <= MathF.PI / 6f;
    }

    public void ResolveDodge()
    {
        if (CheckDodgeSuccess())
        {
            _context.ResetPetReaction();
            _context.SpawnTag("DODGED!", new Color(50, 42, 12), new Color(255, 220, 80));
            _context.Message = "Attack evaded! One final care action remains.";
            _context.Manager.SetScreen(new CareQteScreen(_context));
        }
        else
        {
            _context.TriggerShake(0.35f, 13f);
            _context.SetPetReaction(_context.PetAngry, 0.75f);
            _context.SpawnTag("HIT BY ATTACK! -1 HP", new Color(65, 10, 15), new Color(255, 70, 70));
            _context.Manager.Fail("Failed to dodge attack! Lost 1 Health.");
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Vector2 c = new(640, 360);
        float trackRadius = 215f;

        // 1. Central Creature Display with Red Danger Alert
        spriteBatch.FillRectangle(new RectangleF(c.X - 110, c.Y - 145, 220, 290), new Color(14, 16, 24, 210));
        spriteBatch.Draw(_context.PetImage, new Rectangle((int)c.X - 90, (int)c.Y - 125, 180, 250), Color.White);
        spriteBatch.DrawRectangle(new RectangleF(c.X - 95, c.Y - 130, 190, 260), new Color(255, 65, 75), 3f);

        RectangleF alertRect = new(c.X - 80, c.Y - 26, 160, 48);
        spriteBatch.FillRectangle(alertRect, new Color(175, 25, 35));
        spriteBatch.DrawRectangle(alertRect, Color.White, 2f);
        _context.DrawCenterText(spriteBatch, "ATTACK!", new Vector2(c.X, c.Y - 2), 1.05f, Color.White);

        // 2. Circular Ring Track (Dodge Warning style)
        Color trackBg = new(55, 22, 30);
        Color trackBorder = new(115, 35, 45);
        spriteBatch.DrawCircle(c, trackRadius, 64, trackBg, 18f);
        spriteBatch.DrawCircle(c, trackRadius - 10f, 64, trackBorder, 2f);
        spriteBatch.DrawCircle(c, trackRadius + 10f, 64, trackBorder, 2f);

        // 3. Dodge Zone Arc and Scrap Badge
        float dodgeCenter = MathF.PI * 1.5f;
        float dodgeSpan = MathF.PI / 3f;
        bool inDodge = CheckDodgeSuccess();
        Color dodgeColor = inDodge ? Color.White : new Color(255, 215, 65);

        spriteBatch.DrawArc(c, trackRadius, dodgeCenter - dodgeSpan / 2f, dodgeSpan, 32, dodgeColor, inDodge ? 24f : 20f);

        Vector2 badgePos = c + Dir(dodgeCenter) * (trackRadius + 56f);
        _context.DrawScrapBadge(spriteBatch, badgePos, "DODGE ZONE", "SPACE TO EVADE", dodgeColor, Color.White, inDodge);

        // 4. Rotating Needle Marker
        Vector2 innerPt = c + Dir(_angle) * (trackRadius - 22f);
        Vector2 outerPt = c + Dir(_angle) * (trackRadius + 24f);
        spriteBatch.DrawLine(innerPt, outerPt, Color.White, 8f);
        spriteBatch.DrawCircle(outerPt, 4f, 16, Color.Gold, 2f);

        // 5. HUD & Status
        _context.DrawHUD(spriteBatch);

        Vector2 promptPos = new(640, 610);
        _context.DrawCenterText(spriteBatch, "PRESS SPACE INSIDE THE GOLD DODGE ZONE TO EVADE", promptPos, 0.85f, new Color(255, 215, 65));
        _context.DrawCenterText(spriteBatch, _context.Message, new Vector2(640, 655), 0.72f, new Color(255, 225, 165));

        // 6. Floating Tags
        _context.DrawFloatingTags(spriteBatch);
    }

    private static Vector2 Dir(float a) => new(MathF.Cos(a), MathF.Sin(a));

    private static float Wrap(float a, float b)
    {
        float difference = (a - b + MathF.PI) % Tau;
        return (difference < 0 ? difference + Tau : difference) - MathF.PI;
    }
}
