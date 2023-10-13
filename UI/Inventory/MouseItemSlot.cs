using DungeonMaster.Models.Items;

namespace DungeonMaster.UI.Inventory;

public class MouseItemSlot
{
    public int      CurrentStacksize { get; set; }
    public BaseItem ContainedItem    { get; set; }
}