using System.Linq;
using System.Text;
using DungeonMaster.Enums;
using DungeonMaster.Models.Heroes;
using DungeonMaster.Models.Skills;
using Godot;

namespace DungeonMaster.Models.Items.Equipment;

public abstract partial class BaseEquipment : BaseItem
{
    [Export]
    public BaseWeaponSkill RequiredSkill { get; set; }

    public PackedScene GrantedSkillSceneCache { get; set; }

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

    public abstract EquipSlot EquipSlot { get; }

    public override void _Ready()
    {
        //if(GrantedSkill is not null) hier weiter
    }

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

    public override string GetEffectTooltip()
    {
        var emil = new StringBuilder();

        if (GrantedSkill is not null)
            emil.AppendLine($"Grants [color=darkgreen][{string.Join(" ", GrantedSkill.Name.ToString().AddSpacesToString())}][/color]");

        return emil.ToString();
    }

    public override string GetBoniTooltip()
    {
        var emil = new StringBuilder();

        if (StrengthBonus != 0)
            emil.AppendLine($"Strength\t\t[color={GetDisplayColor(nameof(StrengthBonus))}]{GetVorzeichen(StrengthBonus)}{StrengthBonus}[/color]");

        if (ConstitutionBonus != 0)
            emil.AppendLine($"Constitution\t[color={GetDisplayColor(nameof(ConstitutionBonus))}]{GetVorzeichen(ConstitutionBonus)}{ConstitutionBonus}[/color]");

        if (DexterityBonus != 0)
            emil.AppendLine($"Dexterity\t\t[color={GetDisplayColor(nameof(DexterityBonus))}]{GetVorzeichen(DexterityBonus)}{DexterityBonus}[/color]");

        if (WisdomBonus != 0)
            emil.AppendLine($"Wisdom\t\t[color={GetDisplayColor(nameof(WisdomBonus))}]{GetVorzeichen(WisdomBonus)}{WisdomBonus}[/color]");

        if (QuicknessBonus != 0)
            emil.AppendLine($"Quickness\t\t[color={GetDisplayColor(nameof(QuicknessBonus))}]{GetVorzeichen(QuicknessBonus)}{QuicknessBonus}[/color]");

        if (IntuitionBonus != 0)
            emil.AppendLine($"Intuition\t\t[color={GetDisplayColor(nameof(IntuitionBonus))}]{GetVorzeichen(IntuitionBonus)}{IntuitionBonus}[/color]");

        if (LogicBonus != 0)
            emil.AppendLine($"Logic\t\t[color={GetDisplayColor(nameof(LogicBonus))}]{GetVorzeichen(LogicBonus)}{LogicBonus}[/color]");

        if (WillpowerBonus != 0)
            emil.AppendLine($"Willpower\t\t[color={GetDisplayColor(nameof(WillpowerBonus))}]{GetVorzeichen(WillpowerBonus)}{WillpowerBonus}[/color]");

        if (CharismaBonus != 0)
            emil.AppendLine($"Charisma\t\t[color={GetDisplayColor(nameof(CharismaBonus))}]{GetVorzeichen(CharismaBonus)}{CharismaBonus}[/color]");

        return emil.ToString();
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

    private string GetDisplayColor(string propertyName)
    {
        var propertyInfo = GetType().GetProperties().FirstOrDefault(pi => pi.Name == propertyName);

        if (propertyInfo is null)
            return $"Property {propertyName} not found!";

        var value = (int?)propertyInfo.GetValue(this);

        if (value is null)
            return string.Empty;

        return value > 0 ? "darkgreen" : "red";
    }

    protected string GetVorzeichen(int value) => value >= 0 ? "+" : string.Empty;
}