using System.Linq;
using DungeonMaster.Enums;
using DungeonMaster.Models.Heroes;
using DungeonMaster.Models.Skills;
using Godot;

namespace DungeonMaster.Models.Items.Equipment;

public abstract partial class BaseEquipment : BaseItem
{
    [Export]
    public BaseWeaponSkill RequiredSkill { get; set; }

    [Export]
    public BaseSkill GrantedSkill { get; set; }

    [Export]
    public int RequiredSkilllevel { get; set; }

    [Export]
    public int StrengthBonus { get; set; }

    [Export]
    public int ConstitutionBonus { get; set; }

    [Export]
    public int DexterityBonus { get; set; }

    [Export]
    public int WisdomBonus { get; set; }

    [Export]
    public int QuicknessBonus { get; set; }

    [Export]
    public int IntuitionBonus { get; set; }

    [Export]
    public int LogicBonus { get; set; }

    [Export]
    public int WillpowerBonus { get; set; }

    [Export]
    public int CharismaBonus { get; set; }

    [Export]
    public int HealthBonusFlat { get; set; }

    [Export]
    public double HealthBonusMultiplier { get; set; } = 1;

    public abstract EquipSlot EquipSlot    { get; }

    public virtual void EquipOn(BaseUnit wearer)
    {
        wearer.Strength     += StrengthBonus;
        wearer.Constitution += ConstitutionBonus;
        wearer.Dexterity    += DexterityBonus;
        wearer.Wisdom       += WisdomBonus;
        wearer.Quickness    += QuicknessBonus;
        wearer.Intuition    += IntuitionBonus;
        wearer.Willpower    += WillpowerBonus;
        wearer.Charisma     += CharismaBonus;

        var oldMaxHealth = wearer.MaximumHitpoints;
        var newMaxHealth = wearer.MaximumHitpoints *= HealthBonusMultiplier;
        wearer.CurrentHitpoints += newMaxHealth - oldMaxHealth;

        wearer.MaximumHitpoints += HealthBonusFlat;
        wearer.CurrentHitpoints += HealthBonusFlat;
    }

    public virtual void UnequipFrom(BaseUnit wearer)
    {
        wearer.Strength     -= StrengthBonus;
        wearer.Constitution -= ConstitutionBonus;
        wearer.Dexterity    -= DexterityBonus;
        wearer.Wisdom       -= WisdomBonus;
        wearer.Quickness    -= QuicknessBonus;
        wearer.Intuition    -= IntuitionBonus;
        wearer.Willpower    -= WillpowerBonus;
        wearer.Charisma     -= CharismaBonus;

        wearer.MaximumHitpoints -= HealthBonusFlat;
        wearer.CurrentHitpoints -= HealthBonusFlat;

        var oldMaxHealth = wearer.MaximumHitpoints;
        var newMaxHealth = wearer.MaximumHitpoints /= HealthBonusMultiplier;
        wearer.CurrentHitpoints += newMaxHealth - oldMaxHealth;
    }

    public bool RequirementsMetToEquip(Hero possibleWearer)
    {
        var requirementsMet = possibleWearer.Level >= LevelRequirement;
        requirementsMet &= possibleWearer.Get(RequiredAttribute) >= RequiredLevelOfAttribute;

        if (RequiredSkill is null)
            return requirementsMet;

        var requiredSkillOfWearer = possibleWearer.Skills.FirstOrDefault(s => s.Name == RequiredSkill.Name);

        if (requiredSkillOfWearer is null)
            return false;

        requirementsMet = requiredSkillOfWearer.Level >= RequiredSkilllevel;

        return requirementsMet;
    }
}