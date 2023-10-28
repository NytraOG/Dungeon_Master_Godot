using System;
using DungeonMaster.Enums;
using DungeonMaster.Models.Enemies;
using DungeonMaster.UI;
using Godot;

namespace DungeonMaster.Models.Skills;

public partial class BaseWeaponSkill : BaseDamageSkill
{
    public override Factions TargetableFaction => Factions.Foe;

    public override string Activate(BaseUnit actor, BaseUnit target)
    {
        var isHit = CalculateHit(actor, target, out var hitroll, out var hitResult);

        if (isHit)
        {
            var damage = GetDamage(actor, hitResult);

            var minhit = damage.Item1;
            var maxhit = damage.Item2;

            var rando         = new Random();
            var damageInRange = rando.NextDouble() * (maxhit - minhit) + minhit;

            target.IsStunned        =  AppliesStun;

            DealDamageTo(hitResult, (int)damageInRange, target,  out var finalDamage);
            ApplyDebuffs(actor, target);

            Console.WriteLine($"{actor.Displayname} dealt {finalDamage} Damage with {Displayname} to {target.Displayname}");

            target.InstatiateFloatingCombatText(finalDamage, this, hitResult);

            if (target is BaseCreature { IsDead: true })
                target.GetNode<FloatingCombatText>(nameof(FloatingCombatText)).OnQueueFreed += target.QueueFree;

            return finalDamage.ToString();
        }

        Console.WriteLine($"{actor.Displayname} missed {target.Displayname} with {Displayname}");
        target.InstatiateFloatingCombatText(0, this, HitResult.Miss);

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