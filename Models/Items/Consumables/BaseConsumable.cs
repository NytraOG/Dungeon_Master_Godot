using DungeonMaster.Interfaces;

namespace DungeonMaster.Models.Items.Consumables;

public abstract partial class BaseConsumable : BaseItem,
                                               IStackable
{
    public int MaxStacksize     { get; set; }

    public abstract void Consume(BaseUnit imbiber);
}