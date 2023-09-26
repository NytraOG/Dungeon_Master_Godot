using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DungeonMaster.Enums;
using DungeonMaster.Models;
using DungeonMaster.Models.Enemies;
using DungeonMaster.Models.Heroes;
using DungeonMaster.Models.Skills;
using DungeonMaster.Models.Skills.Statuseffects.Buffs;
using DungeonMaster.Models.Skills.Statuseffects.Debuffs;
using DungeonMaster.UI.Menues.Buttons;
using DungeonMaster.UI.Status;
using Godot;

namespace DungeonMaster;

public partial class Main : Node,
                            INotifyPropertyChanged
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

    public  bool AllesDa      { get; set; }
    private bool combatActive { get; set; }

    [Export]
    public Texture2D DefaultIcon { get; set; }

    public BaseCreature[]                    Enemies            { get; set; } = Array.Empty<BaseCreature>();
    public Healthbar                         Healthbar          { get; set; }
    public Hero[]                            Heroes             { get; set; } = Array.Empty<Hero>();
    public Manabar                           Manabar            { get; set; }
    public BaseCreature                      SelectedEnemy      { get; set; }
    public Hero                              SelectedHero       { get; set; }
    public BaseSkill                         SelectedSkill      { get; set; }
    public List<BaseUnit>                    SelectedTargets    { get; set; } = new();
    public List<BaseSkillButton>             Skillbuttons       { get; set; } = new();
    public List<SkillSelection>              SkillSelection     { get; set; } = new();
    public TextureButton                     ConfirmationButton { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;

    private async void _on_start_round_pressed() => await HandleBattleround();

    public override void _Ready()
    {
        ConfirmationButton = GetNode<TextureButton>("ConfirmationButton");
        Healthbar          = GetNode<Healthbar>("Healthbar");
        Manabar            = GetNode<Manabar>("Manabar");

        SubscribeToSkillbuttons();
        PopulateSkillButtons();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed(Keys.Backspace))
            SelectedTargets.Clear();

        MachEnemiesCombatReady();
        SetupCombatants();
    }

    private async Task HandleBattleround()
    {
        SetProcess(false);
        combatActive = true;

        if (!SkillSelection.Any())
            Console.WriteLine("No Skills have been selected");
        else
        {
            SkillSelection = SkillSelection.OrderByDescending(a => a.Actor.ModifiedInitiative)
                                           .ToList();

            foreach (var selection in SkillSelection)
            {
                if (selection.Actor.IsDead)
                {
                    await WaitFor(500);

                    continue;
                }

                if (selection.Actor.IsStunned)
                {
                    EmitSignal(SignalName.Misc, $"{selection.Actor.Displayname}'s <b><color=yellow>Stun</color></b> expired");

                    //InstantiateFloatingCombatText(selection.Actor, "STUNNED");

                    selection.Actor.IsStunned = false;

                    if (selection.Actor is BaseCreature baseCreature)
                        baseCreature.SelectedSkill = null;

                    await WaitFor(1000);
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

                    await WaitFor(1000);
                }

                await ResolveDebuffs(selection);
                await ResolveBuffs(selection);

                ResolveSupportSkills(selection);

                if (selection.Actor is BaseCreature creature)
                    creature.SelectedSkill = null;
            }

            SelectedEnemy = null;
            SelectedSkill = null;
            SkillSelection.Clear();
            SelectedTargets.Clear();
        }

        //Heroes.ForEach(h => h.GetComponent<SpriteRenderer>().material = heroOutlineMaterial);

        combatActive = false;

        SetProcess(true);
        EmitSignal(SignalName.Misc, "-----------------------------------------------------------------------------------------------");
    }

    private static void ResolveSupportSkills(SkillSelection selection)
    {
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

    private async Task ResolveBuffs(SkillSelection selection)
    {
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

        if (selection.Actor.Buffs.Any() || buffsToKill.Any())
            await WaitFor(1000);
    }

    private async Task ResolveDebuffs(SkillSelection selection)
    {
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

            await WaitFor(1000);
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

            await WaitFor(1000);
        }

        foreach (var debuff in debuffsToKill)
            debuff.Die(selection.Actor);
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

        if(selection.Actor is null)
            return;

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
        if (combatActive || !Enemies.Any())
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

        var maxTargets = skill.GetTargets(creature) > Enemies.Length ? Enemies.Length : skill.GetTargets(creature);
        var retVal     = new List<BaseUnit>();

        var eligableTargets = new List<BaseUnit>();

        if (creature.SelectedSkill is BaseSupportSkill supportSkill)
        {
            if (supportSkill.TargetsWholeGroup)
                maxTargets = Enemies.Length;

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
        if (AllesDa)
            return;

        Enemies = this.GetAllChildren<BaseCreature>();
        Heroes  = this.GetAllChildren<Hero>();

        foreach (var hero in Heroes)
        {
            hero.OnSelected -= HeroOnSelected;
            hero.OnSelected += HeroOnSelected;
        }

        foreach (var creature in Enemies)
        {
            creature.OnSomeSignal -= CreatureOnOnSomeSignal;
            creature.OnSomeSignal += CreatureOnOnSomeSignal;
        }

        AllesDa = true;
    }

    private void CreatureOnOnSomeSignal(BaseCreature creature)
    {
        SelectedEnemy = creature;

        if (SelectedSkill is not BaseTargetingSkill tSkill)
            return;

        var maxTargets        = tSkill.GetTargets(SelectedHero);
        var maxTargetsReached = SelectedTargets.Count == maxTargets;

        if (maxTargetsReached)
        {
            Console.WriteLine($"Target maximum {maxTargets} reached for {tSkill.Displayname}");
            return;
        }

        SelectedTargets.Add(creature);
    }

    private void HeroOnSelected(Hero hero)
    {
        SelectedHero = hero;
        Healthbar.SetDisplayedHero(SelectedHero);
        Manabar.SetDisplayedHero(SelectedHero);

        var amountOfHeroSkills = hero.Skills.Count;

        for (var i = 0; i < Skillbuttons.Count; i++)
        {
            var skillbutton = Skillbuttons[i];

            if (i >= amountOfHeroSkills)
            {
                skillbutton.Skill         = null;
                skillbutton.TextureNormal = DefaultIcon;

                continue;
            }

            skillbutton.Skill         = hero.Skills[i];
            skillbutton.TextureNormal = hero.Skills[i].Icon;
        }
    }

    private void PopulateSkillButtons() { }

    private void SubscribeToSkillbuttons()
    {
        Skillbuttons = GetNode<Control>("SkillBar")
                      .GetNode<HBoxContainer>("SkillContainer")
                      .GetChildren()
                      .Cast<BaseSkillButton>()
                      .ToList();

        foreach (var skill in Skillbuttons)
            skill.SomeSkillbuttonPressed += SkillOnSomeSkillbuttonPressed;
    }

    private void SkillOnSomeSkillbuttonPressed(BaseSkillButton sender) => SelectedSkill = sender.Skill;

    private void _on_undo_pressed()
    {
        var wolf = Enemies[0];
        wolf.PickSkill();
        wolf.SelectedSkill.Activate(wolf);
    }

    public void _on_skill_button_timed_out(BaseSkillButton sender)
    {
        sender.Disabled  = false;
        sender.Time.Text = string.Empty;
        sender.SetProcess(false);
    }

    public void _on_confirmation_button_pressed() => ConfirmSkillselection();

    private void ConfirmSkillselection()
    {
        if (SkillSelection.Any(s => s.Actor == SelectedHero))
            Console.WriteLine("Selected Hero is already acting");
        else if (SelectedSkill is not null && !SelectedTargets.Any())
            Console.WriteLine("No Targets selected");
        else if (SelectedSkill is null)
            Console.WriteLine("No Ability selected");
        else
        {
            SelectedHero.InitiativeBestimmen();

            var selection = new SkillSelection
            {
                Skill = SelectedSkill,
                Actor = SelectedHero
            };

            if (selection.Skill is BaseTargetingSkill { AutoTargeting: true, TargetableFaction: Factions.Foe } skill)
            {
                var remainingTargetsAmount = skill.GetTargets(selection.Actor) - 1;

                var remainingTargets = Enemies.Except(SelectedTargets)
                                              .ToList();

                for (var i = 0; i < remainingTargetsAmount; i++)
                {
                    if (i >= remainingTargets.Count)
                        continue;

                    SelectedTargets.Add(remainingTargets[i]);
                }
            }

            selection.Targets = SelectedTargets.ToArray();

            SkillSelection.Add(selection);
        }

        SelectedTargets.Clear();

        SelectedSkill = null;
    }

    private async Task WaitFor(int milliseconds) => await ToSignal(GetTree().CreateTimer((double)milliseconds / 1000), "timeout");

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public struct SkillSelection
{
    public BaseSkill  Skill   { get; set; }
    public BaseUnit[] Targets { get; set; }
    public BaseUnit   Actor   { get; set; }
}