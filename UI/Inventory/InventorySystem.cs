using System.Collections.Generic;
using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class InventorySystem : PanelContainer
{
    private bool                                  buttonHeldDown;
    public  double                                InventoryDisplayCooldown;
    public  Dictionary<string, InventoryItemSlot> Slots = new();

    [Export]
    public GridContainer ItemGrid { get; set; }

    public PackedScene InventorySlotScene { get; set; }

    public void Initialize(int inventorySize)
    {
        InventorySlotScene = ResourceLoader.Load<PackedScene>("res://UI/Inventory/slot.tscn");

        for (var i = 0; i < inventorySize; i++)
        {
            var slot = InventorySlotScene.Instantiate<InventoryItemSlot>();
            slot.Id = i.ToString();
            Slots.Add(slot.Id, slot);
            ItemGrid.AddChild(slot);
        }
    }

    public InventoryItemSlot FindFirstEmptySlot()
    {
        foreach (var slot in Slots)
        {
            if (slot.Value is null)
                return slot.Value;
        }

        return null;
    }

    public override void _Process(double delta)
    {
        InventoryDisplayCooldown -= delta;

        if (Input.IsKeyPressed(Key.B) && InventoryDisplayCooldown <= 0)
        {
            Visible                  = !Visible;
            InventoryDisplayCooldown = 0.25;
        }
    }

    public void _on_gui_input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButtonEvent)
            buttonHeldDown = mouseButtonEvent.Pressed;

        if (buttonHeldDown && @event is InputEventMouseMotion motionEvent)
            Position = motionEvent.GlobalPosition;
    }
}