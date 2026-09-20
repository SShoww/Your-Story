using BePalV2.Audio;
using BePalV2.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePalV2.Screens;

public sealed class ShopModalScreen : IScreen
{
    public bool IsOverlay => true;
    private readonly ScreenContext _ctx;
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;

    private readonly Rectangle _panelRect = new(200, 60, 880, 600);
    private readonly Rectangle _exitBtn = new(960, 75, 100, 36);

    private readonly List<ItemDefinition> _shopItems = new()
    {
        ItemDefinition.CrabApple,
        ItemDefinition.SeaTea,
        ItemDefinition.CloudyGlasses,
        ItemDefinition.TornNotebook,
        ItemDefinition.CaffeineTonic
    };

    private ItemDefinition? _pendingItem;
    private readonly Rectangle _confirmYesBtn = new(640, 560, 100, 38);
    private readonly Rectangle _confirmNoBtn = new(760, 560, 100, 38);

    private string? _purchaseFeedback;
    private float _feedbackTimer;

    public ShopModalScreen(ScreenContext ctx)
    {
        _ctx = ctx;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_feedbackTimer > 0f)
        {
            _feedbackTimer -= dt;
            if (_feedbackTimer <= 0f) _purchaseFeedback = null;
        }

        var kbd = Keyboard.GetState();
        var mouse = Mouse.GetState();
        Point mPos = mouse.Position;
        bool click = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;

        if (kbd.IsKeyDown(Keys.Escape) && !_prevKeyboard.IsKeyDown(Keys.Escape))
        {
            _ctx.Audio.PlayConfirm();
            _ctx.ScreenManager.PopOverlay();
            return;
        }

        if (click)
        {
            if (_exitBtn.Contains(mPos) || !_panelRect.Contains(mPos))
            {
                _ctx.Audio.PlayConfirm();
                _ctx.ScreenManager.PopOverlay();
                return;
            }

            if (_pendingItem != null)
            {
                if (_confirmYesBtn.Contains(mPos))
                {
                    ConfirmPurchase(_pendingItem);
                    _pendingItem = null;
                }
                else if (_confirmNoBtn.Contains(mPos))
                {
                    _pendingItem = null;
                    _ctx.Audio.PlayConfirm();
                }
                return;
            }

            for (int i = 0; i < _shopItems.Count; i++)
            {
                Rectangle rowBtn = new(_panelRect.X + 30, _panelRect.Y + 80 + i * 72, 420, 60);
                if (rowBtn.Contains(mPos))
                {
                    _pendingItem = _shopItems[i];
                    _ctx.Audio.PlayConfirm();
                    break;
                }
            }
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void ConfirmPurchase(ItemDefinition item)
    {
        if (_ctx.Run.Inventory.IsFull)
        {
            _purchaseFeedback = "Inventory full! (8/8 slots occupied)";
            _feedbackTimer = 2.0f;
            _ctx.Audio.PlayFail();
            return;
        }

        if (!_ctx.Run.Economy.CanAfford(item.Price))
        {
            _purchaseFeedback = "Not enough Gold!";
            _feedbackTimer = 2.0f;
            _ctx.Audio.PlayFail();
            return;
        }

        _ctx.Run.Economy.SpendGold(item.Price);
        _ctx.Run.Inventory.AddItem(item);
        _ctx.Audio.PlayCoins();
        _purchaseFeedback = $"PURCHASE! Acquired {item.Name} for {item.Price}G";
        _feedbackTimer = 2.0f;
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), new Color(0, 0, 0, 190));

        // Slide 59/60 Shop Frame
        batch.FillRectangle(_panelRect, new Color(24, 24, 28));
        batch.DrawRectangle(_panelRect, new Color(80, 80, 90), 2);

        // Header: "Shop Day 3" & Gold
        batch.DrawString(_ctx.Font, $"SHOP DAY {_ctx.Run.DayNumber}", new Vector2(_panelRect.X + 30, _panelRect.Y + 22), Color.Gold, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, $"GOLD: {_ctx.Run.Economy.Gold} G", new Vector2(_panelRect.X + 320, _panelRect.Y + 25), new Color(255, 222, 89));

        // Inventory Capacity Badge: e.g. "7 / 8" (Slide 60 concept)
        int occupied = _ctx.Run.Inventory.Count;
        batch.DrawString(_ctx.Font, $"BAG: {occupied} / {InventoryService.MaxSlots}", new Vector2(_panelRect.X + 540, _panelRect.Y + 25), occupied >= 8 ? Color.Red : Color.White);

