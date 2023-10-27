using DungeonMaster.Enums;
using DungeonMaster.Models.Skills;

namespace DungeonMaster.Models.Items.Equipment;

public abstract partial class BaseEquipment : BaseItem
{
    public abstract EquipSlot EquipSlot    { get; }
    public          BaseSkill GrantedSkill { get; set; }

    public abstract void EquipOn(BaseUnit wearer);

    public abstract void UnequipFrom(BaseUnit wearer);
}