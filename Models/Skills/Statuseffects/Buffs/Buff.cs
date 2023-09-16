using Godot;

namespace DungeonMaster.Models.Skills.Statuseffects.Buffs;

public partial class Buff : BaseUnitModificator
{
    [Export] public BaseSkill AppliedBy;
    [Export] public BaseUnit  AppliedFrom;
    [Export] public Color     CombatlogEffectColor;
    [Export] public int       Duration;
    [Export] public bool      IsStackable;
    [Export] public int       RemainingDuration;

    public bool DurationEnded => RemainingDuration == 0;

    public override void _Ready() => RemainingDuration = Duration;

    public virtual string ResolveTick(BaseUnit applicant)
    {
        RemainingDuration--;

        return $"{Name} ticked";
    }

    public virtual void Die(BaseUnit applicant) => applicant.Buffs.Remove(this);

    public virtual void Reverse() { }
}