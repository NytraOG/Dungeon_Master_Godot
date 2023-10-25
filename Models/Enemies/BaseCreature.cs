using System;
using System.Collections.Generic;
using System.Linq;
using DungeonMaster.Enums;
using DungeonMaster.Models.Enemies.Keywords;
using DungeonMaster.Models.Enemies.MonsterTypes;
using DungeonMaster.Models.Skills;
using DungeonMaster.UI;
using Godot;

namespace DungeonMaster.Models.Enemies;

public abstract partial class BaseCreature : BaseUnit
{
    [Signal]
    public delegate void CreatureClickedEventHandler(BaseCreature creature);

    public delegate void SomeSignalWithIntArgument(BaseCreature creature);

    public          List<Positions> FavouritePositions = new() { Positions.None };
    private         bool            isInitialized;
    [Export] public Keyword[]       Keywords;
    [Export] public double           LevelModifier;
    [Export] public BaseMonstertype Monstertype;

    [Export]
    public PackedScene FloatingCombatText { get; set; }

    public override void _Process(double delta)
    {
        if (isInitialized)
            return;

        Initialize();
        isInitialized = true;
    }

    public override void InstatiateFloatingCombatText(int receivedDamage)
    {
        FloatingCombatText = ResourceLoader.Load<PackedScene>("res://UI/floating_combat_text.tscn");
        var floatingCombatTextInstance = FloatingCombatText.Instantiate<FloatingCombatText>();
        floatingCombatTextInstance.Damage         = receivedDamage;
        floatingCombatTextInstance.GlobalPosition = GlobalPosition += new Vector3(0, 20, 0);
        AddChild(floatingCombatTextInstance);
        floatingCombatTextInstance.Show();
    }

    public override void Initialize()
    {
        Displayname = $"{Monstertype.Displayname} {Keywords[0]?.Displayname}";
        Name        = Displayname;

        MeleeAttackratingModifier  = 1;
        RangedAttackratingModifier = 1;
        MagicAttackratingModifier  = 1;
        SocialAttackratingModifier = 1;

        MeleeDefensmodifier   = 1;
        RangedDefensemodifier = 1;
        MagicDefensemodifier  = 1;
        SocialDefensemodifier = 1;

        InitiativeModifier = 1;

        MeleeDefense  = BaseDefenseMelee;
        RangedDefense = BaseDefenseRanged;
        MagicDefense  = BaseDefenseMagic;
        SocialDefense = BaseDefenseSocial;

        Monstertype.ApplyValues(this);

        SetAttributeByLevel();

        base.Initialize();

        ApplyKeywords();
        SetInitialHitpointsAndMana();

        OnSomeSignal += UpdateSelectedEnemy;

        // var mainNode = GetTree().CurrentScene;
        // var kek      = mainNode.GetNode(mainNode.GetPath());
        // var callable = new Callable(this, nameof(UpdateSelectedEnemy));
        //
        // kek.Connect(SignalName.CreatureClicked, callable);
        // var spriterenderer = GetComponent<SpriteRenderer>();
        // spriterenderer.sprite ??= monstertype.sprite;

        //unitTooltip = GameObject.Find("UiCanvas").transform.Find("UnitTooltip").gameObject;
    }

    public event SomeSignalWithIntArgument OnSomeSignal;

    public void UpdateSelectedEnemy(BaseCreature creature)
    {
        var mainNode = (Main)GetTree().CurrentScene;
        mainNode.SelectedEnemy = this;
    }

    public override (int, int) GetApproximateDamage(BaseSkill ability) => ability switch
    {
        BaseDamageSkill skill => skill.GetDamage(this, HitResult.None),
        _ => throw new ArgumentOutOfRangeException(nameof(ability))
    };

    public override string UseAbility(BaseSkill skill, HitResult hitResult, BaseUnit target = null)
    {
        var result = GibSkillresult(skill, target);

        SelectedSkill = null;

        return result;
    }

    private void ApplyKeywords()
    {
        foreach (var keyword in Keywords)
        {
            keyword.ApplyAttributeModifier(this);
            keyword.ApplyRatingModifier(this);
            keyword.ApplyDamageModifier(this);
            keyword.PopulateSkills(this);
        }
    }

    public void PickSkill()
    {
        if (!Skills.Any())
            return;

        foreach (var skill in Skills.OrderByDescending(s => s.Manacost))
        {
            if (skill.Manacost > CurrentMana)
                continue;

            SelectedSkill = skill;
            return;
        }
    }

    private void SetAttributeByLevel()
    {
        if (Level == 1)
            return;

        var modifier = LevelModifier * Level;

        Strength     += (int)(Monstertype.Strength * modifier);
        Constitution += (int)(Monstertype.Constitution * modifier);
        Dexterity    += (int)(Monstertype.Dexterity * modifier);
        Quickness    += (int)(Monstertype.Quickness * modifier);
        Intuition    += (int)(Monstertype.Intuition * modifier);
        Logic        += (int)(Monstertype.Logic * modifier);
        Wisdom       += (int)(Monstertype.Wisdom * modifier);
        Willpower    += (int)(Monstertype.Willpower * modifier);
        Charisma     += (int)(Monstertype.Charisma * modifier);
    }

    private void _on_creature_input_event(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIndex)
    {
        if (@event is InputEventMouseButton { Pressed: true })
            OnSomeSignal?.Invoke(this);
    }
}