﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DungeonMaster.Enums;
using DungeonMaster.Models.Items.Equipment;
using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class EquipmentItemSlot : PanelContainer,
                                         INotifyPropertyChanged
{
    private bool isHoveredByMouse;

    [Export]
    public EquipSlot SlotType { get; set; }

    [Export]
    public Texture2D DefaultIcon { get; set; }

    [Export]
    public Texture2D NotAllowedIcon { get; set; }

    [Export]
    public Texture2D AllowedIcon { get; set; }

    public BaseEquipment                     EquipedItem   { get; set; }
    public TextureRect                       Icon          { get; set; }
    public string                            Id            { get; set; }
    public WeaponTooltip                     WeaponTooltip { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;

    public override void _Ready()
    {
        Id            = Name;
        Icon          = GetNode<TextureRect>("TextureRect");
        Icon.Texture  = DefaultIcon;
        WeaponTooltip = ((Main)GetTree().CurrentScene).WeaponTooltip;
    }

    public void _on_equipment_slot_gui_input(InputEvent @event)
    {
        if (@event is InputEventMouseButton { Pressed: true })
        {
            var main          = (Main)GetTree().CurrentScene;
            var mouseItemSlot = main.GetNode<MouseItemSlot>("MouseItemSlot");

            if (EquipedItem is null && mouseItemSlot.ContainedItem is not null)
                InsertIntoSlot(main);
            else if (EquipedItem is not null && mouseItemSlot.ContainedItem is not null)
                SwapItems(main);
            else if (EquipedItem is not null)
                ExtractFromSlot(main);
        }
        else if (@event is InputEventMouseMotion && EquipedItem is not null && !WeaponTooltip.Visible)
            ShowToolTip();
        else if(@event is InputEventMouseMotion && EquipedItem is null && WeaponTooltip.Visible)
            WeaponTooltip.Hide();
    }

    public void _on_mouse_entered()
    {
        CheckMouseItemCompatible();
        ShowToolTip();
    }

    private void ShowToolTip()
    {
        if (EquipedItem is null)
            return;

        WeaponTooltip.Show(EquipedItem);
    }

    private void CheckMouseItemCompatible()
    {
        var main          = (Main)GetTree().CurrentScene;
        var mouseItemSlot = main.GetNode<MouseItemSlot>("MouseItemSlot");

        if (mouseItemSlot.ContainedItem is null)
            return;

        if (mouseItemSlot.ContainedItem is not BaseEquipment || (mouseItemSlot.ContainedItem is BaseEquipment equipment && equipment.EquipSlot != SlotType))
            Icon.Texture = NotAllowedIcon;
        else
            Icon.Texture = AllowedIcon;
    }

    public void _on_mouse_exited()
    {
        if (EquipedItem is null)
            Icon.Texture = DefaultIcon;

        WeaponTooltip.Hide();
    }

    private void InsertIntoSlot(Main main)
    {
        var mouseItemSlot = main.GetNode<MouseItemSlot>("MouseItemSlot");

        if (mouseItemSlot.ContainedItem is not BaseEquipment equipment || equipment.EquipSlot != SlotType || !equipment.IsUsableBy(main.SelectedHero))
            return;

        var selectedHero = main.SelectedHero;

        EquipedItem  = equipment;
        Icon.Texture = equipment.Icon;
        equipment.EquipOn(selectedHero);

        selectedHero.Equipment.Slots[Name].EquipedItem = equipment;

        mouseItemSlot.Clear();
        mouseItemSlot.Visible = false;
    }

    private void SwapItems(Main main)
    {
        var mouseItemSlot = main.GetNode<MouseItemSlot>("MouseItemSlot");
        var mouseItem     = mouseItemSlot.ContainedItem;

        if (mouseItem is not BaseEquipment mouseItemEquipment || mouseItemEquipment.EquipSlot != SlotType || !mouseItemEquipment.IsUsableBy(main.SelectedHero))
            return;

        mouseItemSlot.ContainedItem = EquipedItem;
        mouseItemSlot.UpdateData();

        EquipedItem = mouseItemEquipment;

        main.SelectedHero.Equipment.Slots[Name].EquipedItem = mouseItemEquipment;

        ShowToolTip();
    }

    private void ExtractFromSlot(Main main)
    {
        var mouseItemSlot = main.GetNode<MouseItemSlot>("MouseItemSlot");

        EquipedItem.UnequipFrom(main.SelectedHero);

        mouseItemSlot.ContainedItem = EquipedItem;
        mouseItemSlot.SourceSlot    = null;
        mouseItemSlot.Visible       = true;
        mouseItemSlot.UpdateData();

        Clear();
        main.SelectedHero?.Equipment?.Slots[Name]?.Clear();

        WeaponTooltip.Hide();
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