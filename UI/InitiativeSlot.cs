using DungeonMaster.Models;
using Godot;

namespace DungeonMaster.UI;

public partial class InitiativeSlot : PanelContainer
{
    public TextureRect UnitIcon        { get; set; }
    public Label       InitiativeValue { get; set; }
    public BaseUnit    AssignedUnit    { get; set; }

    public override void _Ready()
    {
        UnitIcon        = GetNode<PanelContainer>("%Unit").GetNode<TextureRect>("Icon");
        InitiativeValue = GetNode<PanelContainer>("%Initiative").GetNode<Label>("Value");
    }

    public void AssignUnit(BaseUnit unit)
    {
        AssignedUnit         = unit;
        InitiativeValue.Text = unit.ModifiedInitiative.ToString("N0");
        //UnitIcon.Texture = unit.Icon;
    }
}