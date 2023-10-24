using System.Collections.Generic;
using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class InventorySystem : PanelContainer
{
    private bool                                  buttonHeldDown;
    public  double                                InventoryDisplayCooldown;
    private Vector2                               mousePositionNew;
    private Vector2                               mousePositionOld;
    private Vector2                               mouseVelocity;
    private Vector2                               position;
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
            if (slot.Value?.ContainedItem is null)
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

    public override void _PhysicsProcess(double delta)
    {
        if (buttonHeldDown)
            SetPosition((Position + (mousePositionNew - mousePositionOld)));
    }

    public void _on_gui_input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButtonEvent)
            buttonHeldDown = mouseButtonEvent.Pressed;

        if (!buttonHeldDown || @event is not InputEventMouseMotion motionEvent)
            return;

        if (mousePositionOld == Vector2.Zero && mousePositionNew == Vector2.Zero)
            mousePositionOld = motionEvent.GlobalPosition;
        else if (mousePositionOld != Vector2.Zero && mousePositionNew != Vector2.Zero)
        {
            mousePositionOld = mousePositionNew;
            mousePositionNew = motionEvent.GlobalPosition;
        }
        else
            mousePositionNew = motionEvent.GlobalPosition;

        mouseVelocity = motionEvent.Velocity;
        position      = motionEvent.GlobalPosition - GlobalPosition;
    }
}