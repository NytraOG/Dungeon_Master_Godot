using Godot;

namespace DungeonMaster.Models.Enemies;

public partial class WolfPup : BaseCreature
{
    public override void SetPosition(Vector3 spawnPosition, Vector3 positionToLookAt) => LookAtFromPosition(spawnPosition, positionToLookAt, Vector3.Up);

    protected override void OnClicked() => EmitSignal(BaseCreature.SignalName.CreatureClicked, this);
}