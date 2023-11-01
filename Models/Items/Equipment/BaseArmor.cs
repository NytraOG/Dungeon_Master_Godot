using System.Linq;
using System.Text;
using DungeonMaster.Models.Skills;
using DungeonMaster.UI;
using Godot;

namespace DungeonMaster.Models.Items.Equipment;

public abstract partial class BaseArmor : BaseEquipment
{
    [ExportGroup("Resistances")]
    [Export]
    public int SlashNormal { get; set; }

    [Export]
    public int SlashGood { get; set; }

    [Export]
    public int SlashCritical { get; set; }

    [Export]
    public int PierceNormal { get; set; }

    [Export]
    public int PierceGood { get; set; }

    [Export]
    public int PierceCritical { get; set; }

    [Export]
    public int CrushNormal { get; set; }

    [Export]
    public int CrushGood { get; set; }

    [Export]
    public int CrushCritical { get; set; }

    [Export]
    public int FireNormal { get; set; }

    [Export]
    public int FireGood { get; set; }

    [Export]
    public int FireCritical { get; set; }

    [Export]
    public int IceNormal { get; set; }

    [Export]
    public int IceGood { get; set; }

    [Export]
    public int IceCritical { get; set; }

    [Export]
    public int LightningNormal { get; set; }

    [Export]
    public int LightningGood { get; set; }

    [Export]
    public int LightningCritical { get; set; }

    public override void EquipOn(BaseUnit wearer)
    {
        base.EquipOn(wearer);

        wearer.ArmorCrushNormal   += CrushNormal;
        wearer.ArmorCrushGood     += CrushGood;
        wearer.ArmorCrushCritical += CrushCritical;

        wearer.ArmorSlashNormal   += SlashNormal;
        wearer.ArmorSlashGood     += SlashGood;
        wearer.ArmorSlashCritical += SlashCritical;

        wearer.ArmorPierceNormal   += PierceNormal;
        wearer.ArmorPierceGood     += PierceGood;
        wearer.ArmorPierceCritical += PierceCritical;

        wearer.ArmorFireNormal   += FireNormal;
        wearer.ArmorFireGood     += FireGood;
        wearer.ArmorFireCritical += FireCritical;

        wearer.ArmorIceNormal   += IceNormal;
        wearer.ArmorIceGood     += IceGood;
        wearer.ArmorIceCritical += IceCritical;

        wearer.ArmorLightningNormal   += LightningNormal;
        wearer.ArmorLightningGood     += LightningGood;
        wearer.ArmorLightningCritical += LightningCritical;

        if (GrantedSkill is null)
            return;

        var newInstance = (BaseSkill)GrantedSkill.Duplicate();
        wearer.Skills.Add(newInstance);

        if (newInstance is BaseDefenseSkill defenseSkill)
            defenseSkill.ApplyPassiveEffects(wearer);
    }

    public override void UnequipFrom(BaseUnit wearer)
    {
        wearer.ArmorCrushNormal   -= CrushNormal;
        wearer.ArmorCrushGood     -= CrushGood;
        wearer.ArmorCrushCritical -= CrushCritical;

        wearer.ArmorSlashNormal   -= SlashNormal;
        wearer.ArmorSlashGood     -= SlashGood;
        wearer.ArmorSlashCritical -= SlashCritical;

        wearer.ArmorPierceNormal   -= PierceNormal;
        wearer.ArmorPierceGood     -= PierceGood;
        wearer.ArmorPierceCritical -= PierceCritical;

        wearer.ArmorFireNormal   -= FireNormal;
        wearer.ArmorFireGood     -= FireGood;
        wearer.ArmorFireCritical -= FireCritical;

        wearer.ArmorIceNormal   -= IceNormal;
        wearer.ArmorIceGood     -= IceGood;
        wearer.ArmorIceCritical -= IceCritical;

        wearer.ArmorLightningNormal   -= LightningNormal;
        wearer.ArmorLightningGood     -= LightningGood;
        wearer.ArmorLightningCritical -= LightningCritical;

        if (GrantedSkill is null)
            return;

        if (GrantedSkill is BaseDefenseSkill defenseSkill)
            defenseSkill.UndoPassiveEffects(wearer);

        var skillToRemove = wearer.Skills.FirstOrDefault(s => s.Name == GrantedSkill.Name);
        wearer.Skills.Remove(skillToRemove);

        GrantedSkill.QueueFree();

        base.UnequipFrom(wearer);
    }

