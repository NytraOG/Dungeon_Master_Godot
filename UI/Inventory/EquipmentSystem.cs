using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DungeonMaster.Enums;
using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class EquipmentSystem : PanelContainer
{
    private Label                                    heronameLabel;
    private double                                   inventoryDisplayCooldown;
    private Main                                     main;
    private VBoxContainer                            statDisplayNode;
    public  Dictionary<EquipSlot, EquipmentItemSlot> Slots = new ();

    public override void _Ready()
    {
        if (GetTree().CurrentScene is not Main mainScene)
            return;

        statDisplayNode = GetNode<VBoxContainer>("Background/MarginContainer/HBoxContainer/Control/VBoxContainer/ScrollContainer/MarginContainer/StatsDisplay");
        heronameLabel   = GetNode<Label>("Background/MarginContainer/HBoxContainer/Control/VBoxContainer/TextureRect/HeroName");

        main                 =  mainScene;
        main.PropertyChanged += UpsertStatsDisplayEntries;


        var rightSlots = GetNode<GridContainer>("Background/MarginContainer/HBoxContainer/ContainerRight/MarginContainer/GridContainer").GetAllChildren<EquipmentItemSlot>();
        var leftSlots  = GetNode<GridContainer>("Background/MarginContainer/HBoxContainer/ContainerLeft/MarginContainer/GridContainer").GetAllChildren<EquipmentItemSlot>();

        Slots = rightSlots.Union(leftSlots)
                          .ToDictionary(k => k.SlotType, v => v);
    }

    private void UpsertStatsDisplayEntries(object sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != nameof(Main.SelectedHero) || main.SelectedHero is null)
            return;

        heronameLabel.Text = main.SelectedHero.Name;

        var statsDisplayEntryScrene = ResourceLoader.Load<PackedScene>("res://UI/Inventory/stats_display_entry.tscn");
        var entries                 = statDisplayNode.GetAllChildren<HBoxContainer>();

        var publicProperties = main.SelectedHero
                                   .GetType()
                                   .GetProperties() //BindingFlags gehen irgendwie mit Godot nicht
                                   .Where(pi => (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(double))
                                                && !pi.Name.Contains("Process"));

        foreach (var propertyInfo in publicProperties)
        {
            var entry = FindOrCreateEntry(entries, propertyInfo, statsDisplayEntryScrene);

            UpdateValues(entry, propertyInfo);
        }
    }


    private void UpdateValues(HBoxContainer entry, PropertyInfo propertyInfo)
    {
        var nameNode = entry.GetNode<Label>("Name");
        nameNode.Text = FormatName(propertyInfo);

        var valueNode = entry.GetNode<Label>("Value");
        valueNode.Text = FormatValue(propertyInfo);
    }

    private string FormatName(PropertyInfo propertyInfo)
    {
        var name = propertyInfo.Name;

        if (name.Contains("Xp"))
            return "XP for next Level";

        if (name.Contains("Attack"))
            name = name.Replace("Attack", "A");

        if (name.Contains("rating"))
            name = name.Replace("rating", "R");

        if (name.Contains("Defense"))
            name = name.Replace("Defense", "Def.");

        if (name.ToLower().Contains("modifier"))
        {
            name = name.Replace("Modifier", "Mult.");
            name = name.Replace("modifier", "Mult.");
        }

        return name.AddSpacesToString(true);
    }

    private string FormatValue(PropertyInfo propertyInfo) => propertyInfo.GetValue(main.SelectedHero) switch
    {
        int i => $"{i}",
        double d when propertyInfo.Name.Contains("mod") => $"{(d - 1) * 100:N0}%",
        double d => $"{(int)d}",
        _ => throw new ArgumentOutOfRangeException(nameof(PropertyInfo))
    };

    private HBoxContainer FindOrCreateEntry(HBoxContainer[] entries, PropertyInfo propertyInfo, PackedScene statsDisplayEntryScrene)
    {
        var entry = entries.FirstOrDefault(e => e.GetNode<Label>("Name")?.Text == FormatName(propertyInfo));

        if (entry is null)
        {
            entry      = statsDisplayEntryScrene.Instantiate<HBoxContainer>();
            entry.Name = new StringName(propertyInfo.Name);

            statDisplayNode.AddChild(entry);
        }

        return entry;
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