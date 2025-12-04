using Godot;
using System;

public partial class Settings : Node2D
{
	public bool MusicEnabled { get; set; } = true;
	public bool ActionEnabled { get; set; } = true;
	public bool EasyModeEnabled { get; set; } = false;
}
