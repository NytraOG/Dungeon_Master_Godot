namespace DungeonMaster.Models.Enemies;

public partial class WolfAlpha : BaseCreature
{
    public override void Initialize()
    {
        base.Initialize();
        MaximumMana = 5;
        CurrentMana = 5;
    }
}