using System.Collections.Generic;
using System.Linq;
using Godot;

namespace DungeonMaster.UI.Menues.Buttons;

public partial class SkillContainer : HBoxContainer
{
    public List<BaseSkillButton> Buttons;

    public override void _Process(double delta) => Buttons = GetChildren()
                                                            .Cast<BaseSkillButton>()
                                                            .ToList();
}