    public override string GetContentTooltip()
    {
        var emil = new StringBuilder();

        emil.AppendLine($"Slash \t[{GetVorzeichen(SlashNormal)}{SlashNormal}/[color={CombatLog.GoodDamageColor}]{GetVorzeichen(SlashGood)}{SlashGood}[/color]/[color={CombatLog.CriticalDamageColor}]{GetVorzeichen(SlashCritical)}{SlashCritical}[/color]]\t" +
                        $"Fire\t\t\t\t[{GetVorzeichen(FireNormal)}{FireNormal}/[color={CombatLog.GoodDamageColor}]{GetVorzeichen(FireGood)}{FireGood}[/color]/[color={CombatLog.CriticalDamageColor}]{GetVorzeichen(FireCritical)}{FireCritical}[/color]]");

        emil.AppendLine($"Pierce [{GetVorzeichen(PierceNormal)}{PierceNormal}/[color={CombatLog.GoodDamageColor}]{GetVorzeichen(PierceGood)}{PierceGood}[/color]/[color={CombatLog.CriticalDamageColor}]{GetVorzeichen(PierceCritical)}{PierceCritical}[/color]]\t" +
                        $"Ice\t\t\t\t[{GetVorzeichen(IceNormal)}{IceNormal}/[color={CombatLog.GoodDamageColor}]{GetVorzeichen(IceGood)}{IceGood}[/color]/[color={CombatLog.CriticalDamageColor}]{GetVorzeichen(IceCritical)}{IceCritical}[/color]]");

        emil.AppendLine($"Crush \t[{GetVorzeichen(CrushNormal)}{CrushNormal}/[color={CombatLog.GoodDamageColor}]{GetVorzeichen(CrushGood)}{CrushGood}[/color]/[color={CombatLog.CriticalDamageColor}]{GetVorzeichen(CrushCritical)}{CrushCritical}[/color]]\t" +
                        $"Lightning\t[{GetVorzeichen(LightningNormal)}{LightningNormal}/[color={CombatLog.GoodDamageColor}]{GetVorzeichen(LightningGood)}{LightningGood}[/color]/[color={CombatLog.CriticalDamageColor}]{GetVorzeichen(LightningCritical)}{LightningCritical}[/color]]");

        return emil.ToString();
    }

    public override string GetEffectTooltip()
    {
        var emil = new StringBuilder();

        if (GrantedSkill is not null)
            emil.AppendLine($"Grants [color=darkgreen][{string.Join(" ", GrantedSkill.Name.ToString().AddSpacesToString())}][/color]");

        return emil.ToString();
    }

    public override string GetBoniTooltip()
    {
        var emil = new StringBuilder();

        if (StrengthBonus != 0)
            emil.AppendLine($"Strength\t\t[color={GetDisplayColor(nameof(StrengthBonus))}]{GetVorzeichen(StrengthBonus)}{StrengthBonus}[/color]");

        if (ConstitutionBonus != 0)
            emil.AppendLine($"Constitution\t[color={GetDisplayColor(nameof(ConstitutionBonus))}]{GetVorzeichen(ConstitutionBonus)}{ConstitutionBonus}[/color]");

        if (DexterityBonus != 0)
            emil.AppendLine($"Dexterity\t\t[color={GetDisplayColor(nameof(DexterityBonus))}]{GetVorzeichen(DexterityBonus)}{DexterityBonus}[/color]");

        if (WisdomBonus != 0)
            emil.AppendLine($"Wisdom\t\t[color={GetDisplayColor(nameof(WisdomBonus))}]{GetVorzeichen(WisdomBonus)}{WisdomBonus}[/color]");

        if (QuicknessBonus != 0)
            emil.AppendLine($"Quickness\t\t[color={GetDisplayColor(nameof(QuicknessBonus))}]{GetVorzeichen(QuicknessBonus)}{QuicknessBonus}[/color]");

        if (IntuitionBonus != 0)
            emil.AppendLine($"Intuition\t\t[color={GetDisplayColor(nameof(IntuitionBonus))}]{GetVorzeichen(IntuitionBonus)}{IntuitionBonus}[/color]");

        if (LogicBonus != 0)
            emil.AppendLine($"Logic\t\t[color={GetDisplayColor(nameof(LogicBonus))}]{GetVorzeichen(LogicBonus)}{LogicBonus}[/color]");

        if (WillpowerBonus != 0)
            emil.AppendLine($"Willpower\t\t[color={GetDisplayColor(nameof(WillpowerBonus))}]{GetVorzeichen(WillpowerBonus)}{WillpowerBonus}[/color]");

        if (CharismaBonus != 0)
            emil.AppendLine($"Charisma\t\t[color={GetDisplayColor(nameof(CharismaBonus))}]{GetVorzeichen(CharismaBonus)}{CharismaBonus}[/color]");

        return emil.ToString();
    }

    private string GetVorzeichen(int value) => value >= 0 ? "+" : string.Empty;

    private string GetDisplayColor(string propertyName)
    {
        var propertyInfo = GetType().GetProperties().FirstOrDefault(pi => pi.Name == propertyName);

        if (propertyInfo is null)
            return $"Property {propertyName} not found!";

        var value = (int?)propertyInfo.GetValue(this);

        if (value is null)
            return string.Empty;

        return value > 0 ? "darkgreen" : "red";
    }
}