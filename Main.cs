using System;
using System.Collections.Generic;
using System.Linq;
using DungeonMaster.Enums;
using DungeonMaster.Models;
using DungeonMaster.Models.Enemies;
using DungeonMaster.Models.Heroes;
using DungeonMaster.Models.Skills;
using DungeonMaster.Models.Skills.Statuseffects.Buffs;
using DungeonMaster.Models.Skills.Statuseffects.Debuffs;
using Godot;

namespace DungeonMaster;

public partial class Main : Node
{
    [Signal]
    public delegate void BuffAppliedEventHandler(BaseUnit actor, BaseSkill skill, BaseUnit[] targets, string skillresult);

    [Signal]
    public delegate void BuffTickEventHandler(BaseUnit applicant, string effect, int remainingDuration, Color combatlogEffectColor, Buff buff);

    [Signal]
    public delegate void DebuffTickEventHandler(BaseUnit actor, int damage, int remainingDuration, Color combatlogEffectColor, Debuff debuff);

    [Signal]
    public delegate void HitEventHandler(BaseUnit actor, BaseSkill skill, int hitroll, int hitResult, BaseUnit target, string skillresult);

    [Signal]
    public delegate void MiscEventHandler(string arg);

    [Signal]
    public delegate void MissEventHandler(BaseUnit actor, BaseSkill skill, int hitroll, int hitResult, BaseUnit target, string skillresult);

    private          bool                 allesDa;
    private          bool                 combatActive;
    public           List<BaseCreature>   Enemies = new();
    private readonly List<Hero>           Heroes  = new();
    public           Hero                 SelectedHero;
    public           BaseSkill            SelectedSkill;
    public           List<BaseUnit>       SelectedTargets      = new();
    public           List<SkillSelection> SkillSelection       = new();
    public           List<BaseSkill>      SkillsOfSelectedHero = new();

    public bool PlayerIsTargeting => SelectedHero is not null &&
                                     SelectedSkill is BaseDamageSkill { TargetableFaction: Factions.Foe }
                                             or BaseTargetingSkill { TargetableFaction: Factions.All or Factions.Friend };

    private void _on_start_round_pressed() => HandleBattleround();

