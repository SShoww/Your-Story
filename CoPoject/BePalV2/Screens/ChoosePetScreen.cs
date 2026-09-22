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
    private int _selectedPetIndex = 0; // 0 = Mossling, 1 = BlinkBun, 2 = NibbleClaw

    private readonly Rectangle _continueBtn = new(760, 780, 400, 56);

    // 3 Cards spaced symmetrically in 1920x1080
    private readonly Rectangle _card1 = new(180, 160, 460, 710);
    private readonly Rectangle _card2 = new(730, 160, 460, 710);
    private readonly Rectangle _card3 = new(1280, 160, 460, 710);

    private readonly Rectangle _confirmChoiceBtn = new(760, 910, 400, 56);

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
            if (kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space))
            {
                _showingIntroCutscene = false;
                _ctx.Audio.PlayConfirm();
            }
            else if (click && _continueBtn.Contains(mPos))
            {
                _showingIntroCutscene = false;
                _ctx.Audio.PlayConfirm();
            }
        }
        else
        {
            // Number hotkeys 1, 2, 3
            if (kbd.IsKeyDown(Keys.D1) && !_prevKeyboard.IsKeyDown(Keys.D1))
            {
                _selectedPetIndex = 0;
                _ctx.Audio.PlayConfirm();
            }
            else if (kbd.IsKeyDown(Keys.D2) && !_prevKeyboard.IsKeyDown(Keys.D2))
            {
                _selectedPetIndex = 1;
                _ctx.Audio.PlayConfirm();
            }
            else if (kbd.IsKeyDown(Keys.D3) && !_prevKeyboard.IsKeyDown(Keys.D3))
            {
                _selectedPetIndex = 2;
                _ctx.Audio.PlayConfirm();
            }

            // Arrow keys
            if (kbd.IsKeyDown(Keys.Left) && !_prevKeyboard.IsKeyDown(Keys.Left))
            {
                _selectedPetIndex = (_selectedPetIndex + 2) % 3;
                _ctx.Audio.PlayConfirm();
            }
            else if (kbd.IsKeyDown(Keys.Right) && !_prevKeyboard.IsKeyDown(Keys.Right))
            {
                _selectedPetIndex = (_selectedPetIndex + 1) % 3;
                _ctx.Audio.PlayConfirm();
            }

            // Mouse card selection
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

            // Spacebar confirm
            if (kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space))
            {
                ConfirmSelection();
            }
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void ConfirmSelection()
    {
        _ctx.Audio.PlayConfirm();
        PetSpecies species = _selectedPetIndex switch
        {
            0 => PetSpecies.Coco,
            1 => PetSpecies.Gloomtail,
            2 => PetSpecies.Sproutlet,
            _ => PetSpecies.Coco
        };
        _ctx.Run = new V2RunState(species);
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
        Rectangle modal = new(460, 220, 1000, 620);
        CleanUI.DrawPanel(batch, modal, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(modal.X, modal.Y, modal.Width, 4), UITheme.AccentGold);

        batch.DrawString(_ctx.Font, "FACILITY ORIENTATION - SECTOR 7", new Vector2(modal.X + 40, modal.Y + 36), UITheme.AccentGold, 0f, Vector2.Zero, 1.35f, SpriteEffects.None, 0f);
        batch.DrawLine(modal.X + 40, modal.Y + 85, modal.Right - 40, modal.Y + 85, UITheme.BorderSubtle, 1f);

        string narrative =
            "Welcome, Overseer.\n\n" +
            "You have been assigned to Sanctuary Sector 7, a containment station for sentient\n" +
            "abnormal bio-forms known colloquially as 'Pals'.\n\n" +
            "Your shift spans 3 operational days. Each morning brings unexpected events,\n" +
            "doorstep encounters, and natural biological degradation.\n\n" +
            "Balance your daily 6 Energy Points (AP) wisely across care routines.\n" +
            "A violent thunderstorm approaches on Night 1. Prepare your companion.\n\n" +
            "Select your initial specimen to begin containment.";

        batch.DrawString(_ctx.Font, narrative, new Vector2(modal.X + 40, modal.Y + 120), UITheme.TextSecondary, 0f, Vector2.Zero, 1.05f, SpriteEffects.None, 0f);

        CleanUI.DrawButton(batch, _ctx.Font, _continueBtn, "PROCEED TO ASSIGNMENT", _continueBtn.Contains(mPos), accent: UITheme.AccentGold, hotkey: "[ SPACE ]", isPrimary: true);
    }

    private void DrawPetSelection(SpriteBatch batch, Point mPos)
    {
        // Header
        string title = "SPECIMEN ASSIGNMENT";
        Vector2 titleSize = _ctx.Font.MeasureString(title);
        batch.DrawString(_ctx.Font, title, new Vector2(960 - (titleSize.X * 1.6f) / 2f, 48), UITheme.TextPrimary, 0f, Vector2.Zero, 1.6f, SpriteEffects.None, 0f);

        string subtitle = "Select your primary abnormal companion to begin shelter caretaking";
        Vector2 subSize = _ctx.Font.MeasureString(subtitle);
        batch.DrawString(_ctx.Font, subtitle, new Vector2(960 - subSize.X / 2f, 106), UITheme.TextSecondary);

        // Card 1: Mossling
        DrawSpecimenCard(batch, _card1,
            title: "Mossling",
            speciesCode: "SPECIMEN #01",
            element: "Fire",
            elementColor: UITheme.AccentCoral,
            hp: 100, stomach: 80, clean: 75,
            traitName: "Thermal Surge",
            traitDesc: "Overnight warmth heal when Cleanliness > 70.",
            accent: UITheme.AccentCoral,
            isSelected: _selectedPetIndex == 0,
            isHovered: _card1.Contains(mPos),
            hotkey: "[ 1 ]");

        // Card 2: BlinkBun
        DrawSpecimenCard(batch, _card2,
            title: "BlinkBun",
            speciesCode: "SPECIMEN #02",
            element: "Void",
            elementColor: UITheme.AccentPurple,
            hp: 110, stomach: 90, clean: 60,
            traitName: "Dark Resonance",
            traitDesc: "-25% hazard damage, tough defensive skin.",
            accent: UITheme.AccentPurple,
            isSelected: _selectedPetIndex == 1,
            isHovered: _card2.Contains(mPos),
            hotkey: "[ 2 ]");

        // Card 3: NibbleClaw
        DrawSpecimenCard(batch, _card3,
            title: "NibbleClaw",
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
        batch.FillRectangle(new Rectangle(drawCard.X, drawCard.Y, drawCard.Width, 4), isSelected ? UITheme.AccentGold : accent);

        // Top Row: Code & Hotkey + Element Pill
        batch.DrawString(_ctx.Font, $"{hotkey}  {speciesCode}", new Vector2(drawCard.X + 20, drawCard.Y + 16), UITheme.TextMuted, 0f, Vector2.Zero, 0.95f, SpriteEffects.None, 0f);

        Rectangle elemBadge = new(drawCard.Right - 94, drawCard.Y + 14, 74, 26);
        CleanUI.DrawBadge(batch, _ctx.Font, elemBadge, element, elementColor * 0.25f, elementColor);

        // Name (Strictly: Mossling, BlinkBun, NibbleClaw)
        batch.DrawString(_ctx.Font, title, new Vector2(drawCard.X + 20, drawCard.Y + 48), UITheme.TextPrimary, 0f, Vector2.Zero, 1.35f, SpriteEffects.None, 0f);

        // Pet Texture / Avatar box - cleanly separated so it NEVER covers text
        Rectangle iconRect = new(drawCard.Center.X - 75, drawCard.Y + 98, 150, 150);
        CleanUI.DrawPanel(batch, iconRect, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);

        // Floor shadow ellipse inside box
        batch.FillRectangle(new Rectangle(iconRect.X + 25, iconRect.Bottom - 14, 100, 8), Color.Black * 0.35f);

        if (_ctx.PetIdleTex != null)
        {
            int sprH = 130;
            int sprW = (int)(sprH * (1298f / 1731f)); // 97px (maintain 3:4 portrait aspect ratio)
            Rectangle sprRect = new(iconRect.Center.X - sprW / 2, iconRect.Center.Y - sprH / 2, sprW, sprH);
            batch.Draw(_ctx.PetIdleTex, sprRect, Color.White);
        }

        // Stats with clean micro progress bars starting below the portrait
        int sy = drawCard.Y + 268;
        int barW = drawCard.Width - 40;

        CleanUI.DrawProgressBar(batch, _ctx.Font, new Rectangle(drawCard.X + 20, sy, barW, 24), hp / 120f, UITheme.AccentCoral, leftText: "HP", rightText: $"{hp}");
        CleanUI.DrawProgressBar(batch, _ctx.Font, new Rectangle(drawCard.X + 20, sy + 32, barW, 24), stomach / 100f, UITheme.AccentGold, leftText: "STOMACH", rightText: $"{stomach}%");
        CleanUI.DrawProgressBar(batch, _ctx.Font, new Rectangle(drawCard.X + 20, sy + 64, barW, 24), clean / 100f, UITheme.AccentCyan, leftText: "CLEAN", rightText: $"{clean}%");

        // Special Trait Box (Recessed)
        Rectangle traitBox = new(drawCard.X + 20, sy + 104, barW, 110);
        CleanUI.DrawPanel(batch, traitBox, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);

        batch.DrawString(_ctx.Font, $"TRAIT: {traitName}", new Vector2(traitBox.X + 16, traitBox.Y + 12), UITheme.AccentGold, 0f, Vector2.Zero, 1.05f, SpriteEffects.None, 0f);

        // Trait description wrapping neatly
        batch.DrawString(_ctx.Font, traitDesc, new Vector2(traitBox.X + 16, traitBox.Y + 44), UITheme.TextSecondary, 0f, Vector2.Zero, 0.90f, SpriteEffects.None, 0f);

        // Selection badge / button
        if (isSelected)
        {
            Rectangle selBadge = new(drawCard.X + 20, drawCard.Bottom - 54, barW, 36);
            CleanUI.DrawBadge(batch, _ctx.Font, selBadge, "SELECTED SPECIMEN", UITheme.AccentGold, Color.Black);
        }
        else
        {
            Vector2 clickPrompt = _ctx.Font.MeasureString("Click to Select");
            batch.DrawString(_ctx.Font, "Click to Select", new Vector2(drawCard.Center.X - clickPrompt.X / 2f, drawCard.Bottom - 44), UITheme.TextMuted);
        }
    }
}
