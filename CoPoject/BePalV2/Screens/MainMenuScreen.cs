using BePalV2.Audio;
using BePalV2.UI;
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

    // Center Buttons matching canonical layout
    private readonly Rectangle _titleBox = new(460, 110, 360, 110);
    private readonly Rectangle _playBtn = new(520, 340, 240, 52);
    private readonly Rectangle _optionBtn = new(520, 410, 240, 50);
    private readonly Rectangle _quitBtn = new(520, 480, 240, 50);
    private readonly Rectangle _closeHelpBtn = new(530, 600, 220, 44);

    // Left Spacebar keycap
    private readonly Rectangle _spacebarKeycap = new(170, 320, 220, 64);

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
            if ((kbd.IsKeyDown(Keys.Escape) && !_prevKeyboard.IsKeyDown(Keys.Escape)) ||
                (kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space)))
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
                if (_playBtn.Contains(mPos) || _spacebarKeycap.Contains(mPos))
                {
                    StartNewGame();
                }
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
        // Dark atmospheric slate background
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), UITheme.BgDeep);

        // Subtle geometric grid
        Color gridColor = new(26, 30, 40, 120);
        for (int x = 0; x < _ctx.ScreenWidth; x += 64)
        {
            batch.DrawLine(x, 0, x, _ctx.ScreenHeight, gridColor, 1);
        }
        for (int y = 0; y < _ctx.ScreenHeight; y += 64)
        {
            batch.DrawLine(0, y, _ctx.ScreenWidth, y, gridColor, 1);
        }

        Point mPos = Mouse.GetState().Position;

        // --- LEFT WING: Prominent Spacebar Keycap ---
        bool spaceHovered = _spacebarKeycap.Contains(mPos);
        CleanUI.DrawKeycap(batch, _ctx.Font, _spacebarKeycap, "SPACE", isPressed: spaceHovered, isAccent: true);

        string spaceSubtitle = "Press [ SPACE ] to Start";
        Vector2 spaceSubSize = _ctx.Font.MeasureString(spaceSubtitle);
        batch.DrawString(_ctx.Font, spaceSubtitle, new Vector2(_spacebarKeycap.Center.X - spaceSubSize.X / 2f, _spacebarKeycap.Bottom + 16), UITheme.TextSecondary);

        // --- CENTER COLUMN: Clean Minimalist Title & Menu Buttons ---
        CleanUI.DrawPanel(batch, _titleBox, UITheme.BgPanel, UITheme.BorderSubtle, borderWidth: 1);
        // Accent line on top of title box
        batch.FillRectangle(new Rectangle(_titleBox.X, _titleBox.Y, _titleBox.Width, 3), UITheme.AccentGold);

        string title = "BePal";
        Vector2 titleSize = _ctx.Font.MeasureString(title);
        Vector2 titlePos = new(_titleBox.Center.X - (titleSize.X * 3.2f) / 2f, _titleBox.Center.Y - (titleSize.Y * 3.2f) / 2f - 4);
        batch.DrawString(_ctx.Font, title, titlePos + new Vector2(2, 2), Color.Black * 0.6f, 0f, Vector2.Zero, 3.2f, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, title, titlePos, UITheme.TextPrimary, 0f, Vector2.Zero, 3.2f, SpriteEffects.None, 0f);

        string subtitle = "Abnormal Creature Daycare & Research Facility";
        Vector2 subSize = _ctx.Font.MeasureString(subtitle);
        batch.DrawString(_ctx.Font, subtitle, new Vector2(640 - subSize.X / 2f, 240), UITheme.TextMuted);

        // Center Buttons
        CleanUI.DrawButton(batch, _ctx.Font, _playBtn, "START GAME", _playBtn.Contains(mPos), accent: UITheme.AccentGold, isPrimary: true);
        CleanUI.DrawButton(batch, _ctx.Font, _optionBtn, "SYSTEM GUIDE", _optionBtn.Contains(mPos), accent: UITheme.AccentCyan);
        CleanUI.DrawButton(batch, _ctx.Font, _quitBtn, "QUIT FACILITY", _quitBtn.Contains(mPos), accent: UITheme.AccentCoral);

        // --- RIGHT WING: WASD, Directional Cross & Mouse Graphic ---
        CleanUI.DrawKeycap(batch, _ctx.Font, new Rectangle(1050, 170, 44, 44), "W");
        CleanUI.DrawKeycap(batch, _ctx.Font, new Rectangle(1000, 222, 44, 44), "A");
        CleanUI.DrawKeycap(batch, _ctx.Font, new Rectangle(1050, 222, 44, 44), "S");
        CleanUI.DrawKeycap(batch, _ctx.Font, new Rectangle(1100, 222, 44, 44), "D");

        // Directional Cross / D-Pad
        CleanUI.DrawKeycap(batch, _ctx.Font, new Rectangle(1050, 290, 44, 44), "^");
        CleanUI.DrawKeycap(batch, _ctx.Font, new Rectangle(1000, 342, 44, 44), "<");
        CleanUI.DrawKeycap(batch, _ctx.Font, new Rectangle(1050, 342, 44, 44), "v");
        CleanUI.DrawKeycap(batch, _ctx.Font, new Rectangle(1100, 342, 44, 44), ">");

        // Computer Mouse Graphic with sleek slate styling
        Rectangle mouseRect = new(1042, 416, 60, 94);
        CleanUI.DrawPanel(batch, mouseRect, new Color(24, 28, 36), UITheme.BorderLight, borderWidth: 1);
        // Left button highlight
        batch.FillRectangle(new Rectangle(mouseRect.X + 2, mouseRect.Y + 2, 26, 36), new Color(44, 52, 68));
        // Mouse button horizontal divider
        batch.DrawLine(mouseRect.X, mouseRect.Y + 38, mouseRect.Right, mouseRect.Y + 38, UITheme.BorderSubtle, 1);
        // Vertical divider between left and right button
        batch.DrawLine(mouseRect.X + 29, mouseRect.Y, mouseRect.X + 29, mouseRect.Y + 38, UITheme.BorderSubtle, 1);
        // Scroll wheel pill
        batch.FillRectangle(new Rectangle(mouseRect.X + 26, mouseRect.Y + 12, 7, 16), UITheme.AccentCyan * 0.8f);

        string interactText = "WASD / Mouse to Interact";
        Vector2 interactSize = _ctx.Font.MeasureString(interactText);
        batch.DrawString(_ctx.Font, interactText, new Vector2(1072 - interactSize.X / 2f, 526), UITheme.TextMuted);

        // --- FOOTER PROMPT ---
        string footer = "[ SPACE ] Play    *    [ O ] System Guide    *    [ Q ] Quit";
        Vector2 fSize = _ctx.Font.MeasureString(footer);
        batch.DrawString(_ctx.Font, footer, new Vector2(640 - fSize.X / 2f, 666), UITheme.TextMuted);

        // Help Modal Overlay
        if (_showHelp)
        {
            CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.8f);

            Rectangle modal = new(220, 80, 840, 580);
            CleanUI.DrawPanel(batch, modal, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
            batch.FillRectangle(new Rectangle(modal.X, modal.Y, modal.Width, 3), UITheme.AccentCyan);

            batch.DrawString(_ctx.Font, "FACILITY SURVIVAL MANUAL - SYSTEM OVERVIEW", new Vector2(260, 108), UITheme.AccentGold, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);
            batch.DrawLine(260, 142, modal.Right - 40, 142, UITheme.BorderSubtle, 1f);

            string[] lines =
            {
                "1. DAILY CYCLE: 4 Phases (Morning Briefing, Care Actions, Defense Resolution, Summary).",
                "2. 6 ENERGY POINTS (AP): Spend 1 AP per Care Action (Train, Clean, Heal, or Feed).",
                "3. 10-ATTEMPT CARE QTE: Radial needle evaluation with preferred action timing.",
                "4. NATURAL DECAY: Stomach -20, Clean -15 each morning. Unattended pets panic!",
                "5. DAY 1 DISASTER: Thunderstorm panic emergency calming sequence.",
                "6. DAY 2 KNOCK KNOCK: Toothless creature encounter -> Chase or Tame into shelter.",
                "7. DAY 3 MERCHANT: Buyout dilemma (5,000G buyout vs 3-Phase Boss Battle).",
                "8. 500G EMERGENCY LOAN: 20% compound daily interest. Foreclosure upon default!"
            };

            int ly = 160;
            for (int i = 0; i < lines.Length; i++)
            {
                // Number badge
                Rectangle numBadge = new(260, ly + 2, 22, 22);
                CleanUI.DrawBadge(batch, _ctx.Font, numBadge, (i + 1).ToString(), new Color(34, 42, 56), UITheme.AccentCyan);

                // Line text
                string lineContent = lines[i].Substring(lines[i].IndexOf(':') + 1).Trim();
                string titlePart = lines[i].Substring(3, lines[i].IndexOf(':') - 3);
                batch.DrawString(_ctx.Font, titlePart + ":", new Vector2(290, ly + 4), UITheme.TextPrimary);
                Vector2 titlePartSize = _ctx.Font.MeasureString(titlePart + ":");
                batch.DrawString(_ctx.Font, lineContent, new Vector2(295 + titlePartSize.X, ly + 4), UITheme.TextSecondary);

                ly += 50;
            }

            CleanUI.DrawButton(batch, _ctx.Font, _closeHelpBtn, "CLOSE MANUAL", _closeHelpBtn.Contains(mPos), accent: UITheme.AccentCyan, hotkey: "[ ESC ]");
        }
    }
}
