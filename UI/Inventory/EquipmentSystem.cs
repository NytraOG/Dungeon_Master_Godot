using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DungeonMaster.Models;
using Godot;

namespace DungeonMaster.UI.Inventory;

public partial class EquipmentSystem : PanelContainer
{
    private Label                                 heronameLabel;
    private double                                inventoryDisplayCooldown;
    private Main                                  main;
    public  Dictionary<string, EquipmentItemSlot> Slots = new();
    private VBoxContainer                         statDisplayNode;
    public  Color                                 ColorRed   => new(0.86f, 0.09f, 0.09f);
    public  Color                                 ColorGreen => new(0.02f, 0.7f, 0.08f);

    public override void _Ready()
    {
        if (GetTree().CurrentScene is not Main mainScene)
            return;

        statDisplayNode = GetNode<VBoxContainer>("Background/MarginContainer/HBoxContainer/Control/VBoxContainer/ScrollContainer/MarginContainer/StatsDisplay");
        heronameLabel   = GetNode<Label>("Background/MarginContainer/HBoxContainer/Control/VBoxContainer/TextureRect/HeroName");

        main                 =  mainScene;
        main.PropertyChanged += UpsertStatsDisplayEntries;

        Initialize();
    }

    public void Initialize()
    {
        var rightSlots = GetNode<GridContainer>("Background/MarginContainer/HBoxContainer/ContainerRight/MarginContainer/GridContainer").GetAllChildren<EquipmentItemSlot>();
        var leftSlots  = GetNode<GridContainer>("Background/MarginContainer/HBoxContainer/ContainerLeft/MarginContainer/GridContainer").GetAllChildren<EquipmentItemSlot>();

        Slots = rightSlots.Union(leftSlots)
                          .ToDictionary(k => k.Name.ToString(), v => v);
    }

    private void UpsertStatsDisplayEntries(object sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != nameof(Main.SelectedHero) || main.SelectedHero is null)
            return;

        heronameLabel.Text = main.SelectedHero.Name;

        var statsDisplayEntryScene = ResourceLoader.Load<PackedScene>("res://UI/Inventory/stats_display_entry.tscn");
        var trennerScene           = ResourceLoader.Load<PackedScene>("res://UI/Inventory/trenner.tscn");

        var entries = statDisplayNode.GetAllChildren<HBoxContainer>();

        var publicProperties = main.SelectedHero
                                   .GetType()
                                   .GetProperties() //BindingFlags gehen irgendwie mit Godot nicht
                                   .Where(pi => (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(double))
                                                && !pi.Name.Contains("Process"))
                                   .ToArray();

        foreach (var propertyInfo in publicProperties)
        {
            var entry = FindOrCreateEntry(entries, propertyInfo, statsDisplayEntryScene, trennerScene);

            UpdateValues(entry, propertyInfo);
        }
    }

    private void InsertTrennerinstance(PackedScene trennerScene)
    {
        var instance = trennerScene.Instantiate<TextureRect>();
        statDisplayNode.AddChild(instance);
    }

    private void UpdateValues(HBoxContainer entry, PropertyInfo propertyInfo)
    {
        var nameNode = entry.GetNode<Label>("Name");
        nameNode.Text = FormatName(propertyInfo);

        var rawValue = propertyInfo.GetValue(main.SelectedHero);

        var textColor = rawValue switch
        {
            double when propertyInfo.Name
                    is nameof(BaseUnit.MaximumHitpoints)
                    or nameof(BaseUnit.CurrentHitpoints)
                    or nameof(BaseUnit.MaximumMana)
                    or nameof(BaseUnit.CurrentMana)
                    or nameof(BaseUnit.BaseDefenseMelee)
                    or nameof(BaseUnit.MeleeDefense)
                    or nameof(BaseUnit.ModifiedMeleeDefense)
                    or nameof(BaseUnit.BaseDefenseRanged)
                    or nameof(BaseUnit.RangedDefense)
                    or nameof(BaseUnit.ModifiedRangedDefense)
                    or nameof(BaseUnit.BaseDefenseMagic)
                    or nameof(BaseUnit.MagicDefense)
                    or nameof(BaseUnit.ModifiedMagicDefense)
                    or nameof(BaseUnit.BaseDefenseSocial)
                    or nameof(BaseUnit.SocialDefense)
                    or nameof(BaseUnit.ModifiedSocialDefense) => Colors.White,
            double d when d - 1 < 0 => ColorRed,
            double d when d - 1 > 0 => ColorGreen,
            _ => Colors.White
        };

        var valueNode = entry.GetNode<Label>("Value");
        valueNode.Text = FormatValue(propertyInfo);
        valueNode.AddThemeColorOverride("font_color", textColor);
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
        double d when propertyInfo.Name.ToLower().Contains("modifier") && d - 1 >= 0 => $"+{(d - 1) * 100:N0}%",
        double d when propertyInfo.Name.ToLower().Contains("modifier") => $"{(d - 1) * 100:N0}%",
        double d => $"{(int)d}",
        _ => throw new ArgumentOutOfRangeException(nameof(PropertyInfo))
    };

    private HBoxContainer FindOrCreateEntry(HBoxContainer[] entries, PropertyInfo propertyInfo, PackedScene statsDisplayEntryScrene, PackedScene trennerScene)
    {
        var entry = entries.FirstOrDefault(e => e.GetNode<Label>("Name")?.Text == FormatName(propertyInfo));

        if (entry is null)
        {
            entry      = statsDisplayEntryScrene.Instantiate<HBoxContainer>();
            entry.Name = new StringName(propertyInfo.Name);

            statDisplayNode.AddChild(entry);

            if (propertyInfo.Name == nameof(BaseUnit.Level))
                InsertTrennerinstance(trennerScene);
            else if (propertyInfo.Name == nameof(BaseUnit.Charisma))
                InsertTrennerinstance(trennerScene);
            else if (propertyInfo.Name == nameof(BaseUnit.CurrentHitpoints))
                InsertTrennerinstance(trennerScene);
            else if (propertyInfo.Name == nameof(BaseUnit.ManaregenerationRate))
                InsertTrennerinstance(trennerScene);
            else if (propertyInfo.Name == nameof(BaseUnit.SocialAttackratingModifier))
                InsertTrennerinstance(trennerScene);
            else if (propertyInfo.Name == nameof(BaseUnit.Armour))
                InsertTrennerinstance(trennerScene);
            else if (propertyInfo.Name == nameof(BaseUnit.ModifiedMeleeDefense))
                InsertTrennerinstance(trennerScene);
            else if (propertyInfo.Name == nameof(BaseUnit.ModifiedRangedDefense))
                InsertTrennerinstance(trennerScene);
            else if (propertyInfo.Name == nameof(BaseUnit.ModifiedMagicDefense))
                InsertTrennerinstance(trennerScene);
            else if (propertyInfo.Name == nameof(BaseUnit.ModifiedSocialDefense))
                InsertTrennerinstance(trennerScene);
            else if (propertyInfo.Name == nameof(BaseUnit.ModifiedInitiative))
                InsertTrennerinstance(trennerScene);
            else if (propertyInfo.Name == nameof(BaseUnit.AktionenAktuell))
                InsertTrennerinstance(trennerScene);
            else if (propertyInfo.Name == nameof(BaseUnit.FlatDamagebonus))
                InsertTrennerinstance(trennerScene);
            else if (propertyInfo.Name == nameof(BaseUnit.AktionenAktuell))
                InsertTrennerinstance(trennerScene);
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