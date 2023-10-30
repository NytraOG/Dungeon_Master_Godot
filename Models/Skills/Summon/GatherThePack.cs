using Godot;

namespace DungeonMaster.Models.Skills.Summon;

public partial class GatherThePack : BaseSummonSkill
{
    [Signal]
    public delegate void SummonCompletedEventHandler();

    public override string Activate(BaseUnit actor)
    {
        var mainScene = (Main)GetTree().CurrentScene;

        for (var i = 0; i < UnitsToSpawn.Length; i++)
        {
            var instance = UnitsToSpawn[i].Instantiate<BaseUnit>();
            instance.SetPosition(new Vector3(0, 0, (float)(i + 1) / 2), new Vector3(-2, 0, 0));
            mainScene.AddChild(instance);

            if (actor.Owner is not Main main)
                return string.Empty;

            main.AllesDa = false;
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