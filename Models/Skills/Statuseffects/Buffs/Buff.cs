using Godot;

namespace DungeonMaster.Models.Skills.Statuseffects.Buffs;

public partial class Buff : BaseUnitModificator
{
    public          BaseSkill AppliedBy;
    public          BaseUnit  AppliedFrom;
    [Export] public Color     CombatlogEffectColor;
    [Export] public int       Duration;
    [Export] public bool      IsStackable;
     public int       RemainingDuration;
    public          bool      DurationEnded => RemainingDuration == 0;

    public override void _Ready() => RemainingDuration = Duration;

    public virtual string ResolveTick(BaseUnit applicant)
    {
        RemainingDuration--;

        return $"{Name} ticked";
    }

    public virtual void Die(BaseUnit applicant) => applicant.Buffs.Remove(this);

    public virtual void Reverse() { }
}