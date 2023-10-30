using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DungeonMaster.Enums;
using DungeonMaster.Misc;
using DungeonMaster.Models.Heroes;
using DungeonMaster.Models.Skills;
using DungeonMaster.Models.Skills.Statuseffects.Buffs;
using DungeonMaster.Models.Skills.Statuseffects.Debuffs;
using DungeonMaster.UI;
using Godot;
using Attribute = DungeonMaster.Enums.Attribute;

namespace DungeonMaster.Models;

public abstract partial class BaseUnit : Node3D, INotifyPropertyChanged
{
    public delegate void RoundFinishedEventHandler();

    public  Dictionary<BaseSupportSkill, bool> ActiveSkills = new();
    public  List<Buff>                         Buffs        = new();
    private int                                charisma     = 4;
    private int                                constitution = 4;
    private double                             currentHitpoints;
    private double                             currentMana;
    public  List<Debuff>                       Debuffs   = new();
    private int                                dexterity = 4;
    private int                                intuition = 4;
    private int                                level     = 1;
    private int                                logic     = 4;
    private double                             maximumHitpoints;
    private int                                quickness = 4;
    public  List<BaseSkill>                    Skills    = new();
    private int                                strength  = 4;
    private int                                willpower = 4;
    private int                                wisdom    = 4;

    //Stats
    [Export]
    public string Displayname { get; set; }

    [Export]
    [Statdisplay]
    public int Level
    {
        get => level;
        set
        {
            if (value == level)
                return;

            level = value;
            OnPropertyChanged();
        }
    }

    [Export]
    [Statdisplay]
    public int Strength
    {
        get => strength;
        set
        {
            if (value == strength)
                return;
            strength = value;
            OnPropertyChanged();
        }
    }

    [Export]
    [Statdisplay]
    public int Constitution
    {
        get => constitution;
        set
        {
            if (value == constitution)
                return;
            constitution = value;
            OnPropertyChanged();
        }
    }

