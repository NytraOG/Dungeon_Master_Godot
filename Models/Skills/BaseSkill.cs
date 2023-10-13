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
    [Export]                           public int             AcquisitionLevelHeroBasic      = 1;
    [Export]                           public int             AcquisitionLevelHeroDemanding  = 1;
    [Export]                           public int             AcquisitionLevelOutOfHeroClass = 1;
    [ExportGroup("Details")] [Export]  public SkillCategory   Category;
    [Export]                           public string          DescriptionBase;
    [Export]                           public BaseHeroclass[] DifficultyBasicClasses;
    [Export]                           public BaseHeroclass[] DifficultyDemandingClasses;
    [Export]                           public Texture2D       Icon;
    [ExportGroup("Leveling")] [Export] public int             Level = 1;
    [Export]                           public int             ManacostFlat;
    [Export]                           public float           ManacostLevelScaling;
    [Export]                           public float           MultiplierT = 1;
    [ExportGroup("Tactical Roll")] [Export]
    public Attribute PrimaryAttributeT;
    [Export] public float            PrimaryScalingT = 2f;
    [Export] public Attribute        SecondaryAttributeT;
    [Export] public float            SecondaryScalingT  = 1f;
    [Export] public float            SkillLevelScalingT = 2f;
    [Export] public SkillSubcategory Subcategory;
    [Export] public SkillType        Type;
    [Export] public int              XpBaseBasic      = 16;
    [Export] public int              XpBaseDemanding  = 45;
    [Export] public int              XpBaseOutOfClass = 62;

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

    public int GetTacticalRoll(BaseUnit unit)
    {
        var primaryValue   = PrimaryScalingT * unit.Get(PrimaryAttributeT);
        var secondaryValue = SecondaryScalingT * unit.Get(SecondaryAttributeT);
        var levelValue     = Level * SkillLevelScalingT;

        var tacticalRoll = (primaryValue + secondaryValue + levelValue).InfuseRandomness();
        var finalHitroll = tacticalRoll * MultiplierT * GetAttackmodifier(this, unit);

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

    public virtual string GetTooltip(Hero hero, string damage = "0-0") => $"<b>{Displayname.ToUpper()}</b>{Environment.NewLine}" +
                                                                          $"<i>{Category}, {Subcategory}, {Type}</i>{Environment.NewLine}{Environment.NewLine}";
}