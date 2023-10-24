using System.Text;
using DungeonMaster.Models.Items;
using Godot;

namespace DungeonMaster.UI;

public abstract partial class BaseTooltip : PanelContainer
{
    public Label         Displayname  { get; set; }
    public Label         Content      { get; set; }
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
        Requirements ??= GetNode<HBoxContainer>("Requirements");
        Effects      ??= GetNode<HBoxContainer>("Effects");
        Boni         ??= GetNode<HBoxContainer>("Boni");
        Displayname  ??= Container.GetNode<Label>("Name");
        Content      ??= Container.GetNode<Label>("Content");
        Keywords     ??= Container.GetNode<Label>("Keywords");
        Fluff        ??= Container.GetNode<RichTextLabel>("Fluff");

        Displayname.Text = itemToShow.Name;
        Content.Text     = itemToShow.GetTooltipContent();
        Fluff.Text       = Fluff.Text.Replace(Konstanten.FluffContent, itemToShow.FluffContent);
        Fluff.Text       = Fluff.Text.Replace(Konstanten.FluffAuthor, itemToShow.FluffAuthor);
        Fluff.Text       = Fluff.Text.Replace(Konstanten.FluffDate, itemToShow.FluffDate);
        Keywords.Text    = string.Join(", ", itemToShow.Keywords);
        Visible          = true;
    }

    public new void Hide()
    {
        Visible = false;
        var emil = new StringBuilder();
        emil.AppendLine($"[i]'{Konstanten.FluffContent}'");
        emil.AppendLine($"    -{Konstanten.FluffAuthor}, {Konstanten.FluffDate}[/i]");

        Fluff.Text = emil.ToString();
    }
}