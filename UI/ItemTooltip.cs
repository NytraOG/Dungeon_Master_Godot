using System.Text;
using DungeonMaster.Models.Items;
using Godot;

namespace DungeonMaster.UI;

public partial class ItemTooltip : BaseTooltip
{
    private Vector2 originalSize;

    public override void _Ready() => originalSize = Size;

    public override void Show(BaseItem itemToShow)
    {
        base.Show(itemToShow);

        if (itemToShow is null)
            return;

        Displayname.Text = itemToShow.Name;
        Content.Text     = itemToShow.GetTooltipContent();
        Fluff.Text       = Fluff.Text.Replace(Konstanten.FluffContent, itemToShow.FluffContent);
        Fluff.Text       = Fluff.Text.Replace(Konstanten.FluffAuthor, itemToShow.FluffAuthor);
        Fluff.Text       = Fluff.Text.Replace(Konstanten.FluffDate, itemToShow.FluffDate);
        Keywords.Text    = string.Join(", ", itemToShow.Keywords);
    }

    public override void Hide()
    {
        base.Hide();

        if (Fluff is null)
            return;

        var emil = new StringBuilder();
        emil.AppendLine($"[i]'{Konstanten.FluffContent}'");
        emil.AppendLine($"    -{Konstanten.FluffAuthor}, {Konstanten.FluffDate}[/i]");

        Fluff.Text = emil.ToString();
    }

    //Hack, aus irgendwelche gründen wächst der Tooltip von alleine auf 2800+ px, wtf
    public override void _Process(double delta) => SetSize(originalSize);
}