using System;
using System.Linq;
using DungeonMaster.Enums;
using DungeonMaster.Models.Skills.Statuseffects.Buffs;
using DungeonMaster.Models.Skills.Statuseffects.Debuffs;
using Godot;

namespace DungeonMaster.Models.Skills;

public partial class BaseSupportSkill : BaseTargetingSkill
{
    [Export] public int ActionsModifier;
    [Export] [ExportGroup("Melee, Ranged, Magic, Social, Explosion, Ambush, Sickness, ForceOfNature, Summon, Support, Initiative")]
    public string[] AffectedCategories;
    [Export] [ExportGroup("Misc")] public Buff[]   AppliedBuffs;
    [Export]                       public Debuff[] AppliedDebuffs;
    [Export]                       public float    FlatDamageModifier;
    [Export]                       public float    MagicAttackratingModifier;
    [Export]                       public float    MagicDefensemodifier;
    [Export] [ExportGroup("Attackratingmodifier, additive")]
    public float MeleeAttackratingModifier;
    [Export] [ExportGroup("Defensemodifier, additive")]
    public float MeleeDefensmodifier;
    [Export] public float    RangedAttackratingModifier;
    [Export] public float    RangedDefensemodifier;
    [Export] public bool     SelfcastOnly;
    [Export] public float    SocialAttackratingModifier;
    [Export] public float    SocialDefensemodifier;
    public override Factions TargetableFaction => Factions.All;

    public override void _Ready()
    {
        Category    = SkillCategory.Support;
        Subcategory = SkillSubcategory.Special;
    }

    public override string Activate(BaseUnit _, BaseUnit target)
    {
        if (AffectedCategories.Any())
            target.FlatDamagebonus += FlatDamageModifier;

        foreach (var skillCategory in AffectedCategories)
        {
            if (skillCategory == SkillCategory.Melee.ToString())
            {
                target.MeleeAttackratingModifier += MeleeAttackratingModifier;
                target.MeleeDefensmodifier       += MeleeDefensmodifier;
                target.MeleeDefense              += GetTacticalRoll(target);
            }
            else if (skillCategory == SkillCategory.Ranged.ToString())
            {
                target.RangedAttackratingModifier += RangedAttackratingModifier;
                target.RangedDefensemodifier      += RangedDefensemodifier;
                target.RangedDefense              += GetTacticalRoll(target);
            }
            else if (skillCategory == SkillCategory.Magic.ToString())
            {
                target.MagicAttackratingModifier += MagicAttackratingModifier;
                target.MagicDefensemodifier      += MagicDefensemodifier;
                target.MagicDefense              += GetTacticalRoll(target);
            }
            else if (skillCategory == SkillCategory.Social.ToString())
            {
                target.SocialAttackratingModifier += SocialAttackratingModifier;
                target.SocialDefensemodifier      += SocialDefensemodifier;
                target.SocialDefense              += GetTacticalRoll(target);
            }
            else
                Console.WriteLine($"{skillCategory} not implemented in {nameof(BaseSupportSkill)}");
        }

        PopulateBuffs(target);

        if (!target.ActiveSkills.ContainsKey(this))
            target.ActiveSkills.Add(this, false);

        return $"Activated {Displayname}";
    }

    public void Reverse(BaseUnit target)
    {
        if (AffectedCategories.Any())
            target.FlatDamagebonus -= FlatDamageModifier;

        foreach (var skillCategory in AffectedCategories)
        {
            if (skillCategory == SkillCategory.Melee.ToString())
            {
                target.MeleeAttackratingModifier -= MeleeAttackratingModifier;
                target.MeleeDefensmodifier       -= MeleeDefensmodifier;
                target.MeleeDefense              -= GetTacticalRoll(target);
            }
            else if (skillCategory == SkillCategory.Ranged.ToString())
            {
                target.RangedAttackratingModifier -= RangedAttackratingModifier;
                target.RangedDefensemodifier      -= RangedDefensemodifier;
                target.RangedDefense              -= GetTacticalRoll(target);
            }
            else if (skillCategory == SkillCategory.Magic.ToString())
            {
                target.MagicAttackratingModifier -= MagicAttackratingModifier;
                target.MagicDefensemodifier      -= MagicDefensemodifier;
                target.MagicDefense              -= GetTacticalRoll(target);
            }
            else if (skillCategory == SkillCategory.Social.ToString())
            {
                target.SocialAttackratingModifier -= SocialAttackratingModifier;
                target.SocialDefensemodifier      -= SocialDefensemodifier;
                target.SocialDefense              -= GetTacticalRoll(target);
            }
            else
                Console.WriteLine($"{skillCategory} not implemented in {nameof(BaseSupportSkill)}");
        }
    }

    public void PopulateBuffs(BaseUnit actor)
    {
        if (AppliedBuffs.Any())
            ApplyBuffs(actor, actor);
    }

    private void ApplyBuffs(BaseUnit actor, BaseUnit target)
    {
        foreach (var buff in AppliedBuffs)
        {
            if (target.Buffs.Any(b => b.Displayname == buff.Displayname))
                target.Buffs.First(b => b.Displayname == buff.Displayname).RemainingDuration += buff.Duration;
            else
            {
                var newBuffInstance = buff.ToNewInstance();
                newBuffInstance.AppliedBy   = this;
                newBuffInstance.AppliedFrom = actor;

                newBuffInstance.ApplyAttributeModifier(target);
                newBuffInstance.ApplyRatingModifier(target);
                newBuffInstance.ApplyDamageModifier(target);

                target.Buffs.Add(newBuffInstance);

                target.AddChild(newBuffInstance);
            }
        }
    }

    public override string Activate(BaseUnit actor) => GetTacticalRoll(actor).ToString();
}