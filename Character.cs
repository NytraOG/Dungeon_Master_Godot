using Godot;

namespace DungeonMaster;

public partial class Character : CharacterBody3D
{
    private bool    stopped;
    private Vector3 targetVelocity = Vector3.Zero;

    [Export]
    public int Speed { get; set; } = 14;

    public override void _PhysicsProcess(double delta)
    {
        var animationSprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
        animationSprite.Animation = "charge";
        animationSprite.Play();

        var direction = new Vector3(1, 0, 0);
        targetVelocity.X = direction.X * Speed;
        targetVelocity.Y = direction.Y * Speed;
        Velocity         = targetVelocity;

        MoveAndSlide();
    }
}