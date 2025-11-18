using Godot;
using System;
using System.Collections.Generic;

public partial class Chapitre : Node2D
{
	// old exports replaced by loading the texture from path
	[Export] public string TexturePath = "res://assets/sprites/chap1_total.png";
	public Texture2D BandeTexture;

	public int NbPages = 22;
	[Export] public int NbDechirures = 8;
	public int LargeurBande;
	public int HauteurBande;
	[Export] public int Marge = 40;          // espace visuel entre bandes
	[Export] public ShaderMaterial ShadowMaterial;

	[Export] public PackedScene BandeScenePacked; // link to res://BandeNode.tscn via inspector (recommended)
	private BandePool pool;

	public override void _Ready()
	{
		// load texture
		BandeTexture = ResourceLoader.Load<Texture2D>(TexturePath);
		if (BandeTexture == null)
		{
			GD.PrintErr("Impossible de charger la texture : " + TexturePath);
			return;
		}

		int NbBandes = NbPages * NbDechirures;
		LargeurBande = BandeTexture.GetWidth() / NbBandes;
		HauteurBande = BandeTexture.GetHeight();

		// create pool node
		pool = new BandePool();
		// give it the scene if you want (optional)
		pool.BandeScene = BandeScenePacked;
		AddChild(pool);

		// factory using PackedScene if given, fallback to new BandeNode()
		Func<BandeNode> factory = () =>
		{
			if (BandeScenePacked != null)
			{
				var inst = BandeScenePacked.Instantiate();
				if (inst is BandeNode bn)
					return bn;
				// If the packed scene root isn't BandeNode, try to find BandeNode inside
				if (inst is Node node)
				{
					var bnChild = node.GetNodeOrNull<BandeNode>("BandeNode");
					if (bnChild != null) return bnChild;
				}
			}
			return new BandeNode();
		};

		// Prewarm pool with exact number of bands (optional)
		pool.Prewarm(NbBandes, factory);

		// Generate shuffle order (initial permutation)
		var indices = new List<int>();
		for (int i = 0; i < NbBandes; i++) indices.Add(i);

		// shuffle the indices so the chapter decides the initial order
		var rand = new Random();
		for (int i = indices.Count - 1; i > 0; i--)
		{
			int j = rand.Next(i + 1);
			int tmp = indices[i];
			indices[i] = indices[j];
			indices[j] = tmp;
		}

		// instantiate bands and assign frames according to shuffled indices
		for (int i = 0; i < NbBandes; i++)
		{
			var bande = pool.GetOrCreate(factory);
			bande.Visible = true;

			int frameAssigned = indices[i]; // the frame index from the big texture
			int sourceId = 0; // for now, only one source

			Vector2 pos = new Vector2(i * LargeurBande + i * Marge, 0);

			bande.Configure(sourceId, BandeTexture, NbBandes, frameAssigned, ShadowMaterial, pos, LargeurBande, HauteurBande);

			// add to chapitre (s'assurer de l'ordre dans l'arbre si nécessaire)
			if (bande.GetParent() != this)
				AddChild(bande);
		}
	}

	public override void _Process(double delta)
	{
		// debug: shuffle à la touche "hasard" -> réinitialise la scène
		if (Input.IsActionJustPressed("hasard"))
		{
			ResetAndShuffle();
		}
	}

	/// <summary>
	/// Rebuild the bands with a new shuffle (used for debugging or menu cheat).
	/// </summary>
	private void ResetAndShuffle()
	{
		// retourne toutes les bandes existantes au pool
		var toReturn = new List<BandeNode>();
		foreach (Node child in GetChildren())
		{
			if (child is BandeNode b)
				toReturn.Add(b);
		}
		foreach (var b in toReturn)
		{
			// optionally you might want to remove from tree; pool.Return only hides them
			pool.Return(b);
			RemoveChild(b);
		}

		// now re-run Ready-like setup quickly (simpler: call _Ready logic by duplicating)
		// For clarity and safety, we call the same initialization snippet as in _Ready.
		// (You can refactor to a helper method to avoid duplication; kept explicit here.)

		int NbBandes = NbPages * NbDechirures;

		// shuffle indices
		var indices = new List<int>();
		for (int i = 0; i < NbBandes; i++) indices.Add(i);

		var rand = new Random();
		for (int i = indices.Count - 1; i > 0; i--)
		{
			int j = rand.Next(i + 1);
			int tmp = indices[i];
			indices[i] = indices[j];
			indices[j] = tmp;
		}

		for (int i = 0; i < NbBandes; i++)
		{
			var bande = pool.GetOrCreate(() =>
			{
				if (BandeScenePacked != null)
					return BandeScenePacked.Instantiate() as BandeNode ?? new BandeNode();
				return new BandeNode();
			});
			bande.Visible = true;

			int frameAssigned = indices[i];
			int sourceId = 0;
			Vector2 pos = new Vector2(i * LargeurBande + i * Marge, 0);

			bande.Configure(sourceId, BandeTexture, NbBandes, frameAssigned, ShadowMaterial, pos, LargeurBande, HauteurBande);

			if (bande.GetParent() != this)
				AddChild(bande);
		}
	}
}
