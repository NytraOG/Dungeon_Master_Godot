using DungeonMaster.Interfaces;
using DungeonMaster.Models.Items;
using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class MouseItemSlot : PanelContainer,
                                     IItemSlot
{
    public InventoryItemSlot SourceSlot       { get; set; }
    public int               CurrentStacksize { get; set; }
    public int               Id               { get; set; }
    public BaseItem          ContainedItem    { get; set; }

    public void UpdateData()
    {
        SetMouseItemTexture();
        SetStacksizeInPanel();
    }

    public void Clear()
    {
        ContainedItem    = null;
        CurrentStacksize = 0;

        var stacksizeLabel = GetNode<Label>("CurrentStackSize");
        stacksizeLabel.Text = "x99";
    }

    private void SetMouseItemTexture()
    {
        var rect = GetNode<TextureRect>("TextureRect");
        rect.Texture = ContainedItem?.Icon;
    }

    private void SetStacksizeInPanel()
    {
        var label = GetNode<Label>("CurrentStackSize");

        label.Text = ContainedItem is IStackable ? label.Text = "x" + CurrentStacksize : label.Text = string.Empty;
    }
}