using System;
using DungeonMaster.Enums;

namespace DungeonMaster.Models.Items.Equipment;

public partial class WoodenEnchantedShoulders : BaseArmor
{
	public override string FluffContent => "Hm, mal schauen, ob das geht..";
	public override string FluffAuthor  => "Nytra";
	public override string FluffDate    => "2023";

	public override void Use(BaseUnit actor) => throw new NotImplementedException();

	public override EquipSlot EquipSlot => EquipSlot.Shoulders;
}