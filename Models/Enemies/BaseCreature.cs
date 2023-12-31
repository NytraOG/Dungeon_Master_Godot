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

    public             List<Positions> FavouritePositions = new() { Positions.None };
    private            bool            isInitialized;
    [Export] public    Keyword[]       Keywords;
    [Export] public    double          LevelModifier;
    private            Main            main;
    [Export] public    BaseMonstertype Monstertype;
    public             ShaderMaterial  Shader                    { get; set; }
    protected override Vector4         SpriteOutlineColorHover   => new(1, 0.2f, 0, 0.8f); //Red
    protected override Vector4         SpriteOutlineColorClicked => new(1, 1, 0, 0.8f);    //Orange

    public override void _Process(double delta)
    {
        if (isInitialized)
            return;

        //test
        Initialize();
        isInitialized = true;
    }

    public void _on_mouse_exited()
    {
        if (main.SelectedTargets.Any(t => t.Name == Name) ||
            main.SkillSelection.Any(ss => ss.Actor.Name == main.SelectedHero?.Name && ss.Targets.Any(t => t.Name == Name)))
        {
            Shader.SetShaderParameter("starting_colour", SpriteOutlineColorClicked);
            return;
        }

        Shader.SetShaderParameter("starting_colour", SpriteOutlineColorInvisible);
    }

    public void _on_mouse_entered()
    {
        if (main.SelectedTargets.Any(t => t.Name == Name) ||
            main.SkillSelection.Any(ss => ss.Actor.Name == main.SelectedHero?.Name && ss.Targets.Any(t => t.Name == Name)))
        {
            Shader.SetShaderParameter("starting_colour", SpriteOutlineColorClicked);
            return;
        }

        Shader.SetShaderParameter("starting_colour", SpriteOutlineColorHover);
    }

    public override void Initialize()
    {
        main = (Main)GetTree().CurrentScene;
        var animatedSPrite = GetNode<AnimatedSprite2D>($"%{nameof(AnimatedSprite2D)}");
        Shader = (ShaderMaterial)animatedSPrite.Material.Duplicate();
        Shader.SetShaderParameter("starting_colour", SpriteOutlineColorInvisible);
        animatedSPrite.Material = Shader;

        FloatingCombatText = ResourceLoader.Load<PackedScene>("res://UI/floating_combat_text.tscn");
        Displayname        = $"{Monstertype.Displayname} {Keywords[0]?.Displayname}";
        Name               = Displayname;

        MeleeAttackratingModifier  = 1;
        RangedAttackratingModifier = 1;
        MagicAttackratingModifier  = 1;
        SocialAttackratingModifier = 1;

        MeleeDefensmodifier   = 1;
        RangedDefensemodifier = 1;
        MagicDefensemodifier  = 1;
        SocialDefensemodifier = 1;

        InitiativeModifier = 1;

        MeleeDefense  = MeleeDefenseBase;
        RangedDefense = RangedDefenseBase;
        MagicDefense  = MagicDefenseBase;
        SocialDefense = SocialDefenseBase;

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

    public override void DieProperly(Main mainScene)
    {
        var iniSlot = mainScene.InitiativeContainer.GetChildren()
                               .Cast<InitiativeSlot>()
                               .FirstOrDefault(s => s.AssignedUnit.Name == Name);

        if (iniSlot is not null)
        {
            mainScene.InitiativeContainer.GetChildren().Remove(iniSlot);
            mainScene.Enemies = mainScene.Enemies.Except(new[] { (BaseCreature)iniSlot.AssignedUnit }).ToArray();
            iniSlot.QueueFree();
        }

        GetNode<FloatingCombatText>(nameof(FloatingCombatText)).OnQueueFreed += QueueFree;
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

        var modifier = (LevelModifier - 1) * Level;
        modifier++;

        Strength     = (int)(Monstertype.Strength * modifier);
        Constitution = (int)(Monstertype.Constitution * modifier);
        Dexterity    = (int)(Monstertype.Dexterity * modifier);
        Quickness    = (int)(Monstertype.Quickness * modifier);
        Intuition    = (int)(Monstertype.Intuition * modifier);
        Logic        = (int)(Monstertype.Logic * modifier);
        Wisdom       = (int)(Monstertype.Wisdom * modifier);
        Willpower    = (int)(Monstertype.Willpower * modifier);
        Charisma     = (int)(Monstertype.Charisma * modifier);
    }

    private void _on_creature_input_event(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIndex)
    {
        if (@event is not InputEventMouseButton { Pressed: true })
            return;

        if (main?.SelectedSkill is not null)
            Shader.SetShaderParameter("starting_colour", SpriteOutlineColorClicked);

        OnSomeSignal?.Invoke(this);
    }
}