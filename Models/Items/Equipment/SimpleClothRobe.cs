using System;
using DungeonMaster.Enums;
using Godot;

namespace DungeonMaster.Models.Items.Equipment;

public partial class SimpleClothRobe : BaseArmor
{
    public SimpleClothRobe()
    {
        Icon = ResourceLoader.Load<Texture2D>("res://Graphics/Items/Armor/SimpleRobe.png");

    }

    public override void Use(BaseUnit actor) => throw new NotImplementedException();

    public override EquipSlot EquipSlot => EquipSlot.Torso;
}