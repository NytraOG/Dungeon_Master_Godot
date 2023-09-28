using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class InventorySystem : Node3D
{
    public InventoryItemSlot[] Slots;

    public void Initialize(int inventorySize) => Slots = new InventoryItemSlot[inventorySize];

    public override void _Process(double delta) { }
}