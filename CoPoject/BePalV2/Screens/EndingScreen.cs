using BePalV2.Audio;
using BePalV2.Gameplay;
using BePalV2.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePalV2.Screens;

public sealed class EndingScreen : IScreen
{
    private readonly ScreenContext _ctx;
    private readonly StoryEnding _ending;
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;

    // 1920x1080 Layout Constants
    private readonly Rectangle _menuBtn = new(660, 880, 600, 60);

    public EndingScreen(ScreenContext ctx, StoryEnding ending)
    {
        _ctx = ctx;
        _ending = ending;
    }

    public void Update(GameTime gameTime)
    {
        var kbd = Keyboard.GetState();
        var mouse = Mouse.GetState();
        bool enter = kbd.IsKeyDown(Keys.Enter) && !_prevKeyboard.IsKeyDown(Keys.Enter);
        bool space = kbd.IsKeyDown(Keys.Space) && !_prevKeyboard.IsKeyDown(Keys.Space);
        bool click = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;

        if (enter || space || (click && _menuBtn.Contains(mouse.Position)))
        {
            _ctx.Audio.PlayConfirm();
            _ctx.ScreenManager.SetScreen(new MainMenuScreen(_ctx));
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), new Color(12, 14, 20));

        string title;
        Color titleColor;
        string[] narrative;

        switch (_ending)
        {
            case StoryEnding.EndingA_Betrayal:
                title = "ENDING A: THE WEALTHY BETRAYAL";
                titleColor = Color.Gold;
                narrative = new[]
                {
                    "You accepted the Merchant's tempting buyout of 5,000 Gold.",
                    "Toothless was locked in a heavy steel cage, screeching as the wagon rolled away.",
                    "Your debt is extinguished, and your pockets are heavy with coin.",
                    "Yet, as darkness settles over the shelter, the unnatural silence is deafening.",
                    "You survived... but what was the price of your survival?"
                };
                break;

            case StoryEnding.EndingB_Protector:
                title = "ENDING B: THE SHELTER'S GUARDIAN";
                titleColor = new Color(80, 240, 150);
                narrative = new[]
                {
                    "You stood unwavering against the Traveling Collector's threats.",
                    "Through coordination, quick reflexes, and sheer determination, the Merchant was driven off.",
                    "Toothless and your starter companion curl peacefully beside each other on the habitat rug.",
                    "You have proven that even the most abnormal creatures can find trust and warmth.",
                    "The shelter thrives as a true sanctuary of understanding."
                };
                break;

            case StoryEnding.Ending_VerticalSliceForcedDefeat:
                title = "VERTICAL SLICE FINALE: FORCED RETREAT";
                titleColor = new Color(255, 90, 90);
                narrative = new[]
                {
                    "The Chapter Boss descended upon the sanctuary with overwhelming, cataclysmic force.",
                    "Despite brave resistance and quick evasion, the abnormal incursion forced a tactical retreat.",
                    "Your companions shielded you until the final second, escaping safely into the research vault.",
                    "Vertical Slice Complete! The struggle continues in the full game release.",
                    "Thank you for playing the BePal V2 prototype."
                };
                break;

            default:
                title = "BAD ENDING: FORECLOSURE & RUIN";
                titleColor = new Color(240, 70, 70);
                narrative = new[]
                {
                    "The compound interest on your emergency loan soared beyond reach.",
                    "When the deadline struck, bailiffs arrived bearing the Merchant's contract.",
                    "The shelter, its equipment, and all sheltered abnormal creatures were confiscated.",
                    "You stand outside the locked gates in the cold rain, empty-handed.",
                    "Research terminated. Facility foreclosed."
                };
                break;
        }

        // Draw Title
        Vector2 tSize = _ctx.Font.MeasureString(title);
        batch.DrawString(_ctx.Font, title, new Vector2(960 - (tSize.X * 1.6f) / 2f, 110), titleColor, 0f, Vector2.Zero, 1.6f, SpriteEffects.None, 0f);

        // Narrative box
        Rectangle textRect = new(360, 220, 1200, 480);
        CleanUI.DrawPanel(batch, textRect, new Color(20, 24, 34), titleColor, borderWidth: 1, shadow: true);

        int ny = textRect.Y + 45;
        foreach (var line in narrative)
        {
            Vector2 lSize = _ctx.Font.MeasureString(line);
            batch.DrawString(_ctx.Font, line, new Vector2(960 - (lSize.X * 1.15f) / 2f, ny), Color.White, 0f, Vector2.Zero, 1.15f, SpriteEffects.None, 0f);
            ny += 55;
        }

        // Stats summary
        var run = _ctx.Run;
        string stats = $"Total Sessions Completed: {run.TotalCareSessionsCompleted}   |   Perfect: {run.TotalPerfectAttempts}   |   Good: {run.TotalGoodAttempts}   |   Misses: {run.TotalMissAttempts}";
        Vector2 sSize = _ctx.Font.MeasureString(stats);
        batch.DrawString(_ctx.Font, stats, new Vector2(960 - sSize.X / 2f, 750), Color.Gold);

        string walletInfo = $"Final Wallet: {run.Economy.Gold} G   |   Final Debt: {run.Economy.Debt} G   |   Research Points: {run.Economy.PlayerPoints} PTS";
        Vector2 wSize = _ctx.Font.MeasureString(walletInfo);
        batch.DrawString(_ctx.Font, walletInfo, new Vector2(960 - wSize.X / 2f, 800), new Color(180, 200, 220));

        // Return button
        Point mPos = Mouse.GetState().Position;
        string btnLabel = "RETURN TO MAIN MENU [ ENTER ]";
        CleanUI.DrawButton(batch, _ctx.Font, _menuBtn, btnLabel, _menuBtn.Contains(mPos), accent: UITheme.AccentEmerald, isPrimary: true);
    }
}
