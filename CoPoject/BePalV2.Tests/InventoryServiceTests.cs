using BePalV2.Gameplay;
using Xunit;

namespace BePalV2.Tests;

public class InventoryServiceTests
{
    [Fact]
    public void Inventory_LimitsToEightSlots()
    {
        var inv = new InventoryService();
        Assert.Equal(8, InventoryService.MaxSlots);
        Assert.Equal(0, inv.Count);
        Assert.False(inv.IsFull);

        for (int i = 0; i < 8; i++)
        {
            bool added = inv.AddItem(ItemDefinition.CrabApple);
            Assert.True(added);
        }

        Assert.Equal(8, inv.Count);
        Assert.True(inv.IsFull);

        // Ninth item fails
        bool overflow = inv.AddItem(ItemDefinition.SeaTea);
        Assert.False(overflow);
    }

    [Fact]
    public void EquipAndUnequip_SwapsEquippedSlot()
    {
        var inv = new InventoryService();
        inv.AddItem(ItemDefinition.BalletShoes);

        Assert.Null(inv.EquippedItem);
        bool equipped = inv.Equip(0);
        Assert.True(equipped);
        Assert.Equal("ballet_shoes", inv.EquippedItem?.Id);
        Assert.Null(inv.Slots[0]); // slot 0 is now empty

        bool unequipped = inv.Unequip();
        Assert.True(unequipped);
        Assert.Null(inv.EquippedItem);
        Assert.Equal("ballet_shoes", inv.Slots[0]?.Id);
    }

    [Fact]
    public void ConsumeMedicine_RemovesFromSlot()
    {
        var inv = new InventoryService();
        inv.AddItem(ItemDefinition.FirstAidKit);
        Assert.True(inv.HasMedicine());

        bool consumed = inv.ConsumeMedicine(out var item);
        Assert.True(consumed);
        Assert.NotNull(item);
        Assert.Equal("first_aid_kit", item.Id);
        Assert.False(inv.HasMedicine());
    }
}
