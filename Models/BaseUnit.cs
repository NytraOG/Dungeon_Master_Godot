using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DungeonMaster.Enums;
using DungeonMaster.Models.Skills;
using DungeonMaster.Models.Skills.Statuseffects.Buffs;
using DungeonMaster.Models.Skills.Statuseffects.Debuffs;
using DungeonMaster.UI;
using Godot;
using Attribute = DungeonMaster.Enums.Attribute;

namespace DungeonMaster.Models;

public abstract partial class BaseUnit : Node3D, INotifyPropertyChanged
{
    public  Dictionary<BaseSupportSkill, bool> ActiveSkills = new();
    public  List<Buff>                         Buffs        = new();
    private double                             currentHitpoints;
    private double                             currentMana;
    public  List<Debuff>                       Debuffs = new();
    public  List<BaseSkill>                    Skills  = new();

    //Stats
    [Export]
    public string Displayname { get; set; }

    [Export]
    public int Level { get; set; } = 1;

    [Export]
    public int Strength { get; set; } = 4;

    [Export]
    public int Constitution { get; set; } = 4;

    [Export]
    public int Dexterity { get; set; } = 4;

    [Export]
    public int Wisdom { get; set; } = 4;

    [Export]
    public int Quickness { get; set; } = 4;

    [Export]
    public int Intuition { get; set; } = 4;

    [Export]
    public int Logic { get; set; } = 4;

    [Export]
    public int Willpower { get; set; } = 4;

    [Export]
    public int Charisma { get; set; } = 4;

    [Export]
    public double MaximumHitpoints { get; set; }

    [Export]
    public double CurrentHitpoints
    {
        get => currentHitpoints;
        set
        {
            currentHitpoints = value;
            OnPropertyChanged();
        }
    }

    [Export]
    public double MaximumMana { get; set; }

    [Export]
    public double CurrentMana
    {
        get => currentMana;
        set
        {
            currentMana = value;
            OnPropertyChanged();
        }
    }

    [Export]
    public double ManaregenerationRate { get; set; }

    //Attackratings
    [Export]
    public double MeleeAttackratingModifier { get; set; }

    [Export]
    public double RangedAttackratingModifier { get; set; }

    [Export]
    public double MagicAttackratingModifier { get; set; }

    [Export]
    public double SocialAttackratingModifier { get; set; }

    public bool IsStunned { get; set; }
    public bool IsDead    => CurrentHitpoints <= 0;

    //Defense
    public int Armour { get; set; }

    //Melee
    public double BaseDefenseMelee => 2 * Dexterity + Quickness;

    [Export]
    public double MeleeDefense { get; set; }

    [Export]
    public double MeleeDefensmodifier { get; set; }

    public double ModifiedMeleeDefense => FetchRollFor(SkillCategory.Melee, () => MeleeDefense * MeleeDefensmodifier);

    //Ranged
    public double BaseDefenseRanged => 2 * Quickness + Dexterity;

    [Export]
    public double RangedDefense { get; set; }

    [Export]
    public double RangedDefensemodifier { get; set; }

    public double ModifiedRangedDefense => FetchRollFor(SkillCategory.Ranged, () => RangedDefense * RangedDefensemodifier);

    //Magic
    public double BaseDefenseMagic => 2 * Willpower + Wisdom;

    [Export]
    public double MagicDefense { get; set; }

    [Export]
    public double MagicDefensemodifier { get; set; }

    public double ModifiedMagicDefense => FetchRollFor(SkillCategory.Magic, () => MagicDefense * MagicDefensemodifier);

    //Social
    public double BaseDefenseSocial => 2 * Logic + Charisma;

    [Export]
    public double SocialDefense { get; set; }

    [Export]
    public double SocialDefensemodifier { get; set; }

    public double ModifiedSocialDefense => FetchRollFor(SkillCategory.Social, () => SocialDefense * SocialDefensemodifier);

    //Initiative
    public double BaseInitiative => 2 * Intuition + Quickness;

    [Export]
    public double Initiative { get; set; }

    [Export]
    public double InitiativeModifier { get; set; }

    [Export]
    public double InitiativeFlatAdded { get; set; }

    public double ModifiedInitiative => FetchRollFor(SkillCategory.Initiative, () => Initiative * InitiativeModifier) + InitiativeFlatAdded;

    [Export]
    public int AktionenGesamt { get; set; }

    [Export]
    public int AktionenAktuell { get; set; }

    [Export]
    public double FlatDamagebonus { get; set; }

    public BaseSkill SelectedSkill       { get; set; }
    public int       XpToSpendForLevelUp => this.GetXpToSpendForLevelUp();

    [Export]
    public PackedScene FloatingCombatText { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    //public  PackedScene                        FloatingCombatTextScene { get; set; } = (PackedScene)ResourceLoader.Load("res://UI/doubleing_combat_text.tscn");
    public virtual void InstatiateFloatingCombatText(int receivedDamage, HitResult hitResult = HitResult.None)
    {
        var cameraUnposition           = GetViewport().GetCamera3D().UnprojectPosition(GlobalPosition);
        var floatingCombatTextInstance = FloatingCombatText.Instantiate<FloatingCombatText>();

        floatingCombatTextInstance.Display  = floatingCombatTextInstance.GetNode<Label>("Label");
        floatingCombatTextInstance.Damage   = receivedDamage;
        floatingCombatTextInstance.Position = cameraUnposition + new Vector2(0, -50);
        floatingCombatTextInstance.Show();

        switch (hitResult)
        {
            case HitResult.Good:
                floatingCombatTextInstance.Display.AddThemeColorOverride("font_color", Colors.Orange);
                break;
            case HitResult.Critical:
                floatingCombatTextInstance.Display.AddThemeColorOverride("font_color", Colors.Red);
                break;
        }

        AddChild(floatingCombatTextInstance);
    }

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
        Attribute.None => 0,
        _ => throw new ArgumentOutOfRangeException(nameof(Attribute))
    };

    private double FetchRollFor(SkillCategory category, Func<double> returnDefaultModifiedDefense)
    {
        var matchingDefenseSkills = Skills.Where(s => s.Subcategory == SkillSubcategory.Defense &&
                                                      s.Category == category)
                                          .ToList();

        return !matchingDefenseSkills.Any() ? returnDefaultModifiedDefense() : double.Parse(matchingDefenseSkills.First().Activate(this));
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

    public virtual void InitiativeBestimmen(double modifier = 1) => Initiative = modifier * InitiativeModifier * (InitiativeFlatAdded + BaseInitiative.InfuseRandomness());

    public virtual void SetPosition(Vector3 spawnPosition, Vector3 positionToLookAt) { }

    public virtual void Initialize() => CurrentHitpoints = MaximumHitpoints;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}