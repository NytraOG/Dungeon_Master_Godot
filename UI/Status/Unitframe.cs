using System.ComponentModel;
using DungeonMaster.Models.Heroes;
using Godot;

namespace DungeonMaster.UI.Status;

public partial class Unitframe : PanelContainer
{
    [Export]
    public TextureRect DefaultIcon { get; set; }

    public TextureProgressBar Healthbar   { get; set; }
    public Label              HealthValue { get; set; }
    public TextureProgressBar Manabar     { get; set; }
    public Label              ManaValue   { get; set; }
    public RichTextLabel      Displayname { get; set; }

    public override void _Process(double delta) { }

    public void SetHero(Hero hero)
    {
        EnsurePropertiesExist();

        Displayname.Text = $"[center]{hero.Name}";

        SetHealthbarValue(hero);
        SetManabarValue(hero);

        hero.PropertyChanged += HeroOnPropertyChanged;
    }

    private void EnsurePropertiesExist()
    {
        Displayname ??= GetNode<RichTextLabel>("%Name");
        Healthbar   ??= GetNode<TextureProgressBar>("%Healthbar");
        HealthValue ??= GetNode<Label>("%HealthValue");
        Manabar     ??= GetNode<TextureProgressBar>("%Manabar");
        ManaValue   ??= GetNode<Label>("%ManaValue");
    }

    private void HeroOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var hero = (Hero)sender;

        switch (e.PropertyName)
        {
            case nameof(Hero.CurrentHitpoints):
                SetHealthbarValue(hero);
                break;
            case nameof(Hero.CurrentMana):
                SetManabarValue(hero);
                break;
        }
    }

    private void SetHealthbarValue(Hero hero)
    {
        var healthpercentage = hero.CurrentHitpoints / hero.MaximumHitpoints * 100;
        Healthbar.Value = healthpercentage;
        HealthValue.Text     = $"{(int)hero.CurrentHitpoints}/{(int)hero.MaximumHitpoints}";
    }

    private void SetManabarValue(Hero hero)
    {
        var manaPercentage = hero.CurrentMana / hero.MaximumMana * 100;
        Manabar.Value    = manaPercentage;
        ManaValue.Text = $"{(int)hero.CurrentMana}/{(int)hero.MaximumMana}";
    }

    public void _on_health_mouse_entered() => HealthValue.Show();

    public void _on_health_mouse_exited() => HealthValue.Hide();

    public void _on_mana_mouse_entered() => ManaValue.Show();

    public void _on_mana_mouse_exited() => ManaValue.Hide();
}