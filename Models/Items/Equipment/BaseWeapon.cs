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
        GrantedSkill = GrantedSkillScene.Instantiate<BaseSkill>();
        wearer.Skills.Add(GrantedSkill);
    }

    public override void UnequipFrom(BaseUnit wearer)
    {
        if (GrantedSkill is not null)
            wearer.Skills.Remove(GrantedSkill);
    }
}