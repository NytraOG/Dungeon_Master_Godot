using Godot;

namespace DungeonMaster.UI;

public partial class FloatingCombatText : Node2D
{
    [Export]
    public float DriftVelocity { get; set; } = 0.5f;

    public Label  Display { get; set; }
    public int    Damage  { get; set; }
    public double Elapsed { get; set; }

    public override void _Ready()
    {
        Display      = GetNode<Label>("Label");
        Display.Text = Damage == 0 ? "Miss" : Damage.ToString();
    }

    public override void _Process(double delta)
    {
        Elapsed += delta;

        if (Elapsed >= 1)
            Modulate = new Color(Modulate, 1 - ((float)Elapsed - 1));

        if (Elapsed >= 2)
            QueueFree();

        Position += new Vector2(0, -DriftVelocity);
    }
}