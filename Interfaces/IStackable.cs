using Godot;

namespace DungeonMaster.Interfaces;

public interface IStackable
{
   [Export] public int MaxStacksize { get; set; }
}