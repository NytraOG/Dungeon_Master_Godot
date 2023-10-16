using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class InventorySystem : PanelContainer
{
    private bool                buttonHeldDown;
    public  InventoryItemSlot[] Slots;

    [Export]
    public GridContainer ItemGrid { get; set; }

    public PackedScene InventorySlotScene { get; set; }

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
        if (@event is InputEventMouseButton mouseButtonEvent)
            buttonHeldDown = mouseButtonEvent.Pressed;

        if (buttonHeldDown && @event is InputEventMouseMotion motionEvent)
            Position = motionEvent.Position;
    }
}