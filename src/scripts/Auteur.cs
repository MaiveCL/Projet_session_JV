using Godot;
using System;

public partial class Auteur : Node2D
{
	[Export] public float VitessePlayer = 200f;

	public override void _Process(double delta)
	{
		Vector2 move = Vector2.Zero;

		if (Input.IsActionPressed("ui_right"))
			move.X += VitessePlayer * (float)delta;
		if (Input.IsActionPressed("ui_left"))
			move.X -= VitessePlayer * (float)delta;

		Position += move;
	}
}
