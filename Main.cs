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
    public delegate void HitEventHandler(BaseUnit actor, BaseSkill skill, int hitroll, int hitResult, BaseUnit target, string skillresult);

    [Signal]
    public delegate void MiscEventHandler(string arg);

    [Signal]
    public delegate void MissEventHandler(BaseUnit actor, BaseSkill skill, int hitroll, int hitResult, BaseUnit target, string skillresult);

    private bool combatActive;
    //public  List<Creature>       Enemies = new();
    private List<Hero>           heroes = new();
    public  Hero                 SelectedHero;
    public  BaseSkill            SelectedSkill;
    public  List<BaseUnit>       SelectedTargets      = new();
    public  List<SkillSelection> SkillSelection       = new();
    public  List<BaseSkill>      SkillsOfSelectedHero = new();

    public bool PlayerIsTargeting => SelectedHero is not null &&
                                     SelectedSkill is BaseDamageSkill { TargetableFaction: Factions.Foe }
                                             or BaseTargetingSkill { TargetableFaction: Factions.All or Factions.Friend };

    //public override void _Ready() => ConfigureAbilityButtons();

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed(Keys.Backspace))
        {
            //SelectedTargets.ForEach(t => t.GetComponent<SpriteRenderer>().material = defaultMaterial);
            SelectedTargets.Clear();
        }

        MachEnemiesCombatReady();
        // SetupCombatants();
        // AbilityspritesAuffrischen();
    }

    private void MachEnemiesCombatReady()
    {
        if (combatActive)
            return;

        // var notCombatReadyEnemies = Enemies.Where(e => !e.IsDead &&
        //                                                e.SelectedSkill is null);
        //
        // foreach (var enemy in notCombatReadyEnemies)
        // {
        //     enemy.PickSkill();
        //     enemy.InitiativeBestimmen();
        //
        //     AbilitySelection.Add(new AbilitySelection
        //     {
        //         Skill   = enemy.SelectedSkill,
        //         Actor   = enemy,
        //         Targets = FindTargets(enemy)
        //     });
        // }
    }
    private void _on_start_round_pressed() { }

    private void _on_undo_pressed() { }
}

public struct SkillSelection
{
    public BaseSkill      Skill   { get; set; }
    public List<BaseUnit> Targets { get; set; }
    public BaseUnit       Actor   { get; set; }
}