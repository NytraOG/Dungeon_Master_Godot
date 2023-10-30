using System;
using System.Linq;
using DungeonMaster.Enums;
using DungeonMaster.Models;
using DungeonMaster.Models.Enemies;
using DungeonMaster.Models.Heroes;
using DungeonMaster.Models.Skills;
using DungeonMaster.Models.Skills.Statuseffects.Buffs;
using DungeonMaster.Models.Skills.Statuseffects.Debuffs;
using Godot;

namespace DungeonMaster.UI;

public partial class CombatLog : Control
{
    public const  string          NormalDamageColor   = "#f7f7f7";
    public const  string          GoodDamageColor     = "#e0e00c";
    public const  string          CriticalDamageColor = "#ef6703";
    private const string          HealColor           = "green";
    private const string          CreatureColor       = "#f16a67";
    private const string          HeroColor           = "#5cce5c";
    private const string          NotEnoughManaColor  = "#EA0201";
    private const string          EnoughManaColor     = "#5cce5c";
    private const string          StunnedText         = "[color=yellow]Stun[/color] applied";
    private       string          font                = "font_color";
    private       VBoxContainer   logContainer;
    private       PackedScene     logentryScene;
    private       ScrollContainer scrollContainer;

    [Export]
    public int MaxEntries { get; set; } = 100;

    public override void _Ready()
    {
        logContainer  = GetNode<VBoxContainer>("%LogContainer");
        logentryScene = ResourceLoader.Load<PackedScene>("res://UI/combat_log_entry.tscn");

        scrollContainer = GetNode<PanelContainer>(nameof(PanelContainer))
                         .GetNode<MarginContainer>(nameof(MarginContainer))
                         .GetNode<ScrollContainer>(nameof(ScrollContainer));
    }

    public void LogBuffTick(BaseUnit applicant, Buff buff, string effect, int remainingDuration)
    {
        switch (remainingDuration)
        {
            case < 0: return;
            case 0:
                Log($"[color={buff.CombatlogColorHex}]{buff.Name}[/color] " +
                    $"expired on {FetchUnitnameWithMatchingColor(applicant)}.");
                break;
            default:
                Log($"[color=#{buff.CombatlogColorHex}]{buff.Name}[/color] " +
                    $"{remainingDuration} turns remaining on {FetchUnitnameWithMatchingColor(applicant)}.");
                break;
        }
    }

    public void LogDebuffTick(BaseUnit sufferingBoy, Debuff debuff, string damage, int remainingDuration)
    {
        if (remainingDuration < 0)
            return;

        if (debuff.DamagePerTick != 0)
        {
            Log($"{FetchUnitnameWithMatchingColor(sufferingBoy)} lost {FetchDamageText(damage)} Health " +
                $"to [color={debuff.CombatlogColorHex}]{debuff.Name}[/color], " +
                $"{remainingDuration} turns remaining.");
        }
        else
        {
            if (remainingDuration == 0)
            {
                Log($"[color={debuff.CombatlogColorHex}]{debuff.Name}[/color] " +
                    $"expired on {FetchUnitnameWithMatchingColor(sufferingBoy)}.");
            }
            else
            {
                Log($"[color={debuff.CombatlogColorHex}]{debuff.Name}[/color] " +
                    $"{remainingDuration} turns remaining on {FetchUnitnameWithMatchingColor(sufferingBoy)}.");
            }
        }

        ScrollToBottom();
    }

    public void LogSpawn(BaseUnit spawnedUnit)
    {
        Log($"{FetchUnitnameWithMatchingColor(spawnedUnit)} level {spawnedUnit.Level} appeared.");
    }

    public void LogHit(BaseUnit actor, BaseSkill usedSkill, int hitroll, HitResult hitresult, BaseUnit target, string result)
    {
        var content = $"{FetchUnitnameWithMatchingColor(actor)}'s[{(int)actor.ModifiedInitiative}] {usedSkill.Name} " +
                      $"hit[{hitroll}] {FetchUnitnameWithMatchingColor(target)}[{FetchDefenseattribute(usedSkill.Category, target)}] " +
                      $"for {FetchDamageText(hitresult, result)} damage.";

        if (target.IsStunned)
            content += StunnedText;

        if (target.IsDead)
            content += " [color=red]FATAL![/color]";

        Log(content);
    }

    public void LogMiss(BaseUnit actor, BaseSkill usedSkill, int hitroll, BaseUnit target)
    {
        var content = $"{FetchUnitnameWithMatchingColor(actor)}'s[{(int)actor.ModifiedInitiative}] {usedSkill.Name} " +
                      $"missed[{hitroll}] {FetchUnitnameWithMatchingColor(target)}[{FetchDefenseattribute(usedSkill.Category, target)}].";

        Log(content);
    }

    public void LogBuffApplied(BaseUnit actor, BaseSkill skill, BaseUnit[] targets)
    {
        var content = $"{FetchUnitnameWithMatchingColor(actor)}[{(int)actor.ModifiedInitiative}] used {skill.Name} " +
                      $"on {string.Join(", ", targets.Select(FetchUnitnameWithMatchingColor))}.";

        if (targets.Any(t => t.IsStunned))
            content += StunnedText;

        Log(content);
    }

    public void Log(string message)
    {
        var entry = logentryScene.Instantiate<RichTextLabel>();
        entry.Text = message;

        logContainer.AddChild(entry);

        if (logContainer.GetChildCount() >= MaxEntries)
        {
            var childToKill = logContainer.GetChild(0);
            logContainer.RemoveChild(childToKill);
            childToKill.QueueFree();
        }

        ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        var scrollBar = scrollContainer.GetVScrollBar();

        if (scrollBar is not null && Math.Abs(scrollBar.Value - scrollBar.MaxValue) > 0.00001f)
            scrollBar.Value = scrollBar.MaxValue;
    }

    private string FetchUnitnameWithMatchingColor(BaseUnit unit) => unit switch
    {
        Hero => $"[color={HeroColor}]{unit.Name}[/color]",
        BaseCreature => $"[color={CreatureColor}]{unit.Name}[/color]",
        _ => $"[color=white]{unit.Name}[/color]"
    };

    private string FetchDamageText(string damage) => $"[color={NormalDamageColor}]{damage}[/color]";

    private string FetchDamageText(HitResult hitresult, string result) => hitresult switch
    {
        HitResult.Heal => $"[color={HealColor}]{result}[/color]",
        HitResult.Good => $"[color={GoodDamageColor}]{result}![/color]",
        HitResult.Critical => $"[color={CriticalDamageColor}]{result}!![/color]",
        _ => $"[color={NormalDamageColor}]{result}[/color]"
    };

    private int FetchDefenseattribute(SkillCategory skillCategory, BaseUnit target) => skillCategory switch
    {
        SkillCategory.Melee => (int)target.ModifiedMeleeDefense,
        SkillCategory.Ranged => (int)target.ModifiedRangedDefense,
        SkillCategory.Magic => (int)target.ModifiedMagicDefense,
        SkillCategory.Social => (int)target.ModifiedSocialDefense,
        _ => throw new ArgumentOutOfRangeException()
    };
}