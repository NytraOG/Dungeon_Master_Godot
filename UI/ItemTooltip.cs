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

        Keywords.Text    = $"[i]{string.Join(", ", itemToShow.Keywords)}[/i]";
        Rarity.Text      = itemToShow.GetRarityTooltip();
        Displayname.Text = itemToShow.GetNameTooltip();
        Content.Text     = itemToShow.GetContentTooltip();
        Boni.Text        = itemToShow.GetBoniTooltip();
        Effects.Text     = itemToShow.GetEffectTooltip();
        Fluff.Text       = Fluff.Text.Replace(Konstanten.FluffContent, itemToShow.FluffContent);
        Fluff.Text       = Fluff.Text.Replace(Konstanten.FluffAuthor, itemToShow.FluffAuthor);
        Fluff.Text       = Fluff.Text.Replace(Konstanten.FluffDate, itemToShow.FluffDate);
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
    // public override void _Process(double delta) => SetSize(new Vector2(originalSize.X + 64, 1));
}