using DungeonMaster.Models.Skills.Statuseffects.Buffs;
using Godot;

namespace DungeonMaster.Models.Skills.Statuseffects.Debuffs;

public partial class Debuff : Buff
{
    [Export] public int DamagePerTick;

    public override string ResolveTick(BaseUnit applicant)
    {
        DealDamage(applicant);
        RemainingDuration--;

        return DamagePerTick.ToString();
    }

    public override void Die(BaseUnit applicant)
    {
        Reverse();
        applicant.Debuffs.Remove(this);
    }

    private void DealDamage(BaseUnit unit) => unit.CurrentHitpoints -= DamagePerTick;
}