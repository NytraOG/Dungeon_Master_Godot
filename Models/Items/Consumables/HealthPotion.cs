using System;

namespace DungeonMaster.Models.Items.Consumables;

public class HealthPotion : BaseConsumable
{
    public int AmountHealed { get; set; }

    public override void Use(BaseUnit actor) => throw new NotImplementedException();

    public override void Consume(BaseUnit imbiber)
    {
        if (imbiber.CurrentHitpoints + AmountHealed > imbiber.MaximumHitpoints)
            imbiber.CurrentHitpoints = imbiber.MaximumHitpoints;
        else
            imbiber.CurrentHitpoints += AmountHealed;
    }
}