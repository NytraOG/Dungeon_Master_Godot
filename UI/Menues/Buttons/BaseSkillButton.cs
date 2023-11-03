using DungeonMaster.Models.Skills;
using Godot;

namespace DungeonMaster.UI.Menues.Buttons;

public partial class BaseSkillButton : TextureButton
{
    public delegate void SkillPressedSignalWithSenderArgument(BaseSkillButton sender);

    public BaseSkill                                  Skill;
    // public TextureProgressBar                         TextureProgressBar;
    // public Label                                      Time;
    // public Timer                                      Timer;
    public event SkillPressedSignalWithSenderArgument SomeSkillbuttonPressed;

    public override void _Ready()
    {
        // TextureProgressBar = GetNode<TextureProgressBar>("TextureProgressBar");
        // Timer              = GetNode<Timer>("Timer");
        // Time               = GetNode<Label>("Label");
        //
        // TextureProgressBar.MaxValue = Timer.WaitTime;

        SetProcess(false);
    }

    public override void _Process(double delta)
    {
        // Time.Text                = (3.1 % Timer.TimeLeft).ToString("N");
        // TextureProgressBar.Value = Timer.TimeLeft;
    }

    public void _on_timer_timeout()
    {
       // Disabled  = false;
       // Time.Text = string.Empty;
        SetProcess(false);
    }

    public void _on_pressed()
    {
      //  Timer.Start();
        SomeSkillbuttonPressed?.Invoke(this);
     //   Disabled = true;
        SetProcess(true);
    }
}