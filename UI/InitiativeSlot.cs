using DungeonMaster.Models;
using Godot;

namespace DungeonMaster.UI;

public partial class InitiativeSlot : PanelContainer
{
    public TextureRect UnitIcon        { get; set; }
    public Label       InitiativeValue { get; set; }
    public Label       NameLabel       { get; set; }
    public BaseUnit    AssignedUnit    { get; private set; }

    public void Initialize()
    {
        UnitIcon        = GetNode<PanelContainer>("%Unit").GetNode<TextureRect>("Icon");
        InitiativeValue = GetNode<PanelContainer>("%Initiative").GetNode<Label>("Value");
        NameLabel       = GetNode<Label>("Name");
    }

    public void AssignUnit(BaseUnit unit)
    {
        AssignedUnit         = unit;
        InitiativeValue.Text = unit.ModifiedInitiative.ToString("N0");
        NameLabel.Text       = unit.Name;
        //UnitIcon.Texture = unit.Icon;
    }
}