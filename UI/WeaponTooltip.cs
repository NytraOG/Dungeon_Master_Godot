using Godot;

namespace DungeonMaster.UI;

public partial class WeaponTooltip : PanelContainer
{
    private Vector2 originalSize;

    public override void _Ready() => originalSize = Size;

    public override void _Process(double delta) => SetSize(new Vector2(originalSize.X, 400)); //Hack, aus irgendwelche gründen wächst der Tooltip von alleine auf 2800+ px, wtf
}