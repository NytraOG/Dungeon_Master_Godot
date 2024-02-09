using Godot;
using System;
using DungeonMaster.Enums;
using DungeonMaster.Models;
using DungeonMaster.Models.Items.Equipment;

public partial class ShabbyPants : BaseArmor
{
	public override string FluffContent { get; }
	public override string FluffAuthor { get; }
	public override string FluffDate { get; }
	public override void Use(BaseUnit actor)
	{
		
	}

	public override EquipSlot EquipSlot => EquipSlot.Pants;
}
