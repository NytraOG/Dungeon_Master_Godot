using System;
using DungeonMaster.Models.Items.Consumables;

namespace DungeonMaster.Models.Heroes;

public partial class OrcEnergist : Hero
{
    public override void _Ready()
    {
        base._Ready();
        var rng = new Random();

        var healthPotion = new HealthPotion
        {
            AmountHealed = 5,
            MaxStacksize = 10
        };

        Inventory.Slots[0].ContainedItem    = healthPotion;
        Inventory.Slots[0].CurrentStacksize = rng.Next(1,11);

        var healthPotion2 = new HealthPotion
        {
            AmountHealed = 5,
            MaxStacksize = 10
        };

        Inventory.Slots[12].ContainedItem    = healthPotion2;
        Inventory.Slots[12].CurrentStacksize = rng.Next(1, 11);

        var healthPotion3 = new HealthPotion
        {
            AmountHealed = 5,
            MaxStacksize = 10
        };

        Inventory.Slots[29].ContainedItem    = healthPotion3;
        Inventory.Slots[29].CurrentStacksize = rng.Next(1, 11);
    }
}