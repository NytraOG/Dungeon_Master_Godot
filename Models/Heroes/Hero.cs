using DungeonMaster.Enums;
using DungeonMaster.Models.Heroes.Classes;
using DungeonMaster.Models.Heroes.Races;
using DungeonMaster.Models.Skills;
using Godot;

namespace DungeonMaster.Models.Heroes;

public partial class Hero : BaseUnit
{

    [Export] public BaseHeroclass Class;
    [Export] public BaseSkill     InherentSkill;
    [Export] public int           InventorySize;
    private         bool          isInitialized;
    [Export] public BaseRace      Race;

    public override void _Ready() { }

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

        OnSelected += OnOnSelected;
    }

    private void OnOnSelected()
    {
        var mainNode = (Main)GetTree().CurrentScene;
        mainNode.SelectedHero = this;
    }

    public delegate void SelectedEvent();

    public event SelectedEvent OnSelected;

    public override (int, int) GetApproximateDamage(BaseSkill ability) => ability switch
    {
        BaseDamageSkill skill => skill.GetDamage(this, HitResult.None),
        _ => (0, 0)
    };

    public override string UseAbility(BaseSkill skill, HitResult hitResult, BaseUnit target = null) => GibSkillresult(skill, target);

    private void _on_hero_input_event(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIndex)
    {
        if (@event is InputEventMouseButton { Pressed: true })
            OnSelected?.Invoke();
    }
}