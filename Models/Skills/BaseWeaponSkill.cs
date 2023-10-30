using System;
using System.Linq;
using DungeonMaster.Enums;
using DungeonMaster.Models.Enemies;
using DungeonMaster.Models.Heroes;
using DungeonMaster.Models.Items.Equipment;
using DungeonMaster.UI;

namespace DungeonMaster.Models.Skills;

public partial class BaseWeaponSkill : BaseDamageSkill
{
    private         CombatLog combatLog;
    private         Main      mainScene;
    public override Factions  TargetableFaction => Factions.Foe;

    public override string Activate(BaseUnit actor, BaseUnit target)
    {
        if (target.IsDead)
            return "Target dead";

        mainScene =   (Main)GetTree().CurrentScene;
        combatLog ??= mainScene.CombatLog;

        var isHit = CalculateHit(actor, target, out var hitroll, out var hitResult);

        if (isHit)
        {
            var bonusDamage = 0;

            if (actor is Hero actingHero)
            {
                var equipedWeapon = (BaseWeapon)actingHero.Equipment
                                                          .Slots[EquipSlot.Mainhand.ToString()]
                                                          .EquipedItem;

                bonusDamage = hitResult switch
                {
                    HitResult.Normal when equipedWeapon is not null => equipedWeapon.AddedDamageNormal,
                    HitResult.Good when equipedWeapon is not null => equipedWeapon.AddedDamageGood,
                    HitResult.Critical when equipedWeapon is not null => equipedWeapon.AddedDamageCritical,
                    _ => 0
                };
            }

            var damageRange = GetDamage(actor, hitResult);
            var minhit      = damageRange.Item1 + bonusDamage;
            var maxhit      = damageRange.Item2 + bonusDamage;

            var rando         = new Random();
            var damageInRange = rando.NextDouble() * (maxhit - minhit) + minhit;

            target.IsStunned = AppliesStun;

            DealDamageTo(hitResult, (int)damageInRange, target, out var finalDamage);
            ApplyDebuffs(actor, target);

            combatLog.LogHit(actor, this, hitroll, hitResult, target, finalDamage.ToString("N0"));

            target.InstatiateFloatingCombatText(finalDamage, Name, hitResult);

            if (target is BaseCreature { IsDead: true })
            {
                var iniSlot = mainScene.InitiativeContainer.GetChildren()
                                       .Cast<InitiativeSlot>()
                                       .FirstOrDefault(s => s.AssignedUnit.Name == target.Name);

                if (iniSlot is not null)
                {
                    mainScene.InitiativeContainer.GetChildren().Remove(iniSlot);
                    mainScene.Enemies = mainScene.Enemies.Except(new[] { (BaseCreature)iniSlot.AssignedUnit }).ToArray();
                    iniSlot.QueueFree();
                }

                target.GetNode<FloatingCombatText>(nameof(FloatingCombatText)).OnQueueFreed += target.QueueFree;
            }

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
            SkillCategory.Melee when target is BaseCreature => hitroll / target.ModifiedMeleeDefense.InfuseRandomness(),
            SkillCategory.Melee => hitroll / target.ModifiedMeleeDefense,
            SkillCategory.Ranged when target is BaseCreature => hitroll / target.ModifiedRangedDefense.InfuseRandomness(),
            SkillCategory.Ranged => hitroll / target.ModifiedRangedDefense,
            SkillCategory.Magic when target is BaseCreature => hitroll / target.ModifiedMagicDefense.InfuseRandomness(),
            SkillCategory.Magic => hitroll / target.ModifiedMagicDefense,
            SkillCategory.Social when target is BaseCreature => hitroll / target.ModifiedSocialDefense.InfuseRandomness(),
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