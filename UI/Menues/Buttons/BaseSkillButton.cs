using DungeonMaster.Models.Skills;
using Godot;

namespace DungeonMaster.UI.Menues.Buttons;

public partial class BaseSkillButton : TextureButton
{
    public delegate void SkillPressedSignalWithSenderArgument(BaseSkillButton sender);

    private         string                            changeKey;
    [Export] public Label                             Hotkey;
    [Export] public TextureProgressBar                TextureProgressBar;
    [Export] public Label                             Time;
    [Export] public Timer                             Timer;
    public          BaseSkill                         Skill;
    public event SkillPressedSignalWithSenderArgument SomeSkillbuttonPressed;

    private void SetHotkeyLabel(string value)
    {
        changeKey   = value;
        Hotkey.Text = value;

        // Hotkey shice, braucht (noch) kein mensch
        // var shortcut   = new Shortcut();
        // var inputEvent = new InputEventKey();
        // inputEvent.Keycode = Key.Key1; //(Key)1; //int.Parse(string.Join(string.Empty, value.ToCharArray()));
        // shortcut.Events    = new Array(new[] { inputEvent });
    }

    public override void _Ready()
    {
        SetHotkeyLabel("1");
        TextureProgressBar.MaxValue = Timer.WaitTime;
        SetProcess(false);
    }

    public override void _Process(double delta)
    {
        Time.Text                = (3.1 % Timer.TimeLeft).ToString("N");
        TextureProgressBar.Value = Timer.TimeLeft;
    }

    public void _on_timer_timeout()
    {
        Disabled  = false;
        Time.Text = string.Empty;
        SetProcess(false);
    }

    public void _on_pressed()
    {
        Timer.Start();
        SomeSkillbuttonPressed?.Invoke(this);
        Disabled = true;
        SetProcess(true);
    }
}