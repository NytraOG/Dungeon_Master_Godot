using DungeonMaster.Enums;
using DungeonMaster.Models.Heroes;
using Godot;

namespace DungeonMaster.Models.Skills;

public partial class BaseSummonSkill : BaseSkill
{
    [Export] public BaseUnit[] SpawnsPerCast;

    public override void _Ready()
    {
        Category    = SkillCategory.Summon;
        Subcategory = SkillSubcategory.Special;
    }

    public override string Activate(BaseUnit actor)
    {
        foreach (var unit in SpawnsPerCast)
        {
            // var creatureScript = creature.GetComponent<Creature>();
            // var service        = FindObjectOfType<SpawnController>();
            //
            // var freeFavouritePosition = Positions.None;
            //
            // foreach (var position in creatureScript.favouritePositions)
            // {
            // 	var isOccupied = service.fieldslots[position];
            //
            // 	if (isOccupied)
            // 		continue;
            //
            // 	freeFavouritePosition = position;
            // }
            //
            // if (freeFavouritePosition != Positions.None)
            // 	service.SpawnCreatureAtPosition(creature, freeFavouritePosition);
        }

        return string.Empty;
    }

    public override string GetTooltip(Hero hero, string damage = "0-0") => base.GetTooltip(hero, damage) +
                                                                           System.Environment.NewLine + System.Environment.NewLine +
                                                                           Description;
}