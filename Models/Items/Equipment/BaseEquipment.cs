using DungeonMaster.Enums;

namespace DungeonMaster.Models.Items.Equipment;

public abstract partial class BaseEquipment : BaseItem
{
    public abstract EquipSlot EquipSlot { get; }
}