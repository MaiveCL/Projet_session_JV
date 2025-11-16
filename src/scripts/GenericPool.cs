using Godot;
using System.Collections.Generic;

public partial class GenericPool<T> : Node where T : CanvasItem
{
	[Export] public PackedScene Scene;
	[Export] public int PoolSize = 10;

	private readonly List<T> pool = new();
	private readonly Stack<T> available = new();

	public override void _Ready()
	{
		for (int i = 0; i < PoolSize; i++)
		{
			var obj = Scene.Instantiate() as T;
			AddChild(obj);

			obj.Visible = false;   // maintenant valide
			available.Push(obj);
			pool.Add(obj);
		}
	}

	public T Get()
	{
		if (available.Count > 0)
		{
			var obj = available.Pop();
			obj.Visible = true;
			return obj;
		}
		return null;
	}

	public void Release(T obj)
	{
		obj.Visible = false;
		available.Push(obj);
	}
}
