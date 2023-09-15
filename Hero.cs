using System;
using System.Diagnostics;
using Godot;

namespace DungeonMaster;

public partial class Hero : Area2D
{
	private bool    charging;
	public  Vector2 ScreenSize;
	private bool    stopped;

	[Export]
	public int Speed { get; set; } = 400;

	public override void _Ready() => ScreenSize = GetViewportRect().Size;

	public override void _Process(double delta)
	{
		var velocity       = new Vector2(1, 0);
		var animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		velocity = velocity.Normalized() * Speed;

		if (!stopped)
		{
			animatedSprite.Animation = "charge";
			animatedSprite.Play();
		}

		if (Input.IsActionPressed(Keys.Num7))
		{
			stopped = true;

			animatedSprite.AnimationLooped += () =>
			{
				animatedSprite.Animation = "idle";
				animatedSprite.Stop();
			};
			animatedSprite.Animation = "attack";
			animatedSprite.Play();
		}

		if (stopped)
			return;

		Position += velocity * (float)delta;
		Position =  new Vector2(Mathf.Clamp(Position.X, 0, ScreenSize.X), Mathf.Clamp(Position.Y, 0, ScreenSize.Y));
	}

	private void On_mouse_entered()
	{
		Console.WriteLine("hihi");
	}
}