    public override void _Ready() => PopulateSkillButtons();

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed(Keys.Backspace))
        {
            //SelectedTargets.ForEach(t => t.GetComponent<SpriteRenderer>().material = defaultMaterial);
            SelectedTargets.Clear();
        }

        MachEnemiesCombatReady();
        SetupCombatants();
    }

    private void HandleBattleround()
    {
        combatActive = true;

        if (!SkillSelection.Any())
        {
            //ShowToast("No Abilities have been selected", 1);
        }
        else
        {
            SkillSelection = SkillSelection.OrderByDescending(a => a.Actor.ModifiedInitiative)
                                           .ToList();

            foreach (var selection in SkillSelection)
            {
                if (selection.Actor.IsDead)
                {
                    //yield return new WaitForSeconds(0.5f);
                    continue;
                }

                if (selection.Actor.IsStunned)
                {
                    EmitSignal(SignalName.Misc, $"{selection.Actor.Displayname}'s <b><color=yellow>Stun</color></b> expired");

                    //InstantiateFloatingCombatText(selection.Actor, "STUNNED");

                    selection.Actor.IsStunned = false;

                    if (selection.Actor is BaseCreature baseCreature)
                        baseCreature.SelectedSkill = null;

                    //yield return new WaitForSeconds(1f);
                }
                else if (selection.Targets.Any() && selection.Targets.All(t => t.IsDead) && selection.Actor is BaseCreature baseCreature)
                {
                    baseCreature.SelectedSkill = null;
                    continue;
                }
                else if (selection.Targets.Any() && selection.Targets.All(t => t.IsDead)) { }
                else
                {
                    ProcessSkillactivation(selection);

                    //yield return new WaitForSeconds(1f);
                }

                #region Resolve Debuffs

                var debuffsToKill           = new List<Debuff>();
                var stackableDebuffs        = selection.Actor.Debuffs.Where(d => d.IsStackable).ToList();
                var groupedStackableDebuffs = stackableDebuffs.GroupBy(d => d.Displayname);
                var unstackableDebuffs      = selection.Actor.Debuffs.Except(stackableDebuffs);

                foreach (var debuffs in groupedStackableDebuffs)
                {
                    var cumulatedDamage = ResolveEffect(debuffs, debuffsToKill, selection, out var remainingDuration);

                    EmitSignal(SignalName.DebuffTick, selection.Actor, cumulatedDamage, remainingDuration, debuffs.First().CombatlogEffectColor, debuffs.First());

                    if (cumulatedDamage <= 0)
                        continue;

                    //ProcessFloatingCombatText(cumulatedDamage.ToString(), HitResult.None, selection.Actor);

                    //yield return new WaitForSeconds(1f);
                }

                foreach (var debuff in unstackableDebuffs)
                {
                    debuff.ResolveTick(selection.Actor);

                    if (debuff.DurationEnded)
                        debuffsToKill.Add(debuff);

                    EmitSignal(SignalName.DebuffTick, selection.Actor, debuff.DamagePerTick, debuff.RemainingDuration, debuff.CombatlogEffectColor, debuff);

                    if (debuff.DamagePerTick <= 0)
                        continue;

                    //ProcessFloatingCombatText(debuff.damagePerTick.ToString(), HitResult.None, selection.Actor);

                    //yield return new WaitForSeconds(1f);
                }

                foreach (var debuff in debuffsToKill)
                    debuff.Die(selection.Actor);

                #endregion

                #region Resolve Buffs

                var buffsToKill           = new List<Buff>();
                var stackableBuffs        = selection.Actor.Buffs.Where(b => b.IsStackable).ToList();
                var groupedStackableBuffs = stackableBuffs.GroupBy(b => b.Displayname);
                var unstackableBuffs      = selection.Actor.Buffs.Except(stackableBuffs);

                foreach (var buffs in groupedStackableBuffs)
                {
                    buffs.ToList()
                         .ForEach(b =>
                          {
                              b.ResolveTick(selection.Actor);

                              if (b.DurationEnded)
                                  buffsToKill.Add(b);
                          });

                    EmitSignal(SignalName.BuffTick, selection.Actor, "EFFECT", buffs.Max(b => b.RemainingDuration), buffs.First().CombatlogEffectColor, buffs.First());

                    //ProcessFloatingCombatText($"{buffs.Key} TICK", HitResult.None, selection.Actor);
                }

                foreach (var buff in unstackableBuffs)
                {
                    buff.ResolveTick(selection.Actor);

                    if (buff.DurationEnded)
                        buffsToKill.Add(buff);

                    EmitSignal(SignalName.BuffTick, selection.Actor, "EFFECT", buff.RemainingDuration, buff.CombatlogEffectColor, buff);

                    // if (buff.RemainingDuration >= 0)
                    //     ProcessFloatingCombatText($"{buff.name} TICK", HitResult.None, selection.Actor);
                }

                foreach (var buff in buffsToKill)
                    buff.Die(selection.Actor);

                #endregion

                // if (selection.Actor.Buffs.Any() || buffsToKill.Any())
                //     yield return new WaitForSeconds(1f);

                var skillsToRemove = new List<BaseSupportSkill>();
                var skillsToCheck  = new List<BaseSupportSkill>();

                foreach (var skill in selection.Actor.ActiveSkills)
                {
                    if (skill.Value)
                    {
                        skill.Key.Reverse(selection.Actor);
                        skillsToRemove.Add(skill.Key);
                    }
                    else
                        skillsToCheck.Add(skill.Key);
                }

                foreach (var skill in skillsToCheck)
                    selection.Actor.ActiveSkills[skill] = true;

                foreach (var skill in skillsToRemove)
                    selection.Actor.ActiveSkills.Remove(skill);
            }

            SkillSelection.Clear();
        }

        //Heroes.ForEach(h => h.GetComponent<SpriteRenderer>().material = heroOutlineMaterial);

        combatActive = false;

        EmitSignal(SignalName.Misc, "-----------------------------------------------------------------------------------------------");
    }

    private static int ResolveEffect(IGrouping<string, Debuff> debuffs, List<Debuff> debuffsToKill, SkillSelection selection, out int remainingDuration)
    {
        var cumulatedDamage = debuffs.Sum(d => d.DamagePerTick);
        remainingDuration = debuffs.Max(d => d.RemainingDuration) - 1;

        debuffs.ToList()
               .ForEach(d =>
                {
                    d.RemainingDuration--;

                    if (d.DurationEnded)
                        debuffsToKill.Add(d);
                });

        selection.Actor.CurrentHitpoints -= cumulatedDamage;

        return cumulatedDamage;
    }

    private void ProcessSkillactivation(SkillSelection selection)
    {
        switch (selection.Actor)
        {
            case Hero when selection.Skill is BaseDamageSkill damageSkill:
                ResolveDamageSkill(selection, damageSkill);
                break;
            case Hero hero when selection.Skill is BaseSummonSkill summonSkill:
                ResolveSummonSkill(selection, hero, summonSkill);
                break;

            case BaseCreature when selection.Skill is BaseDamageSkill foeDamageSkill:
                ResolveDamageSkill(selection, foeDamageSkill);
                break;
            case BaseCreature creature when selection.Skill is BaseSummonSkill foeSummonSkill:
                ResolveSummonSkill(selection, creature, foeSummonSkill);
                break;

            default:
                UseSupportskill(selection);
                break;
        }

        selection.Actor.CurrentMana -= selection.Skill.Manacost;
    }

    private void ResolveDamageSkill(SkillSelection selection, BaseDamageSkill damageSkill)
    {
        foreach (var target in selection.Targets)
            damageSkill.Activate(selection.Actor, target);
    }

    private void ResolveSummonSkill(SkillSelection selection, BaseUnit unit, BaseSummonSkill summonSkill) => unit.UseAbility(summonSkill, HitResult.None); //InstantiateFloatingCombatText(selection.Actor, $"<b>{summonSkill.displayName}</b>!");

    private void UseSupportskill(SkillSelection selection)
    {
        foreach (var target in selection.Targets)
        {
            var skillResult = selection.Actor.UseAbility(selection.Skill, HitResult.None, target) + " ";

            EmitSignal(SignalName.BuffApplied, selection.Actor, selection.Skill, selection.Targets.ToArray(), skillResult);

            //ProcessFloatingCombatText(skillResult, HitResult.None, selection.Actor);
        }
    }

    private void MachEnemiesCombatReady()
    {
        if (combatActive)
            return;

        var notCombatReadyEnemies = Enemies.Where(e => !e.IsDead &&
                                                       e.SelectedSkill is null);

        foreach (var enemy in notCombatReadyEnemies)
        {
            enemy.PickSkill();
            enemy.InitiativeBestimmen();

            SkillSelection.Add(new SkillSelection
            {
                Skill   = enemy.SelectedSkill,
                Actor   = enemy,
                Targets = FindTargets(enemy)
            });
        }
    }

    private BaseUnit[] FindTargets(BaseCreature creature)
    {
        if (creature.SelectedSkill is not BaseTargetingSkill skill)
            return Array.Empty<BaseUnit>();

        var maxTargets = skill.GetTargets(creature) > Enemies.Count ? Enemies.Count : skill.GetTargets(creature);
        var retVal     = new List<BaseUnit>();

        var eligableTargets = new List<BaseUnit>();

        if (creature.SelectedSkill is BaseSupportSkill supportSkill)
        {
            if (supportSkill.TargetsWholeGroup)
                maxTargets = Enemies.Count;

            eligableTargets.AddRange(Enemies.Where(e => !e.IsDead));
        }
        else
            eligableTargets.AddRange(Heroes.Where(h => !h.IsDead));

        for (var i = 0; i < maxTargets; i++)
        {
            if (i < eligableTargets.Count)
                retVal.Add(eligableTargets[i]);
        }

        return retVal.ToArray();
    }

    private void SetupCombatants()
    {
        if (allesDa)
            return;

        //Enemies = Seed Enemies hier!
        //Heroes  = Seed Heroes hier!

        allesDa = true;
    }

    private void PopulateSkillButtons() { }

    private void _on_undo_pressed() { }
    private void _on_orc_energist_input_event(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIndex, string argument1)
    {
        if(@event is InputEventMouseButton { Pressed: true } button)
        {
        }
    }
}

public struct SkillSelection
{
    public BaseSkill  Skill   { get; set; }
    public BaseUnit[] Targets { get; set; }
    public BaseUnit   Actor   { get; set; }
}