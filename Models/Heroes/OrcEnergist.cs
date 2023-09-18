using Godot;

namespace DungeonMaster.Models.Heroes;

public partial class OrcEnergist : Hero
{
    [Signal]
    public delegate void HeroClickedEventHandler(Hero hero);
}