using DungeonMaster.Interfaces;

namespace DungeonMaster.Models.Items.Consumables;

public partial class BaseConsumable : BaseItem,
                                      IStackable
{
    public int MaxStacksize { get; set; }
}