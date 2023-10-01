using DungeonMaster.Models.Items.Equipment;

namespace DungeonMaster.Models.Heroes;

public partial class DwarfMage : Hero
{
    public override void _Ready()
    {
        base._Ready();

        var armor = new SimpleClothRobe();
        Inventory.Slots[1].ContainedItem    = armor;
        Inventory.Slots[1].CurrentStacksize = 1;
    }
}