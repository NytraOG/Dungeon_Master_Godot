using Godot;

namespace DungeonMaster;

public partial class Character : CharacterBody3D
{
    private Vector3 destination;
    private bool    stopped;
    private Vector3 targetVelocity = Vector3.Zero;

    [Export]
    public int Speed { get; set; } = 14;

    public override void _Ready() => destination = Position + new Vector3(4, 0, 0);

    public override void _PhysicsProcess(double delta)
    {
        var animationSprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");

        if (!stopped)
        {
            animationSprite.Animation = "charge";
            animationSprite.Play();
        }

        if (Input.IsActionPressed(Keys.Num7))
        {
            animationSprite.AnimationLooped += () =>
            {
                animationSprite.Stop();
            };

            animationSprite.Animation = "attack";
            animationSprite.Play();
            stopped = !stopped;
        }

        if (destination.Length() > Position.Length())
        {
            Velocity = Vector3.Zero;
            return;
        }

        var direction = new Vector3(1, 0, 0);
        targetVelocity.X = direction.X * Speed;
        targetVelocity.Y = direction.Y * Speed;

        Velocity = targetVelocity;

        MoveAndSlide();
    }
}