    [Export]
    [Statdisplay]
    public int Dexterity
    {
        get => dexterity;
        set
        {
            if (value == dexterity)
                return;
            dexterity = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(MeleeDefenseBase));
            OnPropertyChanged(nameof(RangedDefenseBase));
        }
    }

    [Export]
    [Statdisplay]
    public int Wisdom
    {
        get => wisdom;
        set
        {
            if (value == wisdom)
                return;
            wisdom = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(MagicDefenseBase));
        }
    }

    [Export]
    [Statdisplay]
    public int Quickness
    {
        get => quickness;
        set
        {
            if (value == quickness)
                return;
            quickness = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(MeleeDefenseBase));
            OnPropertyChanged(nameof(RangedDefenseBase));
            OnPropertyChanged(nameof(BaseInitiative));
        }
    }

    [Export]
    [Statdisplay]
    public int Intuition
    {
        get => intuition;
        set
        {
            if (value == intuition)
                return;
            intuition = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(BaseInitiative));
        }
    }

    [Export]
    [Statdisplay]
    public int Logic
    {
        get => logic;
        set
        {
            if (value == logic)
                return;
            logic = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SocialDefenseBase));
        }
    }

    [Export]
    [Statdisplay]
    public int Willpower
    {
        get => willpower;
        set
        {
            if (value == willpower)
                return;
            willpower = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(MagicDefenseBase));
        }
    }

    [Export]
    [Statdisplay]
    public int Charisma
    {
        get => charisma;
        set
        {
            if (value == charisma)
                return;
            charisma = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SocialDefenseBase));
        }
    }

    [Export]
    [Statdisplay]
    public double MaximumHitpoints
    {
        get => maximumHitpoints;
        set
        {
            if (value.Equals(maximumHitpoints))
                return;

            maximumHitpoints = value;
            OnPropertyChanged();
        }
    }

    [Export]
    [Statdisplay]
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
    [Statdisplay]
    public double MaximumMana { get; set; }

    [Export]
    [Statdisplay]
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
    [Statdisplay]
    public double ManaregenerationRate { get; set; }

    //Modifier
    [Export]
    [Statdisplay]
    public double InitiativeModifier { get; set; }

    [Export]
    [Statdisplay]
    public double MeleeAttackratingModifier { get; set; }

    [Export]
    [Statdisplay]
    public double RangedAttackratingModifier { get; set; }

    [Export]
    [Statdisplay]
    public double MagicAttackratingModifier { get; set; }

    [Export]
    [Statdisplay]
    public double SocialAttackratingModifier { get; set; }

    public bool IsStunned { get; set; }
    public bool IsDead    => CurrentHitpoints <= 0;

    //Defense
    [Statdisplay]
    public int ArmorSlashNormal { get; set; }

    [Statdisplay]
    public int ArmorSlashGood { get; set; }

    [Statdisplay]
    public int ArmorSlashCritical { get; set; }

    [Statdisplay]
    public int ArmorPierceNormal { get; set; }

    [Statdisplay]
    public int ArmorPierceGood { get; set; }

    [Statdisplay]
    public int ArmorPierceCritical { get; set; }

    [Statdisplay]
    public int ArmorCrushNormal { get; set; }

    [Statdisplay]
    public int ArmorCrushGood { get; set; }

    [Statdisplay]
    public int ArmorCrushCritical { get; set; }

    [Statdisplay]
    public int ArmorFireNormal { get; set; }

    [Statdisplay]
    public int ArmorFireGood { get; set; }

    [Statdisplay]
    public int ArmorFireCritical { get; set; }

    [Statdisplay]
    public int ArmorIceNormal { get; set; }

    [Statdisplay]
    public int ArmorIceGood { get; set; }

    [Statdisplay]
    public int ArmorIceCritical { get; set; }

    [Statdisplay]
    public int ArmorLightningNormal { get; set; }

    [Statdisplay]
    public int ArmorLightningGood { get; set; }

    [Statdisplay]
    public int ArmorLightningCritical { get; set; }

    //Melee
    [Statdisplay]
    public double MeleeDefenseBase => 2 * Dexterity + Quickness;

    [Export]
    public double MeleeDefense { get; set; }

    [Export]
    [Statdisplay]
    public double MeleeDefensmodifier { get; set; }

    public double ModifiedMeleeDefense => FetchRollFor(SkillCategory.Melee, () => MeleeDefense * MeleeDefensmodifier);

    //Ranged
    [Statdisplay]
    public double RangedDefenseBase => 2 * Quickness + Dexterity;

    [Export]
    public double RangedDefense { get; set; }

    [Export]
    [Statdisplay]
    public double RangedDefensemodifier { get; set; }

    public double ModifiedRangedDefense => FetchRollFor(SkillCategory.Ranged, () => RangedDefense * RangedDefensemodifier);

    //Magic
    [Statdisplay]
    public double MagicDefenseBase => 2 * Willpower + Wisdom;

    [Export]
    public double MagicDefense { get; set; }

    [Export]
    [Statdisplay]
    public double MagicDefensemodifier { get; set; }

    public double ModifiedMagicDefense => FetchRollFor(SkillCategory.Magic, () => MagicDefense * MagicDefensemodifier);

    //Social
    [Statdisplay]
    public double SocialDefenseBase => 2 * Logic + Charisma;

    [Export]
    public double SocialDefense { get; set; }

    [Export]
    [Statdisplay]
    public double SocialDefensemodifier { get; set; }

    public double ModifiedSocialDefense => FetchRollFor(SkillCategory.Social, () => SocialDefense * SocialDefensemodifier);

    //Initiative
    public double BaseInitiative => 2 * Intuition + Quickness;

    [Export]
    public double Initiative { get; set; }

    [Export]
    public double InitiativeFlatAdded { get; set; }

    public double ModifiedInitiative => FetchRollFor(SkillCategory.Initiative, () => Initiative * InitiativeModifier) + InitiativeFlatAdded;

    [Export]
    [Statdisplay]
    public int MaximumActionpoints { get; set; }

    [Export]
    [Statdisplay]
    public int CurrentActionpoints { get; set; }

    [Export]
    public double FlatDamagebonus { get; set; }

    public BaseSkill SelectedSkill       { get; set; }
    public int       XpToSpendForLevelUp => this.GetXpToSpendForLevelUp();

    [Export]
    public PackedScene FloatingCombatText { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    public event RoundFinishedEventHandler   OnRoundFinished;

    //public  PackedScene                        FloatingCombatTextScene { get; set; } = (PackedScene)ResourceLoader.Load("res://UI/doubleing_combat_text.tscn");
    public void InvokeRoundFinished() => OnRoundFinished?.Invoke();

    public virtual void InstatiateFloatingCombatText(int receivedDamage, string sourceName, HitResult hitResult = HitResult.None)
    {
        try
        {
            var cameraUnposition           = GetViewport().GetCamera3D().UnprojectPosition(GlobalPosition);
            var floatingCombatTextInstance = FloatingCombatText.Instantiate<FloatingCombatText>();

            floatingCombatTextInstance.Display      = floatingCombatTextInstance.GetNode<Label>("Label");
            floatingCombatTextInstance.Display.Text = receivedDamage.ToString();
            floatingCombatTextInstance.Damage       = receivedDamage;
            floatingCombatTextInstance.Position     = cameraUnposition + new Vector2(0, -50);
            floatingCombatTextInstance.Show();

            switch (hitResult)
            {
                case HitResult.Good:
                    floatingCombatTextInstance.Display.AddThemeColorOverride("font_color", Colors.Orange);
                    floatingCombatTextInstance.Display.Text += "!";
                    break;
                case HitResult.Critical:
                    floatingCombatTextInstance.Display.AddThemeColorOverride("font_color", Colors.Red);
                    floatingCombatTextInstance.Display.Text += "!!";
                    break;
            }

            floatingCombatTextInstance.Display.Text += $" ({sourceName})";

            AddChild(floatingCombatTextInstance);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
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
        var matchingDefenseSkills = Skills.Where(s => s is BaseSupportSkill { Subcategory: SkillSubcategory.Defense or SkillSubcategory.Special, AffectedCategories: not null } supportSkill
                                                      && supportSkill.AffectedCategories.Any(ac => ac == category.ToString()))
                                          .ToList();

        return !matchingDefenseSkills.Any() ? returnDefaultModifiedDefense() : matchingDefenseSkills.First().GetTacticalRoll(this);
    }

    public double GetUnmodifiedNaturalRollFor(SkillCategory category) => category switch
    {
        SkillCategory.Melee => MeleeDefenseBase,
        SkillCategory.Ranged => RangedDefenseBase,
        SkillCategory.Magic => MagicDefenseBase,
        SkillCategory.Social => SocialDefenseBase,
        _ => 0
    };

    protected void SetInitialHitpointsAndMana()
    {
        OnPropertyChanged(nameof(Constitution));
        OnPropertyChanged(nameof(Wisdom));
    }

    protected string GibSkillresult(BaseSkill skill, BaseUnit target) => skill switch
    {
        BaseWeaponSkill weaponSkill => weaponSkill.Activate(this, target),
        _ => skill.Activate(this)
    };

    public abstract (int, int) GetApproximateDamage(BaseSkill ability);

    public abstract string UseAbility(BaseSkill ability, HitResult hitResult, BaseUnit target = null);

    public abstract void DieProperly(Main mainScene);

    public virtual void InitiativeBestimmen(double modifier = 1) => Initiative = modifier * BaseInitiative.InfuseRandomness();

    public virtual void SetPosition(Vector3 spawnPosition, Vector3 positionToLookAt) { }

    public virtual void Initialize() => CurrentHitpoints = MaximumHitpoints;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        switch (propertyName)
        {
            case nameof(Constitution) or nameof(Strength):
                double healthMod = 1;

                if (this is Hero hero1)
                    healthMod += hero1.Class.Health;

                var oldMaxhealth = MaximumHitpoints;
                MaximumHitpoints = (3 * Constitution + 2 * Strength) * healthMod;

                var maxHealthDifference = MaximumHitpoints - oldMaxhealth;
                CurrentHitpoints += maxHealthDifference;
                break;
            case nameof(Wisdom) or nameof(Logic):
                double manaMod = 1;

                if (this is Hero hero2)
                    manaMod += hero2.Class.Mana;

                var oldMaxMana = MaximumMana;
                MaximumMana = (2 * Wisdom + Logic) * manaMod;

                var maxManaDifference = MaximumMana - oldMaxMana;
                CurrentMana += maxManaDifference;
                break;
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}