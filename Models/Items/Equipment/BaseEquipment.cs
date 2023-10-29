using DungeonMaster.Enums;
using DungeonMaster.Models.Skills;
using Godot;

namespace DungeonMaster.Models.Items.Equipment;

public abstract partial class BaseEquipment : BaseItem
{
    [Export]
    public PackedScene GrantedSkillScene { get; set; }

    public          BaseSkill GrantedSkill { get; set; }
    public abstract EquipSlot EquipSlot    { get; }

    public abstract void EquipOn(BaseUnit wearer);

    public abstract void UnequipFrom(BaseUnit wearer);
}