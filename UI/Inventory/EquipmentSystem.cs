using System.ComponentModel;
using System.Linq;
using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class EquipmentSystem : PanelContainer
{
    private double inventoryDisplayCooldown;
    private Main   main;

    public override void _Ready()
    {
        if (GetTree().CurrentScene is not Main mainScene)
            return;

        main                 =  mainScene;
        main.PropertyChanged += UpsertStatsDisplayEntries;
    }

    private void UpsertStatsDisplayEntries(object sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != nameof(Main.SelectedHero) ||  main.SelectedHero is null)
            return;

        var heronameLabel = GetNode<Label>("Background/MarginContainer/HBoxContainer/Control/VBoxContainer/TextureRect/HeroName");
        heronameLabel.Text = main.SelectedHero.Name;

        var statsDisplayEntryScrene = ResourceLoader.Load<PackedScene>("res://UI/Inventory/stats_display_entry.tscn");
        var publicProperties        = main.SelectedHero.GetType().GetProperties();
        var entries                 = this.GetAllChildren<HBoxContainer>();

        foreach (var propertyInfo in publicProperties)
        {
            var entry = entries.FirstOrDefault(e => e.Name == propertyInfo.Name);

            if (entry is null)
            {
                entry      = statsDisplayEntryScrene.Instantiate<HBoxContainer>();
                entry.Name = new StringName(propertyInfo.Name);
            }

            var valueNode = entry.GetNode<Label>("Name");
            valueNode.Text = propertyInfo.GetValue(main.SelectedHero)?.ToString();
        }
    }

    public override void _Process(double delta)
    {
        inventoryDisplayCooldown -= delta;

        if (Input.IsKeyPressed(Key.C) && inventoryDisplayCooldown <= 0)
        {
            Visible                  = !Visible;
            inventoryDisplayCooldown = 0.25;
        }
    }
}