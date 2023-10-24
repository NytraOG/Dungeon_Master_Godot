using System.Collections.Generic;
using DungeonMaster.Enums;
using Godot;

namespace DungeonMaster.Models.Items;

public abstract partial class BaseItem : Node3D
{
    public abstract string FluffContent { get; }
    public abstract string FluffAuthor  { get; }
    public abstract string FluffDate    { get; }

    [Export]
    public Texture2D Icon { get; set; }

    public List<Keywords> Keywords { get; set; } = new();

    [ExportGroup("Effect")]
    [Export]
    public string Effect { get; set; } = "TODO";

    [Export]
    public string Boni { get; set; } = "TODO";

    [ExportGroup("Requirements")]
    [Export]
    public int LevelRequirement { get; set; }

    [Export]
    public Attribute RequiredAttribute { get; set; }

    [Export]
    public int RequiredLevelOfAttribute { get; set; }

    public abstract void Use(BaseUnit actor);

    public bool IsUsableBy(BaseUnit actor) => actor.Level >= LevelRequirement && actor.Get(RequiredAttribute) >= RequiredLevelOfAttribute;

    public abstract string GetTooltipContent();
}