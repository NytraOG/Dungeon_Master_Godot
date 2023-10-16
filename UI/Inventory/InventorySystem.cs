using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class InventorySystem : PanelContainer
{
    public delegate void InventoryClicked(InventorySystem sender);

    public delegate void InventoryClickRelease(InventorySystem sender);

    public InventoryItemSlot[] Slots;

    [Export]
    public GridContainer ItemGrid { get; set; }

    public PackedScene                 InventorySlotScene { get; set; }
    public event InventoryClicked      OnInventoryClicked;
    public event InventoryClickRelease OnInventoryClickRelease;

    public void Initialize(int inventorySize)
    {
        InventorySlotScene = ResourceLoader.Load<PackedScene>("res://UI/Inventory/slot.tscn");
        Slots              = new InventoryItemSlot[inventorySize];

        for (var i = 0; i < inventorySize; i++)
        {
            var slot = InventorySlotScene.Instantiate<InventoryItemSlot>();
            slot.Id  = i;
            Slots[i] = slot;
            ItemGrid.AddChild(slot);
        }
    }

    public InventoryItemSlot FindFirstEmptySlot()
    {
        foreach (var slot in Slots)
        {
            if (slot.ContainedItem is null)
                return slot;
        }

        return null;
    }

    public override void _Process(double delta) { }

    public void _on_gui_input(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseButton { Pressed: true }:  OnInventoryClicked?.Invoke(this);
                break;
            case InputEventMouseButton { Pressed: false }: OnInventoryClickRelease?.Invoke(this);
                break;
        }
    }
}