using Godot;

namespace DungeonMaster.Models.Skills.Statuseffects;

public abstract partial class BaseUnitModificator : Node3D
{
	[Export] [ExportGroup("Attributes")] public string Displayname;
	[Export]                             public float  StrengthMultiplier     = 1;
	[Export]                             public float  ConstitutionMultiplier = 1;
	[Export]                             public float  DexterityMultiplier    = 1;
	[Export]                             public float  QuicknessMultiplier    = 1;
	[Export]                             public float  IntuitionMultiplier    = 1;
	[Export]                             public float  LogicMultiplier        = 1;
	[Export]                             public float  WisdomMultiplier       = 1;
	[Export]                             public float  WillpowerMultiplier    = 1;
	[Export]                             public float  CharismaMultiplier     = 1;
	[Export] [ExportGroup("Ratings")]    public int    ActionsModifier;
	[Export]                             public float  FlatInititiveModifier;
	[Export]                             public float  FlatDamageModifier;
	[Export]                             public float  MeleeAttackratingModifier;
	[Export]                             public float  RangedAttackratingModifier;
	[Export]                             public float  MagicAttackratingModifier;
	[Export]                             public float  SocialAttackratingModifier;
	[Export]                             public float  MeleeDefensmodifier;
	[Export]                             public float  RangedDefensemodifier;
	[Export]                             public float  MagicDefensemodifier;
	[Export]                             public float  SocialDefensemodifier;

	public virtual void ApplyAttributeModifier(BaseUnit creature)
	{
		creature.Strength     *= (int)StrengthMultiplier;
		creature.Constitution *= (int)ConstitutionMultiplier;
		creature.Dexterity    *= (int)DexterityMultiplier;
		creature.Quickness    *= (int)QuicknessMultiplier;
		creature.Intuition    *= (int)IntuitionMultiplier;
		creature.Logic        *= (int)LogicMultiplier;
		creature.Wisdom       *= (int)WisdomMultiplier;
		creature.Willpower    *= (int)WillpowerMultiplier;
		creature.Charisma     *= (int)CharismaMultiplier;
	}

	public virtual void ApplyRatingModifier(BaseUnit unit)
	{
		unit.MeleeAttackratingModifier  += MeleeAttackratingModifier;
		unit.RangedAttackratingModifier += RangedAttackratingModifier;
		unit.MagicAttackratingModifier  += MagicAttackratingModifier;
		unit.SocialAttackratingModifier += SocialAttackratingModifier;

		unit.MeleeDefensmodifier   += MeleeDefensmodifier;
		unit.RangedDefensemodifier += RangedDefensemodifier;
		unit.MagicDefensemodifier  += MagicDefensemodifier;
		unit.SocialDefensemodifier += SocialDefensemodifier;
	}

	public virtual void ApplyDamageModifier(BaseUnit creature)
	{
		creature.InitiativeFlatAdded += FlatInititiveModifier;
		creature.AktionenGesamt      += ActionsModifier;
		creature.AktionenAktuell     =  creature.AktionenGesamt;
		//creature.FlatDamageModifier  += FlatDamageModifier;
	}
}