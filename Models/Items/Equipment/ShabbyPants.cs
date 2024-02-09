using Godot;
using System;
using DungeonMaster.Enums;
using DungeonMaster.Models;
using DungeonMaster.Models.Items.Equipment;

public partial class ShabbyPants : BaseArmor
{
	public override string FluffContent => "Worn and tattered, these pants are barely holding on.";
	public override string FluffAuthor { get; }
	public override string FluffDate { get; }
	public override void Use(BaseUnit actor)
	{
		
	}

	public override EquipSlot EquipSlot => EquipSlot.Pants;
}
