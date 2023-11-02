using System.Text;
using DungeonMaster.Enums;
using DungeonMaster.Models.Items.Crafting;
using DungeonMaster.Models.Skills;
using DungeonMaster.UI;
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

    public override string GetContentTooltip()
    {
        var emil = new StringBuilder();
        emil.AppendLine($"Added Damage \t[{GetVorzeichen(AddedDamageNormal)}{AddedDamageNormal}/[color={CombatLog.GoodDamageColor}]{GetVorzeichen(AddedDamageGood)}{AddedDamageGood}[/color]/[color={CombatLog.CriticalDamageColor}]{GetVorzeichen(AddedDamageCritical)}{AddedDamageCritical}[/color]]\t");
                  //      $"Fire\t\t\t\t[{GetVorzeichen(FireNormal)}{FireNormal}/[color={CombatLog.GoodDamageColor}]{GetVorzeichen(FireGood)}{FireGood}[/color]/[color={CombatLog.CriticalDamageColor}]{GetVorzeichen(FireCritical)}{FireCritical}[/color]]");
        //
        // emil.AppendLine($"Pierce [{GetVorzeichen(PierceNormal)}{PierceNormal}/[color={CombatLog.GoodDamageColor}]{GetVorzeichen(PierceGood)}{PierceGood}[/color]/[color={CombatLog.CriticalDamageColor}]{GetVorzeichen(PierceCritical)}{PierceCritical}[/color]]\t" +
        //                 $"Ice\t\t\t\t[{GetVorzeichen(IceNormal)}{IceNormal}/[color={CombatLog.GoodDamageColor}]{GetVorzeichen(IceGood)}{IceGood}[/color]/[color={CombatLog.CriticalDamageColor}]{GetVorzeichen(IceCritical)}{IceCritical}[/color]]");
        //
        // emil.AppendLine($"Slash \t[{GetVorzeichen(SlashNormal)}{SlashNormal}/[color={CombatLog.GoodDamageColor}]{GetVorzeichen(SlashGood)}{SlashGood}[/color]/[color={CombatLog.CriticalDamageColor}]{GetVorzeichen(SlashCritical)}{SlashCritical}[/color]]\t" +
        //                 $"Lightning\t[{GetVorzeichen(LightningNormal)}{LightningNormal}/[color={CombatLog.GoodDamageColor}]{GetVorzeichen(LightningGood)}{LightningGood}[/color]/[color={CombatLog.CriticalDamageColor}]{GetVorzeichen(LightningCritical)}{LightningCritical}[/color]]");

        return emil.ToString();
    }

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