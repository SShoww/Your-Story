#nullable enable
using BePal.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BePal.Screens;

/// <summary>
/// Debriefing screen at the end of a 5-day simulation cycle displaying run statistics.
/// </summary>
public sealed class SummaryScreen : IScreen
{
    private static readonly Rectangle BackToMenuRect = ScreenContext.ButtonRect(585);

    private readonly ScreenContext _context;

    public SummaryScreen(ScreenContext context)
    {
        _context = context;
    }

    public void Update(GameTime gameTime)
    {
        if (_context.IsButtonClicked(BackToMenuRect))
        {
            _context.Manager.ShowMenu();
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        _context.DrawCenterText(spriteBatch, "Run Summary", new Vector2(640, 115), 1.45f, new Color(255, 203, 139));
        _context.DrawText(spriteBatch, $"Forced Retreats: {_context.Run.ForcedRetreats}", new Vector2(420, 260), Color.White);
        _context.DrawText(spriteBatch, $"{PetCatalog.Get(PetKind.Baseline).Name} sessions: {_context.Run.CompletedSessions(PetKind.Baseline)}", new Vector2(420, 310), Color.White);
        _context.DrawText(spriteBatch, $"{PetCatalog.Get(PetKind.Attacker).Name} sessions: {_context.Run.CompletedSessions(PetKind.Attacker)}", new Vector2(420, 360), Color.White);
        _context.DrawText(spriteBatch, $"{PetCatalog.Get(PetKind.Trickster).Name} sessions: {_context.Run.CompletedSessions(PetKind.Trickster)}", new Vector2(420, 410), Color.White);
        _context.DrawButton(spriteBatch, BackToMenuRect, "Back to Menu", true);
    }
}
