using System;
using System.Text;
using DungeonMaster.Enums;
using Godot;
using Attribute = DungeonMaster.Enums.Attribute;
using Environment = System.Environment;

namespace DungeonMaster.Models.Skills;

public abstract partial class BaseSkill : Node3D
{
    public int           AcquisitionLevelHeroBasic      = 1;
    public int           AcquisitionLevelHeroDemanding  = 1;
    public int           AcquisitionLevelOutOfHeroClass = 1;
    public SkillCategory Category;
    public string        DescriptionBase;
    // public List<HeroClass>  DifficultyBasicClasses;
    // public List<HeroClass>  DifficultyDemandingClasses;
    public string           DisplayName;
    public int              Level = 1;
    public int              ManacostFlat;
    public float            ManacostLevelScaling;
    public float            MultiplierT = 1;
    public Attribute        PrimaryAttributeT;
    public float            PrimaryScalingT = 2f;
    public Attribute        SecondaryAttributeT;
    public float            SecondaryScalingT  = 1f;
    public float            SkillLevelScalingT = 2f;
    public SkillSubcategory Subcategory;
    public SkillType        Type;
    //public GameObject       Weapon;
    public int XpBaseBasic      = 16;
    public int XpBaseDemanding  = 45;
    public int XpBaseOutOfClass = 62;
    public int Manacost => (int)(ManacostFlat + Level * ManacostLevelScaling);

    protected string Description
    {
        get
        {
            var input             = DescriptionBase;
            var lineBreakInterval = 60;

            var lineBreak = Environment.NewLine;

            var emil  = new StringBuilder();
            var count = 0;

            for (var i = 0; i < input.Length; i++)
            {
                emil.Append(input[i]);
                count++;

                if (count < lineBreakInterval)
                    continue;

                if (char.IsWhiteSpace(input[i]) && i < input.Length - 1 && !char.IsWhiteSpace(input[i + 1]))
                {
                    emil.Append(lineBreak);
                    count = 0;
                }
                else
                {
                    var index = i;

                    while (index > 0 && !char.IsWhiteSpace(input[index]))
                        index--;

                    if (index > 0)
                    {
                        emil.Insert(index + 1, lineBreak);
                        count = i - (index + 1);
                    }
                    else
                    {
                        emil.Append(lineBreak);
                        count = 0;
                    }
                }
            }

            return emil.ToString();
        }
    }

    public int GetTacticalRoll(BaseUnit unit)
    {
        var primaryValue   = PrimaryScalingT * unit.Get(PrimaryAttributeT);
        var secondaryValue = SecondaryScalingT * unit.Get(SecondaryAttributeT);
        var levelValue     = Level * SkillLevelScalingT;

        var tacticalRoll = (primaryValue + secondaryValue + levelValue).InfuseRandomness();
        var finalHitroll = tacticalRoll * MultiplierT * GetAttackmodifier(this, unit);

        return (int)finalHitroll;
    }

    // public int GetAcquisitionLevel(Hero hero)
    // {
    //     var difficulty = GetDifficultyByHero(hero);
    //
    //     return difficulty switch
    //     {
    //         SkillDifficulty.Basic => AcquisitionLevelHeroBasic,
    //         SkillDifficulty.Demanding => AcquisitionLevelHeroDemanding,
    //         SkillDifficulty.OutOfClass => AcquisitionLevelOutOfHeroClass,
    //         _ => throw new ArgumentOutOfRangeException()
    //     };
    // }
    //
    // public SkillDifficulty GetDifficultyByHero(Hero hero)
    // {
    //     var heroclass = hero.heroClass;
    //
    //     if (DifficultyBasicClasses.Any(db => db.name == heroclass.name))
    //         return SkillDifficulty.Basic;
    //
    //     return DifficultyDemandingClasses.Any(dd => dd.name == heroclass.name) ? SkillDifficulty.Demanding : SkillDifficulty.OutOfClass;
    // }

    protected float GetAttackmodifier(BaseSkill skill, BaseUnit actor) => skill.Category switch
    {
        SkillCategory.Melee => actor.MeleeAttackratingModifier,
        SkillCategory.Ranged => actor.RangedAttackratingModifier,
        SkillCategory.Magic => actor.MagicAttackratingModifier,
        SkillCategory.Social => actor.SocialAttackratingModifier,
        SkillCategory.Summon => 0,
        SkillCategory.Support => 0,
        SkillCategory.Initiative => 0,
        _ => throw new ArgumentOutOfRangeException()
    };

    public abstract string Activate(BaseUnit actor);
    //
    // public virtual string GetTooltip(BaseHero hero, string damage = "0-0") => $"<b>{DisplayName.ToUpper()}</b>{System.Environment.NewLine}" +
    //                                                                           $"<i>{Category}, {Subcategory}, {Type}</i>{System.Environment.NewLine}{System.Environment.NewLine}";
}