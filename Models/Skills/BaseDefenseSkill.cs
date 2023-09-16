using DungeonMaster.Enums;

namespace DungeonMaster.Models.Skills;

public partial class BaseDefenseSkill : BaseSkill
{
    public override void _Ready() => Subcategory = SkillSubcategory.Defense;

    public override string Activate(BaseUnit actor) => GetTacticalRoll(actor).ToString();
}