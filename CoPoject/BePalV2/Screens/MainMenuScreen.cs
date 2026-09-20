using BePalV2.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePalV2.Screens;

public sealed class MainMenuScreen : IScreen
{
    private readonly ScreenContext _ctx;
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;
    private bool _showHelp;

    // Stylized buttons matching Slide 4
    private readonly Rectangle _playBtn = new(460, 380, 360, 60);
    private readonly Rectangle _optionBtn = new(460, 460, 360, 50);
    private readonly Rectangle _quitBtn = new(460, 530, 360, 50);
    private readonly Rectangle _closeHelpBtn = new(540, 590, 200, 40);

    public MainMenuScreen(ScreenContext ctx)
    {
        _ctx = ctx;
    }

    public void Update(GameTime gameTime)
    {
        var kbd = Keyboard.GetState();
        var mouse = Mouse.GetState();
        Point mPos = mouse.Position;
        bool click = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;

        if (_showHelp)
        {
            if (kbd.IsKeyDown(Keys.Escape) && !_prevKeyboard.IsKeyDown(Keys.Escape))
            {
                _showHelp = false;
                _ctx.Audio.PlayConfirm();
            }
            if (click && _closeHelpBtn.Contains(mPos))
            {
                _showHelp = false;
                _ctx.Audio.PlayConfirm();
            }
        }
        else
        {
            if ((kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space)) ||
                (kbd.IsKeyDown(Keys.Enter) && !_prevKeyboard.IsKeyDown(Keys.Enter)))
            {
                StartNewGame();
            }
            else if (kbd.IsKeyDown(Keys.O) && !_prevKeyboard.IsKeyDown(Keys.O))
            {
                _showHelp = true;
                _ctx.Audio.PlayConfirm();
            }
            else if (kbd.IsKeyDown(Keys.Q) && !_prevKeyboard.IsKeyDown(Keys.Q))
            {
                Environment.Exit(0);
            }

            if (click)
            {
                if (_playBtn.Contains(mPos)) StartNewGame();
                else if (_optionBtn.Contains(mPos))
                {
                    _showHelp = true;
                    _ctx.Audio.PlayConfirm();
                }
                else if (_quitBtn.Contains(mPos))
                {
                    Environment.Exit(0);
                }
            }
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void StartNewGame()
    {
        _ctx.Audio.PlayConfirm();
        _ctx.ScreenManager.SetScreen(new ChoosePetScreen(_ctx));
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        // Dark charcoal background RGB(29, 29, 27) matching Slide 4
        Color bgCharcoal = new(29, 29, 27);
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), bgCharcoal);

        // Subtle tech grid lines
        for (int x = 0; x < _ctx.ScreenWidth; x += 60)
        {
            batch.DrawLine(x, 0, x, _ctx.ScreenHeight, new Color(42, 42, 40));
        }
        for (int y = 0; y < _ctx.ScreenHeight; y += 60)
        {
            batch.DrawLine(0, y, _ctx.ScreenWidth, y, new Color(42, 42, 40));
        }

        // Title "BePal" - massive bold minimalist font
        string title = "BePal";
        Vector2 titleSize = _ctx.Font.MeasureString(title);
        Vector2 titlePos = new(640 - titleSize.X * 1.8f, 150);
        batch.DrawString(_ctx.Font, title, titlePos + new Vector2(4, 4), Color.Black, 0f, Vector2.Zero, 3.6f, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, title, titlePos, Color.White, 0f, Vector2.Zero, 3.6f, SpriteEffects.None, 0f);

        string subtitle = "Abnormal Creature Daycare & Research Facility";
        Vector2 subSize = _ctx.Font.MeasureString(subtitle);
        batch.DrawString(_ctx.Font, subtitle, new Vector2(640 - subSize.X / 2f, 280), new Color(160, 160, 160));

        Point mPos = Mouse.GetState().Position;

