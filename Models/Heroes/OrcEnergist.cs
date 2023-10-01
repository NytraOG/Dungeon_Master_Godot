using DungeonMaster.Models.Items.Consumables;

namespace DungeonMaster.Models.Heroes;

public partial class OrcEnergist : Hero
{
    public override void _Ready()
    {
        base._Ready();

        var healthPotion = new HealthPotion
        {
            AmountHealed = 5,
            MaxStacksize = 10
        };

        Inventory.Slots[0].ContainedItem    = healthPotion;
        Inventory.Slots[0].CurrentStacksize = 4;
    }
}