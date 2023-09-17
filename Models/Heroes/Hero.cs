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
    [Export] public BaseRace      Race;

    public override void _Ready()
    {
        InitializeHero();
        Displayname = $"{Race.Displayname}_{Class.Displayname}";
    }

    private void InitializeHero()
    {
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

        base._Ready();

        Class.ApplyModifiers(this);
    }

    public override (int, int) GetApproximateDamage(BaseSkill ability) => ability switch
    {
        BaseDamageSkill skill => skill.GetDamage(this, HitResult.None),
        _ => (0, 0)
    };

    public override string UseAbility(BaseSkill skill, HitResult hitResult, BaseUnit target = null) => GibSkillresult(skill, target);


    // private void ChangeSelectedHero(BattleController controller)
    // {
    //     controller.selectedHero             = this;
    //     controller.skillsOfSelectedHero     = skills;
    //     controller.abilityanzeigeIstAktuell = false;
    //     controller.selectedTargets.ForEach(t => t.GetComponent<SpriteRenderer>().material = controller.defaultMaterial);
    //     controller.selectedTargets.Clear();
    //
    //     var inventoryDisplay = inventoryPanel.GetComponent<StaticInventoryDisplay>();
    //     inventoryDisplay.ChangeHero(this);
    //
    //     statusPanel.GetComponent<StatusPanel>().ChangeHero(this);
    // }
}