using Godot;

namespace DungeonMaster.UI;

public partial class PauseMenu : PanelContainer
{

    public void _on_button_pressed()
    {
        Hide();
        GetTree().Paused = false;
    }
}