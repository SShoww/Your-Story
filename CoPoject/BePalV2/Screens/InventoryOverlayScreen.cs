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
    private readonly Rectangle _panelRect = new(200, 60, 880, 600);
    private readonly Rectangle _closeBtn = new(1000, 76, 48, 36);

    // Action Buttons
    private readonly Rectangle _useBtn = new(660, 485, 170, 46);
    private readonly Rectangle _infoBtn = new(850, 485, 170, 46);

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
            if (_closeBtn.Contains(mPos) || !_panelRect.Contains(mPos))
            {
                _ctx.Audio.PlayConfirm();
                _ctx.ScreenManager.PopOverlay();
                return;
            }

            // Click slot
            for (int i = 0; i < InventoryService.MaxSlots; i++)
            {
                int row = i / 4;
                int col = i % 4;
                Rectangle slotRect = new(_panelRect.X + 40 + col * 195, _panelRect.Y + 75 + row * 125, 185, 110);
                if (slotRect.Contains(mPos))
                {
                    _selectedIndex = i;
                    _ctx.Audio.PlayConfirm();
                    break;
                }
            }

            // Action buttons
            var inv = _ctx.Run.Inventory;
            if (_selectedIndex >= 0 && _selectedIndex < inv.Slots.Count)
            {
                var item = inv.Slots[_selectedIndex];
                if (item != null)
                {
                    if (_useBtn.Contains(mPos))
                    {
                        if (item.Category == ItemCategory.Equipment)
                        {
                            inv.Equip(_selectedIndex);
                            _actionFeedback = $"Equipped {item.Name}!";
                            _feedbackTimer = 2.0f;
                            _ctx.Audio.PlaySuccess();
                        }
                        else
                        {
                            UseConsumableItem(item, _selectedIndex);
                        }
                    }
                    else if (_infoBtn.Contains(mPos))
                    {
                        _ctx.Audio.PlayConfirm();
                    }
                }
            }
        }

        _prevKeyboard = kbd;
        _prevMouse = mouse;
    }

    private void UseConsumableItem(ItemDefinition item, int slotIndex)
    {
        var pet = _ctx.Run.ActivePet;
        if (item.HealthRestore > 0) pet.Heal(item.HealthRestore);
        if (item.StomachRestore > 0) pet.ExecuteCareAction(CareActionType.Feed, PrecisionTier.Perfect);
        if (item.EnergyRestore > 0) _ctx.Run.Energy.AddBonus(item.EnergyRestore);
        if (item.DodgeZoneBonus > 0f) _ctx.Run.Inventory.ActiveDodgeBonus += item.DodgeZoneBonus;
        if (item.ShieldHits > 0) _ctx.Run.Inventory.ActiveShieldHits += item.ShieldHits;
        if (item.TrainExpMultiplier > 1f) _ctx.Run.Inventory.PermanentTrainExpMultiplier *= item.TrainExpMultiplier;

        _ctx.Run.Inventory.RemoveItemAt(slotIndex);
        _actionFeedback = $"Applied {item.Name} to {pet.Name}!";
        _feedbackTimer = 2.0f;
        _ctx.Audio.PlaySuccess();
        _selectedIndex = -1;
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        CleanUI.DrawModalBackdrop(batch, _ctx.ScreenWidth, _ctx.ScreenHeight, alpha: 0.8f);

        // Frame
        CleanUI.DrawPanel(batch, _panelRect, UITheme.BgPanel, UITheme.BorderLight, borderWidth: 1, shadow: true);
        batch.FillRectangle(new Rectangle(_panelRect.X, _panelRect.Y, _panelRect.Width, 3), UITheme.AccentEmerald);

        batch.DrawString(_ctx.Font, "FACILITY INVENTORY & SPECIMEN GEAR", new Vector2(_panelRect.X + 30, _panelRect.Y + 22), UITheme.AccentEmerald, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);

        string eqName = _ctx.Run.Inventory.EquippedItem?.Name ?? "None";
        batch.DrawString(_ctx.Font, $"Equipped: {eqName}", new Vector2(_panelRect.X + 460, _panelRect.Y + 24), UITheme.AccentGold);

        Point mPos = Mouse.GetState().Position;

        // Close Button
        CleanUI.DrawButton(batch, _ctx.Font, _closeBtn, "X", _closeBtn.Contains(mPos), accent: UITheme.BorderSubtle, hotkey: null);

        var slots = _ctx.Run.Inventory.Slots;

        // Draw 8 Inventory Slots (2 Rows x 4 Columns)
        for (int i = 0; i < InventoryService.MaxSlots; i++)
        {
            int row = i / 4;
            int col = i % 4;
            Rectangle slotRect = new(_panelRect.X + 40 + col * 195, _panelRect.Y + 75 + row * 125, 185, 110);

            var item = slots[i];
            bool isSelected = _selectedIndex == i;
            bool hovered = slotRect.Contains(mPos);

            Color bg = isSelected ? UITheme.BgPanelHover : (hovered ? new Color(28, 32, 42) : UITheme.BgCardRecessed);
            Color border = isSelected ? UITheme.AccentEmerald : (hovered ? UITheme.BorderHighlight : UITheme.BorderSubtle);

            CleanUI.DrawPanel(batch, slotRect, bg, border, borderWidth: isSelected ? 2 : 1, shadow: false);

            if (item != null)
            {
                batch.DrawString(_ctx.Font, item.Name, new Vector2(slotRect.X + 12, slotRect.Y + 12), UITheme.TextPrimary);

                Color catCol = item.Category switch
                {
                    ItemCategory.Consumable => UITheme.AccentGold,
                    ItemCategory.Medicine => UITheme.AccentCoral,
                    ItemCategory.Equipment => UITheme.AccentPurple,
                    _ => UITheme.AccentEmerald
                };

                Rectangle catBadge = new(slotRect.X + 12, slotRect.Y + 40, 95, 20);
                CleanUI.DrawBadge(batch, _ctx.Font, catBadge, item.Category.ToString(), catCol * 0.25f, catCol);

                batch.DrawString(_ctx.Font, $"SLOT 0{i + 1}", new Vector2(slotRect.X + 12, slotRect.Bottom - 26), UITheme.TextMuted);
            }
            else
            {
                Vector2 empSz = _ctx.Font.MeasureString("[ EMPTY ]");
                batch.DrawString(_ctx.Font, "[ EMPTY ]", new Vector2(slotRect.Center.X - empSz.X / 2f, slotRect.Center.Y - empSz.Y / 2f), UITheme.TextMuted);
            }
        }

        // Details and Use Section
        Rectangle detailBox = new(_panelRect.X + 40, _panelRect.Y + 345, 600, 210);
        CleanUI.DrawPanel(batch, detailBox, UITheme.BgCardRecessed, UITheme.BorderSubtle, borderWidth: 1, shadow: false);

        if (_selectedIndex >= 0 && _selectedIndex < InventoryService.MaxSlots && slots[_selectedIndex] != null)
        {
            var item = slots[_selectedIndex]!;
            batch.DrawString(_ctx.Font, $"ITEM: {item.Name}", new Vector2(detailBox.X + 20, detailBox.Y + 18), UITheme.AccentGold, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
            batch.DrawString(_ctx.Font, $"Category: {item.Category}   |   Value: {item.Price} G", new Vector2(detailBox.X + 20, detailBox.Y + 54), UITheme.AccentCyan);
            batch.DrawString(_ctx.Font, item.Description, new Vector2(detailBox.X + 20, detailBox.Y + 86), UITheme.TextSecondary);

            // Use / Equip Button
            string useLabel = item.Category == ItemCategory.Equipment ? "EQUIP GEAR" : "DEPLOY ITEM";
            CleanUI.DrawButton(batch, _ctx.Font, _useBtn, useLabel, _useBtn.Contains(mPos), accent: UITheme.AccentEmerald, isPrimary: true);
            CleanUI.DrawButton(batch, _ctx.Font, _infoBtn, "INSPECT", _infoBtn.Contains(mPos), accent: UITheme.AccentCyan);
        }
        else
        {
            batch.DrawString(_ctx.Font, "Select an item slot from the inventory to view specifications.", new Vector2(detailBox.X + 20, detailBox.Y + 40), UITheme.TextMuted);
        }

        // Action Feedback Notification
        if (!string.IsNullOrEmpty(_actionFeedback))
        {
            batch.DrawString(_ctx.Font, _actionFeedback, new Vector2(_panelRect.X + 40, _panelRect.Bottom - 28), UITheme.AccentEmerald);
        }
        else
        {
            batch.DrawString(_ctx.Font, "Press [ ESC ] to return to shelter", new Vector2(_panelRect.X + 40, _panelRect.Bottom - 28), UITheme.TextMuted);
        }
    }
}
