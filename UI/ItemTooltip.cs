using Godot;

namespace DungeonMaster.UI;

public partial class ItemTooltip : BaseTooltip
{
    private Vector2 originalSize;

    public override void _Ready() => originalSize = Size;

    //Hack, aus irgendwelche gründen wächst der Tooltip von alleine auf 2800+ px, wtf
    public override void _Process(double delta) => SetSize(new Vector2(originalSize.X, 400));
}