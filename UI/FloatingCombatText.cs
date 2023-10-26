using Godot;

namespace DungeonMaster.UI;

public partial class FloatingCombatText : Node2D
{
    public Label Display { get; set; }
    public int   Damage  { get; set; }

    public override void _Ready()
    {
        Display      = GetNode<Label>("Label");
        Display.Text = Damage.ToString("N");
    }

    public override void _Process(double delta) => Position += new Vector2(0, -1);
}