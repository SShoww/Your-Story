#nullable enable
using BePal.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BePal.Screens;

/// <summary>
/// Handles the main menu and how-to-play help overlay views.
/// </summary>
public sealed class MainMenuScreen : IScreen
{
    private readonly ScreenContext _context;
    private bool _showHelp;

    public MainMenuScreen(ScreenContext context)
    {
        _context = context;
    }

    public void Update(GameTime gameTime)
    {
        if (_showHelp)
        {
            if (_context.IsButtonClicked(ScreenContext.ButtonRect(570)))
                _showHelp = false;
        }
        else
        {
            if (_context.IsButtonClicked(ScreenContext.ButtonRect(250)))
            {
                _context.Run = new PrototypeRun();
                _context.Manager.ShowHome("Day 1 begins. Click the pet to begin.");
            }
            else if (_context.IsButtonClicked(ScreenContext.ButtonRect(330)))
            {
                _showHelp = true;
            }
            else if (_context.IsButtonClicked(ScreenContext.ButtonRect(410)))
            {
                _context.ExitGame();
            }
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (_showHelp)
        {
            _context.DrawCenterText(spriteBatch, "How to Play", new Vector2(640, 105), 1.35f, new Color(255, 203, 139));
            _context.DrawText(spriteBatch, "Click the pet at home. Press Space on its preferred Care Action.", new Vector2(180, 220), Color.White);
            _context.DrawText(spriteBatch, "Attacks use a gold Dodge Zone. Three Care successes complete a session.", new Vector2(180, 280), Color.White);
            _context.DrawText(spriteBatch, "Three sessions unlock a Survival Log entry. Zero Health forces a retreat.", new Vector2(180, 340), Color.White);
            _context.DrawButton(spriteBatch, ScreenContext.ButtonRect(570), "Back", true);
        }
        else
        {
            _context.DrawCenterText(spriteBatch, "BePal", new Vector2(640, 120), 1.8f, new Color(255, 203, 139));
            _context.DrawCenterText(spriteBatch, "Pet-care timing prototype", new Vector2(640, 175), 0.85f, Color.LightGray);
            _context.DrawButton(spriteBatch, ScreenContext.ButtonRect(250), "Start Prototype", true);
            _context.DrawButton(spriteBatch, ScreenContext.ButtonRect(330), "How to Play", true);
            _context.DrawButton(spriteBatch, ScreenContext.ButtonRect(410), "Quit", true);
        }
    }
}
