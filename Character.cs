using System;
using Godot;

namespace DungeonMaster;

public partial class Character : CharacterBody3D
{
    private AnimatedSprite3D animationSprite;
    private Vector3          destination;
    private bool             hasAttacked;
    private Vector3          start;
    private bool             stopped;
    private Vector3          targetVelocity = Vector3.Zero;

    [Export]
    public int Speed { get; set; } = 14;

    public override void _Ready()
    {
        animationSprite           = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
        animationSprite.Animation = "idle";
        destination               = Position + new Vector3(4, 0, 0);
        start                     = Position;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionPressed(Keys.Num7) && !animationSprite.IsPlaying())
        {
            animationSprite.Animation = "charge";
            animationSprite.Play();

            var direction = new Vector3(1, 0, 0);
            targetVelocity.X = direction.X * Speed;
            targetVelocity.Y = direction.Y * Speed;
        }

        if (Math.Abs(destination.X - Position.X) < .1 && !hasAttacked)
        {
            targetVelocity = Vector3.Zero;

            animationSprite.AnimationLooped += () =>
            {
                animationSprite.Stop();
                animationSprite.Animation = "idle";
            };

            animationSprite.Animation = "attack";
            hasAttacked               = !hasAttacked;
        }

        Velocity = targetVelocity;

        MoveAndSlide();
    }
}