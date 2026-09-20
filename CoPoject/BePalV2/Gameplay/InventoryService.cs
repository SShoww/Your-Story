namespace BePalV2.Gameplay;

public sealed class InventoryService
{
    public const int MaxSlots = 8;
    private readonly ItemDefinition?[] _slots = new ItemDefinition?[MaxSlots];

    public IReadOnlyList<ItemDefinition?> Slots => _slots;
    public ItemDefinition? EquippedItem { get; private set; }

    public float ActiveDodgeBonus { get; set; }
    public int ActiveShieldHits { get; set; }
    public float PermanentTrainExpMultiplier { get; set; } = 1.0f;

    public int Count => _slots.Count(s => s != null);
    public bool IsFull => Count >= MaxSlots;

    public bool AddItem(ItemDefinition item)
    {
        for (int i = 0; i < MaxSlots; i++)
        {
            if (_slots[i] == null)
            {
                _slots[i] = item;
                return true;
            }
        }
        return false;
    }

    public ItemDefinition? RemoveItemAt(int index)
    {
        if (index < 0 || index >= MaxSlots) return null;
        var item = _slots[index];
        _slots[index] = null;
        return item;
    }

    public bool HasItem(string id) => _slots.Any(item => item?.Id == id);

    public bool HasMedicine() => _slots.Any(item => item?.Category == ItemCategory.Medicine);

    public bool ConsumeItem(string id, out ItemDefinition? consumed)
    {
        for (int i = 0; i < MaxSlots; i++)
        {
            if (_slots[i]?.Id == id)
            {
                consumed = _slots[i];
                _slots[i] = null;
                return true;
            }
        }
        consumed = null;
        return false;
    }

    public bool ConsumeMedicine(out ItemDefinition? medicine)
    {
        for (int i = 0; i < MaxSlots; i++)
        {
            if (_slots[i]?.Category == ItemCategory.Medicine)
            {
                medicine = _slots[i];
                _slots[i] = null;
                return true;
            }
        }
        medicine = null;
        return false;
    }

    public bool Equip(int index)
    {
        if (index < 0 || index >= MaxSlots) return false;
        var item = _slots[index];
        if (item == null || item.Category != ItemCategory.Equipment) return false;

        var previousEquipped = EquippedItem;
        EquippedItem = item;
        _slots[index] = previousEquipped;
        return true;
    }

    public bool Unequip()
    {
        if (EquippedItem == null || IsFull) return false;
        AddItem(EquippedItem);
        EquippedItem = null;
        return true;
    }

    public void ResetDailyCombatBuffs()
    {
        ActiveDodgeBonus = 0f;
    }
}
