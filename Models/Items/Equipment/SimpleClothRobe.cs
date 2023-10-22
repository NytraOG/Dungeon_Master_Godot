using System;
using DungeonMaster.Enums;
using Godot;

namespace DungeonMaster.Models.Items.Equipment;

public partial class SimpleClothRobe : BaseArmor
{
    public SimpleClothRobe() => Icon = ResourceLoader.Load<Texture2D>("res://Graphics/Items/Armor/SimpleRobe.png");

    public override EquipSlot EquipSlot => EquipSlot.Torso;

    public override void EquipOn(BaseUnit wearer) => Console.WriteLine("Cloth Robe applied Armor and Stats!");

    public override void Use(BaseUnit actor) => Console.WriteLine("Cloth Robe applied Buffs or whatever!");
}