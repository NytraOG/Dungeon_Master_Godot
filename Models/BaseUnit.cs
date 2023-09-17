using System;
using System.Collections.Generic;
using System.Linq;
using DungeonMaster.Enums;
using DungeonMaster.Models.Skills;
using DungeonMaster.Models.Skills.Statuseffects.Buffs;
using DungeonMaster.Models.Skills.Statuseffects.Debuffs;
using Godot;
using Attribute = DungeonMaster.Enums.Attribute;

namespace DungeonMaster.Models;

public abstract partial class BaseUnit : Node3D
{
    public Dictionary<BaseSupportSkill, bool> ActiveSkills = new();
    public List<Buff>                         Buffs        = new();
    public List<Debuff>                       Debuffs      = new();
    public List<BaseSkill>                    Skills       = new();

    //Stats
    [Export]
    public string Displayname { get; set; }

    [Export]
    public int Level { get; set; }

    public int XpToSpendForLevelUp => this.GetXpToSpendForLevelUp();

    [Export]
    public int Strength { get; set; } = 1;

    [Export]
    public int Constitution { get; set; } = 1;

    [Export]
    public int Dexterity { get; set; } = 1;

    [Export]
    public int Wisdom { get; set; } = 1;

    [Export]
    public int Quickness { get; set; } = 1;

    [Export]
    public int Intuition { get; set; } = 1;

    [Export]
    public int Logic { get; set; } = 1;

    [Export]
    public int Willpower { get; set; } = 1;

    [Export]
    public int Charisma { get; set; } = 1;

    [Export]
    public float MaximumHitpoints { get; set; }

    [Export]
    public float CurrentHitpoints { get; set; }

    [Export]
    public float MaximumMana { get; set; }

    [Export]
    public float CurrentMana { get; set; }

    [Export]
    public float ManaregenerationRate { get; set; }

    [Export]
    public int Armour { get; set; }

    public bool IsStunned { get; set; }
    public bool IsDead    => CurrentHitpoints <= 0;

    //Magic
    public float BaseMagicDefense => 2 * Willpower + Wisdom;

    [Export]
    public float MagicAttackratingModifier { get; set; }

    [Export]
    public float CurrentMagicDefense { get; set; }

    [Export]
    public float MagicDefensemodifier { get; set; }

    public float ModifiedMagicDefense => FetchRollFor(SkillCategory.Magic, () => CurrentMagicDefense * MagicDefensemodifier);

    //Social
    public float BaseSocialDefense => 2 * Logic + Charisma;

    [Export]
    public float SocialAttackratingModifier { get; set; }

    [Export]
    public float CurrentSocialDefense { get; set; }

    [Export]
    public float SocialDefensemodifier { get; set; }

    public float ModifiedSocialDefense => FetchRollFor(SkillCategory.Social, () => CurrentSocialDefense * SocialDefensemodifier);

    //Melee
    public float BaseMeleeDefense => 2 * Dexterity + Quickness;

    [Export]
    public float MeleeAttackratingModifier { get; set; }

    [Export]
    public float CurrentMeleeDefense { get; set; }

    [Export]
    public float MeleeDefensmodifier { get; set; }

    public float ModifiedMeleeDefense => FetchRollFor(SkillCategory.Melee, () => CurrentMeleeDefense * MeleeDefensmodifier);

    //Ranged
    public float BaseRangedDefense => 2 * Quickness + Dexterity;

    [Export]
    public float RangedAttackratingModifier { get; set; }

    [Export]
    public float CurrentRangedDefense { get; set; }

    [Export]
    public float RangedDefensemodifier { get; set; }

    public float ModifiedRangedDefense => FetchRollFor(SkillCategory.Ranged, () => CurrentRangedDefense * RangedDefensemodifier);

    //Initiative
    public float BaseInitiative => 2 * Intuition + Quickness;

    [Export]
    public float CurrentInitiative { get; set; }

    [Export]
    public float InitiativeFlatAdded { get; set; }

    [Export]
    public float InitiativeModifier { get; set; }

    public float ModifiedInitiative => FetchRollFor(SkillCategory.Initiative, () => CurrentInitiative * InitiativeModifier) + InitiativeFlatAdded;

    [Export]
    public int AktionenGesamt { get; set; }

    [Export]
    public int AktionenAktuell { get; set; }

    [Export]
    public float FlatDamageModifier { get; set; }

    public BaseSkill SelectedSkill { get; set; }

    public int Get(Attribute attribute) => attribute switch
    {
        Attribute.Strength => Strength,
        Attribute.Constitution => Constitution,
        Attribute.Dexterity => Dexterity,
        Attribute.Quickness => Quickness,
        Attribute.Intuition => Intuition,
        Attribute.Logic => Logic,
        Attribute.Willpower => Willpower,
        Attribute.Wisdom => Wisdom,
        Attribute.Charisma => Charisma,
        _ => throw new ArgumentOutOfRangeException(nameof(Attribute))
    };

    private float FetchRollFor(SkillCategory category, Func<float> returnDefaultModifiedDefense)
    {
        var matchingDefenseSkills = Skills.Where(s => s.Subcategory == SkillSubcategory.Defense &&
                                                      s.Category == category)
                                          .ToList();

        return !matchingDefenseSkills.Any() ? returnDefaultModifiedDefense() : float.Parse(matchingDefenseSkills.First().Activate(this));
    }

    protected void SetInitialHitpointsAndMana()
    {
        MaximumMana      = 2 * Wisdom + Logic;
        CurrentMana      = MaximumMana;
        MaximumHitpoints = 3 * Constitution + 2 * Strength;
        CurrentHitpoints = MaximumHitpoints;
    }

    protected string GibSkillresult(BaseSkill skill, BaseUnit target) => skill switch
    {
        BaseWeaponSkill weaponSkill => weaponSkill.Activate(this, target),
        _ => skill.Activate(this)
    };

    public abstract (int, int) GetApproximateDamage(BaseSkill ability);

    public abstract string UseAbility(BaseSkill ability, HitResult hitResult, BaseUnit target = null);

    public virtual void InitiativeBestimmen(double modifier = 1) => CurrentInitiative = (float)modifier * InitiativeModifier * (InitiativeFlatAdded + BaseInitiative.InfuseRandomness());

    public virtual void SetPosition(Vector3 spawnPosition, Vector3 positionToLookAt) { }

    public virtual void Initialize() => CurrentHitpoints = MaximumHitpoints;
}