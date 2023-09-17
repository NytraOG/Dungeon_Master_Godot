using System;
using System.Collections.Generic;
using System.Linq;
using DungeonMaster.Models;
using DungeonMaster.Models.Skills.Statuseffects;
using DungeonMaster.Models.Skills.Statuseffects.Debuffs;
using Godot;

namespace DungeonMaster;

public static class Extensions
{
    /// <summary>
    ///     Returns (XpCost, GoldCost)
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="difficulty"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    // public static (int, int) GetUpgradeCosts(this BaseSkill skill, SkillDifficulty difficulty) => difficulty switch
    // {
    //     SkillDifficulty.Basic => (GetXpTotal(skill.level + 1, skill.xpBaseBasic), GetGoldTotal(skill.level + 1, skill.xpBaseBasic)),
    //     SkillDifficulty.Demanding => (GetXpTotal(skill.level + 1, skill.xpBaseDemanding), GetGoldTotal(skill.level + 1, skill.xpBaseDemanding)),
    //     SkillDifficulty.OutOfClass => (GetXpTotal(skill.level + 1, skill.xpBaseOutOfClass), GetGoldTotal(skill.level + 1, skill.xpBaseOutOfClass)),
    //     _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
    // };
    //
    // public static int GetAttributeUpgradeCost(this BaseUnit unit, string attributeName) => attributeName switch
    // {
    //     nameof(BaseUnit.Strength) => GetAttributeXpTotal(unit.xpBaseAttribut, unit.Strength + 1),
    //     nameof(BaseUnit.Constitution) => GetAttributeXpTotal(unit.xpBaseAttribut, unit.Constitution + 1),
    //     nameof(BaseUnit.Dexterity) => GetAttributeXpTotal(unit.xpBaseAttribut, unit.Dexterity + 1),
    //     nameof(BaseUnit.Quickness) => GetAttributeXpTotal(unit.xpBaseAttribut, unit.Quickness + 1),
    //     nameof(BaseUnit.Intuition) => GetAttributeXpTotal(unit.xpBaseAttribut, unit.Intuition + 1),
    //     nameof(BaseUnit.Logic) => GetAttributeXpTotal(unit.xpBaseAttribut, unit.Logic + 1),
    //     nameof(BaseUnit.Willpower) => GetAttributeXpTotal(unit.xpBaseAttribut, unit.Willpower + 1),
    //     nameof(BaseUnit.Wisdom) => GetAttributeXpTotal(unit.xpBaseAttribut, unit.Wisdom + 1),
    //     _ => throw new ArgumentOutOfRangeException(attributeName)
    // };
    public static int GetXpToSpendForLevelUp(this BaseUnit unit) => GetXpTotalForLevelup(unit.Level + 1);

    public static T[] GetAllChildren<T>(this Node node)
            where T : BaseUnit
    {
        var myChildren = node.GetChildren()
                             .Where(mc => mc is T)
                             .Cast<T>()
                             .ToList();

        var retVal = new List<T>();

        foreach (var child in myChildren)
        {
            var grandChildren = child.GetAllChildren<T>().ToList();

            retVal.AddRange(grandChildren);
        }

        retVal.AddRange(myChildren);

        return retVal.ToArray();
    }

    private static int GetAttributeXpTotal(int xpBase, int inputLevel)
    {
        if (inputLevel == 0)
            return inputLevel;

        return GetAttributeXpTotal(xpBase, inputLevel - 1) + GetAttributeXpDelta(xpBase, inputLevel);
    }

    private static int GetAttributeXpDelta(int xpBase, int inputLevel)
    {
        if (inputLevel == 0)
            return inputLevel;

        return GetAttributeXpDelta(xpBase, inputLevel - 1) + GetAttributeXpDeltaPlus(xpBase, inputLevel);
    }

    private static int GetAttributeXpDeltaPlus(int xpBase, int inputLevel)
    {
        var deltaPlus = inputLevel * xpBase;

        deltaPlus += inputLevel switch
        {
            7 => 150,
            12 or 17 or 22 or 27 or 32 or 37 or 42 => 150,
            _ => 0
        };

        return deltaPlus;
    }

    private static int GetXpTotalForLevelup(int inputLevel)
    {
        if (inputLevel == 0)
            return inputLevel;

        return GetXpTotalForLevelup(inputLevel - 1) + GetHeroXpDelta(inputLevel);
    }

