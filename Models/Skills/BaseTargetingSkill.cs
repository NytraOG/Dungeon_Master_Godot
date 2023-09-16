using DungeonMaster.Enums;
using DungeonMaster.Models.Heroes;
using Godot;
using Environment = System.Environment;

namespace DungeonMaster.Models.Skills;

public abstract partial class BaseTargetingSkill : BaseSkill
{
    [Export] public bool     AppliesStun;
    [Export] public bool     AutoTargeting;
    [Export] public int      Cooldown;
    [Export] public int      TargetsFlat = 1;
    [Export] public float    TargetsHeroScaling;
    [Export] public bool     TargetsWholeGroup;
    public abstract Factions TargetableFaction { get; }

    public int GetTargets(BaseUnit unit) => TargetsFlat + (int)(TargetsHeroScaling * unit.Level);

    public override string GetTooltip(Hero selectedHero, string damage = "0-0") => base.GetTooltip(selectedHero, damage) +
                                                                                   GetManacostText(selectedHero) +
                                                                                   $"Targets:\t{GetTargets(selectedHero)}" + Environment.NewLine;

    public abstract string Activate(BaseUnit actor, BaseUnit target);

    private string GetManacostText(Hero hero)
    {
        if (Manacost == 0)
            return string.Empty;

        // var hexColor = Manacost > hero.CurrentMana ? Konstanten.NotEnoughManaColor : Konstanten.EnoughManaColor;
        //
        // return $"Manacost:\t<b><color={hexColor}>{Manacost}</color></b>{System.Environment.NewLine}";
        return $"Manacost:\t<b>{Manacost}</color></b>{Environment.NewLine}";
    }
}