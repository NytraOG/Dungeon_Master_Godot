using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class InventorySystem : Node3D
{
    public InventoryItemSlot[] Slots;

    public void Initialize(int inventorySize)
    {
        Slots = new InventoryItemSlot[inventorySize];

        for (var i = 0; i < inventorySize; i++)
            Slots[i] = new InventoryItemSlot();
    }

    public override void _Process(double delta) { }
}