using Godot;
using System;

public partial class Settings : Node2D
{
	public bool MusicEnabled { get; set; } = true;
	public bool ActionEnabled { get; set; } = true;
	public bool EasyModeEnabled { get; set; } = false;
	public bool IsPaused { get; set; } = false;
	
	// Config de niveau
	public int FansDepart { get; set; } = 10000;
	public int Tranches { get; set; } = 5;
	public float PerteParSeconde { get; set; } = 22f;

	public bool IsMuted { get; private set; } = false;

	public override void _Process(double delta)
	{
		// Écoute directe de la touche mute
		if (Input.IsActionJustPressed("mute"))
		{
			ToggleMute();
		}
	}

	public void ToggleMute()
	{
		IsMuted = !IsMuted;
		int masterIndex = AudioServer.GetBusIndex("Master");
		AudioServer.SetBusMute(masterIndex, IsMuted);
		GD.Print("Mute toggled: ", IsMuted);
	}
}
