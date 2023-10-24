using Godot;

namespace DungeonMaster.UI.Status;

public partial class Unitframe : PanelContainer
{
    [Export]
    public TextureRect DefaultIcon { get; set; }

    public override void _Ready() { }

    public override void _Process(double delta) { }
}