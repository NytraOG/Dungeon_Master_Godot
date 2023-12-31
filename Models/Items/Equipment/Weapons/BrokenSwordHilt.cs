using System;
using DungeonMaster.Enums;

namespace DungeonMaster.Models.Items.Equipment.Weapons;

public partial class BrokenSwordHilt : BaseWeapon
{
    public override string    FluffContent => "Half of the blade of this straight sword is broken off.\nOnly the truly desperate would even consider using this as a weapon.";
    public override string    FluffAuthor  => "Kevin";
    public override string    FluffDate    => "2011";
    public override EquipSlot EquipSlot    => EquipSlot.Mainhand;

    public override void _Ready() => Initialize();

    public override void Initialize() => Keywords.AddRange(new[] { Enums.Keywords.Melee, Enums.Keywords.Onehanded, Enums.Keywords.Conventional });

    public override void Use(BaseUnit actor) => throw new NotImplementedException();
}