namespace DungeonMaster.Models.Items.Consumables;

public partial class HealthPotion : BaseConsumable
{
    public int AmountHealed { get; set; }

    public override void Use(BaseUnit actor) => Consume(actor);

    public override void Consume(BaseUnit imbiber)
    {
        if (imbiber.CurrentHitpoints + AmountHealed > imbiber.MaximumHitpoints)
            imbiber.CurrentHitpoints = imbiber.MaximumHitpoints;
        else
            imbiber.CurrentHitpoints += AmountHealed;
    }
}