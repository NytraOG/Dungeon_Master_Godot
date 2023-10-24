using Godot;

namespace DungeonMaster.UI;

public partial class WeaponTooltip : PanelContainer
{
    private Vector2 originalSize;

    public override void _Ready()
    {
        originalSize = Size;
        SetSize(new Vector2(originalSize.X, 350));
    }

}