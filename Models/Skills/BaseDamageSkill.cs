using System;
using System.Linq;
using DungeonMaster.Enums;
using DungeonMaster.Models.Heroes;
using DungeonMaster.Models.Skills.Statuseffects.Debuffs;
using Godot;
using Attribute = DungeonMaster.Enums.Attribute;
using Environment = System.Environment;

namespace DungeonMaster.Models.Skills;

public abstract partial class BaseDamageSkill : BaseTargetingSkill
{
    [Export]                            public int      AddedFlatDamage;
    [Export]                            public Debuff[] AppliedDebuffs;
    [Export]                            public int      Critical;
    [Export(PropertyHint.Range, "0,1")] public double   DamageRange = 1;
    [Export]                            public int      Good;
    [Export]                            public double   MultiplierD = 1;
    [Export]                            public double   MultiplierH = 1;
    [ExportGroup("On Hit added Damage")] [Export]
    public int Normal;
    [ExportGroup("Effect and Damageroll")] [Export]
    public Attribute PrimaryAttributeD;
    [ExportGroup("Hitroll")] [Export] public Attribute PrimaryAttributeH;
    [Export]                          public double    PrimaryScalingD = 0.5f;
    [Export]                          public double    PrimaryScalingH = 2f;
    [Export]                          public Attribute SecondaryAttributeD;
    [Export]                          public Attribute SecondaryAttributeH;
    [Export]                          public double    SecondaryScalingD  = 0.34f;
    [Export]                          public double    SecondaryScalingH  = 1f;
    [Export]                          public double    SkillLevelScalingD = 0.5f;
    [Export]                          public double    SkillLevelScalingH = 2f;

    public int GetHitroll(BaseUnit actor)
    {
        var primaryValue   = PrimaryScalingH * actor.Get(PrimaryAttributeH);
        var secondaryValue = SecondaryScalingH * actor.Get(SecondaryAttributeH);
        var levelValue     = Level * SkillLevelScalingH;

        var hitrollBase  = (primaryValue + secondaryValue + levelValue).InfuseRandomness();
        var finalHitroll = hitrollBase * MultiplierH * GetAttackmodifier(this, actor);

        return (int)finalHitroll;
    }

    public (int, int) GetDamage(BaseUnit actor, HitResult hitresult)
    {
        var primaryValue   = PrimaryScalingD * actor.Get(PrimaryAttributeD);
        var secondaryValue = SecondaryScalingD * actor.Get(SecondaryAttributeD);
        var levelValue     = Level * SkillLevelScalingD;

        var maxhit = (primaryValue + secondaryValue + levelValue + AddedFlatDamage) * MultiplierD + actor.FlatDamagebonus;

        maxhit += hitresult switch
        {
            HitResult.None => 0,
            HitResult.Normal => Normal,
            HitResult.Good => Good,
            HitResult.Critical => Critical,
            _ => throw new ArgumentOutOfRangeException(nameof(hitresult), hitresult, null)
        };

        var minhit = (int)(maxhit * (1 - DamageRange));

        return (minhit, (int)maxhit);
    }

    protected void ApplyDebuffs(BaseUnit actor, BaseUnit target)
    {
        if (!AppliedDebuffs.Any())
            return;

        foreach (var debuff in AppliedDebuffs)
        {
            if (target.Debuffs.Any(d => d.Displayname == debuff.Displayname) && !debuff.IsStackable)
                continue;

            var rando  = GD.RandRange(0d, 1d);
            var erTuts = debuff.ProbabilityToApply >= rando;

            if (erTuts)
                AddDebuff(actor, target, debuff);
        }
    }

    private void AddDebuff(BaseUnit actor, BaseUnit target, Debuff debuff)
    {
        var newInstance = debuff.ToNewInstance();
        newInstance.AppliedBy   = this;
        newInstance.AppliedFrom = actor;

        newInstance.ApplyDamageModifier(target);
        newInstance.ApplyRatingModifier(target);

        target.Debuffs.Add(newInstance);
        target.AddChild(newInstance);
    }

    private string GetDamageText(string damage) => damage == "0-0" ? string.Empty : $"Damage:\t<b>{damage}</b>{Environment.NewLine}";

    public override string GetTooltip(Hero selectedHero, string damage = "0-0") => base.GetTooltip(selectedHero, damage) +
                                                                                   GetDamageText(damage) +
                                                                                   Environment.NewLine + Environment.NewLine +
                                                                                   Description;
}