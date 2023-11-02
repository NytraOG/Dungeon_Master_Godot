using System;
using System.Linq;
using System.Text;
using DungeonMaster.Enums;
using DungeonMaster.Models.Heroes;
using DungeonMaster.Models.Heroes.Classes;
using Godot;
using Attribute = DungeonMaster.Enums.Attribute;
using Environment = System.Environment;

namespace DungeonMaster.Models.Skills;

public abstract partial class BaseSkill : Node3D
{
    [ExportGroup("Details")] [Export]  public SkillCategory    Category;
    [Export]                           public SkillSubcategory Subcategory;
    [Export]                           public SkillType        Type;
    [Export]                           public int              ManacostFlat;
    [Export]                           public double           ManacostLevelScaling;
    [Export]                           public string           DescriptionBase;
    [Export]                           public BaseHeroclass[]  DifficultyBasicClasses;
    [Export]                           public BaseHeroclass[]  DifficultyDemandingClasses;
    [Export]                           public Texture2D        Icon;
    [ExportGroup("Leveling")] [Export] public int              Level                          = 1;
    [Export]                           public int              XpBaseBasic                    = 16;
    [Export]                           public int              XpBaseDemanding                = 45;
    [Export]                           public int              XpBaseOutOfClass               = 62;
    [Export]                           public int              AcquisitionLevelHeroBasic      = 1;
    [Export]                           public int              AcquisitionLevelHeroDemanding  = 1;
    [Export]                           public int              AcquisitionLevelOutOfHeroClass = 1;
    [ExportGroup("Tactical Roll")] [Export]
    public Attribute PrimaryAttributeT;
    [Export] public double    PrimaryScalingT = 2f;
    [Export] public Attribute SecondaryAttributeT;
    [Export] public double    SecondaryScalingT  = 1f;
    [Export] public double    SkillLevelScalingT = 2f;
    [Export] public double    MultiplierT        = 1;

    //public GameObject       Weapon;
    public int Manacost => (int)(ManacostFlat + Level * ManacostLevelScaling);

    [Export]
    public string Displayname { get; set; }

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

    public virtual int GetTacticalRoll(BaseUnit actor)
    {

        var primaryValue   = PrimaryScalingT * actor.Get(PrimaryAttributeT);
        var secondaryValue = SecondaryScalingT * actor.Get(SecondaryAttributeT);
        var levelValue     = Level * SkillLevelScalingT;

        var tacticalRoll = (primaryValue + secondaryValue + levelValue).InfuseRandomness();
        var finalHitroll = tacticalRoll * MultiplierT * GetDefensemodifier(this, actor);

        return (int)finalHitroll;
    }

    public int GetAcquisitionLevel(Hero hero)
    {
        var difficulty = GetDifficultyByHero(hero);

        return difficulty switch
        {
            SkillDifficulty.Basic => AcquisitionLevelHeroBasic,
            SkillDifficulty.Demanding => AcquisitionLevelHeroDemanding,
            SkillDifficulty.OutOfClass => AcquisitionLevelOutOfHeroClass,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public SkillDifficulty GetDifficultyByHero(Hero hero)
    {
        var heroclass = hero.Class;

        if (DifficultyBasicClasses.Any(db => db.Name == heroclass.Name))
            return SkillDifficulty.Basic;

        return DifficultyDemandingClasses.Any(dd => dd.Name == heroclass.Name) ? SkillDifficulty.Demanding : SkillDifficulty.OutOfClass;
    }

    protected double GetDefensemodifier(BaseSkill skill, BaseUnit actor) => skill.Category switch
    {
        SkillCategory.Melee => actor.MeleeDefensmodifier,
        SkillCategory.Ranged => actor.RangedDefensemodifier,
        SkillCategory.Magic => actor.MagicDefensemodifier,
        SkillCategory.Social => actor.SocialDefensemodifier,
        SkillCategory.Support when skill is BaseSupportSkill supportSkill && supportSkill.AffectedCategories.Any(ac => ac == SkillCategory.Melee.ToString()) => actor.MeleeDefensmodifier,
        SkillCategory.Support when skill is BaseSupportSkill supportSkill && supportSkill.AffectedCategories.Any(ac => ac == SkillCategory.Ranged.ToString()) => actor.RangedDefensemodifier,
        SkillCategory.Support when skill is BaseSupportSkill supportSkill && supportSkill.AffectedCategories.Any(ac => ac == SkillCategory.Magic.ToString()) => actor.MagicDefensemodifier,
        SkillCategory.Support when skill is BaseSupportSkill supportSkill && supportSkill.AffectedCategories.Any(ac => ac == SkillCategory.Social.ToString()) => actor.SocialDefensemodifier,
        _ => 0
    };

    protected double GetAttackmodifier(BaseSkill skill, BaseUnit actor) => skill.Category switch
    {
        SkillCategory.Melee => actor.MeleeAttackratingModifier,
        SkillCategory.Ranged => actor.RangedAttackratingModifier,
        SkillCategory.Magic => actor.MagicAttackratingModifier,
        SkillCategory.Social => actor.SocialAttackratingModifier,
        _ => 0
    };

    public abstract string Activate(BaseUnit actor);

    public virtual string GetTooltip(Hero hero, string damage = "0-0") => $"<b>{Displayname.ToUpper()}</b>{Environment.NewLine}" +
                                                                          $"<i>{Category}, {Subcategory}, {Type}</i>{Environment.NewLine}{Environment.NewLine}";
}