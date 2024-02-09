using System;
using DungeonMaster.Enums;

namespace DungeonMaster.Models.Items.Equipment.Weapons;

public partial class ChainWhip : BaseWeapon
{
	public override string FluffContent =>
		"A whip consisting of several small blades, which can be contracted into a sword by magnetic fields";
	public override string FluffAuthor { get; }
	public override string FluffDate { get; }
	public override void Use(BaseUnit actor)
	{
		throw new NotImplementedException();
	}

	public override EquipSlot EquipSlot { get; }
	public override void Initialize()
	{
		throw new NotImplementedException();
	} 
}