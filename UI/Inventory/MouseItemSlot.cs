using DungeonMaster.Models.Items;
using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class MouseItemSlot : PanelContainer
{
    public int               CurrentStacksize { get; set; }
    public BaseItem          ContainedItem    { get; set; }
    public InventoryItemSlot SourceSlot       { get; set; }
}