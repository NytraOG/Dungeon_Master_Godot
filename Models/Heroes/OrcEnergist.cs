using Godot;

namespace DungeonMaster.Models.Heroes;

public partial class OrcEnergist : Hero
{
    public override void _Ready()
    {
        base._Ready();

        var animationSprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
        animationSprite.Animation = "idle";
        animationSprite.Play();
    }
}