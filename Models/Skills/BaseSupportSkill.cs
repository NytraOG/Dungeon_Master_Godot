using System;
using System.Collections.Generic;
using System.Linq;
using DungeonMaster.Enums;
using DungeonMaster.Models.Skills.Statuseffects.Buffs;
using DungeonMaster.Models.Skills.Statuseffects.Debuffs;
using Godot;

namespace DungeonMaster.Models.Skills;

public partial class BaseSupportSkill : BaseTargetingSkill
{
    [Export] public Buff[]   AppliedBuffs;
    [Export] public Debuff[] AppliedDebuffs;
    [Export] public bool     SelfcastOnly;
    [Export] public int      ActionsModifier;
    [Export] public float    FlatDamageModifier;
    [Export] [ExportGroup("Attackratingmodifier, additive")]
    public float MeleeAttackratingModifier;
    [Export] public float RangedAttackratingModifier;
    [Export] public float MagicAttackratingModifier;
    [Export] public float SocialAttackratingModifier;
    [Export] [ExportGroup("Defensemodifier, additive")]
    public float MeleeDefensmodifier;
    [Export] public float           RangedDefensemodifier;
    [Export] public float           MagicDefensemodifier;
    [Export] public float           SocialDefensemodifier;
    [Export] public SkillCategory[] AffectedCategories;
    public override Factions        TargetableFaction => Factions.All;

   public override void _Ready()
   {
       Category    = SkillCategory.Support;
       Subcategory = SkillSubcategory.Special;
   }

        public override string Activate(BaseUnit _, BaseUnit target)
        {
            foreach (var skillCategory in AffectedCategories)
            {
                switch (skillCategory)
                {
                    case SkillCategory.Melee:
                        target.MeleeAttackratingModifier += MeleeAttackratingModifier;
                        target.FlatDamageModifier        += FlatDamageModifier;
                        target.MeleeDefensmodifier       += MeleeDefensmodifier;
                        target.CurrentMeleeDefense              += GetTacticalRoll(target);

                        break;
                    case SkillCategory.Ranged:
                        target.RangedAttackratingModifier += RangedAttackratingModifier;
                        target.FlatDamageModifier         += FlatDamageModifier;
                        target.RangedDefensemodifier      += RangedDefensemodifier;
                        target.CurrentRangedDefense       += GetTacticalRoll(target);
                        break;
                    case SkillCategory.Magic:
                        target.MagicAttackratingModifier += MagicAttackratingModifier;
                        target.FlatDamageModifier        += FlatDamageModifier;
                        target.MagicDefensemodifier      += MagicDefensemodifier;
                        target.CurrentMagicDefense       += GetTacticalRoll(target);
                        break;
                    case SkillCategory.Social:
                        target.SocialAttackratingModifier += SocialAttackratingModifier;
                        target.SocialDefensemodifier      += SocialDefensemodifier;
                        target.CurrentSocialDefense       += GetTacticalRoll(target);
                        break;
                    case SkillCategory.Summon:     break;
                    case SkillCategory.Initiative: break;
                    default:                       throw new ArgumentOutOfRangeException();
                }
            }

            PopulateBuffs(target);

            if (!target.ActiveSkills.ContainsKey(this))
                target.ActiveSkills.Add(this, false);

            return $"Activated {Displayname}";
        }

        public void Reverse(BaseUnit target)
        {
            foreach (var skillCategory in AffectedCategories)
            {
                switch (skillCategory)
                {
                    case SkillCategory.Melee:
                        target.MeleeAttackratingModifier -= MeleeAttackratingModifier;
                        target.FlatDamageModifier        -= FlatDamageModifier;
                        target.MeleeDefensmodifier       -= MeleeDefensmodifier;
                        target.CurrentMeleeDefense       -= GetTacticalRoll(target);
                        break;
                    case SkillCategory.Ranged:
                        target.RangedAttackratingModifier -= RangedAttackratingModifier;
                        target.FlatDamageModifier         -= FlatDamageModifier;
                        target.RangedDefensemodifier      -= RangedDefensemodifier;
                        target.CurrentRangedDefense       -= GetTacticalRoll(target);
                        break;
                    case SkillCategory.Magic:
                        target.MagicAttackratingModifier -= MagicAttackratingModifier;
                        target.FlatDamageModifier        -= FlatDamageModifier;
                        target.MagicDefensemodifier      -= MagicDefensemodifier;
                        target.CurrentMagicDefense       -= GetTacticalRoll(target);
                        break;
                    case SkillCategory.Social:
                        target.SocialAttackratingModifier -= SocialAttackratingModifier;
                        target.SocialDefensemodifier      -= SocialDefensemodifier;
                        target.CurrentSocialDefense       -= GetTacticalRoll(target);
                        break;
                    case SkillCategory.Summon:     break;
                    case SkillCategory.Initiative: break;
                    default:                       throw new ArgumentOutOfRangeException();
                }
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
                    //Muss man schauen, obs das übers new keyword geht oder man ne godot methode braucht
                    var newBuffInstance = new Buff
                    {
                        AppliedBy   = this,
                        AppliedFrom = actor
                    };

                    newBuffInstance.ApplyAttributeModifier(target);
                    newBuffInstance.ApplyRatingModifier(target);
                    newBuffInstance.ApplyDamageModifier(target);

                    target.Buffs.Add(newBuffInstance);
                }
            }
        }

        public override string Activate(BaseUnit actor) => throw new NotImplementedException();
}