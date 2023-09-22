using System.ComponentModel;
using DungeonMaster.Models.Heroes;
using Godot;

namespace DungeonMaster.UI.Status;

public partial class Manabar : TextureProgressBar
{
    public Hero   DisplayedHero;
    public bool   HeroChanged;
    public double HeroManaSnapshot;
    public Label  Mana;
    public bool   ManaChanged;

    public override void _Ready()
    {
        Mana  = GetNode<Label>("Mana");
        Value = 0;
    }

    public override void _Process(double delta)
    {
        if (!HeroChanged && !ManaChanged)
            return;

        var currentMana = DisplayedHero.CurrentMana;
        var maximumMana = DisplayedHero.MaximumMana;

        Mana.Text        = $"{(int)currentMana}/{(int)maximumMana}";
        Value            = currentMana / maximumMana * 100;
        HeroChanged      = false;
        ManaChanged      = false;
        HeroManaSnapshot = DisplayedHero.CurrentMana;
    }

    public void SetDisplayedHero(Hero hero)
    {
        if (DisplayedHero is not null)
            DisplayedHero.PropertyChanged -= DisplayedHeroOnPropertyChanged;

        DisplayedHero    = hero;
        HeroManaSnapshot = hero.CurrentMana;
        HeroChanged      = true;

        hero.PropertyChanged += DisplayedHeroOnPropertyChanged;
    }

    private void DisplayedHeroOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Hero.CurrentMana))
            ManaChanged = true;
    }
}