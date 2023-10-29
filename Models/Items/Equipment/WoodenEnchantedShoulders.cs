using System;
using DungeonMaster.Enums;

namespace DungeonMaster.Models.Items.Equipment;

public partial class WoodenEnchantedShoulders : BaseArmor
{
	public override string FluffContent { get; }
	public override string FluffAuthor  { get; }
	public override string FluffDate    { get; }

	public override void Use(BaseUnit actor) => throw new NotImplementedException();

	public override EquipSlot EquipSlot { get; }
}