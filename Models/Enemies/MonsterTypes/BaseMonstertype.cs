using Godot;

namespace DungeonMaster.Models.Enemies.MonsterTypes;

public abstract partial class BaseMonstertype : Node3D
{
    [Export] public int    Charisma     = 1;
    [Export] public int    Constitution = 1;
    [Export] public double CurrentHealth;
    [Export] public int    Dexterity = 1;
    [Export] public string Displayname;
    [Export] public int    Intuition = 1;
    [Export] public int    Logic     = 1;
    [Export] public double MaximumHealth;
    [Export] public int    Quickness = 1;
    [Export] public int    Strength  = 1;
    [Export] public int    Willpower = 1;
    [Export] public int    Wisdom    = 1;
    public          double BaseMeleeDefense  => 2 * Dexterity + Quickness;
    public          double BaseRangedDefense => 2 * Quickness + Dexterity;
    public          double BaseMagicDefense  => 2 * Willpower + Wisdom;
    public          double BaseSocialDefense => 2 * Logic + Charisma;
    public          int    BaseInitiative    => 2 * Intuition + Quickness;

    public void ApplyValues(BaseCreature creature)
    {
        creature.Strength     = Strength;
        creature.Constitution = Constitution;
        creature.Dexterity    = Dexterity;
        creature.Quickness    = Quickness;
        creature.Intuition    = Intuition;
        creature.Logic        = Logic;
        creature.Wisdom       = Wisdom;
        creature.Willpower    = Willpower;
        creature.Charisma     = Charisma;
    }
}