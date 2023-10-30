using System;

namespace DungeonMaster.Models.Items.Consumables;

public partial class ArmorPotion : BaseConsumable
{
    public override string FluffContent { get; }
    public override string FluffAuthor  { get; }
    public override string FluffDate    { get; }

    public override void Use(BaseUnit actor) => throw new NotImplementedException();

    public override string GetBoniTooltip() => throw new NotImplementedException();

    public override void Consume(BaseUnit imbiber) => throw new NotImplementedException();
}