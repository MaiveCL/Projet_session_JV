using Godot;
using System;

public partial class Settings : Node2D
{
	public int ChapitreCourant { get; set; } = 0;   // index du chapitre actif
	public int Victoires { get; set; } = 0;         // compteur de victoires

	[Export] public Godot.Collections.Array<string> ChapitreTextures = new Godot.Collections.Array<string>
	{
		"res://assets/sprites/chap1_total.png",
		"res://assets/sprites/chap2_total.png",
		"res://assets/sprites/chap3_total.png",
		"res://assets/sprites/chap4_total.png"
	};
	
	[Export] public Godot.Collections.Array<int> ChapitrePages = new Godot.Collections.Array<int>
	{
		22, // chap1 - 1 a 22
		21, // chap2 - 23 a 43
		22, // chap3 - 44 a 65
		26  // chap4 - 66 a 91
	};
	
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
