using DungeonMaster.Enums;
using DungeonMaster.Models.Heroes;
using Godot;
using Environment = System.Environment;

namespace DungeonMaster.Models.Skills;

public abstract partial class BaseSummonSkill : BaseSkill
{
    [Export] public PackedScene[] UnitsToSpawn;

    public override void _Ready()
    {
        Category    = SkillCategory.Summon;
        Subcategory = SkillSubcategory.Special;
    }

    public override string GetTooltip(Hero hero, string damage = "0-0") => base.GetTooltip(hero, damage) +
                                                                           Environment.NewLine + Environment.NewLine +
                                                                           Description;
}