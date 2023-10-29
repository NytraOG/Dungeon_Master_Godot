using System;
using DungeonMaster.Models.Items.Consumables;
using DungeonMaster.Models.Items.Equipment;
using DungeonMaster.Models.Items.Equipment.Weapons;
using Godot;

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
        Inventory.Slots[slot.ToString()].ContainedItem    = healthPotion;
        Inventory.Slots[slot.ToString()].CurrentStacksize = rng.Next(1, 11);

        var healthPotion3 = new HealthPotion
        {
            AmountHealed = 5,
            MaxStacksize = 10
        };

        slot                                              = rng.Next(0, 31);
        Inventory.Slots[slot.ToString()].ContainedItem    = healthPotion3;
        Inventory.Slots[slot.ToString()].CurrentStacksize = rng.Next(7, 11);

        var swordHilt = ResourceLoader.Load<PackedScene>("res://Models/Items/Equipment/Weapons/broken_sword_hilt.tscn")
                                      .Instantiate<BrokenSwordHilt>();
        swordHilt.Initialize();
        Inventory.Slots["0"].ContainedItem = swordHilt;

        var natureShoulders = ResourceLoader.Load<PackedScene>("res://Models/Items/Equipment/wooden_enchanted_shoulders.tscn")
                                            .Instantiate<WoodenEnchantedShoulders>();
        Inventory.Slots["1"].ContainedItem = natureShoulders;
    }
}