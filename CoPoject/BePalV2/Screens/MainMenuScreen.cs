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

    // Center Buttons matching canonical layout (1920x1080)
    private readonly Rectangle _titleBox = new(740, 160, 440, 130);
    private readonly Rectangle _playBtn = new(800, 470, 320, 60);
    private readonly Rectangle _optionBtn = new(800, 550, 320, 56);
    private readonly Rectangle _quitBtn = new(800, 630, 320, 56);
    private readonly Rectangle _closeHelpBtn = new(850, 880, 220, 48);

    // Left Spacebar keycap
    private readonly Rectangle _spacebarKeycap = new(240, 480, 260, 72);

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
            else if (click && _closeHelpBtn.Contains(mPos))
            {
                _showHelp = false;
                _ctx.Audio.PlayConfirm();
            }
        }
        else
        {
            if (kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space))
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
                // Quit facility
                _ctx.Audio.PlayConfirm();
                _ctx.ScreenManager.ExitGame();
            }
            else if (click)
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
                    _ctx.Audio.PlayConfirm();
                    _ctx.ScreenManager.ExitGame();
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
        for (int x = 0; x < _ctx.ScreenWidth; x += 80)
        {
            batch.DrawLine(x, 0, x, _ctx.ScreenHeight, gridColor, 1);
        }
        for (int y = 0; y < _ctx.ScreenHeight; y += 80)
        {
            batch.DrawLine(0, y, _ctx.ScreenWidth, y, gridColor, 1);
        }

        Point mPos = Mouse.GetState().Position;

        // --- LEFT WING: Prominent Spacebar Keycap ---
        bool spaceHovered = _spacebarKeycap.Contains(mPos);
        CleanUI.DrawKeycap(batch, _ctx.Font, _spacebarKeycap, "SPACEBAR", isPressed: spaceHovered, isAccent: true);

        string spaceSubtitle = "Press [ SPACE ] to Start";
        Vector2 spaceSubSize = _ctx.Font.MeasureString(spaceSubtitle);
        batch.DrawString(_ctx.Font, spaceSubtitle, new Vector2(_spacebarKeycap.Center.X - spaceSubSize.X / 2f, _spacebarKeycap.Bottom + 18), UITheme.TextSecondary);

        // --- CENTER COLUMN: Clean Minimalist Title & Menu Buttons ---
        CleanUI.DrawPanel(batch, _titleBox, UITheme.BgPanel, UITheme.BorderSubtle, borderWidth: 1);
        batch.FillRectangle(new Rectangle(_titleBox.X, _titleBox.Y, _titleBox.Width, 4), UITheme.AccentGold);

        string title = "BePal";
        Vector2 titleSize = _ctx.Font.MeasureString(title);
        float titleScale = 3.6f;
        Vector2 titlePos = new(_titleBox.Center.X - (titleSize.X * titleScale) / 2f, _titleBox.Center.Y - (titleSize.Y * titleScale) / 2f - 4);
        batch.DrawString(_ctx.Font, title, titlePos + new Vector2(2, 2), Color.Black * 0.6f, 0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, title, titlePos, UITheme.TextPrimary, 0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

        string subtitle = "Abnormal Creature Daycare & Research Facility";
        Vector2 subSize = _ctx.Font.MeasureString(subtitle);
        batch.DrawString(_ctx.Font, subtitle, new Vector2(960 - subSize.X / 2f, 320), UITheme.TextMuted);

        // Center Buttons
        CleanUI.DrawButton(batch, _ctx.Font, _playBtn, "START GAME", _playBtn.Contains(mPos), accent: UITheme.AccentGold, isPrimary: true);
        CleanUI.DrawButton(batch, _ctx.Font, _optionBtn, "SYSTEM GUIDE", _optionBtn.Contains(mPos), accent: UITheme.AccentCyan);
        CleanUI.DrawButton(batch, _ctx.Font, _quitBtn, "QUIT FACILITY", _quitBtn.Contains(mPos), accent: UITheme.AccentCoral);

        // --- RIGHT WING: WASD, Controls & Mouse Graphic ---
        // Generously positioned on the right wing without blocking any text
        CleanUI.DrawKeycap(batch, _ctx.Font, new Rectangle(1550, 360, 52, 52), "W");
        CleanUI.DrawKeycap(batch, _ctx.Font, new Rectangle(1490, 420, 52, 52), "A");
        CleanUI.DrawKeycap(batch, _ctx.Font, new Rectangle(1550, 420, 52, 52), "S");
        CleanUI.DrawKeycap(batch, _ctx.Font, new Rectangle(1610, 420, 52, 52), "D");

        // Computer Mouse Graphic
        Rectangle mouseRect = new(1542, 510, 68, 106);
        CleanUI.DrawPanel(batch, mouseRect, new Color(24, 28, 36), UITheme.BorderLight, borderWidth: 1);
        batch.FillRectangle(new Rectangle(mouseRect.X + 2, mouseRect.Y + 2, 30, 42), new Color(44, 52, 68));
        batch.DrawLine(mouseRect.X, mouseRect.Y + 44, mouseRect.Right, mouseRect.Y + 44, UITheme.BorderSubtle, 1);
        batch.DrawLine(mouseRect.X + 33, mouseRect.Y, mouseRect.X + 33, mouseRect.Y + 44, UITheme.BorderSubtle, 1);
        batch.FillRectangle(new Rectangle(mouseRect.X + 30, mouseRect.Y + 14, 8, 18), UITheme.AccentCyan * 0.8f);

        string interactText = "WASD / Mouse to Interact";
        Vector2 interactSize = _ctx.Font.MeasureString(interactText);
        batch.DrawString(_ctx.Font, interactText, new Vector2(1576 - interactSize.X / 2f, 636), UITheme.TextMuted);

        // --- FOOTER PROMPT ---
        string footer = "[ SPACE ] Play    *    [ O ] System Guide    *    [ Q ] Quit";
        Vector2 fSize = _ctx.Font.MeasureString(footer);
        batch.DrawString(_ctx.Font, footer, new Vector2(960 - fSize.X / 2f, 980), UITheme.TextMuted);

        // Help Modal Overlay
        if (_showHelp)
        {
            CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.85f);

            Rectangle modal = new(360, 120, 1200, 840);
            CleanUI.DrawPanel(batch, modal, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
            batch.FillRectangle(new Rectangle(modal.X, modal.Y, modal.Width, 4), UITheme.AccentCyan);

            batch.DrawString(_ctx.Font, "FACILITY SURVIVAL MANUAL - SYSTEM OVERVIEW", new Vector2(modal.X + 40, modal.Y + 36), UITheme.AccentGold, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 0f);
            batch.DrawLine(modal.X + 40, modal.Y + 80, modal.Right - 40, modal.Y + 80, UITheme.BorderSubtle, 1f);

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

            int ly = modal.Y + 110;
            float lineScale = 0.95f;
            for (int i = 0; i < lines.Length; i++)
            {
                Rectangle numBadge = new(modal.X + 40, ly + 2, 28, 28);
                CleanUI.DrawBadge(batch, _ctx.Font, numBadge, (i + 1).ToString(), new Color(34, 42, 56), UITheme.AccentCyan);

                string lineContent = lines[i].Substring(lines[i].IndexOf(':') + 1).Trim();
                string titlePart = lines[i].Substring(3, lines[i].IndexOf(':') - 3);
                batch.DrawString(_ctx.Font, titlePart + ":", new Vector2(modal.X + 80, ly + 4), UITheme.TextPrimary, 0f, Vector2.Zero, lineScale, SpriteEffects.None, 0f);
                Vector2 titlePartSize = _ctx.Font.MeasureString(titlePart + ":") * lineScale;
                batch.DrawString(_ctx.Font, lineContent, new Vector2(modal.X + 90 + titlePartSize.X, ly + 4), UITheme.TextSecondary, 0f, Vector2.Zero, lineScale, SpriteEffects.None, 0f);

                ly += 68;
            }

            CleanUI.DrawButton(batch, _ctx.Font, _closeHelpBtn, "CLOSE MANUAL", _closeHelpBtn.Contains(mPos), accent: UITheme.AccentCyan, hotkey: "[ ESC ]");
        }
    }
}
