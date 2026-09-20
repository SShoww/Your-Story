using BePalV2.Audio;
using BePalV2.Gameplay;
using BePalV2.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePalV2.Screens;

public sealed class BaseHabitatScreen : IScreen
{
    private readonly ScreenContext _ctx;
    private readonly DialogueBox _dialogue = new();
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;

    private float _breathTimer;

    // Command buttons in exact Slide 15 order: Train, Clean, Heal, Feed
    private readonly Rectangle _trainBtn = new(180, 600, 190, 52);
    private readonly Rectangle _cleanBtn = new(390, 600, 190, 52);
    private readonly Rectangle _healBtn = new(600, 600, 190, 52);
    private readonly Rectangle _feedBtn = new(810, 600, 190, 52);

    // Quick access buttons (Top Right / Bottom Right)
    private readonly Rectangle _bagBtn = new(1030, 600, 105, 52);
    private readonly Rectangle _shopBtn = new(1150, 600, 105, 52);

    public BaseHabitatScreen(ScreenContext ctx)
    {
        _ctx = ctx;

        if (_ctx.Run.CurrentPhase == DailyPhase.MorningEvent)
        {
            SetupMorningBriefing();
        }
    }

    private void SetupMorningBriefing()
    {
        int day = _ctx.Run.DayNumber;
        if (day == 1)
        {
            _dialogue.StartDialogue("MORNING BRIEFING - DAY 1", new[]
            {
                "Warning: Thunderstorm disaster detected approaching the shelter vents.",
                "Wind and muddy dust will reduce cleanliness later today.",
                "Allocate your 6 Energy Points (AP) across Train, Clean, Heal, and Feed wisely!"
            }, onCompleted: () => _ctx.Run.AdvanceFromMorningToCare());
        }
        else if (day == 2)
        {
            _dialogue.StartDialogue("MORNING BRIEFING - DAY 2", new[]
            {
                "A strange sizzle greets you at dawn: purple acid droplets on the doorway.",
                "A feral creature is roaming the woods outside... Knock Knock !!",
                "Prepare your pet before the afternoon defense encounter."
            }, onCompleted: () => _ctx.Run.AdvanceFromMorningToCare());
        }
        else if (day == 3)
        {
            _dialogue.StartDialogue("MORNING BRIEFING - DAY 3", new[]
            {
                "The Traveling Merchant's wagon has arrived outside the facility gate.",
                "Browse rare items in the [ SHOP ], or brace for the final shift dilemma."
            }, onCompleted: () => _ctx.Run.AdvanceFromMorningToCare());
        }
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _breathTimer += dt;

        var kbd = Keyboard.GetState();
        var mouse = Mouse.GetState();
        Point mPos = mouse.Position;
        bool click = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;

        if (_dialogue.IsActive)
        {
            _dialogue.UpdateInput(dt, kbd, _prevKeyboard, mouse, _prevMouse);
            _prevKeyboard = kbd;
            _prevMouse = mouse;
            return;
        }

        if (_ctx.Run.ActivePet.IsIncapacitated)
        {
            _dialogue.ShowPrompt("EMERGENCY MEDICAL REVIVE",
                "Your pet is incapacitated! Call the emergency service for 500 Gold (or a 20% interest loan)?",
                onYes: () =>
                {
                    _ctx.Run.ResolveEmergencyRevive();
                    _ctx.Audio.PlaySuccess();
                });
            _prevKeyboard = kbd;
            _prevMouse = mouse;
            return;
        }

        if (_ctx.Run.CurrentPhase == DailyPhase.DefenseResolution)
        {
            RouteDefensePhase();
            return;
        }

        if (kbd.IsKeyDown(Keys.B) && !_prevKeyboard.IsKeyDown(Keys.B)) OpenBag();
        else if (kbd.IsKeyDown(Keys.S) && !_prevKeyboard.IsKeyDown(Keys.S) && _ctx.Run.DayNumber == 3) OpenShop();
        else if (kbd.IsKeyDown(Keys.D1) && !_prevKeyboard.IsKeyDown(Keys.D1)) TryExecuteAction(CareActionType.Train);
        else if (kbd.IsKeyDown(Keys.D2) && !_prevKeyboard.IsKeyDown(Keys.D2)) TryExecuteAction(CareActionType.Clean);
        else if (kbd.IsKeyDown(Keys.D3) && !_prevKeyboard.IsKeyDown(Keys.D3)) TryExecuteAction(CareActionType.Heal);
        else if (kbd.IsKeyDown(Keys.D4) && !_prevKeyboard.IsKeyDown(Keys.D4)) TryExecuteAction(CareActionType.Feed);

        if (click)
        {
            if (_trainBtn.Contains(mPos)) TryExecuteAction(CareActionType.Train);
            else if (_cleanBtn.Contains(mPos)) TryExecuteAction(CareActionType.Clean);
            else if (_healBtn.Contains(mPos)) TryExecuteAction(CareActionType.Heal);
            else if (_feedBtn.Contains(mPos)) TryExecuteAction(CareActionType.Feed);
            else if (_bagBtn.Contains(mPos)) OpenBag();
            else if (_shopBtn.Contains(mPos) && _ctx.Run.DayNumber == 3) OpenShop();
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void TryExecuteAction(CareActionType action)
    {
        int cost = 1;
        if (action == CareActionType.Heal && !_ctx.Run.Inventory.HasMedicine()) cost = 2;

        if (!_ctx.Run.Energy.CanSpend(cost))
        {
            _ctx.Audio.PlayFail();
            return;
        }

        if (action == CareActionType.Train && !_ctx.Run.ActivePet.CanTrain())
        {
            _ctx.Audio.PlayFail();
            return;
        }

        _ctx.Audio.PlayConfirm();
        _ctx.ScreenManager.SetScreen(new CareQteScreen(_ctx, action));
    }

    private void OpenBag()
    {
        _ctx.Audio.PlayConfirm();
        _ctx.ScreenManager.PushOverlay(new InventoryOverlayScreen(_ctx));
    }

    private void OpenShop()
    {
        _ctx.Audio.PlayConfirm();
        _ctx.ScreenManager.PushOverlay(new ShopModalScreen(_ctx));
    }

    private void RouteDefensePhase()
    {
        if (_ctx.Run.DayNumber == 1)
        {
            _ctx.ScreenManager.SetScreen(new CalmingQteScreen(_ctx));
        }
        else if (_ctx.Run.DayNumber == 2)
        {
            _dialogue.ShowPrompt("KNOCK KNOCK !!",
                "Toothless is pounding the door! [ CHASE ] or [ TAME ]?",
                onYes: () =>
                {
                    _ctx.ScreenManager.SetScreen(new CombatArenaScreen(_ctx, CombatMode.ToothlessTaming));
                },
                onNo: () =>
                {
                    _ctx.Run.ResolveDay2Encounter(chooseTame: false, tameSuccess: false);
                    _ctx.ScreenManager.SetScreen(new DailySummaryScreen(_ctx));
                });
        }
        else if (_ctx.Run.DayNumber == 3)
        {
            _dialogue.ShowPrompt("MERCHANT FIGHT / BUYOUT",
                "Merchant: 'I'm quite interested in your Toothless... can you sell it for 5,000G?' [ YES ] or [ NO ]?",
                onYes: () =>
                {
                    _ctx.Run.AcceptMerchantBuyout();
                    _ctx.ScreenManager.SetScreen(new EndingScreen(_ctx, StoryEnding.EndingA_Betrayal));
                },
                onNo: () =>
                {
                    _ctx.ScreenManager.SetScreen(new CombatArenaScreen(_ctx, CombatMode.MerchantBoss));
                });
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        // Dark Habitat Room Background matching Slide 13
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), new Color(26, 26, 28));

        // Top Bar matching Slide 13/14
        DrawTopBar(batch);

        // Disaster / Event Notification Banner (Slide 23/31 concept)
        DrawEventBanner(batch);

        // Center Habitat Stage
        DrawCenterStage(batch);

        // Status Panel (Slide 25 concept: Stomach, Clean, Health, Pet Name)
        DrawStatusPanel(batch);

        // Bottom Command Console (Slide 15 concept: Train, Clean, Heal, Feed)
        DrawConsole(batch);

        // Dialogue Box
        if (_dialogue.IsActive)
        {
            Rectangle diagRect = new(200, 470, 880, 180);
            _dialogue.Draw(batch, _ctx.Font, _ctx.Pixel, diagRect);
        }
    }

    private void DrawTopBar(SpriteBatch batch)
    {
        Rectangle bar = new(0, 0, _ctx.ScreenWidth, 60);
        batch.FillRectangle(bar, new Color(18, 18, 20));
        batch.DrawLine(0, 60, _ctx.ScreenWidth, 60, new Color(60, 60, 65), 2f);

        // Coin Icon + Gold (Slide 13 concept)
        Rectangle coin = new(35, 18, 24, 24);
        batch.DrawCircle(new Vector2(coin.Center.X, coin.Center.Y), 6f, 16, new Color(255, 222, 89), 12f);
        batch.DrawString(_ctx.Font, "G", new Vector2(coin.X + 7, coin.Y + 4), Color.Black);
        batch.DrawString(_ctx.Font, $"{_ctx.Run.Economy.Gold} G", new Vector2(70, 18), new Color(255, 222, 89), 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);

        // Center: Player & Day X (Slide 13 concept)
        batch.DrawString(_ctx.Font, "PLAYER", new Vector2(400, 20), Color.White);
        batch.DrawString(_ctx.Font, $"DAY {_ctx.Run.DayNumber}", new Vector2(500, 18), Color.Gold, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

        // Energy Pips (Slide 14/27 concept: 6 Energy points)
        batch.DrawString(_ctx.Font, "ENERGY:", new Vector2(620, 20), new Color(180, 180, 180));
        for (int i = 0; i < EnergyAccount.DefaultMaxEnergy; i++)
        {
            Rectangle pip = new(710 + i * 32, 20, 24, 22);
            bool active = i < _ctx.Run.Energy.CurrentEnergy;
            batch.FillRectangle(pip, active ? new Color(93, 176, 70) : new Color(45, 45, 50));
            batch.DrawRectangle(pip, Color.White, 1);
        }

        // Debt warning if present
        if (_ctx.Run.Economy.HasDebt)
        {
            batch.DrawString(_ctx.Font, $"DEBT: {_ctx.Run.Economy.Debt} G", new Vector2(930, 20), new Color(231, 25, 31));
        }

        // Phase tag
        string phaseStr = _ctx.Run.CurrentPhase.ToString().ToUpper();
        batch.DrawString(_ctx.Font, phaseStr, new Vector2(1100, 20), new Color(140, 180, 240));
    }

    private void DrawEventBanner(SpriteBatch batch)
    {
        int day = _ctx.Run.DayNumber;
        if (day == 1)
        {
            // Slide 23 concept: "Disaster - Thunder storm"
            Rectangle banner = new(350, 70, 580, 32);
            batch.FillRectangle(banner, new Color(208, 38, 54, 220));
            batch.DrawRectangle(banner, Color.White, 1);
            string dText = "! DISASTER EVENT: THUNDER STORM APPROACHING !";
            Vector2 sz = _ctx.Font.MeasureString(dText);
            batch.DrawString(_ctx.Font, dText, new Vector2(banner.Center.X - sz.X / 2f, banner.Center.Y - sz.Y / 2f), Color.White);
        }
        else if (day == 2)
        {
            // Slide 31 concept: "Knock Knock !!"
            Rectangle banner = new(440, 70, 400, 32);
            batch.FillRectangle(banner, new Color(150, 60, 200, 220));
            batch.DrawRectangle(banner, Color.White, 1);
            string dText = "!! KNOCK KNOCK !! TOOTHLESS AT GATE";
            Vector2 sz = _ctx.Font.MeasureString(dText);
            batch.DrawString(_ctx.Font, dText, new Vector2(banner.Center.X - sz.X / 2f, banner.Center.Y - sz.Y / 2f), Color.White);
        }
        else if (day == 3)
        {
            // Slide 47 concept: "Merchant Fight Day 3"
            Rectangle banner = new(430, 70, 420, 32);
            batch.FillRectangle(banner, new Color(255, 180, 40, 220));
            batch.DrawRectangle(banner, Color.White, 1);
            string dText = "MERCHANT CARAVAN ARRIVAL (DAY 3)";
            Vector2 sz = _ctx.Font.MeasureString(dText);
            batch.DrawString(_ctx.Font, dText, new Vector2(banner.Center.X - sz.X / 2f, banner.Center.Y - sz.Y / 2f), Color.Black);
        }
    }

    private void DrawCenterStage(SpriteBatch batch)
    {
        float breathOffset = (float)Math.Sin(_breathTimer * 3.0f) * 6f;
        Rectangle stage = new(380, 115, 520, 460);
        batch.FillRectangle(stage, new Color(34, 34, 38));
        batch.DrawRectangle(stage, new Color(70, 70, 75), 1);

        // Habitat Floor mat
        batch.FillRectangle(new Rectangle(460, 440, 360, 40), new Color(48, 48, 54));

        // Active Pet Sprite
        Rectangle petRect = new(560, (int)(270 + breathOffset), 160, 160);
        if (_ctx.PetIdleTex != null)
        {
            batch.Draw(_ctx.PetIdleTex, petRect, Color.White);
        }
        else
        {
            batch.DrawCircle(new Vector2(640, 350 + breathOffset), 30f, 24, new Color(93, 176, 70), 60f);
        }

        // Secondary Pet (Toothless)
        if (_ctx.Run.HasToothlessAlly)
        {
            batch.DrawCircle(new Vector2(790, 410), 20f, 20, new Color(60, 40, 70), 40f);
            batch.DrawString(_ctx.Font, "Toothless", new Vector2(760, 455), new Color(200, 140, 255));
        }

        // Dynamic Mood Tag
        string mood = _ctx.Run.ActivePet.IsStarving ? "STARVING!" : (_ctx.Run.ActivePet.IsGrimy ? "Muddy & Grimy..." : "Observing calmly ~");
        Vector2 mSize = _ctx.Font.MeasureString(mood);
        Rectangle tag = new((int)(640 - mSize.X / 2f - 14), (int)(220 + breathOffset), (int)mSize.X + 28, 30);
        batch.FillRectangle(tag, Color.White);
        batch.DrawRectangle(tag, Color.Black, 1);
        batch.DrawString(_ctx.Font, mood, new Vector2(tag.X + 14, tag.Y + 6), Color.Black);
    }

    private void DrawStatusPanel(SpriteBatch batch)
    {
        // Status Panel matching Slide 25: Stomach, Clean, Health, (4) COCO
        Rectangle panel = new(920, 115, 320, 460);
        batch.FillRectangle(panel, new Color(30, 30, 34));
        batch.DrawRectangle(panel, new Color(70, 70, 75), 1);

        var pet = _ctx.Run.ActivePet;
        batch.DrawString(_ctx.Font, $"(4) {pet.Name.ToUpper()}", new Vector2(panel.X + 20, panel.Y + 24), Color.Gold, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, $"Level {pet.Level}  ({pet.CurrentExp} / {pet.MaxExp} EXP)", new Vector2(panel.X + 20, panel.Y + 60), new Color(180, 180, 180));

        int sy = panel.Y + 110;
        // Slide 25 format: "Stomach : 20 Clean : 50 Health : 100"
        DrawStatusRow(batch, panel.X + 20, sy, "STOMACH", pet.Stomach, 100, new Color(255, 222, 89));
        DrawStatusRow(batch, panel.X + 20, sy + 75, "CLEAN", pet.Clean, 100, new Color(13, 153, 255));
        DrawStatusRow(batch, panel.X + 20, sy + 150, "HEALTH", (int)pet.Health, (int)pet.MaxHealth, new Color(231, 25, 31));

        int flagY = sy + 235;
        if (pet.IsStarving)
        {
            batch.DrawString(_ctx.Font, "! STARVING (-10 HP, -2/Act) !", new Vector2(panel.X + 20, flagY), Color.Red);
            flagY += 25;
        }
        if (pet.IsInfected)
        {
            batch.DrawString(_ctx.Font, "! INFECTED (-15 HP, -30% Perf) !", new Vector2(panel.X + 20, flagY), Color.Red);
            flagY += 25;
        }
        else if (pet.IsGrimy)
        {
            batch.DrawString(_ctx.Font, "! GRIMY (+15% Speed) !", new Vector2(panel.X + 20, flagY), Color.Orange);
        }
    }

    private void DrawStatusRow(SpriteBatch batch, int x, int y, string label, int current, int max, Color fillColor)
    {
        batch.DrawString(_ctx.Font, $"{label} : {current} / {max}", new Vector2(x, y), Color.White);
        Rectangle bg = new(x, y + 24, 280, 18);
        float ratio = Math.Clamp((float)current / Math.Max(1, max), 0f, 1f);
        Rectangle fill = new(x, y + 24, (int)(280 * ratio), 18);
        batch.FillRectangle(bg, new Color(45, 45, 50));
        batch.FillRectangle(fill, fillColor);
        batch.DrawRectangle(bg, Color.White, 1);
    }

    private void DrawConsole(SpriteBatch batch)
    {
        Point mPos = Mouse.GetState().Position;

        // Slide 15 Command Buttons: Train, Clean, Heal, Feed
        bool canTrain = _ctx.Run.ActivePet.CanTrain();
        DrawActionButton(batch, _trainBtn, "TRAIN", "EXP (1 AP)", new Color(93, 176, 70), _trainBtn.Contains(mPos), canTrain, "[ 1 ]");
        DrawActionButton(batch, _cleanBtn, "CLEAN", "Clean (1 AP)", new Color(13, 153, 255), _cleanBtn.Contains(mPos), true, "[ 2 ]");

        int healCost = _ctx.Run.Inventory.HasMedicine() ? 1 : 2;
        DrawActionButton(batch, _healBtn, "HEAL", $"Health ({healCost} AP)", new Color(231, 25, 31), _healBtn.Contains(mPos), true, "[ 3 ]");
        DrawActionButton(batch, _feedBtn, "FEED", "Stomach (1 AP)", new Color(255, 222, 89), _feedBtn.Contains(mPos), true, "[ 4 ]");

        // Bag & Shop
        DrawSmallButton(batch, _bagBtn, "BAG [ B ]", new Color(70, 70, 85), _bagBtn.Contains(mPos));
        if (_ctx.Run.DayNumber == 3)
        {
            DrawSmallButton(batch, _shopBtn, "SHOP [ S ]", new Color(200, 150, 40), _shopBtn.Contains(mPos));
        }
    }

    private void DrawActionButton(SpriteBatch batch, Rectangle rect, string title, string sub, Color themeColor, bool hovered, bool enabled, string key)
    {
        Color bg = enabled ? (hovered ? themeColor * 1.2f : themeColor) : new Color(50, 50, 55);
        batch.FillRectangle(rect, bg);
        batch.DrawRectangle(rect, enabled ? (hovered ? Color.White : Color.Black) : Color.DarkGray, hovered ? 2 : 1);

        Color textColor = (themeColor == new Color(255, 222, 89) && enabled) ? Color.Black : Color.White;
        if (!enabled) textColor = Color.Gray;

        batch.DrawString(_ctx.Font, $"{key} {title}", new Vector2(rect.X + 16, rect.Y + 8), textColor);
        batch.DrawString(_ctx.Font, sub, new Vector2(rect.X + 16, rect.Y + 28), textColor * 0.85f);
    }

    private void DrawSmallButton(SpriteBatch batch, Rectangle rect, string text, Color color, bool hovered)
    {
        batch.FillRectangle(rect, hovered ? color * 1.2f : color);
        batch.DrawRectangle(rect, hovered ? Color.White : Color.Gray, 1);
        Vector2 sz = _ctx.Font.MeasureString(text);
        batch.DrawString(_ctx.Font, text, new Vector2(rect.Center.X - sz.X / 2f, rect.Center.Y - sz.Y / 2f), Color.White);
    }
}
