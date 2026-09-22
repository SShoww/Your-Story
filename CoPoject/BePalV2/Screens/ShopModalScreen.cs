using BePalV2.Audio;
using BePalV2.Gameplay;
using BePalV2.UI;
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

    // 1920x1080 Layout Constants
    private readonly Rectangle _panelRect = new(260, 100, 1400, 880);
    private readonly Rectangle _exitBtn = new(1480, 124, 140, 42);

    // 5 Canonical Items
    private readonly IReadOnlyList<ItemDefinition> _shopItems = ItemDefinition.CanonicalFive;

    private ItemDefinition? _pendingItem;
    private readonly Rectangle _confirmYesBtn = new(1060, 800, 200, 52);
    private readonly Rectangle _confirmNoBtn = new(1310, 800, 200, 52);

    private string? _purchaseFeedback;
    private float _feedbackTimer;

    public ShopModalScreen(ScreenContext ctx)
    {
        _ctx = ctx;
        if (_shopItems.Count > 0)
        {
            _pendingItem = _shopItems[0];
        }
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_feedbackTimer > 0f)
        {
            _feedbackTimer -= dt;
            if (_feedbackTimer <= 0f)
            {
                _purchaseFeedback = null;
            }
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
            // Exit button
            if (_exitBtn.Contains(mPos))
            {
                _ctx.Audio.PlayConfirm();
                _ctx.ScreenManager.PopOverlay();
                return;
            }

            // Click item card to select
            for (int i = 0; i < _shopItems.Count; i++)
            {
                int y = _panelRect.Y + 95 + i * 135;
                Rectangle itemRect = new(_panelRect.X + 40, y, 640, 115);
                if (itemRect.Contains(mPos))
                {
                    _pendingItem = _shopItems[i];
                    _ctx.Audio.PlayConfirm();
                    break;
                }
            }

            // Purchase confirmation buttons
            if (_pendingItem != null)
            {
                if (_confirmYesBtn.Contains(mPos))
                {
                    ConfirmPurchase(_pendingItem);
                }
                else if (_confirmNoBtn.Contains(mPos))
                {
                    _ctx.Audio.PlayConfirm();
                    _pendingItem = null;
                }
            }
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void ConfirmPurchase(ItemDefinition item)
    {
        var run = _ctx.Run;
        if (!run.Economy.CanAfford(item.Price))
        {
            _purchaseFeedback = $"Not enough Gold! Needs {item.Price} G.";
            _feedbackTimer = 2.0f;
            _ctx.Audio.PlayWarning();
            return;
        }

        if (run.Inventory.IsFull)
        {
            _purchaseFeedback = "Inventory is full (8/8 slots occupied)!";
            _feedbackTimer = 2.0f;
            _ctx.Audio.PlayWarning();
            return;
        }

        bool spent = run.Economy.SpendGold(item.Price);
        if (spent)
        {
            run.Inventory.AddItem(item);
            _purchaseFeedback = $"PURCHASE CONFIRMED: {item.Name} acquired!";
            _feedbackTimer = 2.5f;
            _ctx.Audio.PlaySuccess();
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.85f);

        // Shop Frame
        CleanUI.DrawPanel(batch, _panelRect, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(_panelRect.X, _panelRect.Y, _panelRect.Width, 4), UITheme.AccentGold);

        // Header: "MERCHANT OUTPOST" & Gold & Bag capacity
        batch.DrawString(_ctx.Font, $"MERCHANT OUTPOST - DAY {_ctx.Run.DayNumber}", new Vector2(_panelRect.X + 40, _panelRect.Y + 28), UITheme.AccentGold, 0f, Vector2.Zero, 1.35f, SpriteEffects.None, 0f);

        Rectangle goldPill = new(_panelRect.X + 640, _panelRect.Y + 24, 180, 38);
        CleanUI.DrawBadge(batch, _ctx.Font, goldPill, $"GOLD: {_ctx.Run.Economy.Gold} G", new Color(34, 42, 58), UITheme.AccentGold);

        int occupied = _ctx.Run.Inventory.Count;
        Color bagColor = occupied >= 8 ? UITheme.AccentCoral : UITheme.AccentEmerald;
        Rectangle bagPill = new(_panelRect.X + 840, _panelRect.Y + 24, 180, 38);
        CleanUI.DrawBadge(batch, _ctx.Font, bagPill, $"BAG: {occupied} / {InventoryService.MaxSlots}", new Color(28, 36, 44), bagColor);

        Point mPos = Mouse.GetState().Position;

        // Exit Shop Button (cleanly sized and positioned at top-right)
        CleanUI.DrawButton(batch, _ctx.Font, _exitBtn, "EXIT SHOP", _exitBtn.Contains(mPos), accent: UITheme.BorderSubtle, hotkey: "[ ESC ]");

        // 5 Canonical Items
        for (int i = 0; i < _shopItems.Count; i++)
        {
            var item = _shopItems[i];
            int y = _panelRect.Y + 95 + i * 135;
            Rectangle itemRect = new(_panelRect.X + 40, y, 640, 115);

            bool isSelected = _pendingItem == item;
            bool hovered = itemRect.Contains(mPos);

            Color bg = isSelected ? UITheme.BgPanelHover : (hovered ? new Color(28, 32, 42) : UITheme.BgCardRecessed);
            Color border = isSelected ? UITheme.AccentGold : (hovered ? UITheme.BorderHighlight : UITheme.BorderSubtle);

            CleanUI.DrawPanel(batch, itemRect, bg, border, borderWidth: isSelected ? 2 : 1, shadow: false);

            // Item Name
            batch.DrawString(_ctx.Font, item.Name, new Vector2(itemRect.X + 20, y + 16), UITheme.TextPrimary, 0f, Vector2.Zero, 1.15f, SpriteEffects.None, 0f);

            // Price badge
            Rectangle pricePill = new(itemRect.Right - 110, y + 14, 90, 28);
            CleanUI.DrawBadge(batch, _ctx.Font, pricePill, $"{item.Price} G", UITheme.AccentGold * 0.25f, UITheme.AccentGold);

            // Description: clear vertical separation from item name (NOT cramped!)
            batch.DrawString(_ctx.Font, item.Description, new Vector2(itemRect.X + 20, y + 54), UITheme.TextSecondary, 0f, Vector2.Zero, 0.88f, SpriteEffects.None, 0f);
        }

        // Merchant Counter Frame
        Rectangle merchRect = new(_panelRect.X + 710, _panelRect.Y + 95, 650, 750);
        CleanUI.DrawPanel(batch, merchRect, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);

        batch.DrawString(_ctx.Font, "MERCHANT'S COUNTER", new Vector2(merchRect.X + 30, merchRect.Y + 24), UITheme.AccentGold, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
        batch.DrawLine(merchRect.X + 30, merchRect.Y + 65, merchRect.Right - 30, merchRect.Y + 65, UITheme.BorderSubtle, 1f);

        // Speech line (NO redundant "Merchant:" prefix!)
        string dialogue = "\"What are you lookin for? Care to trade some rare specimen sundries?\"";
        batch.DrawString(_ctx.Font, dialogue, new Vector2(merchRect.X + 30, merchRect.Y + 90), UITheme.TextSecondary, 0f, Vector2.Zero, 0.95f, SpriteEffects.None, 0f);

        if (_pendingItem != null)
        {
            batch.DrawString(_ctx.Font, _pendingItem.Name, new Vector2(merchRect.X + 30, merchRect.Y + 160), UITheme.TextPrimary, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0f);
            batch.DrawString(_ctx.Font, $"Price: {_pendingItem.Price} Gold   |   Category: {_pendingItem.Category}", new Vector2(merchRect.X + 30, merchRect.Y + 215), UITheme.AccentCyan, 0f, Vector2.Zero, 1.05f, SpriteEffects.None, 0f);
            batch.DrawString(_ctx.Font, _pendingItem.Description, new Vector2(merchRect.X + 30, merchRect.Y + 265), UITheme.TextSecondary, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);

            // Purchase confirmation prompt
            batch.DrawString(_ctx.Font, $"Acquire {_pendingItem.Name} for facility containment?", new Vector2(merchRect.X + 30, merchRect.Y + 420), UITheme.AccentGold, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);

            CleanUI.DrawButton(batch, _ctx.Font, _confirmYesBtn, "BUY ITEM", _confirmYesBtn.Contains(mPos), accent: UITheme.AccentEmerald, isPrimary: true);
            CleanUI.DrawButton(batch, _ctx.Font, _confirmNoBtn, "CANCEL", _confirmNoBtn.Contains(mPos), accent: UITheme.AccentCoral);
        }

        // Notification / Feedback banner
        if (!string.IsNullOrEmpty(_purchaseFeedback))
        {
            Color fbCol = _purchaseFeedback.Contains("CONFIRMED") ? UITheme.AccentEmerald : UITheme.AccentCoral;
            batch.DrawString(_ctx.Font, _purchaseFeedback, new Vector2(_panelRect.X + 40, _panelRect.Bottom - 45), fbCol, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);
        }
    }
}
