#nullable enable
using BePal.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace BePal.Screens;

/// <summary>
/// Main pet-care shelter room screen displaying active creature status and day actions.
/// </summary>
public sealed class HomeScreen : IScreen
{
    private static readonly Rectangle PetFrame = new(530, 165, 220, 290);
    private static readonly Rectangle CareButtonRect = new(490, 480, 300, 48);
    private static readonly Rectangle EndDayButtonRect = new(50, 595, 250, 60);
    private static readonly Rectangle SurvivalLogButtonRect = new(980, 595, 250, 60);

    private readonly ScreenContext _context;

    public HomeScreen(ScreenContext context)
    {
        _context = context;
    }

    public void Update(GameTime gameTime)
    {
        if (_context.IsButtonClicked(PetFrame) || _context.IsButtonClicked(CareButtonRect))
        {
            _context.Manager.BeginCare();
        }
        else if (_context.IsButtonClicked(EndDayButtonRect) && _context.Run.CanEndDay)
        {
            _context.Run.EndDay();
            _context.Manager.AdvanceDay("You ended the day safely.");
        }
        else if (_context.IsButtonClicked(SurvivalLogButtonRect))
        {
            _context.Manager.ShowSurvivalLog();
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        _context.DrawHUD(spriteBatch);

        Vector2 c = new(640, 310);
        Rectangle frame = new((int)c.X - 110, (int)c.Y - 145, 220, 290);
        spriteBatch.FillRectangle(new RectangleF(frame.X, frame.Y, frame.Width, frame.Height), new Color(14, 16, 24, 210));
        spriteBatch.Draw(_context.PetImage, new Rectangle((int)c.X - 90, (int)c.Y - 125, 180, 250), Color.White);
        spriteBatch.DrawRectangle(new RectangleF(frame.X, frame.Y, frame.Width, frame.Height), new Color(255, 203, 139), 2f);

        PetDefinition activePet = PetCatalog.Get(_context.Run.ActivePet);
        _context.DrawButton(spriteBatch, CareButtonRect, $"CARE FOR {activePet.Name.ToUpperInvariant()}", true);

        _context.DrawCenterText(spriteBatch, _context.Message, new Vector2(640, 550), 0.76f, new Color(255, 225, 165));
        _context.DrawButton(spriteBatch, EndDayButtonRect, "End Day", _context.Run.CanEndDay);
        _context.DrawButton(spriteBatch, SurvivalLogButtonRect, "Survival Log", true);
    }
}
