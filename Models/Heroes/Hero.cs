using System.ComponentModel;
using DungeonMaster.Enums;
using DungeonMaster.Models.Heroes.Classes;
using DungeonMaster.Models.Heroes.Races;
using DungeonMaster.Models.Skills;
using DungeonMaster.UI;
using DungeonMaster.UI.Inventory;
using Godot;

namespace DungeonMaster.Models.Heroes;

public partial class Hero : BaseUnit
{
    public delegate void SelectedEvent(Hero sender);

    private         AnimatedSprite3D animatedSprite;
    [Export] public BaseHeroclass    Class;
    [Export] public BaseSkill        InherentSkill;
    [Export] public InventorySystem  Inventory;
    [Export] public EquipmentSystem  Equipment;
    [Export] public int              InventorySize;
    private         bool             isInitialized;
    [Export] public BaseRace         Race;

    [Export]
    public PackedScene FloatingCombatText { get; set; }

    public override void _Ready()
    {
        FloatingCombatText = ResourceLoader.Load<PackedScene>("res://UI/floating_combat_text.tscn");

        Inventory = ResourceLoader.Load<PackedScene>("res://UI/Inventory/inventory.tscn")
                                  .Instantiate<InventorySystem>();
        Equipment = ResourceLoader.Load<PackedScene>("res://UI/Inventory/equipment_system.tscn")
                                  .Instantiate<EquipmentSystem>();

        // AddChild(Inventory); das macht die Inventories im UI sichtbar
        Inventory.Initialize(InventorySize);

        animatedSprite           = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
        animatedSprite.Animation = "idle";
        animatedSprite.Play();

        PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(CurrentHitpoints) || !IsDead)
            return;

        animatedSprite.AnimationLooped += () =>
        {
            animatedSprite.Stop();
            SetProcess(false);
            animatedSprite.Frame = 4;
        };

        animatedSprite.Animation = "die";
    }

    public override void _Process(double delta)
    {
        if (isInitialized)
            return;

        InitializeHero();
        isInitialized = true;
    }

    private void InitializeHero()
    {
        Displayname = $"{Race.Displayname}_{Class.Displayname}";

        if (InherentSkill is not null)
            Skills.Add(InherentSkill);

        MeleeAttackratingModifier  = 1;
        RangedAttackratingModifier = 1;
        MagicAttackratingModifier  = 1;
        SocialAttackratingModifier = 1;

        MeleeDefensmodifier   = 1;
        RangedDefensemodifier = 1;
        MagicDefensemodifier  = 1;
        SocialDefensemodifier = 1;

        InitiativeModifier = 1;

        CurrentMeleeDefense  = BaseMeleeDefense;
        CurrentRangedDefense = BaseRangedDefense;
        CurrentMagicDefense  = BaseMagicDefense;
        CurrentSocialDefense = BaseSocialDefense;

        Race.ApplySkills(this);
        Class.ApplySkills(this);

        SetInitialHitpointsAndMana();

        Race.ApplyModifiers(this);
        Class.ApplyModifiers(this);
    }

    public event SelectedEvent OnSelected;

    public override void InstatiateFloatingCombatText(int receivedDamage)
    {
        var floatingCombatTextInstance = FloatingCombatText.Instantiate<FloatingCombatText>();
        floatingCombatTextInstance.Damage = int.Parse(receivedDamage.ToString());
        AddChild(floatingCombatTextInstance);
    }

    public override (int, int) GetApproximateDamage(BaseSkill ability) => ability switch
    {
        BaseDamageSkill skill => skill.GetDamage(this, HitResult.None),
        _ => (0, 0)
    };

    public override string UseAbility(BaseSkill skill, HitResult hitResult, BaseUnit target = null) => GibSkillresult(skill, target);

    private void _on_hero_input_event(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIndex)
    {
        if (@event is InputEventMouseButton { Pressed: true })
            OnSelected?.Invoke(this);
    }
}