using System;
using DungeonMaster.Enums;

namespace DungeonMaster.Models.Items.Equipment;

public partial class WoodenEnchantedShoulders : BaseArmor
{
	public override string FluffContent => "Hm, mal schauen, ob das geht..";
	public override string FluffAuthor  => "Nytra";
	public override string FluffDate    => "2023";

	public override void Use(BaseUnit actor) => throw new NotImplementedException();

	public override void EquipOn(BaseUnit wearer)
	{
		base.EquipOn(wearer);

		wearer.Constitution += 3;
	}

	public override void UnequipFrom(BaseUnit wearer)
	{
		base.UnequipFrom(wearer);

		wearer.Constitution -= 3;
	}

	public override EquipSlot EquipSlot => EquipSlot.Shoulders;
}