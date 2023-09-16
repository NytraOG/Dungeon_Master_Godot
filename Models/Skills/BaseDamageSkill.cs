using DungeonMaster.Enums;
using Godot;

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
    [ExportGroup("Effect and Damageroll")] [Export]     public Attribute PrimaryAttributeD;
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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}