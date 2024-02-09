using System;
using DungeonMaster.Enums;

namespace DungeonMaster.Models.Items.Equipment;

public partial class ShabbyBoots : BaseArmor
{

	public override string FluffContent => "Worn and scuffed, these boots are in need of repair.";
	public override string FluffAuthor { get; }
	public override string FluffDate { get; }
	public override void Use(BaseUnit actor)
	{
		throw new NotImplementedException();
	}

	public override EquipSlot EquipSlot => EquipSlot.Boots;
}