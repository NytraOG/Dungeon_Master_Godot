using System.ComponentModel;
using System.Linq;
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

    private            AnimatedSprite2D animatedSprite;
    public             bool             CanStillAct = true;
    [Export] public    BaseHeroclass    Class;
    [Export] public    EquipmentSystem  Equipment;
    [Export] public    BaseSkill        InherentSkill;
    [Export] public    InventorySystem  Inventory;
    [Export] public    int              InventorySize;
    private            bool             isInitialized;
    private            Main             main;
    [Export] public    BaseRace         Race;
    private            bool             shadowIsGrowing;
    private            int              shadowValue = 300;
    public             ShaderMaterial   Shader                    { get; set; }
    protected override Vector4          SpriteOutlineColorHover   => new(0, 1, 1, 0.8f);    //Türkis
    protected override Vector4          SpriteOutlineColorClicked => new(0, 1, 0, 0.8f);    //Green

    public override void _Ready()
    {
        Shader = (ShaderMaterial)GetNode<AnimatedSprite2D>($"%{nameof(AnimatedSprite2D)}").Material;
        Shader.SetShaderParameter("starting_colour", SpriteOutlineColorHover);

        FloatingCombatText = ResourceLoader.Load<PackedScene>("res://UI/floating_combat_text.tscn");
        main               = (Main)GetTree().CurrentScene;

        Inventory = ResourceLoader.Load<PackedScene>("res://UI/Inventory/inventory.tscn")
                                  .Instantiate<InventorySystem>();

        Equipment = ResourceLoader.Load<PackedScene>("res://UI/Inventory/equipment_display.tscn")
                                  .Instantiate<EquipmentSystem>();

        Inventory.Initialize(main.InventorySize);
        Equipment.Initialize();

        animatedSprite           = GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
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
        if (CanStillAct)
        {
            var maxValue = 300;
            var minValue = maxValue / 2;

            if (shadowValue >= maxValue)
            {
                shadowIsGrowing = false;
                shadowValue--;
            }

            if (shadowValue <= minValue)
            {
                shadowIsGrowing = true;
                shadowValue++;
            }

            if (shadowIsGrowing && shadowValue < maxValue)
            {
                var colorValue = shadowValue / (float)maxValue;
                animatedSprite.Modulate = new Color(colorValue, colorValue, colorValue);
                shadowValue++;
            }

            if (!shadowIsGrowing && shadowValue > minValue)
            {
                var colorValue = shadowValue / (float)maxValue;
                animatedSprite.Modulate = new Color(colorValue, colorValue, colorValue);
                shadowValue--;
            }
        }

        if (isInitialized)
            return;

        InitializeHero();
        isInitialized = true;
    }

    public void StopBlinking()
    {
        CanStillAct             = false;
        animatedSprite.Modulate = new Color(1, 1, 1);
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

        Race.ApplySkills(this);
        Class.ApplySkills(this);

        SetInitialHitpointsAndMana();

        Race.ApplyModifiers(this);
        Class.ApplyModifiers(this);

        MeleeDefense  = MeleeDefenseBase;
        RangedDefense = RangedDefenseBase;
        MagicDefense  = MagicDefenseBase;
        SocialDefense = SocialDefenseBase;
    }

    public event SelectedEvent OnSelected;

    public override (int, int) GetApproximateDamage(BaseSkill ability) => ability switch
    {
        BaseDamageSkill skill => skill.GetDamage(this, HitResult.None),
        _ => (0, 0)
    };

    public override string UseAbility(BaseSkill skill, HitResult hitResult, BaseUnit target = null) => GibSkillresult(skill, target);

    public override void DieProperly(Main mainScene)
    {
        var iniSlot = mainScene.InitiativeContainer.GetChildren()
                               .Cast<InitiativeSlot>()
                               .FirstOrDefault(s => s.AssignedUnit.Name == Name);

        if (iniSlot is not null)
        {
            mainScene.InitiativeContainer.GetChildren().Remove(iniSlot);
            mainScene.Heroes = mainScene.Heroes.Except(new[] { (Hero)iniSlot.AssignedUnit }).ToArray();
            iniSlot.QueueFree();
        }

        GetNode<FloatingCombatText>(nameof(FloatingCombatText)).OnQueueFreed += QueueFree;
    }

    private void _on_hero_input_event(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIndex)
    {
        if (@event is InputEventMouseButton { Pressed: true })
        {
            OnSelected?.Invoke(this);

            var mainEquipment = main.GetNode<EquipmentSystem>("EquipmentDisplay");

            foreach (var equipmentSlot in Equipment.Slots.Select(s => s.Value))
            {
                if (equipmentSlot.EquipedItem is null)
                    mainEquipment.Slots[equipmentSlot.Name].Clear();
                else
                {
                    var slot = mainEquipment.Slots[equipmentSlot.Name];
                    slot.EquipedItem  = equipmentSlot.EquipedItem;
                    slot.Icon.Texture = equipmentSlot.EquipedItem.Icon;
                }
            }
        }
    }
}