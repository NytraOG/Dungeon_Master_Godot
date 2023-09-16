using System;
using Godot;

namespace DungeonMaster;

public partial class Character : CharacterBody3D
{
    private          AnimatedSprite3D animationSprite;
    [Export] private Vector3          destination;
    private          Vector3          direction;
    private          AnimatedSprite3D fallbackAnimation;
    private          bool             hasAttacked;
    private          Vector3          start;
    private          bool             stopped;
    private          Vector3          targetVelocity = Vector3.Zero;

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
            Charge();

        var destinationReached = Math.Abs(destination.X - Position.X) < .1;

        if (destinationReached && !hasAttacked)
            Attack();

        var startPositionReached = Math.Abs(Position.X - start.X) < .1;

        if (startPositionReached && hasAttacked)
            Reset();

        Velocity = targetVelocity;

        MoveAndSlide();
    }

    private void Reset()
    {
        targetVelocity            = Vector3.Zero;
        animationSprite.Animation = "idle";

        animationSprite.Stop();

        hasAttacked                     =  !hasAttacked;
        animationSprite.AnimationLooped -= OnAnimationLooped;
    }

    private void Attack()
    {
        targetVelocity = Vector3.Zero;

        animationSprite.AnimationLooped += OnAnimationLooped;
        animationSprite.Animation       =  "attack";

        if (!animationSprite.IsPlaying())
            animationSprite.Play();

        hasAttacked = !hasAttacked;
    }

    private void Charge()
    {
        animationSprite.Animation = "charge";

        animationSprite.Play();

        direction = new Vector3(1, 0, 0);

        Accelerate();
    }

    private void Accelerate()
    {
        targetVelocity.X = direction.X * Speed;
        targetVelocity.Y = direction.Y * Speed;
    }

    private void OnAnimationLooped() => FallBack();

    private void FallBack()
    {
        fallbackAnimation           = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
        fallbackAnimation.Animation = "fall_back";

        fallbackAnimation.AnimationLooped += FallbackAnimationOnAnimationLooped;

        if (!fallbackAnimation.IsPlaying())
            fallbackAnimation.Play();

        direction *= -1;

        Accelerate();
    }

    private void FallbackAnimationOnAnimationLooped()
    {
        fallbackAnimation.Stop();

        fallbackAnimation.AnimationLooped -= FallbackAnimationOnAnimationLooped;
    }
}