using System;
using System.Collections.Generic;
using DungeonMaster.Enums;
using Godot;
using Attribute = DungeonMaster.Enums.Attribute;

namespace DungeonMaster.Models.Items;

public abstract partial class BaseItem : Node3D
{
    public abstract string     FluffContent { get; }
    public abstract string     FluffAuthor  { get; }
    public abstract string     FluffDate    { get; }
    [Export]
    public          ItemRarity Rarity       { get; set; }

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

    public abstract string GetContentTooltip();

    public abstract string GetEffectTooltip();

    public abstract string GetBoniTooltip();

    public string GetRarityTooltip() => Rarity switch
    {
        ItemRarity.Bad => $"[color=gray][i][{Rarity.ToString()}][/i][/color]",
        ItemRarity.Normal => $"[color=white][i][{Rarity.ToString()}][/i][/color]",
        ItemRarity.Uncommon => $"[color=Greenyellow][i][{Rarity.ToString()}][/i][/color]",
        ItemRarity.Rare => $"[color=Royalblue][i][{Rarity.ToString()}][/i][/color]",
        ItemRarity.Legendary => $"[color=orange][i][{Rarity.ToString()}][/i][/color]",
        ItemRarity.Unique => $"[color=Mediumvioletred][i][{Rarity.ToString()}][/i][/color]",
        _ => throw new ArgumentOutOfRangeException()
    };

    public string GetNameTooltip() => Rarity switch
    {
        ItemRarity.Bad => $"[color=gray][u]{Name.ToString().AddSpacesToString()}[/u][/color]",
        ItemRarity.Normal => $"[color=white][u]{Name.ToString().AddSpacesToString()}[/u][/color]",
        ItemRarity.Uncommon => $"[color=Greenyellow][u]{Name.ToString().AddSpacesToString()}[/u][/color]",
        ItemRarity.Rare => $"[color=Royalblue][u]{Name.ToString().AddSpacesToString()}[/u][/color]",
        ItemRarity.Legendary => $"[color=orange][u]{Name.ToString().AddSpacesToString()}[/u][/color]",
        ItemRarity.Unique => $"[color=Mediumvioletred][u]{Name.ToString().AddSpacesToString()}[/u][/color]",
        _ => throw new ArgumentOutOfRangeException()
    };
}