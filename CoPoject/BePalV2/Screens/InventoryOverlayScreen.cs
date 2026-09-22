using BePalV2.Audio;
using BePalV2.Gameplay;
using BePalV2.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BePalV2.Screens;

public sealed class InventoryOverlayScreen : IScreen
{
    public bool IsOverlay => true;
    private readonly ScreenContext _ctx;
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;

    private int _selectedIndex = -1;

    // 1920x1080 Layout Constants
    private readonly Rectangle _panelRect = new(310, 120, 1300, 840);
    private readonly Rectangle _closeBtn = new(1520, 144, 60, 42);

    // Action Buttons
    private readonly Rectangle _useBtn = new(1070, 810, 220, 52);
    private readonly Rectangle _infoBtn = new(1320, 810, 220, 52);

    private string? _actionFeedback;
    private float _feedbackTimer;

    public InventoryOverlayScreen(ScreenContext ctx)
    {
        _ctx = ctx;
        for (int i = 0; i < InventoryService.MaxSlots; i++)
        {
            if (_ctx.Run.Inventory.Slots[i] != null)
            {
                _selectedIndex = i;
                break;
            }
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
                _actionFeedback = null;
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
            if (_closeBtn.Contains(mPos))
            {
                _ctx.Audio.PlayConfirm();
                _ctx.ScreenManager.PopOverlay();
                return;
            }

            var slots = _ctx.Run.Inventory.Slots;
            for (int i = 0; i < InventoryService.MaxSlots; i++)
            {
                int row = i / 4;
                int col = i % 4;
                Rectangle slotRect = new(_panelRect.X + 50 + col * 295, _panelRect.Y + 90 + row * 155, 280, 135);
                if (slotRect.Contains(mPos))
                {
                    _selectedIndex = i;
                    _ctx.Audio.PlayConfirm();
                    break;
                }
            }

            if (_selectedIndex >= 0 && _selectedIndex < InventoryService.MaxSlots && slots[_selectedIndex] != null)
            {
                var item = slots[_selectedIndex]!;
                if (_useBtn.Contains(mPos))
                {
                    if (item.Category == ItemCategory.Equipment)
                    {
                        bool equipped = _ctx.Run.Inventory.Equip(_selectedIndex);
                        if (equipped)
                        {
                            _actionFeedback = $"EQUIPPED: {item.Name} active on specimen!";
                            _feedbackTimer = 2.0f;
                            _ctx.Audio.PlaySuccess();
                        }
                    }
                    else
                    {
                        UseConsumableItem(item, _selectedIndex);
                    }
                }
                else if (_infoBtn.Contains(mPos))
                {
                    _actionFeedback = $"{item.Name}: {item.Description}";
                    _feedbackTimer = 3.0f;
                    _ctx.Audio.PlayConfirm();
                }
            }
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void UseConsumableItem(ItemDefinition item, int slotIndex)
    {
        var run = _ctx.Run;
        var pet = run.ActivePet;

        if (item.HealthRestore > 0) pet.Heal(item.HealthRestore);
        if (item.StomachRestore > 0) pet.FeedDirect(item.StomachRestore);
        if (item.CleanRestore > 0) pet.CleanDirect(item.CleanRestore);
        if (item.ExpGain > 0) pet.AddExp(item.ExpGain);

        run.Inventory.RemoveItemAt(slotIndex);
        _actionFeedback = $"DEPLOYED: {item.Name}! Specimen stats restored.";
        _feedbackTimer = 2.5f;
        _ctx.Audio.PlaySuccess();
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.85f);

        // Frame
        CleanUI.DrawPanel(batch, _panelRect, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(_panelRect.X, _panelRect.Y, _panelRect.Width, 4), UITheme.AccentEmerald);

        batch.DrawString(_ctx.Font, "FACILITY INVENTORY & SPECIMEN GEAR", new Vector2(_panelRect.X + 40, _panelRect.Y + 28), UITheme.AccentEmerald, 0f, Vector2.Zero, 1.35f, SpriteEffects.None, 0f);

        string eqName = _ctx.Run.Inventory.EquippedItem?.Name ?? "None";
        batch.DrawString(_ctx.Font, $"Equipped Gear: {eqName}", new Vector2(_panelRect.X + 680, _panelRect.Y + 30), UITheme.AccentGold, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);

        Point mPos = Mouse.GetState().Position;

        // Close Button
        CleanUI.DrawButton(batch, _ctx.Font, _closeBtn, "X", _closeBtn.Contains(mPos), accent: UITheme.BorderSubtle, hotkey: null);

        var slots = _ctx.Run.Inventory.Slots;

        // Draw 8 Inventory Slots (2 Rows x 4 Columns)
        for (int i = 0; i < InventoryService.MaxSlots; i++)
        {
            int row = i / 4;
            int col = i % 4;
            Rectangle slotRect = new(_panelRect.X + 50 + col * 295, _panelRect.Y + 90 + row * 155, 280, 135);

            var item = slots[i];
            bool isSelected = _selectedIndex == i;
            bool hovered = slotRect.Contains(mPos);

            Color bg = isSelected ? UITheme.BgPanelHover : (hovered ? new Color(28, 32, 42) : UITheme.BgCardRecessed);
            Color border = isSelected ? UITheme.AccentEmerald : (hovered ? UITheme.BorderHighlight : UITheme.BorderSubtle);

            CleanUI.DrawPanel(batch, slotRect, bg, border, borderWidth: isSelected ? 2 : 1, shadow: false);

            if (item != null)
            {
                batch.DrawString(_ctx.Font, item.Name, new Vector2(slotRect.X + 16, slotRect.Y + 14), UITheme.TextPrimary, 0f, Vector2.Zero, 1.15f, SpriteEffects.None, 0f);

                Color catCol = item.Category switch
                {
                    ItemCategory.Consumable => UITheme.AccentGold,
                    ItemCategory.Medicine => UITheme.AccentCoral,
                    ItemCategory.Equipment => UITheme.AccentPurple,
                    _ => UITheme.AccentEmerald
                };

                Rectangle catBadge = new(slotRect.X + 16, slotRect.Y + 48, 120, 26);
                CleanUI.DrawBadge(batch, _ctx.Font, catBadge, item.Category.ToString(), catCol * 0.25f, catCol);

                batch.DrawString(_ctx.Font, $"SLOT 0{i + 1}", new Vector2(slotRect.X + 16, slotRect.Bottom - 30), UITheme.TextMuted);
            }
            else
            {
                Vector2 empSz = _ctx.Font.MeasureString("[ EMPTY ]");
                batch.DrawString(_ctx.Font, "[ EMPTY ]", new Vector2(slotRect.Center.X - empSz.X / 2f, slotRect.Center.Y - empSz.Y / 2f), UITheme.TextMuted);
            }
        }

        // Details and Use Section (Cleanly spanning 1200px width)
        Rectangle detailBox = new(_panelRect.X + 50, _panelRect.Y + 430, 1200, 340);
        CleanUI.DrawPanel(batch, detailBox, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);

        if (_selectedIndex >= 0 && _selectedIndex < InventoryService.MaxSlots && slots[_selectedIndex] != null)
        {
            var item = slots[_selectedIndex]!;
            batch.DrawString(_ctx.Font, $"ITEM: {item.Name}", new Vector2(detailBox.X + 30, detailBox.Y + 24), UITheme.AccentGold, 0f, Vector2.Zero, 1.35f, SpriteEffects.None, 0f);
            batch.DrawString(_ctx.Font, $"Category: {item.Category}   |   Value: {item.Price} Gold", new Vector2(detailBox.X + 30, detailBox.Y + 68), UITheme.AccentCyan, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);
            batch.DrawString(_ctx.Font, item.Description, new Vector2(detailBox.X + 30, detailBox.Y + 110), UITheme.TextSecondary, 0f, Vector2.Zero, 1.05f, SpriteEffects.None, 0f);

            // Use / Equip Button (Cleanly contained inside detailBox)
            string useLabel = item.Category == ItemCategory.Equipment ? "EQUIP GEAR" : "DEPLOY ITEM";
            CleanUI.DrawButton(batch, _ctx.Font, _useBtn, useLabel, _useBtn.Contains(mPos), accent: UITheme.AccentEmerald, isPrimary: true);
            CleanUI.DrawButton(batch, _ctx.Font, _infoBtn, "INSPECT ITEM", _infoBtn.Contains(mPos), accent: UITheme.AccentCyan);
        }
        else
        {
            batch.DrawString(_ctx.Font, "Select an item slot from the inventory grid to view specifications and deploy.", new Vector2(detailBox.X + 30, detailBox.Y + 50), UITheme.TextMuted);
        }

        // Action Feedback Notification
        if (!string.IsNullOrEmpty(_actionFeedback))
        {
            batch.DrawString(_ctx.Font, _actionFeedback, new Vector2(_panelRect.X + 50, _panelRect.Bottom - 36), UITheme.AccentEmerald);
        }
        else
        {
            batch.DrawString(_ctx.Font, "Press [ ESC ] to return to sanctuary", new Vector2(_panelRect.X + 50, _panelRect.Bottom - 36), UITheme.TextMuted);
        }
    }
}
