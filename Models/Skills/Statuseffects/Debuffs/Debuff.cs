using DungeonMaster.Models.Skills.Statuseffects.Buffs;
using Godot;

namespace DungeonMaster.Models.Skills.Statuseffects.Debuffs;

public partial class Debuff : Buff
{
    [Export]                            public int    DamagePerTick;
    [Export(PropertyHint.Range, "0,1")] public double ProbabilityToApply = 1;

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
        QueueFree();
    }

    private void DealDamage(BaseUnit unit) => unit.CurrentHitpoints -= DamagePerTick;
}