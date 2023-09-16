using System;
using DungeonMaster.Enums;
using DungeonMaster.Models.Heroes;
using Godot;
using Attribute = DungeonMaster.Enums.Attribute;
using Environment = System.Environment;

namespace DungeonMaster.Models.Skills;

public abstract partial class BaseDamageSkill : BaseTargetingSkill
{
    //[Export] public List<Debuff> appliedDebuffs = new();
    [ExportGroup("Hitroll")]    [Export]             public Attribute PrimaryAttributeH;
    [Export]                                         public float     PrimaryScalingH = 2f;
    [Export]                                         public Attribute SecondaryAttributeH;
    [Export]                                         public float     SecondaryScalingH  = 1f;
    [Export]                                         public float     SkillLevelScalingH = 2f;
    [Export]                                         public float     MultiplierH        = 1;
    [ExportGroup("Effect and Damageroll")] [Export]  public Attribute PrimaryAttributeD;
    [Export]                                         public float     PrimaryScalingD = 0.5f;
    [Export]                                         public Attribute SecondaryAttributeD;
    [Export]                                         public float     SecondaryScalingD  = 0.34f;
    [Export]                                         public float     SkillLevelScalingD = 0.5f;
    [Export]                                         public float     MultiplierD        = 1;
    [ExportGroup("On Hit added Damage")]    [Export] public int       Normal;
    [Export]                                         public int       Good;
    [Export]                                         public int       Critical;
    [Export(PropertyHint.Range, "0,1")]              public float     DamageRange;
    [Export]                                         public int       AddedFlatDamage;

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

        var maxhit = (primaryValue + secondaryValue + levelValue + AddedFlatDamage) * MultiplierD;

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
        // if (!AppliedDebuffs.Any())
        //     return;
        //
        // foreach (var debuff in AppliedDebuffs)
        // {
        //     if (target.debuffs.Any(d => d.displayname == debuff.displayname) && !debuff.isStackable)
        //         continue;
        //
        //     AddDebuff(actor, target, debuff);
        // }
    }

    private void AddDebuff(BaseUnit actor, BaseUnit target) //, Debuff debuff
    {
        // var newInstance = debuff.ToNewInstance();
        //
        // newInstance.appliedBy   = this;
        // newInstance.appliedFrom = actor;
        // target.debuffs.Add(newInstance);
        // newInstance.ApplyDamageModifier(target);
        // newInstance.ApplyRatingModifier(target);
    }

    private string GetDamageText(string damage) => damage == "0-0" ? string.Empty : $"Damage:\t<b>{damage}</b>{Environment.NewLine}";

    public override string GetTooltip(Hero selectedHero, string damage = "0-0") => base.GetTooltip(selectedHero, damage) +
                                                                                   GetDamageText(damage) +
                                                                                   Environment.NewLine + Environment.NewLine +
                                                                                   Description;
}