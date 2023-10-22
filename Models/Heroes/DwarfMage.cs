using DungeonMaster.Models.Items.Consumables;
using DungeonMaster.Models.Items.Equipment;

namespace DungeonMaster.Models.Heroes;

public partial class DwarfMage : Hero
{
    public override void _Ready()
    {
        base._Ready();

        var potion = new HealthPotion();
        potion.MaxStacksize                   = 10;
        Inventory.Slots["0"].ContainedItem    = potion;
        Inventory.Slots["0"].CurrentStacksize = 2;

        var armor = new SimpleClothRobe();
        Inventory.Slots["1"].ContainedItem    = armor;
        Inventory.Slots["1"].CurrentStacksize = 1;
    }
}