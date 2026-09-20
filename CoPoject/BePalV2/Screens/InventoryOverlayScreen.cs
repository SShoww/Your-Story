using BePalV2.Audio;
using BePalV2.Gameplay;
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
    private readonly Rectangle _panelRect = new(240, 70, 800, 580);
    private readonly Rectangle _closeBtn = new(980, 85, 40, 36);

    // Slide 64 Action Buttons: "USE" and "INFO"
    private readonly Rectangle _useBtn = new(660, 490, 150, 44);
    private readonly Rectangle _infoBtn = new(830, 490, 150, 44);

    private bool _showInfoDetails = true;

    public InventoryOverlayScreen(ScreenContext ctx)
    {
        _ctx = ctx;
    }

    public void Update(GameTime gameTime)
    {
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

            // Check 8 slot clicks (2 rows x 4 cols)
            for (int i = 0; i < InventoryService.MaxSlots; i++)
            {
                int row = i / 4;
                int col = i % 4;
                Rectangle slotRect = new(_panelRect.X + 40 + col * 175, _panelRect.Y + 80 + row * 115, 155, 95);
                if (slotRect.Contains(mPos))
                {
                    _selectedIndex = i;
                    _ctx.Audio.PlayConfirm();
                    break;
                }
            }

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
                            _ctx.Audio.PlaySuccess();
                        }
                        else
                        {
                            UseConsumableItem(item, _selectedIndex);
                        }
                    }
                    else if (_infoBtn.Contains(mPos))
                    {
                        _showInfoDetails = !_showInfoDetails;
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
        _ctx.Audio.PlaySuccess();
        _selectedIndex = -1;
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        batch.FillRectangle(new Rectangle(0, 0, _ctx.ScreenWidth, _ctx.ScreenHeight), new Color(0, 0, 0, 190));

        // Slide 63/64 Inventory Frame
        batch.FillRectangle(_panelRect, new Color(24, 24, 28));
        batch.DrawRectangle(_panelRect, new Color(80, 80, 90), 2);

        // Header: "Use Item Day 3" (matching Slide 63/64)
        batch.DrawString(_ctx.Font, $"USE ITEM - DAY {_ctx.Run.DayNumber}", new Vector2(_panelRect.X + 30, _panelRect.Y + 22), Color.Gold, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        batch.DrawString(_ctx.Font, $"Equipped: {_ctx.Run.Inventory.EquippedItem?.Name ?? "None"}", new Vector2(_panelRect.X + 460, _panelRect.Y + 25), new Color(93, 176, 70));

        // Close Button
        batch.FillRectangle(_closeBtn, new Color(50, 50, 55));
        batch.DrawRectangle(_closeBtn, Color.White, 1);
        batch.DrawString(_ctx.Font, "X", new Vector2(_closeBtn.X + 13, _closeBtn.Y + 8), Color.White);

        // 8 Slots Grid (Slide 64 concept)
        var inv = _ctx.Run.Inventory;
        for (int i = 0; i < InventoryService.MaxSlots; i++)
        {
            int row = i / 4;
            int col = i % 4;
            Rectangle slotRect = new(_panelRect.X + 40 + col * 175, _panelRect.Y + 80 + row * 115, 155, 95);
            bool isSelected = i == _selectedIndex;

            Color slotBg = isSelected ? new Color(50, 50, 60) : new Color(34, 34, 40);
            batch.FillRectangle(slotRect, slotBg);
            batch.DrawRectangle(slotRect, isSelected ? Color.Gold : new Color(70, 70, 80), isSelected ? 2 : 1);

            var item = inv.Slots[i];
            if (item != null)
            {
                batch.DrawString(_ctx.Font, item.Name, new Vector2(slotRect.X + 10, slotRect.Y + 10), Color.White);
                batch.DrawString(_ctx.Font, $"[{item.Category}]", new Vector2(slotRect.X + 10, slotRect.Y + 36), new Color(160, 180, 210));
                batch.DrawString(_ctx.Font, $"{item.Price}G", new Vector2(slotRect.X + 10, slotRect.Y + 64), Color.Gold);
            }
            else
            {
                batch.DrawString(_ctx.Font, $"[ Empty {i + 1} ]", new Vector2(slotRect.X + 35, slotRect.Y + 38), new Color(80, 80, 90));
            }
        }

        // Details & Inspection Pane (Slide 64 concept)
        Rectangle detailsRect = new(_panelRect.X + 40, _panelRect.Y + 340, _panelRect.Width - 80, 130);
        batch.FillRectangle(detailsRect, new Color(20, 20, 24));
        batch.DrawRectangle(detailsRect, new Color(60, 60, 70), 1);

        if (_selectedIndex >= 0 && _selectedIndex < inv.Slots.Count && inv.Slots[_selectedIndex] != null)
        {
            var item = inv.Slots[_selectedIndex]!;
            batch.DrawString(_ctx.Font, item.Name, new Vector2(detailsRect.X + 16, detailsRect.Y + 12), Color.Gold, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
            batch.DrawString(_ctx.Font, item.Description, new Vector2(detailsRect.X + 16, detailsRect.Y + 45), Color.White);

            // Slide 64 buttons: "USE" and "INFO"
            string useLabel = item.Category == ItemCategory.Equipment ? "EQUIP" : "USE ITEM";
            batch.FillRectangle(_useBtn, new Color(40, 130, 70));
            batch.DrawRectangle(_useBtn, Color.White, 1);
            batch.DrawString(_ctx.Font, useLabel, new Vector2(_useBtn.Center.X - _ctx.Font.MeasureString(useLabel).X / 2f, _useBtn.Y + 12), Color.White);

            batch.FillRectangle(_infoBtn, new Color(60, 70, 90));
            batch.DrawRectangle(_infoBtn, Color.White, 1);
            batch.DrawString(_ctx.Font, "INFO", new Vector2(_infoBtn.Center.X - _ctx.Font.MeasureString("INFO").X / 2f, _infoBtn.Y + 12), Color.White);
        }
        else
        {
            batch.DrawString(_ctx.Font, "Select an item slot above to inspect properties, USE consumables, or EQUIP gear.", new Vector2(detailsRect.X + 24, detailsRect.Y + 50), new Color(130, 130, 140));
        }

        batch.DrawString(_ctx.Font, "Press [ ESC ] to close backpack", new Vector2(_panelRect.X + 40, _panelRect.Bottom - 35), new Color(120, 120, 130));
    }
}