    private static int GetHeroXpDelta(int inputLevel)
    {
        if (inputLevel == 0)
            return inputLevel;

        return GetHeroXpDeltaPlus(inputLevel) + GetHeroXpDelta(inputLevel - 1);
    }

    private static int GetHeroXpDeltaPlus(int inputLevel) => inputLevel switch
    {
        <= 1 => 0,
        <= 2 => 500,
        _ => 1000
    };

    private static int GetXpDelta(int inputLevel, int xpBase)
    {
        if (inputLevel == 0)
            return inputLevel;

        var xpDelta = inputLevel * xpBase + GetXpDelta(inputLevel - 1, xpBase);

        return xpDelta;
    }

    private static int GetXpTotal(int inputLevel, int xpBase)
    {
        if (inputLevel == 0)
            return inputLevel;

        var xpTotal = GetXpDelta(inputLevel, xpBase) + GetXpTotal(inputLevel - 1, xpBase);

        return xpTotal;
    }

    private static int GetGoldDelta(int inputLevel, int xpBase) => (int)(GetXpDelta(inputLevel, xpBase) * 0.9);

    private static int GetGoldTotal(int inputLevel, int xpBase)
    {
        if (inputLevel == 0)
            return inputLevel;

        var goldTotal = GetGoldDelta(inputLevel, xpBase) + GetGoldTotal(inputLevel - 1, xpBase);

        return goldTotal;
    }

    public static T ToNewInstance<T>(this T from, PackedScene targetModificatorScene)
            where T : BaseUnitModificator
    {
        var newInstance = targetModificatorScene.Instantiate<T>();

        newInstance.Displayname = from.Displayname;

        newInstance.ActionsModifier    = from.ActionsModifier;
        newInstance.FlatDamageModifier = from.FlatDamageModifier;

        newInstance.ConstitutionMultiplier = from.ConstitutionMultiplier;
        newInstance.DexterityMultiplier    = from.DexterityMultiplier;
        newInstance.IntuitionMultiplier    = from.IntuitionMultiplier;
        newInstance.LogicMultiplier        = from.LogicMultiplier;
        newInstance.WillpowerMultiplier    = from.WillpowerMultiplier;
        newInstance.WisdomMultiplier       = from.WisdomMultiplier;
        newInstance.CharismaMultiplier     = from.CharismaMultiplier;

        newInstance.MeleeAttackratingModifier  = from.MeleeAttackratingModifier;
        newInstance.RangedAttackratingModifier = from.RangedAttackratingModifier;
        newInstance.MagicAttackratingModifier  = from.MagicAttackratingModifier;
        newInstance.SocialAttackratingModifier = from.SocialAttackratingModifier;

        newInstance.MeleeDefensmodifier   = from.MeleeDefensmodifier;
        newInstance.RangedDefensemodifier = from.RangedDefensemodifier;
        newInstance.MagicDefensemodifier  = from.MagicDefensemodifier;
        newInstance.SocialDefensemodifier = from.SocialDefensemodifier;

        if (from is Debuff debuff && newInstance is Debuff newInstanceDebuff)
        {
            newInstanceDebuff.DamagePerTick        = debuff.DamagePerTick;
            newInstanceDebuff.Duration             = debuff.Duration;
            newInstanceDebuff.RemainingDuration    = debuff.RemainingDuration;
            newInstanceDebuff.IsStackable          = debuff.IsStackable;
            newInstanceDebuff.CombatlogEffectColor = debuff.CombatlogEffectColor;
        }

        return newInstance;
    }

    public static float ApplyOperation(this float attributeValue, string op, float modifier) => op switch
    {
        "+" => attributeValue + modifier,
        "-" => attributeValue - modifier,
        "*" => attributeValue * modifier,
        "/" => attributeValue / modifier,
        _ => throw new Exception("Ins Operatorfield kommen nur +, -, *, / rein >:C")
    };

    public static float InfuseRandomness(this float luckyBoyWhoIsAboutToBeRandomized)
    {
        var          random   = new Random();
        const double minValue = 0.5;
        const double maxValue = 1.5;

        var modifier = random.NextDouble() * (maxValue - minValue) + minValue;

        return (float)(modifier * luckyBoyWhoIsAboutToBeRandomized);
    }
}