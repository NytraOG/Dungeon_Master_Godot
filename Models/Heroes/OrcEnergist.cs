using Godot;

namespace DungeonMaster.Models.Heroes;

public partial class OrcEnergist : CharacterBody3D
{
    public override void _Ready()
    {
        var animatedSprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
        animatedSprite.Animation = "idle";
        animatedSprite.Play();
    }
}