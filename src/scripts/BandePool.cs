using Godot;
using System;
using System.Linq;
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
			b.Visible = false;   // ← important
			AddChild(b);
			pool.Enqueue(b);
		}
	}
	
	public Vector2 GetCenterPosition()
	{
		if (pool.Count == 0) return Vector2.Zero;
		var liste = pool.ToList();
		var gauche = liste.OrderBy(b => b.GlobalPosition.X).First();
		var droite = liste.OrderBy(b => b.GlobalPosition.X).Last();
		return (gauche.GlobalPosition + droite.GlobalPosition) / 2;
	}

}
