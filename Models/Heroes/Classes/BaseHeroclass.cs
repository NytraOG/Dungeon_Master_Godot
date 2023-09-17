using System.Linq;
using DungeonMaster.Models.Skills;
using Godot;

namespace DungeonMaster.Models.Heroes.Classes;

public abstract partial class BaseHeroclass : Node3D
{
    [Export]                          public BaseSkill[] ProvidedSkills;
    [Export] [ExportGroup("Attack")]  public float       MeleeAttack;
    [Export]                          public float       RangedAttack;
    [Export]                          public float       MagicAttack;
    [Export]                          public float       SocialAttack;
    [Export] [ExportGroup("Defense")] public float       MeleeDefense;
    [Export]                          public float       RangedDefense;
    [Export]                          public float       MagicDefense;
    [Export]                          public float       SocialDefense;
    [Export] [ExportGroup("Misc")]    public float       Explosion;
    [Export]                          public float       Health;
    [Export]                          public float       Mana;

    [Export]
    public string Displayname { get; set; }

    public virtual void ApplyModifiers(BaseUnit unit)
    {
        unit.MeleeAttackratingModifier  += MeleeAttack;
        unit.RangedAttackratingModifier += RangedAttack;
        unit.MagicAttackratingModifier  += MagicAttack;
        unit.SocialAttackratingModifier += SocialAttack;
        unit.MeleeDefensmodifier        += MeleeDefense;
        unit.RangedDefensemodifier      += RangedDefense;
        unit.MagicDefensemodifier       += MagicDefense;
        unit.SocialDefensemodifier      += SocialDefense;
        unit.MaximumHitpoints           += unit.MaximumHitpoints * Health;
        unit.CurrentHitpoints           =  unit.MaximumHitpoints;
        unit.MaximumMana                += unit.MaximumMana * Mana;
        unit.CurrentMana                =  unit.MaximumMana;
    }

    public virtual void ApplySkills(BaseUnit unit)
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