﻿using DungeonMaster.Interfaces;
using DungeonMaster.Models.Items;
using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class MouseItemSlot : PanelContainer,
                                     IItemSlot
{
    private Resource          clenchedCursor;
    private Resource          defaultCursor;
    public  InventoryItemSlot SourceSlot       { get; set; }
    public  int               CurrentStacksize { get; set; }
    public  string            Id               { get; set; }
    public  BaseItem          ContainedItem    { get; set; }

    public override void _Ready()
    {
        defaultCursor  = ResourceLoader.Load("res://Graphics/UI/Cursors 64/G_Cursor_Hand3.png");
        clenchedCursor = ResourceLoader.Load("res://Graphics/UI/Cursors 64/G_Cursor_Hand4.png");

        Input.SetCustomMouseCursor(defaultCursor);
    }

    public void UpdateData()
    {
        SetMouseItemTexture();
        SetStacksizeInPanel();
        SetCursorSprite();
    }

    private void SetCursorSprite()
    {
        if(ContainedItem is null)
            Input.SetCustomMouseCursor(defaultCursor);
        else
            Input.SetCustomMouseCursor(clenchedCursor);
    }

    public void Clear()
    {
        ContainedItem    = null;
        CurrentStacksize = 0;

        var stacksizeLabel = GetNode<Label>("CurrentStackSize");
        stacksizeLabel.Text = "x99";

        SetCursorSprite();
    }

    private void SetMouseItemTexture()
    {
        var rect = GetNode<TextureRect>("TextureRect");
        rect.Texture = ContainedItem?.Icon;
    }

    private void SetStacksizeInPanel()
    {
        var label = GetNode<Label>("CurrentStackSize");

        label.Text = ContainedItem is IStackable ? label.Text = "x" + CurrentStacksize : label.Text = string.Empty;
    }
}