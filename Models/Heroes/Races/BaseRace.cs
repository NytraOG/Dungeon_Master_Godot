using System.Linq;
using DungeonMaster.Models.Skills;
using Godot;

namespace DungeonMaster.Models.Heroes.Races;

public abstract partial class BaseRace : Node3D
{
    [Export] public string      DisplayName;
    [Export] public int         ModifierCharisma;
    [Export] public int         ModifierConstitution;
    [Export] public int         ModifierDexterity;
    [Export] public int         ModifierIntuition;
    [Export] public int         ModifierLogic;
    [Export] public int         ModifierQuickness;
    [Export] public int         ModifierStrength;
    [Export] public int         ModifierWillpower;
    [Export] public int         ModifierWisdom;
    [Export] public BaseSkill[] ProvidedSkills;

    [Export]
    public string Displayname { get; set; }

    public void ApplyModifiers(BaseUnit unit)
    {
        unit.Strength     += ModifierStrength;
        unit.Constitution += ModifierConstitution;
        unit.Dexterity    += ModifierDexterity;
        unit.Quickness    += ModifierQuickness;
        unit.Intuition    += ModifierIntuition;
        unit.Logic        += ModifierLogic;
        unit.Willpower    += ModifierWillpower;
        unit.Wisdom       += ModifierWisdom;
        unit.Charisma     += ModifierCharisma;
    }

    public void ApplySkills(BaseUnit unit)
    {
        foreach (var skillToProvide in ProvidedSkills)
        {
            if (unit.Skills.Any(a => a.Name == skillToProvide.Name))
                return;

            if (skillToProvide is BaseSupportSkill supportSkill)
                supportSkill.PopulateBuffs(unit);

            unit.Skills.Add(skillToProvide);
        }
    }
}