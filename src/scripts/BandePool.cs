using Godot;
using System;
using System.Collections.Generic;

public partial class BandePool : Node
{
	[Export] public PackedScene BandeScene;
	private Queue<BandeNode> pool = new();

	/// <summary>
	/// Préchauffe le pool
	/// Chaque instance est ajoutée comme enfant du pool et masquée.
	/// </summary>
	public void Prewarm(int count, Func<BandeNode> factory)
	{
		for (int i = 0; i < count; i++)
		{
			var b = factory();
			AddChild(b);
			b.Visible = false;
			pool.Enqueue(b);
		}
	}

	/// <summary>
	/// Récupère une bande du pool ou en crée une nouvelle.
	/// </summary>
	public BandeNode GetOrCreate(Func<BandeNode> factory)
	{
		if (pool.Count > 0)
		{
			var b = pool.Dequeue();
			b.Visible = true;
			return b;
		}
		else
		{
			var b = factory();
			AddChild(b);
			return b;
		}
	}

	/// <summary>
	/// Retourne une bande au pool.
	/// </summary>
	public void Return(BandeNode b)
	{
		if (b == null) return;
		b.Visible = false;
		pool.Enqueue(b);
	}
}
