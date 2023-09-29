using DungeonMaster.Models.Items;
using DungeonMaster.Models.Items.Consumables;
using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class InventoryItemSlot : Node3D
{
    [Export]
    public Texture2D DefaultIcon { get; set; }

    public int      CurrentStacksize { get; set; }
    public BaseItem ContainedItem    { get; set; }

    public void AddToStack(int amount) => CurrentStacksize += amount;

    public void RemoveFromStack(int amount) => CurrentStacksize -= amount;

    public bool RoomLeftInStack(int amount, out int remainingStackSize)
    {
        remainingStackSize = 0;

        if (ContainedItem is BaseConsumable consumable)
            remainingStackSize = consumable.MaxStacksize - CurrentStacksize;

        return RoomLeftInStack(amount);
    }

    public bool RoomLeftInStack(int amount)
    {
        if (ContainedItem is BaseConsumable consumable)
            return CurrentStacksize + amount <= consumable.MaxStacksize;

        return ContainedItem is null;
    }

    public void ClearSlot()
    {
        ContainedItem    = null;
        CurrentStacksize = 0;
    }
}