using Godot;

namespace DungeonMaster.Models.Heroes;

public partial class OrcEnergist : Hero
{
    [Signal]
    public delegate void HeroClickedEventHandler(Hero hero);

    // private void _on_hero_input_event(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIndex)
    // {
    //     if (@event is InputEventMouseButton { Pressed: true })
    //     {
    //         EmitSignal(SignalName.HeroClicked, this);
    //     }
    // }

    public override void _Ready()
    {
        var animatedSprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
        animatedSprite.Animation = "idle";
        animatedSprite.Play();
    }
}