using System;
using System.Collections.Generic;
using System.Linq;
using DungeonMaster.Enums;
using DungeonMaster.Models.Skills;
using Godot;

namespace DungeonMaster.Models.Enemies;

public abstract partial class BaseCreature : BaseUnit
{
    public          List<Positions> FavouritePositions = new() { Positions.None };
    [Export] public Keyword[]       Keywords;
    [Export] public float           LevelModifier;
    [Export] public BaseMonstertype Monstertype;

    public override void _Ready()
    {
        Displayname = $"{Monstertype.Displayname} {Keywords[0]?.Displayname}";
        Name        = Displayname;

        MeleeAttackratingModifier  = 1;
        RangedAttackratingModifier = 1;
        MagicAttackratingModifier  = 1;
        SocialAttackratingModifier = 1;

        MeleeDefensmodifier   = 1;
        RangedDefensemodifier = 1;
        MagicDefensemodifier  = 1;
        SocialDefensemodifier = 1;

        InitiativeModifier = 1;

        Monstertype.ApplyValues(this);

        SetAttributeByLevel();

        base._Ready();

        ApplyKeywords();
        SetInitialHitpointsAndMana();

        // var spriterenderer = GetComponent<SpriteRenderer>();
        // spriterenderer.sprite ??= monstertype.sprite;

        //unitTooltip = GameObject.Find("UiCanvas").transform.Find("UnitTooltip").gameObject;
    }

    public override (int, int) GetApproximateDamage(BaseSkill ability) => ability switch
    {
        BaseDamageSkill skill => skill.GetDamage(this, HitResult.None),
        _ => throw new ArgumentOutOfRangeException(nameof(ability))
    };

    public override string UseAbility(BaseSkill skill, HitResult hitResult, BaseUnit target = null)
    {
        var result = GibSkillresult(skill, target);

        SelectedSkill = null;

        return result;
    }

    private void ApplyKeywords()
    {
        foreach (var keyword in Keywords)
        {
            keyword.ApplyAttributeModifier(this);
            keyword.ApplyRatingModifier(this);
            keyword.ApplyDamageModifier(this);
            keyword.PopulateSkills(this);
        }
    }

    public void PickSkill()
    {
        if (!Skills.Any())
            return;

        foreach (var skill in Skills.OrderByDescending(s => s.Manacost))
        {
            if (skill.Manacost > CurrentMana)
                continue;

            SelectedSkill = skill;
            return;
        }
    }

    private void SetAttributeByLevel()
    {
        if (Level == 1)
            return;

        var modifier = LevelModifier * Level;

        Strength     += (int)(Monstertype.Strength * modifier);
        Constitution += (int)(Monstertype.Constitution * modifier);
        Dexterity    += (int)(Monstertype.Dexterity * modifier);
        Quickness    += (int)(Monstertype.Quickness * modifier);
        Intuition    += (int)(Monstertype.Intuition * modifier);
        Logic        += (int)(Monstertype.Logic * modifier);
        Wisdom       += (int)(Monstertype.Wisdom * modifier);
        Willpower    += (int)(Monstertype.Willpower * modifier);
        Charisma     += (int)(Monstertype.Charisma * modifier);
    }

    public override void _Process(double delta) { }
}