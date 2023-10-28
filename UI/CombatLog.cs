using System;
using Godot;

namespace DungeonMaster.UI;

public partial class CombatLog : Control
{
    private int             count;
    private VBoxContainer   logContainer;
    private PackedScene     logentryScene;
    private ScrollContainer scrollContainer;

    [Export]
    public int MaxEntries { get; set; } = 100;

    public override void _Ready()
    {
        logContainer  = GetNode<VBoxContainer>("%LogContainer");
        logentryScene = ResourceLoader.Load<PackedScene>("res://UI/combat_log_entry.tscn");

        scrollContainer = GetNode<PanelContainer>(nameof(PanelContainer))
                         .GetNode<MarginContainer>(nameof(MarginContainer))
                         .GetNode<ScrollContainer>(nameof(ScrollContainer));
    }

    public void InsertEntry()
    {
        var entry = logentryScene.Instantiate<Label>();
        entry.Text = $"TESTINSERT #{++count}";

        logContainer.AddChild(entry);

        if (logContainer.GetChildCount() >= MaxEntries)
        {
            var childToKill = logContainer.GetChild(0);
            logContainer.RemoveChild(childToKill);
            childToKill.QueueFree();
        }

        ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        var scrollBar = scrollContainer.GetVScrollBar();

        if (scrollBar is not null && Math.Abs(scrollBar.Value - scrollBar.MaxValue) > 0.00001f)
            scrollBar.Value = scrollBar.MaxValue;
    }
}