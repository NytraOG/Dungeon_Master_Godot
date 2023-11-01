﻿using DungeonMaster.Enums;
using DungeonMaster.Models.Items.Crafting;
using DungeonMaster.Models.Skills;
using Godot;

namespace DungeonMaster.Models.Items.Equipment;

public abstract partial class BaseWeapon : BaseEquipment
{
    [Export]
    public DamageType Damagetype { get; set; }

    [Export]
    public int AddedDamageNormal { get; set; }

    [Export]
    public int AddedDamageGood { get; set; }

    [Export]
    public int AddedDamageCritical { get; set; }

    [Export]
    public double ApCost { get; set; }

    [Export]
    public BaseMaterial NeededMaterial { get; set; }

    [Export]
    public int NeededAmountOfMaterial { get; set; }

    public abstract void Initialize();

    public override void EquipOn(BaseUnit wearer)
    {
        base.EquipOn(wearer);

        if (GrantedSkill is null)
            return;

        var copy = (BaseSkill)GrantedSkill.Duplicate();

        wearer.Skills.Add(copy);
    }

    public override void UnequipFrom(BaseUnit wearer)
    {
        base.UnequipFrom(wearer);

        if (GrantedSkill is null)
            return;

        if (GrantedSkill is not null)
            wearer.Skills.Remove(GrantedSkill);
    }
}