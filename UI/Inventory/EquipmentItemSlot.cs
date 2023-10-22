using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DungeonMaster.Enums;
using DungeonMaster.Models.Items.Equipment;
using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class EquipmentItemSlot : PanelContainer,
                                         INotifyPropertyChanged
{
    [Export]
    public EquipSlot SlotType { get; set; }

    [Export]
    public Texture2D DefaultIcon { get; set; }

    public BaseEquipment                     EquipedItem { get; set; }
    public TextureRect                       Icon        { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;

    public override void _Ready()
    {
        Icon         = GetNode<TextureRect>("TextureRect");
        Icon.Texture = DefaultIcon;
    }

    public void _on_equipment_slot_gui_input(InputEvent @event)
    {
        if (@event is InputEventMouseButton { Pressed: true })
        {
            var main          = (Main)GetTree().CurrentScene;
            var mouseItemSlot = main.GetNode<MouseItemSlot>("MouseItemSlot");
            var item          = mouseItemSlot.ContainedItem;
            var selectedHero  = main.SelectedHero;

            if (item is not BaseEquipment equipment || equipment.EquipSlot != SlotType || !equipment.IsUsableBy(selectedHero))
                return;

            selectedHero.Equipment.Slots[equipment.EquipSlot].EquipedItem = equipment;
            EquipedItem                                                   = equipment;
            Icon.Texture                                                  = equipment.Icon;
            equipment.EquipOn(selectedHero);

            mouseItemSlot.Clear();
            mouseItemSlot.Visible = false;
        }
    }

    public void Clear()
    {
        EquipedItem  = null;
        Icon.Texture = DefaultIcon;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}