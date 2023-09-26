using Godot;

namespace DungeonMaster.Models.Items.Equipment;

public class BaseArmor : BaseEquipment
{
    [ExportGroup("Resistances")]
    [Export]
    public int SlashNormal { get; set; }

    [Export]
    public int SlashGood { get; set; }

    [Export]
    public int SlashCritical { get; set; }

    [Export]
    public int PierceNormal { get; set; }

    [Export]
    public int PierceGood { get; set; }

    [Export]
    public int PierceCritical { get; set; }

    [Export]
    public int CrushNormal { get; set; }

    [Export]
    public int CrushGood { get; set; }

    [Export]
    public int CrushCritical { get; set; }

    [Export]
    public int FireNormal { get; set; }

    [Export]
    public int FireGood { get; set; }

    [Export]
    public int FireCritical { get; set; }

    [Export]
    public int IceNormal { get; set; }

    [Export]
    public int IceGood { get; set; }

    [Export]
    public int IceCritical { get; set; }

    [Export]
    public int LightningNormal { get; set; }

    [Export]
    public int LightningGood { get; set; }

    [Export]
    public int LightningCritical { get; set; }
}