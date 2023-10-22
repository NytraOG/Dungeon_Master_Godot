using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DungeonMaster.Interfaces;
using DungeonMaster.Models.Items;
using DungeonMaster.Models.Items.Consumables;
using Godot;

namespace DungeonMaster.UI.Inventory;

public interface IItemSlot
{
    public BaseItem ContainedItem    { get; set; }
    public int      CurrentStacksize { get; set; }
    public string   Id               { get; set; }
}

public partial class InventoryItemSlot : PanelContainer,
                                         IItemSlot,
                                         INotifyPropertyChanged
{
    public delegate void SlotLeftClickedSignal(InventoryItemSlot clickedSlot);

    public delegate void SlotStrgLeftClickedSignal(InventoryItemSlot clickedSlot);

    private BaseItem containedItem;

    [Export]
    public Texture2D DefaultIcon { get; set; }

    public TextureRect TextureRect      { get; set; }
    public string      Id               { get; set; }
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

        if (ContainedItem is IStackable stackable)
            remainingStackSize = stackable.MaxStacksize - CurrentStacksize;

        return RoomLeftInStack(amount);
    }

    public bool RoomLeftInStack(int amount)
    {
        if (ContainedItem is IStackable stackable)
            return CurrentStacksize + amount <= stackable.MaxStacksize;

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

    public void UpdateData() => OnPropertyChanged();

    public event SlotLeftClickedSignal     OnSlotLeftClicked;
    public event SlotStrgLeftClickedSignal OnSlotStrgLeftClicked;

    public void _on_gui_input(InputEvent inputEvent)
    {
        switch (inputEvent)
        {
            case InputEventMouseButton { Pressed: true, CtrlPressed: false, ButtonIndex: MouseButton.Left }:
                OnSlotLeftClicked?.Invoke(this);
                break;
            case InputEventMouseButton { CtrlPressed: true, ButtonIndex: MouseButton.Left }:
                OnSlotStrgLeftClicked?.Invoke(this);
                break;
        }
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