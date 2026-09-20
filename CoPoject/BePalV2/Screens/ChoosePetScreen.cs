using BePalV2.Audio;
using BePalV2.Gameplay;
using BePalV2.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePalV2.Screens;

public sealed class ChoosePetScreen : IScreen
{
    private readonly ScreenContext _ctx;
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;

    private bool _showingIntroCutscene;
    private int _selectedPetIndex = 0; // 0 = Coco (Red), 1 = Gloomtail (Log), 2 = Sproutlet (Green)

    private readonly Rectangle _continueBtn = new(490, 530, 300, 52);

    private readonly Rectangle _card1 = new(120, 145, 320, 460);
    private readonly Rectangle _card2 = new(480, 145, 320, 460);
    private readonly Rectangle _card3 = new(840, 145, 320, 460);

    private readonly Rectangle _confirmChoiceBtn = new(480, 625, 320, 52);

    public ChoosePetScreen(ScreenContext ctx, bool showIntro = true)
    {
        _ctx = ctx;
        _showingIntroCutscene = showIntro;
    }

    public void Update(GameTime gameTime)
    {
        var kbd = Keyboard.GetState();
        var mouse = Mouse.GetState();
        Point mPos = mouse.Position;
        bool click = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;

        if (_showingIntroCutscene)
        {
            if ((kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space)) ||
                (kbd.IsKeyDown(Keys.Enter) && !_prevKeyboard.IsKeyDown(Keys.Enter)))
            {
                _showingIntroCutscene = false;
                _ctx.Audio.PlayConfirm();
            }

            if (click && _continueBtn.Contains(mPos))
            {
                _showingIntroCutscene = false;
                _ctx.Audio.PlayConfirm();
            }
        }
        else
        {
            // Keyboard selection
            if (kbd.IsKeyDown(Keys.D1) || kbd.IsKeyDown(Keys.NumPad1))
            {
                if (_selectedPetIndex != 0) { _selectedPetIndex = 0; _ctx.Audio.PlayConfirm(); }
            }
            else if (kbd.IsKeyDown(Keys.D2) || kbd.IsKeyDown(Keys.NumPad2))
            {
                if (_selectedPetIndex != 1) { _selectedPetIndex = 1; _ctx.Audio.PlayConfirm(); }
            }
            else if (kbd.IsKeyDown(Keys.D3) || kbd.IsKeyDown(Keys.NumPad3))
            {
                if (_selectedPetIndex != 2) { _selectedPetIndex = 2; _ctx.Audio.PlayConfirm(); }
            }
            else if (kbd.IsKeyDown(Keys.Left) && !_prevKeyboard.IsKeyDown(Keys.Left))
            {
                _selectedPetIndex = (_selectedPetIndex + 2) % 3;
                _ctx.Audio.PlayConfirm();
            }
            else if (kbd.IsKeyDown(Keys.Right) && !_prevKeyboard.IsKeyDown(Keys.Right))
            {
                _selectedPetIndex = (_selectedPetIndex + 1) % 3;
                _ctx.Audio.PlayConfirm();
            }
            else if ((kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space)) ||
                     (kbd.IsKeyDown(Keys.Enter) && !_prevKeyboard.IsKeyDown(Keys.Enter)))
            {
                ConfirmSelection();
            }

