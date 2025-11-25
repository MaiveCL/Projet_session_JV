using Godot;
using System;
using System.Collections.Generic;

public partial class BandePool : Node
{
	[Export] public PackedScene BandeScene;
	private Queue<BandeNode> pool = new();

	// Préchauffe le pool
	// Chaque instance est ajoutée comme enfant du pool et masquée.
	public void Prewarm(int count)
	{
		for (int i = 0; i < count; i++)
		{
			var b = BandeScene.Instantiate<BandeNode>();
			AddChild(b);
			pool.Enqueue(b);
		}
	}
}
