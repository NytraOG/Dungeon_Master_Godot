using DungeonMaster.Models;
using Godot;

namespace DungeonMaster.UI;

public partial class InitiativeContainer : HBoxContainer
{
    private PackedScene initiativeSlotScene;

    public override void _Ready() => initiativeSlotScene = ResourceLoader.Load<PackedScene>("res://UI/initiative_slot.tscn");

    public void AddParticipant(BaseUnit unit)
    {
        var slotInstance = initiativeSlotScene.Instantiate<InitiativeSlot>();
        slotInstance.Initialize();
        slotInstance.AssignUnit(unit);

        AddChild(slotInstance);
    }

    public void Clear()
    {
        foreach (var child in GetChildren())
            child.QueueFree();
    }

    public override void _Process(double delta) { }
}