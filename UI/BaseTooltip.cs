using DungeonMaster.Models.Items;
using Godot;

namespace DungeonMaster.UI;

public abstract partial class BaseTooltip : PanelContainer
{
    public Label TooltipName { get; set; }
    public virtual void Show(BaseItem itemToShow)
    {
        TooltipName ??= TooltipName = GetNode<MarginContainer>("MarginContainer")
                                     .GetNode<VBoxContainer>("VBoxContainer")
                                     .GetNode<Label>("Name");

        TooltipName.Text = itemToShow?.Name;
        Visible          = true;
    }
}