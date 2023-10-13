using System;

namespace DungeonMaster.Models.Items.Consumables;

public partial class ArmorPotion : BaseConsumable
{
    public override void Use(BaseUnit actor) => throw new NotImplementedException();

    public override void Consume(BaseUnit imbiber) => throw new NotImplementedException();
}