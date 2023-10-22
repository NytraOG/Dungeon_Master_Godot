using DungeonMaster.Enums;

namespace DungeonMaster.Models.Items.Equipment;

public abstract partial class BaseEquipment : BaseItem
{
    public abstract EquipSlot EquipSlot { get; }

    public abstract void EquipOn(BaseUnit wearer);
    public abstract void UnequipFrom(BaseUnit wearer);
}