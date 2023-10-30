using DungeonMaster.Enums;
using Godot;

namespace DungeonMaster.Models.Items.Consumables;

public partial class HealthPotion : BaseConsumable
{
    public HealthPotion()
    {
        Icon   = ResourceLoader.Load<Texture2D>("res://Graphics/Items/Consumables/HealthPotion.png");
        Name   = "Health Potion";
        Rarity = ItemRarity.Normal;

        Keywords.AddRange(new[]
        {
            Items.Keywords.Consumable,
            Items.Keywords.Onehanded,
            Items.Keywords.Heal
        });
    }

    public          int    AmountHealed { get; set; }
    public override string FluffContent => "It's good for you!";
    public override string FluffAuthor  => "Unknown";
    public override string FluffDate    => "????";

    public override void Use(BaseUnit actor) => Consume(actor);

    public override string GetBoniTooltip() => "throw new System.NotImplementedException()";

    public override void Consume(BaseUnit imbiber)
    {
        if (imbiber.CurrentHitpoints + AmountHealed > imbiber.MaximumHitpoints)
            imbiber.CurrentHitpoints = imbiber.MaximumHitpoints;
        else
            imbiber.CurrentHitpoints += AmountHealed;
    }
}