using System;
using DungeonMaster.Enums;
using DungeonMaster.Models.Enemies;
using DungeonMaster.UI;

namespace DungeonMaster.Models.Skills;

public partial class BaseWeaponSkill : BaseDamageSkill
{
    private         CombatLog combatLog;
    public override Factions  TargetableFaction => Factions.Foe;

    public override string Activate(BaseUnit actor, BaseUnit target)
    {
        combatLog ??= ((Main)GetTree().CurrentScene).CombatLog;

        var isHit = CalculateHit(actor, target, out var hitroll, out var hitResult);

        if (isHit)
        {
            var damage = GetDamage(actor, hitResult);

            var minhit = damage.Item1;
            var maxhit = damage.Item2;

            var rando         = new Random();
            var damageInRange = rando.NextDouble() * (maxhit - minhit) + minhit;

            target.IsStunned = AppliesStun;

            DealDamageTo(hitResult, (int)damageInRange, target, out var finalDamage);
            ApplyDebuffs(actor, target);

            combatLog.LogHit(actor, this, hitroll, hitResult, target, finalDamage.ToString("N0"));

            target.InstatiateFloatingCombatText(finalDamage, Name, hitResult);

            if (target is BaseCreature { IsDead: true })
                target.GetNode<FloatingCombatText>(nameof(FloatingCombatText)).OnQueueFreed += target.QueueFree;

            return finalDamage.ToString();
        }

        combatLog.LogMiss(actor, this, hitroll, target);
        target.InstatiateFloatingCombatText(0, Name, HitResult.Miss);

        return HitResult.Miss.ToString();
    }

    private bool CalculateHit(BaseUnit actor, BaseUnit target, out int hitroll, out HitResult hitResult)
    {
        hitroll = GetHitroll(actor);

        var relation = Category switch
        {
            SkillCategory.Melee => hitroll / target.ModifiedMeleeDefense,
            SkillCategory.Ranged => hitroll / target.ModifiedRangedDefense,
            SkillCategory.Magic => hitroll / target.ModifiedMagicDefense,
            SkillCategory.Social => hitroll / target.ModifiedSocialDefense,
            _ => 0f
        };

        hitResult = relation switch
        {
            >= 2f => HitResult.Critical,
            >= 1.5f => HitResult.Good,
            >= 1f => HitResult.Normal,
            _ => HitResult.None
        };

        return Category switch
        {
            SkillCategory.Melee => (int)target.ModifiedMeleeDefense < hitroll,
            SkillCategory.Ranged => (int)target.ModifiedRangedDefense < hitroll,
            SkillCategory.Magic => (int)target.ModifiedMagicDefense < hitroll,
            SkillCategory.Social => (int)target.ModifiedSocialDefense < hitroll,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override string Activate(BaseUnit actor) => throw new NotImplementedException();
}