namespace DungeonMaster.Models.Skills.Defense;

public partial class HorribleParade : BaseDefenseSkill
{
    public override int GetTacticalRoll(BaseUnit actor) => 15;
}