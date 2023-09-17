using Godot;

namespace DungeonMaster.Models.Skills.Summon;

public partial class GatherThePack : BaseSummonSkill
{
    public override string Activate(BaseUnit actor)
    {
        for (var i = 0; i < UnitsToSpawn.Length; i++)
        {
            var instance = UnitsToSpawn[i].Instantiate<BaseUnit>();
            instance.SetPosition(new Vector3(0, 0, (float)(i + 1) / 2), new Vector3(-2, 0, 0));
            actor.AddChild(instance);
        }

        // foreach (var unit in UnitsToSpawn)
        // {
        //     // var creatureScript = creature.GetComponent<Creature>();
        //     // var service        = FindObjectOfType<SpawnController>();
        //     //
        //     // var freeFavouritePosition = Positions.None;
        //     //
        //     // foreach (var position in creatureScript.favouritePositions)
        //     // {
        //     // 	var isOccupied = service.fieldslots[position];
        //     //
        //     // 	if (isOccupied)
        //     // 		continue;
        //     //
        //     // 	freeFavouritePosition = position;
        //     // }
        //     //
        //     // if (freeFavouritePosition != Positions.None)
        //     // 	service.SpawnCreatureAtPosition(creature, freeFavouritePosition);
        // }

        return string.Empty;
    }
}