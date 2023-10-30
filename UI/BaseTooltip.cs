using DungeonMaster.Models.Items;
using Godot;

namespace DungeonMaster.UI;

public abstract partial class BaseTooltip : PanelContainer
{
    public RichTextLabel Displayname  { get; set; }
    public RichTextLabel Rarity       { get; set; }
    public RichTextLabel Content      { get; set; }
    public RichTextLabel Keywords     { get; set; }
    public VBoxContainer Container    { get; set; }
    public HBoxContainer Requirements { get; set; }
    public RichTextLabel Effects      { get; set; }
    public RichTextLabel Boni         { get; set; }
    public RichTextLabel Fluff        { get; set; }
    public Color         ColorRed     => new(0.86f, 0.09f, 0.09f);
    public Color         ColorGreen   => new(0.02f, 0.7f, 0.08f);

    public virtual void Show(BaseItem itemToShow)
    {
        if (itemToShow is null)
            return;

        Container    ??= GetNode<MarginContainer>("MarginContainer").GetNode<VBoxContainer>("VBoxContainer");
        Requirements ??= Container.GetNode<HBoxContainer>("Requirements");
        Effects      ??= Container.GetNode<RichTextLabel>("%Effects");
        Boni         ??= Container.GetNode<RichTextLabel>("%Boni");
        Content      ??= Container.GetNode<RichTextLabel>("%Content");
        Fluff        ??= Container.GetNode<RichTextLabel>("Fluff");
        Displayname  ??= Container.GetNode<RichTextLabel>("Name");
        Rarity       ??= Container.GetNode<RichTextLabel>("Rarity");
        Keywords     ??= Container.GetNode<RichTextLabel>("Keywords");

        Visible = true;
    }

    public new virtual void Hide() => Visible = false;
}