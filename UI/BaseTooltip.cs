using DungeonMaster.Models.Items;
using Godot;

namespace DungeonMaster.UI;

public abstract partial class BaseTooltip : PanelContainer
{
    public Label         Displayname  { get; set; }
    public RichTextLabel Content      { get; set; }
    public Label         Keywords     { get; set; }
    public VBoxContainer Container    { get; set; }
    public HBoxContainer Requirements { get; set; }
    public HBoxContainer Effects      { get; set; }
    public HBoxContainer Boni         { get; set; }
    public RichTextLabel Fluff        { get; set; }
    public Color         ColorRed     => new(0.86f, 0.09f, 0.09f);
    public Color         ColorGreen   => new(0.02f, 0.7f, 0.08f);

    public virtual void Show(BaseItem itemToShow)
    {
        if (itemToShow is null)
            return;

        Container    ??= GetNode<MarginContainer>("MarginContainer").GetNode<VBoxContainer>("VBoxContainer");
        Requirements ??= Container.GetNode<HBoxContainer>("Requirements");
        Effects      ??= Container.GetNode<HBoxContainer>("Effects");
        Boni         ??= Container.GetNode<HBoxContainer>("Boni");
        Displayname  ??= Container.GetNode<Label>("Name");
        Content      ??= Container.GetNode<RichTextLabel>("%Content");
        Keywords     ??= Container.GetNode<Label>("Keywords");
        Fluff        ??= Container.GetNode<RichTextLabel>("Fluff");

        Visible = true;
    }

    public new virtual void Hide() => Visible = false;
}