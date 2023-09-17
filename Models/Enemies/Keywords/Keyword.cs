using System.Linq;
using DungeonMaster.Models.Skills;
using DungeonMaster.Models.Skills.Statuseffects;
using Godot;

namespace DungeonMaster.Models.Enemies.Keywords;

public abstract partial class Keyword : BaseUnitModificator
{
    [Export] public BaseSkill[] Skills;

    public void PopulateSkills(BaseUnit creature)
    {
        foreach (var skill in Skills)
        {
            if (creature.Skills.Any(s => s.Displayname == skill.Displayname))
                continue;

            if (skill is BaseSupportSkill supportSkill) //??
                supportSkill.PopulateBuffs(creature);

            creature.Skills.Add(skill);
        }
    }
}