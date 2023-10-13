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

        var slot = rng.Next(0, 31);
        Inventory.Slots[slot].ContainedItem    = healthPotion;
        Inventory.Slots[slot].CurrentStacksize = rng.Next(1,11);

        var healthPotion2 = new HealthPotion
        {
            AmountHealed = 5,
            MaxStacksize = 10
        };

        slot                                   = rng.Next(0, 31);
        Inventory.Slots[slot].ContainedItem    = healthPotion2;
        Inventory.Slots[slot].CurrentStacksize = rng.Next(1, 11);

        var healthPotion3 = new HealthPotion
        {
            AmountHealed = 5,
            MaxStacksize = 10
        };

        slot                                   = rng.Next(0, 31);
        Inventory.Slots[slot].ContainedItem    = healthPotion3;
        Inventory.Slots[slot].CurrentStacksize = rng.Next(1, 11);
    }
}