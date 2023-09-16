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
    public delegate void BuffAppliedEventHandler(BaseUnit actor, BaseSkill skill, BaseUnit[] targets, string skillresult);

    [Signal]
    public delegate void BuffTickEventHandler(BaseUnit applicant, string effect, int remainingDuration, Color combatlogEffectColor); //Buff     Buff

    [Signal]
    public delegate void DebuffTickEventHandler(BaseUnit actor, int damage, int remainingDuration, Color combatlogEffectColor); //Debuff   Debuff

    [Signal]
    public delegate void HitEventHandler(BaseUnit actor, BaseSkill skill, int hitroll, HitResult hitResult, BaseUnit target, string skillresult);

    [Signal]
    public delegate void MiscEventHandler(string arg);

    [Signal]
    public delegate void MissEventHandler(BaseUnit actor, BaseSkill skill, int hitroll, HitResult hitResult, BaseUnit target, string skillresult);

    private bool combatActive;
    //public  List<Creature>       enemies = new();
    private List<Hero>           heroes = new();
    public  Hero                 SelectedHero;
    public  BaseSkill            SelectedSkill;
    public  List<BaseUnit>       SelectedTargets      = new();
    public  List<SkillSelection> SkillSelection       = new();
    public  List<BaseSkill>      SkillsOfSelectedHero = new();


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