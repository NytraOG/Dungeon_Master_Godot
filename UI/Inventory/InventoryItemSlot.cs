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
    public delegate void SlotClickedSignal(InventoryItemSlot clickedSlot);

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
            if (ContainedItem is null)
                return;

            TextureRect         = GetNode<MarginContainer>("MarginContainer").GetNode<TextureRect>("TextureRect");
            TextureRect.Texture = ContainedItem.Icon;

            if (ContainedItem is not BaseConsumable)
                return;

            var stacksizeLabel = GetNode<Label>("CurrentStacksize");
            stacksizeLabel.Text    = "x" + CurrentStacksize;
            stacksizeLabel.Visible = true;
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

    public void Clear()
    {
        ContainedItem       = null;
        CurrentStacksize    = 0;
        TextureRect         = GetNode<MarginContainer>("MarginContainer").GetNode<TextureRect>("TextureRect");
        TextureRect.Texture = DefaultIcon;

        var stacksizeLabel = GetNode<Label>("CurrentStacksize");
        stacksizeLabel.Text    = "x99";
        stacksizeLabel.Visible = false;
    }

    public event SlotClickedSignal OnSlotClicked;

    public void _on_gui_input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton)
            OnSlotClicked?.Invoke(this);
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