        // Big Play Button with Spacebar Keycap (Slide 4 concept: "Spacebar Play / Space to Enter")
        bool playHovered = _playBtn.Contains(mPos);
        batch.FillRectangle(_playBtn, playHovered ? new Color(50, 50, 48) : new Color(38, 38, 36));
        batch.DrawRectangle(_playBtn, Color.White, playHovered ? 3 : 2);

        // Keycap icon
        Rectangle keyCap = new(_playBtn.X + 24, _playBtn.Y + 12, 110, 36);
        batch.FillRectangle(keyCap, Color.White);
        batch.DrawString(_ctx.Font, "SPACE", new Vector2(keyCap.X + 24, keyCap.Y + 8), Color.Black);

        batch.DrawString(_ctx.Font, "PLAY GAME", new Vector2(_playBtn.X + 160, _playBtn.Y + 18), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);

        // Option Button (Slide 4 concept)
        DrawMenuButton(batch, _optionBtn, "OPTION / GUIDE", _optionBtn.Contains(mPos));

        // Quit Button (Slide 4 concept)
        DrawMenuButton(batch, _quitBtn, "QUIT", _quitBtn.Contains(mPos));

        // Footer prompt matching Slide 4
        string footer = "[ SPACEBAR ] Play   |   [ O ] Option   |   [ Q ] Quit";
        Vector2 fSize = _ctx.Font.MeasureString(footer);
        batch.DrawString(_ctx.Font, footer, new Vector2(640 - fSize.X / 2f, 660), new Color(140, 140, 140));

        // Help Modal
        if (_showHelp)
        {
            Rectangle modal = new(240, 100, 800, 550);
            batch.FillRectangle(modal, new Color(20, 20, 20, 250));
            batch.DrawRectangle(modal, Color.White, 2);

            batch.DrawString(_ctx.Font, "NEW GDD 2.0 - GAME RULES & SYSTEM GUIDE", new Vector2(280, 130), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
            batch.DrawLine(280, 165, 1000, 165, Color.Gray, 1f);

            string[] lines =
            {
                "1. DAILY CYCLE: 4 Phases (Morning Briefing, Care Actions, Defense Resolution, Summary).",
                "2. 6 ENERGY POINTS (AP): Spend 1 AP on Train, Clean, Heal, or Feed.",
                "3. 10-ATTEMPT CARE QTE: Circular needle evaluation. Perfect (1.5 Pi) gives +150 pts.",
                "4. NATURAL DECAY: Stomach -20, Clean -15 each morning. Grimy/Starving causes damage.",
                "5. DAY 1 DISASTER: Thunderstorm panic emergency calming QTE.",
                "6. DAY 2 KNOCK KNOCK: Toothless acid monster encounter -> Chase or Tame into shelter.",
                "7. DAY 3 MERCHANT: Buyout dilemma (5,000G buyout vs 3-Phase Boss Battle).",
                "8. 500G REVIVE LOAN: 20% compound daily interest. Unpaid debt triggers foreclosure!"
            };

            int ly = 185;
            foreach (var l in lines)
            {
                batch.DrawString(_ctx.Font, l, new Vector2(280, ly), new Color(220, 220, 220));
                ly += 44;
            }

            batch.FillRectangle(_closeHelpBtn, new Color(50, 50, 50));
            batch.DrawRectangle(_closeHelpBtn, Color.White, 1);
            batch.DrawString(_ctx.Font, "CLOSE [ ESC ]", new Vector2(_closeHelpBtn.X + 42, _closeHelpBtn.Y + 10), Color.White);
        }
    }

    private void DrawMenuButton(SpriteBatch batch, Rectangle rect, string text, bool hovered)
    {
        batch.FillRectangle(rect, hovered ? new Color(50, 50, 48) : new Color(35, 35, 33));
        batch.DrawRectangle(rect, hovered ? Color.White : new Color(120, 120, 120), hovered ? 2 : 1);
        Vector2 size = _ctx.Font.MeasureString(text);
        Vector2 pos = new(rect.Center.X - size.X / 2f, rect.Center.Y - size.Y / 2f);
        batch.DrawString(_ctx.Font, text, pos, hovered ? Color.White : new Color(200, 200, 200));
    }
}
