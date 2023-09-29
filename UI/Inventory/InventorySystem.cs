using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class InventorySystem : PanelContainer
{
    public InventoryItemSlot[] Slots;

    [Export]
    public GridContainer ItemGrid { get; set; }

    public PackedScene InventorySlotScene { get; set; }

    public void Initialize(int inventorySize)
    {
       //ItemGrid           = GetNode<GridContainer>("ItemGrid");
        InventorySlotScene = ResourceLoader.Load<PackedScene>("res://UI/Inventory/slot.tscn");
        Slots              = new InventoryItemSlot[inventorySize];

        for (var i = 0; i < inventorySize; i++)
        {
            var slot = InventorySlotScene.Instantiate<InventoryItemSlot>();
            Slots[i] = slot;
            ItemGrid.AddChild(slot);
        }
    }

    public override void _Process(double delta) { }
}