using System.Collections.Generic;
using DungeonMaster.Enums;
using DungeonMaster.Models;
using DungeonMaster.Models.Heroes;
using DungeonMaster.Models.Skills;
using Godot;

namespace DungeonMaster;

public partial class Main : Node
{
    [Signal]
    public delegate void BuffAppliedEventHandler(BaseUnit actor, BaseSkill skill, List<BaseUnit> targets, string skillresult);

    [Signal]
    public delegate void BuffTickEventHandler(BuffResolutionArgs args);

    [Signal]
    public delegate void DebuffTickEventHandler(DebuffResolutionArgs args);

    [Signal]
    public delegate void HitEventHandler(CombatskillResolutionArgs args);

    [Signal]
    public delegate void MiscEventHandler(string arg);

    [Signal]
    public delegate void MissEventHandler(CombatskillResolutionArgs args);

    private bool                 combatActive;
    //public  List<Creature>       enemies = new();
    private List<Hero>           heroes  = new();
    public  Hero                 selectedHero;
    public  BaseSkill            selectedSkill;
    public  List<BaseUnit>       selectedTargets      = new();
    public  List<SkillSelection> SkillSelection       = new();
    public  List<BaseSkill>      skillsOfSelectedHero = new();

    // [Signal]
    // public delegate void HealthDepletedEventHandler();

    public override void _Ready() { }

    public override void _Process(double delta) { }

    private void _on_start_round_pressed() { }

    private void _on_undo_pressed() { }
}

public struct SkillSelection
{
    public BaseSkill      Skill   { get; set; }
    public List<BaseUnit> Targets { get; set; }
    public BaseUnit       Actor   { get; set; }
}

public struct CombatskillResolutionArgs
{
    public BaseUnit  Actor       { get; set; }
    public BaseSkill Skill       { get; set; }
    public int       Hitroll     { get; set; }
    public HitResult HitResult   { get; set; }
    public BaseUnit  Target      { get; set; }
    public string    Skillresult { get; set; }
}

public struct BuffResolutionArgs
{
    //public Buff     Buff                 { get; set; }
    public BaseUnit Applicant            { get; set; }
    public string   Effect               { get; set; }
    public int      RemainingDuration    { get; set; }
    public Color    CombatlogEffectColor { get; set; }
}

public struct DebuffResolutionArgs
{
    //public Debuff   Debuff               { get; set; }
    public BaseUnit Actor                { get; set; }
    public int      Damage               { get; set; }
    public int      RemainingDuration    { get; set; }
    public Color    CombatlogEffectColor { get; set; }
}