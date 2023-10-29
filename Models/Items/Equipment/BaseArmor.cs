using System.Text;
using DungeonMaster.Models.Skills;
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

        if (GrantedSkillScene  is null)
            return;

        GrantedSkill = GrantedSkillScene.Instantiate<BaseSkill>();
        wearer.Skills.Add(GrantedSkill);

        if(GrantedSkill is BaseDefenseSkill defenseSkill)
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

        if(GrantedSkill is BaseDefenseSkill defenseSkill)
            defenseSkill.UndoPassiveEffects(wearer);

        wearer.Skills.Remove(GrantedSkill);
        GrantedSkill.QueueFree();

        base.UnequipFrom(wearer);
    }

    public override string GetTooltipContent()
    {
        var emil = new StringBuilder();

        emil.AppendLine("Armor:");

        if (SlashNormal != 0 || SlashGood != 0 || SlashCritical != 0)
            emil.AppendLine($"Slash {SlashNormal}/{SlashGood}/{SlashCritical}");

        if (PierceNormal != 0 || PierceGood != 0 || PierceCritical != 0)
            emil.AppendLine($"Pierce {PierceNormal}/{PierceGood}/{PierceCritical}");

        if (CrushNormal != 0 || CrushGood != 0 || CrushCritical != 0)
            emil.AppendLine($"Crush {CrushNormal}/{CrushGood}/{CrushCritical}");

        if (FireNormal != 0 || FireGood != 0 || FireCritical != 0)
            emil.AppendLine($"Fire {FireNormal}/{FireGood}/{FireCritical}");

        if (IceNormal != 0 || IceGood != 0 || IceCritical != 0)
            emil.AppendLine($"Ice {IceNormal}/{IceGood}/{IceCritical}");

        if (LightningNormal != 0 || LightningGood != 0 || LightningCritical != 0)
            emil.AppendLine($"Lightning {LightningNormal}/{LightningGood}/{LightningCritical}");

        return emil.ToString();
    }
}