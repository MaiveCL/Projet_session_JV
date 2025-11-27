using Godot;
using System;

public partial class Auteur : Node2D
{
	public bool IsMoving { get; private set; }
	[Export] public float VitessePlayer = 1000;
	public BandeNode BandeProche { get; private set; }

	public override void _Process(double delta)
	{
		Vector2 move = Vector2.Zero;

		if (Input.IsActionPressed("right"))
			move.X += VitessePlayer * (float)delta;
		if (Input.IsActionPressed("left"))
			move.X -= VitessePlayer * (float)delta;
			
		IsMoving = move != Vector2.Zero;

		Position += move;
	}
}
