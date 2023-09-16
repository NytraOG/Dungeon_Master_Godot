using System.Linq;
using DungeonMaster.Models.Skills;
using DungeonMaster.Models.Skills.Statuseffects;

namespace DungeonMaster.Models.Enemies;

public abstract partial class Keyword : BaseUnitModificator
{
    public BaseSkill[] Skills;

    public void PopulateSkills(BaseUnit creature)
    {
        foreach (var skill in Skills)
        {
            if (creature.Skills.Any(s => s.DisplayName == skill.DisplayName))
                continue;

            if (skill is BaseSupportSkill supportSkill)
                supportSkill.PopulateBuffs(creature);

            creature.Skills.Add(skill);
        }
    }
}