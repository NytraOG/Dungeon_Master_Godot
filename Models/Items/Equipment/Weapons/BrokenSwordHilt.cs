using System;
using DungeonMaster.Enums;

namespace DungeonMaster.Models.Items.Equipment.Weapons;

public partial class BrokenSwordHilt : BaseWeapon
{
    public override string    FluffContent { get; }
    public override string    FluffAuthor  { get; }
    public override string    FluffDate    { get; }
    public override EquipSlot EquipSlot    { get; }

    public override void Use(BaseUnit actor) => throw new NotImplementedException();

    public override string GetTooltipContent() => throw new NotImplementedException();
}