        // Exit Shop Button (Slide 59 concept)
        batch.FillRectangle(_exitBtn, new Color(50, 50, 55));
        batch.DrawRectangle(_exitBtn, Color.White, 1);
        batch.DrawString(_ctx.Font, "EXIT SHOP", new Vector2(_exitBtn.X + 10, _exitBtn.Y + 8), Color.White);

        Point mPos = Mouse.GetState().Position;

        // Item Catalog List (Slide 59 concept: Crab Apple 25G, Sea Tea 18G, etc.)
        for (int i = 0; i < _shopItems.Count; i++)
        {
            var item = _shopItems[i];
            int y = _panelRect.Y + 80 + i * 72;
            Rectangle itemRect = new(_panelRect.X + 30, y, 420, 60);

            bool isSelected = _pendingItem == item;
            bool hovered = itemRect.Contains(mPos);

            batch.FillRectangle(itemRect, isSelected ? new Color(50, 50, 60) : (hovered ? new Color(40, 40, 48) : new Color(32, 32, 36)));
            batch.DrawRectangle(itemRect, isSelected ? Color.Gold : (hovered ? Color.White : new Color(70, 70, 80)), isSelected ? 2 : 1);

            batch.DrawString(_ctx.Font, item.Name, new Vector2(itemRect.X + 16, y + 10), Color.White);
            batch.DrawString(_ctx.Font, $"{item.Price} G", new Vector2(itemRect.Right - 80, y + 10), Color.Gold);
            batch.DrawString(_ctx.Font, item.Description, new Vector2(itemRect.X + 16, y + 34), new Color(170, 170, 180));
        }

        // Slide 59/60 Merchant Inspector & Dialogue Frame (Right / Bottom)
        Rectangle merchRect = new(_panelRect.X + 470, _panelRect.Y + 80, 380, 420);
        batch.FillRectangle(merchRect, new Color(28, 28, 34));
        batch.DrawRectangle(merchRect, new Color(70, 70, 80), 1);

        batch.DrawString(_ctx.Font, "MERCHANT", new Vector2(merchRect.X + 20, merchRect.Y + 16), Color.Gold);
        batch.DrawLine(merchRect.X + 20, merchRect.Y + 45, merchRect.Right - 20, merchRect.Y + 45, Color.Gray, 1f);

        if (_pendingItem != null)
        {
            batch.DrawString(_ctx.Font, _pendingItem.Name, new Vector2(merchRect.X + 20, merchRect.Y + 65), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
            batch.DrawString(_ctx.Font, $"Price: {_pendingItem.Price} Gold", new Vector2(merchRect.X + 20, merchRect.Y + 100), Color.Gold);
            batch.DrawString(_ctx.Font, _pendingItem.Description, new Vector2(merchRect.X + 20, merchRect.Y + 140), new Color(200, 200, 200));

            // Slide 60 concept: "Buy it for 25G? Yes / No"
            batch.DrawString(_ctx.Font, $"Buy {_pendingItem.Name} for {_pendingItem.Price} G?", new Vector2(merchRect.X + 20, merchRect.Y + 230), Color.Yellow);

            batch.FillRectangle(_confirmYesBtn, new Color(40, 140, 70));
            batch.DrawRectangle(_confirmYesBtn, Color.White, 1);
            batch.DrawString(_ctx.Font, "YES", new Vector2(_confirmYesBtn.X + 32, _confirmYesBtn.Y + 8), Color.White);

            batch.FillRectangle(_confirmNoBtn, new Color(140, 40, 40));
            batch.DrawRectangle(_confirmNoBtn, Color.White, 1);
            batch.DrawString(_ctx.Font, "NO", new Vector2(_confirmNoBtn.X + 36, _confirmNoBtn.Y + 8), Color.White);
        }
        else
        {
            // Slide 58 concept: "Merchant: What are you lookin for? I have something to sell... do you want to buy it?"
            string npcSpeech = "Merchant: 'What are you lookin for?\nI have rare goods from beyond the boundary.\nSelect any merchandise on the left to inspect.'";
            batch.DrawString(_ctx.Font, npcSpeech, new Vector2(merchRect.X + 20, merchRect.Y + 80), new Color(190, 190, 200));
        }

        // Notification / Feedback banner
        if (!string.IsNullOrEmpty(_purchaseFeedback))
        {
            batch.DrawString(_ctx.Font, _purchaseFeedback, new Vector2(_panelRect.X + 30, _panelRect.Bottom - 35), Color.Yellow);
        }
    }
}
