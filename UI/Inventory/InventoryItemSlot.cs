using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DungeonMaster.Models.Items;
using DungeonMaster.Models.Items.Consumables;
using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class InventoryItemSlot : PanelContainer,
                                         INotifyPropertyChanged
{
    private BaseItem containedItem;

    [Export]
    public Texture2D DefaultIcon { get; set; }

    public TextureRect TextureRect      { get; set; }
    public int         CurrentStacksize { get; set; }

    public BaseItem ContainedItem
    {
        get => containedItem;
        set
        {
            containedItem = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public override void _Ready()
    {
        TextureRect         = GetNode<MarginContainer>("MarginContainer").GetNode<TextureRect>("TextureRect");
        TextureRect.Texture = DefaultIcon;

        PropertyChanged += (_, _) =>
        {
            if(ContainedItem is null)
                return;

            TextureRect         = GetNode<MarginContainer>("MarginContainer").GetNode<TextureRect>("TextureRect");
            TextureRect.Texture = ContainedItem.Icon;
        };
    }

    public void AddToStack(int amount) => CurrentStacksize += amount;

    public void RemoveFromStack(int amount) => CurrentStacksize -= amount;

    public bool RoomLeftInStack(int amount, out int remainingStackSize)
    {
        remainingStackSize = 0;

        if (ContainedItem is BaseConsumable consumable)
            remainingStackSize = consumable.MaxStacksize - CurrentStacksize;

        return RoomLeftInStack(amount);
    }

    public bool RoomLeftInStack(int amount)
    {
        if (ContainedItem is BaseConsumable consumable)
            return CurrentStacksize + amount <= consumable.MaxStacksize;

        return ContainedItem is null;
    }

    public void ClearSlot()
    {
        ContainedItem    = null;
        CurrentStacksize = 0;
        TextureRect         = GetNode<MarginContainer>("MarginContainer").GetNode<TextureRect>("TextureRect");
        TextureRect.Texture = DefaultIcon;
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