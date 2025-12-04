using Godot;
using System;

public partial class TableJeu : Node2D
{
	[Export] public Parallax2D parallax;

	// Vitesse par défaut
	[Export] public Vector2 defaultScrollSpeed = Vector2.Zero;

	public Vector2 ScrollSpeed { get; set; }

	public override void _Ready()
	{
		if (parallax != null)
		{
			ScrollSpeed = defaultScrollSpeed;
		}
	}

	public override void _Process(double delta)
	{
		if (parallax != null)
		{
			parallax.ScrollOffset += ScrollSpeed * (float)delta;
		}
	}
}
