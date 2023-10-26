using System.Linq;
using DungeonMaster.Models;
using Godot;

namespace DungeonMaster.UI;

public partial class InitiativeContainer : HBoxContainer
{
    private PackedScene initiativeSlotScene;

    public override void _Ready() => initiativeSlotScene = ResourceLoader.Load<PackedScene>("res://UI/initiative_slot.tscn");

    public void AddParticipant(BaseUnit unit, bool slotVisible = false)
    {
        var slotInstance = initiativeSlotScene.Instantiate<InitiativeSlot>();
        slotInstance.Initialize();
        slotInstance.AssignUnit(unit);

        slotInstance.GetNode<PanelContainer>("%Initiative")
                    .Visible = slotVisible;

        AddChild(slotInstance);
    }

    public void OrderParticipants()
    {
        var participants = this.GetAllChildren<InitiativeSlot>()
                               .Select(s => s.AssignedUnit)
                               .ToArray();

        var orderedParticipants = QuicksortParticipants(participants, 0, participants.Length - 1);

        Clear();

        foreach (var participant in orderedParticipants)
            AddParticipant(participant, true);
    }

    public void EnlargeNextInLine() { }

    public void RevertEnlargement() { }

    public void Clear()
    {
        foreach (var child in GetChildren())
            child.QueueFree();
    }

    public override void _Process(double delta) { }

    public BaseUnit[] QuicksortParticipants(BaseUnit[] array, int leftIndex, int rightIndex)
    {
        var i     = leftIndex;
        var j     = rightIndex;
        var pivot = array[leftIndex].ModifiedInitiative;

        while (i <= j)
        {
            while (array[i].ModifiedInitiative < pivot)
                i++;

            while (array[j].ModifiedInitiative > pivot)
                j--;

            if (i > j)
                continue;

            (array[i], array[j]) = (array[j], array[i]);
            i++;
            j--;
        }

        if (leftIndex < j)
            QuicksortParticipants(array, leftIndex, j);

        if (i < rightIndex)
            QuicksortParticipants(array, i, rightIndex);

        return array.Reverse().ToArray();
    }
}