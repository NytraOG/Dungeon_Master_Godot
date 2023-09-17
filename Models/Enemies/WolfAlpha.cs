namespace DungeonMaster.Models.Enemies;

public partial class WolfAlpha : BaseCreature
{
    protected override void OnClicked() => EmitSignal(BaseCreature.SignalName.CreatureClicked, this);
}