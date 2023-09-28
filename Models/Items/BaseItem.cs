using DungeonMaster.Enums;
using Godot;

namespace DungeonMaster.Models.Items;

public abstract partial class BaseItem : Node3D
{
    public Keywords[] Keywords { get; set; }

    [ExportGroup("Effect")]
    [Export]
    public string Effect { get; set; } = "TODO";

    [Export]
    public string Boni { get; set; } = "TODO";

    [ExportGroup("Requirements")]
    [Export]
    public int LevelRequirement { get; set; }

    [Export]
    public Attribute RequiredAttribute        { get; set; }

    [Export]
    public int       RequiredLevelOfAttribute { get; set; }

    [ExportGroup("Misc")]
    [Export]
    public string Fluff { get; set; }
}