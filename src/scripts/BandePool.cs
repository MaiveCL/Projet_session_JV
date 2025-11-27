using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class BandePool : Node
{
	[Export] public PackedScene BandeScene;
	private Queue<BandeNode> pool = new();

	public void Prewarm(int count)
	{
		for (int i = 0; i < count; i++)
		{
			var b = BandeScene.Instantiate<BandeNode>();
			b.Visible = false;   // pour éviter des bugs d'affichage
			AddChild(b);
			pool.Enqueue(b);
		}
	}
	
	public Vector2 GetCenterPosition()
	{
		if (pool.Count == 0) return Vector2.Zero;
		var liste = pool.ToList();
		var droite = liste.OrderBy(b => b.GlobalPosition.X).Last();
		return (GetGauche() + droite.GlobalPosition) / 2;
	}
	
	public Vector2 GetGauche()
	{
		if (pool.Count == 0) return Vector2.Zero;
		var liste = pool.ToList();
		var gauche = liste.OrderBy(b => b.GlobalPosition.X).First();
		return gauche.GlobalPosition;
	}
}
