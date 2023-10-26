using Godot;

namespace DungeonMaster.UI;

public partial class FloatingCombatText : Node2D
{
    public delegate void QueueFreedSignal();

    [Export]
    public float DriftVelocity { get; set; } = 0.3f;

    public Label                  Display { get; set; }
    public int                    Damage  { get; set; }
    public double                 Elapsed { get; set; }
    public event QueueFreedSignal OnQueueFreed;

    public override void _Process(double delta)
    {
        Elapsed += delta;

        if (Elapsed >= 1)
            Modulate = new Color(Modulate, 1 - ((float)Elapsed - 1));

        if (Elapsed >= 2)
            QueueFree();

        Position += new Vector2(0, -DriftVelocity);
    }

    public void _freed() => OnQueueFreed?.Invoke();
}