using DungeonMaster.Enums;

namespace DungeonMaster.Models.Items.Equipment;

public partial class ShabbyPants : BaseArmor
{
    public override string FluffContent => "Worn and tattered, these pants are barely holding on.";
    public override string FluffAuthor { get; }
    public override string FluffDate { get; }

    public override EquipSlot EquipSlot => EquipSlot.Pants;

    public override void Use(BaseUnit actor)
    {
    }
}