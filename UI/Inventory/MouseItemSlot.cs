using DungeonMaster.Models.Items;
using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class MouseItemSlot : PanelContainer
{
    public int               CurrentStacksize { get; set; }
    public BaseItem          ContainedItem    { get; set; }
    public InventoryItemSlot SourceSlot       { get; set; }

    public void SetData()
    {
        SetMouseItemTexture();
        SetStacksizeInPanel();
    }

    public void Clear()
    {
        ContainedItem       = null;
        CurrentStacksize    = 0;

        var stacksizeLabel = GetNode<Label>("CurrentStacksize");
        stacksizeLabel.Text    = "x99";
    }

    private void SetMouseItemTexture()
    {
        var rect = GetNode<TextureRect>("TextureRect");
        rect.Texture = ContainedItem.Icon;
    }

    private void SetStacksizeInPanel()
    {
        var label = GetNode<Label>("CurrentStackSize");
        label.Text = "x" + CurrentStacksize;
    }
}