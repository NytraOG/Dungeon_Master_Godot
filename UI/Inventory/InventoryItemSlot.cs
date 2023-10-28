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

    private BaseItem      containedItem;
    private ItemTooltip   itemTooltip;
    private MouseItemSlot mouseItemSlot;

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
        var main = (Main)GetTree().CurrentScene;

        itemTooltip         = main.ItemTooltip;
        mouseItemSlot       = main.GetNode<MouseItemSlot>("MouseItemSlot");
        TextureRect         = GetNode<MarginContainer>("MarginContainer").GetNode<TextureRect>("TextureRect");
        TextureRect.Texture = DefaultIcon;

        PropertyChanged += (_, _) =>
        {
            itemTooltip.Visible = ContainedItem is not null;

            if (ContainedItem is null)
                return;

            TextureRect         = GetNode<MarginContainer>("MarginContainer").GetNode<TextureRect>("TextureRect");
            TextureRect.Texture = ContainedItem.Icon;

            if (ContainedItem is not BaseConsumable)
                return;

            var stacksizeLabel = GetNode<Label>("CurrentStacksize");
            stacksizeLabel.Text    = "x" + CurrentStacksize;
            stacksizeLabel.Visible = ContainedItem is IStackable;
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
        var container = GetNode<MarginContainer>("MarginContainer");
        ContainedItem       = null;
        CurrentStacksize    = 0;
        TextureRect         = container.GetNode<TextureRect>("TextureRect");
        TextureRect.Texture = DefaultIcon;

        var stacksizeLabel = container.GetNode<Label>("CurrentStacksize");
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
                ShowToolTip();
                OnSlotLeftClicked?.Invoke(this);
                break;
            case InputEventMouseButton { CtrlPressed: true, ButtonIndex: MouseButton.Left }:
                OnSlotStrgLeftClicked?.Invoke(this);
                break;
            case InputEventMouseMotion when ContainedItem is not null:
                ShowToolTip();
                break;
            case InputEventMouseMotion when ContainedItem is null && itemTooltip.Visible:
                itemTooltip.Hide();
                break;
        }
    }

    public void _on_mouse_entered() => ShowToolTip();

    public void _on_mouse_exited() => itemTooltip.Hide();

    private void ShowToolTip()
    {
        if(mouseItemSlot.ContainedItem is not null)
            return;

        itemTooltip.Show(ContainedItem);
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