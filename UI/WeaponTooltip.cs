using Godot;

namespace DungeonMaster.UI;

public partial class WeaponTooltip : PanelContainer
{
    private Vector2 globalPositionMouse;

    public override void _Process(double delta) => GlobalPosition = globalPositionMouse;

    public void _on_gui_input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mousMotionEvent)
            globalPositionMouse = mousMotionEvent.GlobalPosition;
    }
}