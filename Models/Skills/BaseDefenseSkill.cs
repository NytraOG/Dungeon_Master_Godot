using DungeonMaster.Enums;
using Godot;

namespace DungeonMaster.Models.Skills;

public partial class BaseDefenseSkill : BaseSkill
{
    [Export]
    public int ArmorSlashNormal { get; set; }

    [Export]
    public int ArmorSlashGood { get; set; }

    [Export]
    public int ArmorSlashCritical { get; set; }

    [Export]
    public int ArmorPierceNormal { get; set; }

    [Export]
    public int ArmorPierceGood { get; set; }

    [Export]
    public int ArmorPierceCritical { get; set; }

    [Export]
    public int ArmorCrushNormal { get; set; }

    [Export]
    public int ArmorCrushGood { get; set; }

    [Export]
    public int ArmorCrushCritical { get; set; }

    [Export]
    public int ArmorFireNormal { get; set; }

    [Export]
    public int ArmorFireGood { get; set; }

    [Export]
    public int ArmorFireCritical { get; set; }

    [Export]
    public int ArmorIceNormal { get; set; }

    [Export]
    public int ArmorIceGood { get; set; }

    [Export]
    public int ArmorIceCritical { get; set; }

    [Export]
    public int ArmorLightningNormal { get; set; }

    [Export]
    public int ArmorLightningGood { get; set; }

    [Export]
    public int ArmorLightningCritical { get; set; }

    public override void _Ready() => Subcategory = SkillSubcategory.Defense;

    public virtual void ApplyPassiveEffects(BaseUnit applicant)
    {
        applicant.ArmorSlashNormal       += ArmorSlashNormal;
        applicant.ArmorSlashGood         += ArmorSlashGood;
        applicant.ArmorSlashCritical     += ArmorSlashCritical;
        applicant.ArmorPierceNormal      += ArmorPierceNormal;
        applicant.ArmorPierceGood        += ArmorPierceGood;
        applicant.ArmorPierceCritical    += ArmorPierceCritical;
        applicant.ArmorCrushNormal       += ArmorCrushNormal;
        applicant.ArmorCrushGood         += ArmorCrushGood;
        applicant.ArmorCrushCritical     += ArmorCrushCritical;
        applicant.ArmorFireNormal        += ArmorFireNormal;
        applicant.ArmorFireGood          += ArmorFireGood;
        applicant.ArmorFireCritical      += ArmorFireCritical;
        applicant.ArmorIceNormal         += ArmorIceNormal;
        applicant.ArmorIceGood           += ArmorIceGood;
        applicant.ArmorIceCritical       += ArmorIceCritical;
        applicant.ArmorLightningCritical += ArmorLightningNormal;
        applicant.ArmorLightningCritical += ArmorLightningGood;
        applicant.ArmorLightningCritical += ArmorLightningCritical;
    }

    public virtual void UndoPassiveEffects(BaseUnit applicant)
    {
        applicant.ArmorSlashNormal       -= ArmorSlashNormal;
        applicant.ArmorSlashGood         -= ArmorSlashGood;
        applicant.ArmorSlashCritical     -= ArmorSlashCritical;
        applicant.ArmorPierceNormal      -= ArmorPierceNormal;
        applicant.ArmorPierceGood        -= ArmorPierceGood;
        applicant.ArmorPierceCritical    -= ArmorPierceCritical;
        applicant.ArmorCrushNormal       -= ArmorCrushNormal;
        applicant.ArmorCrushGood         -= ArmorCrushGood;
        applicant.ArmorCrushCritical     -= ArmorCrushCritical;
        applicant.ArmorFireNormal        -= ArmorFireNormal;
        applicant.ArmorFireGood          -= ArmorFireGood;
        applicant.ArmorFireCritical      -= ArmorFireCritical;
        applicant.ArmorIceNormal         -= ArmorIceNormal;
        applicant.ArmorIceGood           -= ArmorIceGood;
        applicant.ArmorIceCritical       -= ArmorIceCritical;
        applicant.ArmorLightningCritical -= ArmorLightningNormal;
        applicant.ArmorLightningCritical -= ArmorLightningGood;
        applicant.ArmorLightningCritical -= ArmorLightningCritical;
    }

    public override string Activate(BaseUnit actor) => GetTacticalRoll(actor).ToString();
}