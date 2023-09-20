using DungeonMaster.Models.Skills;
using Godot;

namespace DungeonMaster.UI.Menues.Buttons;

public partial class BaseSkillButton : TextureButton
{
    public delegate void SkillPressedSignalWithSenderArgument(BaseSkillButton sender);
    //
    // [Signal]
    // public delegate void TimedOutEventHandler(BaseSkillButton sender);

    public          BaseSkill                         Skill;
    [Export] public TextureProgressBar                TextureProgressBar;
    [Export] public Label                             Time;
    [Export] public Timer                             Timer;
    public event SkillPressedSignalWithSenderArgument SomeSkillbuttonPressed;

    public override void _Ready()
    {
        TextureProgressBar = GetNode<TextureProgressBar>("TextureProgressBar"); //TextureProgressBarScene.Instantiate<TextureProgressBar>();
        Timer              = GetNode<Timer>("Timer");                           //TimerScene.Instantiate<Timer>();
        Time               = GetNode<Label>("Label");                           //TimeScene.Instantiate<Label>();

        TextureProgressBar.MaxValue = Timer.WaitTime;
        SetProcess(false);
    }

    public override void _Process(double delta)
    {
        Time.Text                = (3.1 % Timer.TimeLeft).ToString("N");
        TextureProgressBar.Value = Timer.TimeLeft;
    }

    //
    // public void _on_timed_out(BaseSkillButton sender)
    // {
    //     sender.Disabled  = false;
    //     sender.Time.Text = string.Empty;
    //     sender.SetProcess(false);
    // }
    //
    public void _on_timer_timeout()
    {
        Disabled  = false;
        Time.Text = string.Empty;
        SetProcess(false);
        //EmitSignal(SignalName.TimedOut, this);
    }

    public void _on_pressed()
    {
        Timer.Start();
        SomeSkillbuttonPressed?.Invoke(this);
        Disabled = true;
        SetProcess(true);
    }
}