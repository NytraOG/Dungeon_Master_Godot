using Godot;

namespace DungeonMaster.Models.Enemies;

public partial class WolfAlpha : BaseCreature
{
    [Signal]
    public delegate void CreatureClickedEventHandler(BaseCreature creature);

    public void _on_creature_input_event(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIndex)
    {
        if (@event is InputEventMouseButton { Pressed: true })
        {
            EmitSignal(SignalName.CreatureClicked, this);
        }
    }
}