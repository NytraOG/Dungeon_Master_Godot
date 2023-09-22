using System.ComponentModel;
using DungeonMaster.Models.Heroes;
using Godot;

namespace DungeonMaster.UI.Status;

public partial class Healthbar : TextureProgressBar
{
    public Hero   DisplayedHero;
    public bool   HealthChanged;
    public bool   HeroChanged;
    public double HeroHpSnapshot;
    public Label  Hitpoints;

    public override void _Ready()
    {
        Hitpoints = GetNode<Label>("Hitpoints");
        Value     = 0;
    }

    public void SetDisplayedHero(Hero hero)
    {
        if (DisplayedHero is not null)
            DisplayedHero.PropertyChanged -= DisplayedHeroOnPropertyChanged;

        DisplayedHero  = hero;
        HeroHpSnapshot = hero.CurrentHitpoints;
        HeroChanged    = true;

        hero.PropertyChanged += DisplayedHeroOnPropertyChanged;
    }

    private void DisplayedHeroOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Hero.CurrentHitpoints))
            HealthChanged = true;
    }

    public override void _Process(double delta)
    {
        if (!HeroChanged && !HealthChanged)
            return;

        var currentHp = DisplayedHero.CurrentHitpoints;
        var maxHp     = DisplayedHero.MaximumHitpoints;

        Hitpoints.Text = $"{(int)currentHp}/{maxHp}";
        Value          = currentHp / maxHp * 100;
        HeroChanged    = false;
        HealthChanged  = false;
        HeroHpSnapshot = DisplayedHero.CurrentHitpoints;
    }
}