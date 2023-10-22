using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class EquipmentSystem : PanelContainer
{
    private Label         heronameLabel;
    private double        inventoryDisplayCooldown;
    private Main          main;
    private VBoxContainer statDisplayNode;

    public override void _Ready()
    {
        if (GetTree().CurrentScene is not Main mainScene)
            return;

        statDisplayNode = GetNode<VBoxContainer>("Background/MarginContainer/HBoxContainer/Control/VBoxContainer/ScrollContainer/MarginContainer/StatsDisplay");
        heronameLabel   = GetNode<Label>("Background/MarginContainer/HBoxContainer/Control/VBoxContainer/TextureRect/HeroName");

        main                 =  mainScene;
        main.PropertyChanged += UpsertStatsDisplayEntries;
    }

    private void UpsertStatsDisplayEntries(object sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != nameof(Main.SelectedHero) || main.SelectedHero is null)
            return;

        heronameLabel.Text = main.SelectedHero.Name;

        var statsDisplayEntryScrene = ResourceLoader.Load<PackedScene>("res://UI/Inventory/stats_display_entry.tscn");
        var publicProperties        = main.SelectedHero.GetType().GetProperties().Where(pi => (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(float)) && !pi.Name.Contains("Process"));
        var entries                 = statDisplayNode.GetAllChildren<HBoxContainer>();

        foreach (var propertyInfo in publicProperties)
        {
            var entry = entries.FirstOrDefault(e => e.Name == propertyInfo.Name);

            if (entry is null)
            {
                entry      = statsDisplayEntryScrene.Instantiate<HBoxContainer>();
                entry.Name = new StringName(propertyInfo.Name);

                statDisplayNode.AddChild(entry);
            }

            var nameNode = entry.GetNode<Label>("Name");
            nameNode.Text = propertyInfo.Name;
            var valueNode = entry.GetNode<Label>("Value");
            valueNode.Text = ((int)propertyInfo.GetValue(main.SelectedHero)!).ToString();
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