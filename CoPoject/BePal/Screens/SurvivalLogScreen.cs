#nullable enable
using BePal.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace BePal.Screens;

/// <summary>
/// Modal overlay screen presenting unlocked pet behavior research and action pattern notes.
/// </summary>
public sealed class SurvivalLogScreen : IScreen
{
    private static readonly Rectangle ModalBounds = new(130, 80, 1020, 570);
    private static readonly Rectangle CloseButtonRect = new(510, 565, 260, 60);

    private readonly ScreenContext _context;

    public bool IsOverlay => true;

    public SurvivalLogScreen(ScreenContext context)
    {
        _context = context;
    }

    public void Update(GameTime gameTime)
    {
        if (_context.IsButtonClicked(CloseButtonRect))
        {
            _context.Manager.PopScreen();
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_context.Pixel, ModalBounds, new Color(22, 25, 38));
        spriteBatch.DrawRectangle(new RectangleF(ModalBounds.X, ModalBounds.Y, ModalBounds.Width, ModalBounds.Height), new Color(255, 203, 139), 3f);

        _context.DrawCenterText(spriteBatch, "Survival Log", new Vector2(640, 125), 1.25f, new Color(255, 203, 139));

        DrawPetLog(spriteBatch, PetKind.Baseline, 200);
        DrawPetLog(spriteBatch, PetKind.Attacker, 315);
        DrawPetLog(spriteBatch, PetKind.Trickster, 430);

        _context.DrawButton(spriteBatch, CloseButtonRect, "Close", true);
    }

    private void DrawPetLog(SpriteBatch spriteBatch, PetKind pet, int y)
    {
        PetDefinition def = PetCatalog.Get(pet);
        bool unlocked = _context.Run.IsLogUnlocked(pet);
        string text = unlocked
            ? $"{def.Name}: prefers {def.Pattern.PreferredAction}. {def.Pattern.Description}"
            : $"{def.Name}: locked ({_context.Run.CompletedSessions(pet)} / 3 sessions)";

        _context.DrawText(spriteBatch, text, new Vector2(205, y), unlocked ? Color.White : Color.Gray);
    }
}
