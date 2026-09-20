using BePalV2.Audio;
using BePalV2.Gameplay;
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

    private readonly Rectangle _cardCoco = new(100, 180, 340, 460);
    private readonly Rectangle _cardSproutlet = new(470, 180, 340, 460);
    private readonly Rectangle _cardGloomtail = new(840, 180, 340, 460);

    public ChoosePetScreen(ScreenContext ctx)
    {
        _ctx = ctx;
    }

    public void Update(GameTime gameTime)
    {
        var kbd = Keyboard.GetState();
        var mouse = Mouse.GetState();
        Point mPos = mouse.Position;
        bool click = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;

        if (kbd.IsKeyDown(Keys.D1) && !_prevKeyboard.IsKeyDown(Keys.D1)) SelectStarter(PetSpecies.Coco);
        else if (kbd.IsKeyDown(Keys.D2) && !_prevKeyboard.IsKeyDown(Keys.D2)) SelectStarter(PetSpecies.Sproutlet);
        else if (kbd.IsKeyDown(Keys.D3) && !_prevKeyboard.IsKeyDown(Keys.D3)) SelectStarter(PetSpecies.Gloomtail);

        if (click)
        {
            if (_cardCoco.Contains(mPos)) SelectStarter(PetSpecies.Coco);
            else if (_cardSproutlet.Contains(mPos)) SelectStarter(PetSpecies.Sproutlet);
            else if (_cardGloomtail.Contains(mPos)) SelectStarter(PetSpecies.Gloomtail);
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void SelectStarter(PetSpecies species)
    {
        _ctx.Audio.PlayConfirm();
        _ctx.Run = new V2RunState(species);
        _ctx.ScreenManager.SetScreen(new BaseHabitatScreen(_ctx));
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        // Dark background matching Slide 9
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), new Color(24, 24, 26));

        // Header: "Choose your pet" (exact text from Slide 9/11)
        string title = "Choose your pet";
        Vector2 titleSize = _ctx.Font.MeasureString(title);
        batch.DrawString(_ctx.Font, title, new Vector2(640 - titleSize.X * 1.5f, 70), Color.White, 0f, Vector2.Zero, 3.0f, SpriteEffects.None, 0f);

        string hint = "Select one abnormal specimen to observe, shelter, and rehabilitate:";
        Vector2 hSize = _ctx.Font.MeasureString(hint);
        batch.DrawString(_ctx.Font, hint, new Vector2(640 - hSize.X / 2f, 135), new Color(160, 160, 160));

        Point mPos = Mouse.GetState().Position;

        // Card 1: Coco (Green RGB(93, 176, 70))
        DrawPetCard(batch, _cardCoco, "COCO (MOSSLING)",
            hp: 100, stomach: 80, clean: 70,
            passive: "Photosynthesis\nOvernight +5 HP heal when Clean > 80.",
            preferred: "Care Preference: Feed & Clean",
            themeColor: new Color(93, 176, 70),
            isHovered: _cardCoco.Contains(mPos),
            hotkey: "[ 1 ]");

        // Card 2: Sproutlet (Amber RGB(255, 222, 89))
        DrawPetCard(batch, _cardSproutlet, "SPROUTLET",
            hp: 90, stomach: 70, clean: 80,
            passive: "Agile Reflex\nTrain Perfect zone widened by +15%.",
            preferred: "Care Preference: Train & Clean",
            themeColor: new Color(255, 222, 89),
            isHovered: _cardSproutlet.Contains(mPos),
            hotkey: "[ 2 ]");

        // Card 3: Gloomtail (Crimson RGB(208, 38, 54))
        DrawPetCard(batch, _cardGloomtail, "GLOOMTAIL",
            hp: 110, stomach: 60, clean: 60,
            passive: "Shadow Barrier\n-25% dodge damage, -50% boss miss dmg.",
            preferred: "Care Preference: Heal & Feed",
            themeColor: new Color(208, 38, 54),
            isHovered: _cardGloomtail.Contains(mPos),
            hotkey: "[ 3 ]");

        string footer = "Press [ 1 ], [ 2 ], or [ 3 ] or Click a card to confirm your adoption";
        Vector2 fSize = _ctx.Font.MeasureString(footer);
        batch.DrawString(_ctx.Font, footer, new Vector2(640 - fSize.X / 2f, 665), new Color(130, 130, 130));
    }

    private void DrawPetCard(SpriteBatch batch, Rectangle card, string name, int hp, int stomach, int clean, string passive, string preferred, Color themeColor, bool isHovered, string hotkey)
    {
        Color bg = isHovered ? new Color(34, 34, 38) : new Color(28, 28, 30);
        batch.FillRectangle(card, bg);
        batch.DrawRectangle(card, isHovered ? Color.White : themeColor, isHovered ? 3 : 2);

        // Header Banner with theme color
        Rectangle header = new(card.X, card.Y, card.Width, 50);
        batch.FillRectangle(header, themeColor * 0.35f);
        batch.DrawString(_ctx.Font, $"{hotkey} {name}", new Vector2(card.X + 16, card.Y + 14), Color.White);

        // Pet Icon / Silhouette
        Rectangle iconRect = new(card.Center.X - 50, card.Y + 70, 100, 100);
        batch.FillRectangle(iconRect, new Color(18, 18, 20));
        batch.DrawRectangle(iconRect, themeColor, 1);
        if (_ctx.PetIdleTex != null)
        {
            batch.Draw(_ctx.PetIdleTex, iconRect, Color.White);
        }

        // Stats
        int sy = card.Y + 190;
        batch.DrawString(_ctx.Font, $"Health (HP):    {hp}", new Vector2(card.X + 24, sy), new Color(240, 100, 100));
        batch.DrawString(_ctx.Font, $"Stomach:        {stomach}%", new Vector2(card.X + 24, sy + 32), new Color(255, 180, 60));
        batch.DrawString(_ctx.Font, $"Cleanliness:    {clean}%", new Vector2(card.X + 24, sy + 64), new Color(80, 200, 240));

        // Passive
        batch.DrawString(_ctx.Font, "Special Ability:", new Vector2(card.X + 24, sy + 110), Color.Gold);
        batch.DrawString(_ctx.Font, passive, new Vector2(card.X + 24, sy + 135), new Color(210, 210, 210));

        // Preference
        batch.DrawString(_ctx.Font, preferred, new Vector2(card.X + 24, sy + 195), themeColor);

        // Select Button
        Rectangle btn = new(card.X + 25, card.Bottom - 50, card.Width - 50, 36);
        batch.FillRectangle(btn, isHovered ? themeColor : themeColor * 0.75f);
        batch.DrawRectangle(btn, Color.White, 1);
        string selectText = "ADOPT SPECIMEN";
        Vector2 sSize = _ctx.Font.MeasureString(selectText);
        batch.DrawString(_ctx.Font, selectText, new Vector2(btn.Center.X - sSize.X / 2f, btn.Center.Y - sSize.Y / 2f), Color.White);
    }
}