            // Mouse selection
            if (click)
            {
                if (_card1.Contains(mPos))
                {
                    _selectedPetIndex = 0;
                    _ctx.Audio.PlayConfirm();
                }
                else if (_card2.Contains(mPos))
                {
                    _selectedPetIndex = 1;
                    _ctx.Audio.PlayConfirm();
                }
                else if (_card3.Contains(mPos))
                {
                    _selectedPetIndex = 2;
                    _ctx.Audio.PlayConfirm();
                }
                else if (_confirmChoiceBtn.Contains(mPos))
                {
                    ConfirmSelection();
                }
            }
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void ConfirmSelection()
    {
        _ctx.Audio.PlayConfirm();

        PetSpecies chosen = _selectedPetIndex switch
        {
            0 => PetSpecies.Coco,
            1 => PetSpecies.Gloomtail,
            2 => PetSpecies.Sproutlet,
            _ => PetSpecies.Coco
        };

        _ctx.Run = new V2RunState(chosen);
        _ctx.ScreenManager.SetScreen(new BaseHabitatScreen(_ctx));
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), UITheme.BgDeep);

        Point mPos = Mouse.GetState().Position;

        if (_showingIntroCutscene)
        {
            DrawIntroCutscene(batch, mPos);
        }
        else
        {
            DrawPetSelection(batch, mPos);
        }
    }

    private void DrawIntroCutscene(SpriteBatch batch, Point mPos)
    {
        Rectangle card = new(220, 110, 840, 500);
        CleanUI.DrawPanel(batch, card, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(card.X, card.Y, card.Width, 3), UITheme.AccentGold);

        // Facility Intake Banner
        string introHeader = "FACILITY INTAKE MANIFEST - DAY 1";
        batch.DrawString(_ctx.Font, introHeader, new Vector2(card.X + 50, card.Y + 36), UITheme.AccentGold, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0f);

        string classification = "CONFIDENTIAL RESEARCH SPECIMENS - CLEARANCE LEVEL 1";
        batch.DrawString(_ctx.Font, classification, new Vector2(card.X + 50, card.Y + 76), UITheme.TextMuted);

        batch.DrawLine(card.X + 50, card.Y + 104, card.Right - 50, card.Y + 104, UITheme.BorderSubtle, 1f);

        string[] story =
        {
            "Unidentified abnormal specimens have been delivered to the daycare containment facility.",
            "Our research division requires a dedicated handler to nurture, monitor, and train them.",
            "Each creature exhibits unique behavioral triggers, dietary needs, and defense reflexes.",
            "Care routines and interaction profiles must be deduced through empirical observation.",
            "Select your initial specimen assignment to initiate Shelter Operations."
        };

        int y = card.Y + 130;
        for (int i = 0; i < story.Length; i++)
        {
            Rectangle dot = new(card.X + 54, y + 6, 6, 6);
            batch.FillRectangle(dot, UITheme.AccentGold);
            batch.DrawString(_ctx.Font, story[i], new Vector2(card.X + 72, y), UITheme.TextSecondary, 0f, Vector2.Zero, 1.05f, SpriteEffects.None, 0f);
            y += 42;
        }

        // Continue Button
        CleanUI.DrawButton(batch, _ctx.Font, _continueBtn, "ENTER SHELTER", _continueBtn.Contains(mPos), accent: UITheme.AccentGold, hotkey: "[ SPACE ]", isPrimary: true);

        string hint = "Press [ SPACEBAR ] or Click to Continue";
        Vector2 hintSize = _ctx.Font.MeasureString(hint);
        batch.DrawString(_ctx.Font, hint, new Vector2(card.Center.X - hintSize.X / 2f, card.Bottom - 32), UITheme.TextMuted);
    }

    private void DrawPetSelection(SpriteBatch batch, Point mPos)
    {
        // Header
        string title = "SPECIMEN ASSIGNMENT";
        Vector2 titleSize = _ctx.Font.MeasureString(title);
        batch.DrawString(_ctx.Font, title, new Vector2(640 - (titleSize.X * 1.6f) / 2f, 38), UITheme.TextPrimary, 0f, Vector2.Zero, 1.6f, SpriteEffects.None, 0f);

        string subtitle = "Select your primary abnormal companion to begin shelter caretaking";
        Vector2 subSize = _ctx.Font.MeasureString(subtitle);
        batch.DrawString(_ctx.Font, subtitle, new Vector2(640 - subSize.X / 2f, 86), UITheme.TextSecondary);

        // Card 1: Red / Coco
        DrawSpecimenCard(batch, _card1,
            title: "Coco",
            speciesCode: "SPECIMEN #01",
            element: "Fire",
            elementColor: UITheme.AccentCoral,
            hp: 100, stomach: 80, clean: 75,
            traitName: "Thermal Surge",
            traitDesc: "Overnight warmth heal when Clean > 70.",
            accent: UITheme.AccentCoral,
            isSelected: _selectedPetIndex == 0,
            isHovered: _card1.Contains(mPos),
            hotkey: "[ 1 ]");

        // Card 2: Log / Gloomtail
        DrawSpecimenCard(batch, _card2,
            title: "Gloomtail",
            speciesCode: "SPECIMEN #02",
            element: "Void",
            elementColor: UITheme.AccentPurple,
            hp: 110, stomach: 90, clean: 60,
            traitName: "Dark Resonance",
            traitDesc: "-25% dodge damage, tough hide vs hazards.",
            accent: UITheme.AccentPurple,
            isSelected: _selectedPetIndex == 1,
            isHovered: _card2.Contains(mPos),
            hotkey: "[ 2 ]");

        // Card 3: Green / Sproutlet
        DrawSpecimenCard(batch, _card3,
            title: "Sproutlet",
            speciesCode: "SPECIMEN #03",
            element: "Flora",
            elementColor: UITheme.AccentEmerald,
            hp: 90, stomach: 70, clean: 90,
            traitName: "Chlorophyll Flow",
            traitDesc: "QTE success timing window expanded by +15%.",
            accent: UITheme.AccentEmerald,
            isSelected: _selectedPetIndex == 2,
            isHovered: _card3.Contains(mPos),
            hotkey: "[ 3 ]");

        // Bottom "Confirm Adoption" action button
        CleanUI.DrawButton(batch, _ctx.Font, _confirmChoiceBtn, "CONFIRM SPECIMEN", _confirmChoiceBtn.Contains(mPos), accent: UITheme.AccentGold, hotkey: "[ SPACE ]", isPrimary: true);
    }

    private void DrawSpecimenCard(
        SpriteBatch batch,
        Rectangle card,
        string title,
        string speciesCode,
        string element,
        Color elementColor,
        int hp, int stomach, int clean,
        string traitName,
        string traitDesc,
        Color accent,
        bool isSelected,
        bool isHovered,
        string hotkey)
    {
        Rectangle drawCard = isSelected ? new(card.X, card.Y - 4, card.Width, card.Height) : card;

        Color bg = isSelected ? UITheme.BgPanelHover : (isHovered ? new Color(26, 30, 40) : UITheme.BgPanel);
        Color border = isSelected ? UITheme.AccentGold : (isHovered ? UITheme.BorderHighlight : UITheme.BorderSubtle);

        CleanUI.DrawPanel(batch, drawCard, bg, border, borderWidth: isSelected ? 2 : 1, shadow: true);

        // Header Accent Strip
        batch.FillRectangle(new Rectangle(drawCard.X, drawCard.Y, drawCard.Width, 3), isSelected ? UITheme.AccentGold : accent);

        // Top Row: Code & Hotkey + Element Pill
        batch.DrawString(_ctx.Font, $"{hotkey}  {speciesCode}", new Vector2(drawCard.X + 16, drawCard.Y + 14), UITheme.TextMuted);

        Rectangle elemBadge = new(drawCard.Right - 76, drawCard.Y + 12, 60, 20);
        CleanUI.DrawBadge(batch, _ctx.Font, elemBadge, element, elementColor * 0.25f, elementColor);

        // Name
        batch.DrawString(_ctx.Font, title, new Vector2(drawCard.X + 16, drawCard.Y + 36), UITheme.TextPrimary, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

        // Pet Texture / Avatar box with recessed background
        Rectangle iconRect = new(drawCard.Center.X - 48, drawCard.Y + 70, 96, 96);
        CleanUI.DrawPanel(batch, iconRect, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);

        // Floor shadow ellipse
        batch.FillRectangle(new Rectangle(iconRect.X + 16, iconRect.Bottom - 10, 64, 6), Color.Black * 0.35f);

        if (_ctx.PetIdleTex != null)
        {
            batch.Draw(_ctx.PetIdleTex, new Rectangle(iconRect.X + 8, iconRect.Y + 8, 80, 80), Color.White);
        }

        // Stats with clean micro progress bars
        int sy = drawCard.Y + 180;
        int barW = drawCard.Width - 32;

        CleanUI.DrawProgressBar(batch, _ctx.Font, new Rectangle(drawCard.X + 16, sy, barW, 20), hp / 120f, UITheme.AccentCoral, leftText: "HP", rightText: $"{hp}");
        CleanUI.DrawProgressBar(batch, _ctx.Font, new Rectangle(drawCard.X + 16, sy + 26, barW, 20), stomach / 100f, UITheme.AccentGold, leftText: "STOMACH", rightText: $"{stomach}%");
        CleanUI.DrawProgressBar(batch, _ctx.Font, new Rectangle(drawCard.X + 16, sy + 52, barW, 20), clean / 100f, UITheme.AccentCyan, leftText: "CLEAN", rightText: $"{clean}%");

        // Special Trait Box (Recessed)
        Rectangle traitBox = new(drawCard.X + 16, sy + 82, barW, 86);
        CleanUI.DrawPanel(batch, traitBox, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);

        batch.DrawString(_ctx.Font, $"TRAIT: {traitName}", new Vector2(traitBox.X + 10, traitBox.Y + 8), UITheme.AccentGold);

        // Trait description wrapping
        batch.DrawString(_ctx.Font, traitDesc, new Vector2(traitBox.X + 10, traitBox.Y + 32), UITheme.TextSecondary);

        // Selection badge
        if (isSelected)
        {
            Rectangle selBadge = new(drawCard.X + 16, drawCard.Bottom - 38, barW, 26);
            CleanUI.DrawBadge(batch, _ctx.Font, selBadge, "SELECTED", UITheme.AccentGold, Color.Black);
        }
        else
        {
            Vector2 clickPrompt = _ctx.Font.MeasureString("Click to Select");
            batch.DrawString(_ctx.Font, "Click to Select", new Vector2(drawCard.Center.X - clickPrompt.X / 2f, drawCard.Bottom - 32), UITheme.TextMuted);
        }
    }
}
