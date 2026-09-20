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

    private readonly Rectangle _panelRect = new(180, 50, 920, 620);
    private readonly Rectangle _exitBtn = new(970, 68, 110, 36);

    // 5 Canonical Items Matching Slide 62
    private readonly IReadOnlyList<ItemDefinition> _shopItems = ItemDefinition.CanonicalFive;

    private ItemDefinition? _pendingItem;
    private readonly Rectangle _confirmYesBtn = new(690, 560, 120, 42);
    private readonly Rectangle _confirmNoBtn = new(830, 560, 120, 42);

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
            if (_exitBtn.Contains(mPos))
            {
                _ctx.Audio.PlayConfirm();
                _ctx.ScreenManager.PopOverlay();
                return;
            }

            // Click an item in the list
            for (int i = 0; i < _shopItems.Count; i++)
            {
                int y = _panelRect.Y + 80 + i * 85;
                Rectangle itemRect = new(_panelRect.X + 30, y, 430, 74);
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
            _purchaseFeedback = "INSUFFICIENT FUNDS! Cannot afford this item.";
            _feedbackTimer = 2.0f;
            _ctx.Audio.PlayWarning();
            return;
        }

        if (run.Inventory.IsFull)
        {
            _purchaseFeedback = "STORAGE CAPACITY FULL! Discard or use an item first.";
            _feedbackTimer = 2.0f;
            _ctx.Audio.PlayWarning();
            return;
        }

        run.Economy.SpendGold(item.Price);
        run.Inventory.AddItem(item);
        _ctx.Audio.PlaySuccess();

        _purchaseFeedback = $"TRANSACTION CONFIRMED: Acquired {item.Name} for {item.Price} G";
        _feedbackTimer = 2.0f;
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.8f);

        // Shop Frame
        CleanUI.DrawPanel(batch, _panelRect, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(_panelRect.X, _panelRect.Y, _panelRect.Width, 3), UITheme.AccentGold);

        // Header: "Shop" & Gold & Bag capacity
        batch.DrawString(_ctx.Font, $"MERCHANT OUTPOST - DAY {_ctx.Run.DayNumber}", new Vector2(_panelRect.X + 30, _panelRect.Y + 22), UITheme.AccentGold, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);

        Rectangle goldPill = new(_panelRect.X + 410, _panelRect.Y + 18, 140, 28);
        CleanUI.DrawBadge(batch, _ctx.Font, goldPill, $"GOLD: {_ctx.Run.Economy.Gold} G", new Color(34, 42, 58), UITheme.AccentGold);

        int occupied = _ctx.Run.Inventory.Count;
        Color bagColor = occupied >= 8 ? UITheme.AccentCoral : UITheme.AccentEmerald;
        Rectangle bagPill = new(_panelRect.X + 570, _panelRect.Y + 18, 140, 28);
        CleanUI.DrawBadge(batch, _ctx.Font, bagPill, $"BAG: {occupied} / {InventoryService.MaxSlots}", new Color(28, 36, 44), bagColor);

        Point mPos = Mouse.GetState().Position;

        // Exit Shop Button
        CleanUI.DrawButton(batch, _ctx.Font, _exitBtn, "CLOSE", _exitBtn.Contains(mPos), accent: UITheme.BorderSubtle, hotkey: "[ ESC ]");

        // 5 Canonical Items
        for (int i = 0; i < _shopItems.Count; i++)
        {
            var item = _shopItems[i];
            int y = _panelRect.Y + 80 + i * 85;
            Rectangle itemRect = new(_panelRect.X + 30, y, 430, 74);

            bool isSelected = _pendingItem == item;
            bool hovered = itemRect.Contains(mPos);

            Color bg = isSelected ? UITheme.BgPanelHover : (hovered ? new Color(28, 32, 42) : UITheme.BgCardRecessed);
            Color border = isSelected ? UITheme.AccentGold : (hovered ? UITheme.BorderHighlight : UITheme.BorderSubtle);

            CleanUI.DrawPanel(batch, itemRect, bg, border, borderWidth: isSelected ? 2 : 1, shadow: false);

            batch.DrawString(_ctx.Font, item.Name, new Vector2(itemRect.X + 16, y + 10), UITheme.TextPrimary);

            // Price pill
            Rectangle pricePill = new(itemRect.Right - 90, y + 10, 76, 22);
            CleanUI.DrawBadge(batch, _ctx.Font, pricePill, $"{item.Price} G", UITheme.AccentGold * 0.25f, UITheme.AccentGold);

            batch.DrawString(_ctx.Font, item.Description, new Vector2(itemRect.X + 16, y + 36), UITheme.TextSecondary);
        }

        // Merchant Portrait & Dialogue Frame
        Rectangle merchRect = new(_panelRect.X + 480, _panelRect.Y + 80, 410, 460);
        CleanUI.DrawPanel(batch, merchRect, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);

        batch.DrawString(_ctx.Font, "MERCHANT'S COUNTER", new Vector2(merchRect.X + 20, merchRect.Y + 16), UITheme.AccentGold);
        batch.DrawLine(merchRect.X + 20, merchRect.Y + 44, merchRect.Right - 20, merchRect.Y + 44, UITheme.BorderSubtle, 1f);

        string dialogue = "\"I carry rare specimen sundries... care to trade?\"";
        batch.DrawString(_ctx.Font, dialogue, new Vector2(merchRect.X + 20, merchRect.Y + 62), UITheme.TextSecondary);

        if (_pendingItem != null)
        {
            batch.DrawString(_ctx.Font, _pendingItem.Name, new Vector2(merchRect.X + 20, merchRect.Y + 110), UITheme.TextPrimary, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
            batch.DrawString(_ctx.Font, $"Price: {_pendingItem.Price} Gold", new Vector2(merchRect.X + 20, merchRect.Y + 144), UITheme.AccentGold);
            batch.DrawString(_ctx.Font, _pendingItem.Description, new Vector2(merchRect.X + 20, merchRect.Y + 178), UITheme.TextSecondary);

            // Purchase confirmation prompt
            batch.DrawString(_ctx.Font, $"Acquire {_pendingItem.Name} for containment?", new Vector2(merchRect.X + 20, merchRect.Y + 270), UITheme.AccentCyan);

            CleanUI.DrawButton(batch, _ctx.Font, _confirmYesBtn, "BUY", _confirmYesBtn.Contains(mPos), accent: UITheme.AccentEmerald, isPrimary: true);
            CleanUI.DrawButton(batch, _ctx.Font, _confirmNoBtn, "CANCEL", _confirmNoBtn.Contains(mPos), accent: UITheme.AccentCoral);
        }

        // Notification / Feedback banner
        if (!string.IsNullOrEmpty(_purchaseFeedback))
        {
            Color fbCol = _purchaseFeedback.Contains("CONFIRMED") ? UITheme.AccentEmerald : UITheme.AccentCoral;
            batch.DrawString(_ctx.Font, _purchaseFeedback, new Vector2(_panelRect.X + 30, _panelRect.Bottom - 36), fbCol);
        }
    }
}
