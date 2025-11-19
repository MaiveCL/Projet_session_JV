/*
	bande.GetParent().RemoveChild(bande); // enlève de son ancien parent
	
	Algo de bande pooling
	utiliser le state poolled state ?
	utiliser le state Moving state pour calculer les 2 limites ?
	calculer les 2 limites en début de partie selon la taille de l'écran ?
	
	
	
	
	
*/
using Godot;
using System;
using System.Collections.Generic;

public partial class Chapitre : Node2D
{
	[Export] public int PoolSize = 50; // nombre de bandes dans le pool via l'inspecteur
	// Chemin vers l'image totale du chapitre
	[Export] public string TexturePath = "res://assets/sprites/chap1_total.png";
	public Texture2D BandeTexture;

	public int NbPages = 22;
	[Export] public int NbDechirures = 8;
	public int LargeurBande;
	public int HauteurBande;
	[Export] public int Marge = 40;          // espace visuel entre bandes
	[Export] public ShaderMaterial ShadowMaterial;

	[Export] public PackedScene BandeScenePacked; // Scene de bande à instancier
	private BandePool pool; // notre pool de bandes

	public override void _Ready()
	{
		// Charger la texture principale du chapitre
		BandeTexture = ResourceLoader.Load<Texture2D>(TexturePath);
		if (BandeTexture == null)
		{
			GD.PrintErr("Impossible de charger la texture : " + TexturePath);
			return;
		}

		// Calculer la largeur et hauteur d'une bande
		int NbBandes = NbPages * NbDechirures;
		LargeurBande = BandeTexture.GetWidth() / NbBandes;
		HauteurBande = BandeTexture.GetHeight();

		// Créer le pool de bandes
		pool = new BandePool();
		pool.BandeScene = BandeScenePacked; // on lui passe la scène de bande si besoin
		AddChild(pool); // on ajoute le pool dans l'arbre de scène

		// Factory : fonction pour créer une bande si le pool est vide
		Func<BandeNode> factory = () =>
		{
			if (BandeScenePacked != null)
			{
				// Instancie la scène
				var inst = BandeScenePacked.Instantiate();
				if (inst is BandeNode bn)
					return bn;

				// Si le root n'est pas une BandeNode, cherche un enfant appelé "BandeNode"
				if (inst is Node node)
				{
					var bnChild = node.GetNodeOrNull<BandeNode>("BandeNode");
					if (bnChild != null) return bnChild;
				}
			}
			// Fallback : crée une BandeNode pure si pas de scène
			return new BandeNode();
		};

		// Préchauffer le pool avec le nombre exact de bandes
		// Ici toutes les bandes sont instanciées et mises invisibles dans le pool
		pool.Prewarm(PoolSize, factory);

		// Générer un ordre aléatoire pour mélanger les bandes
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

		// Instancier les bandes à partir du pool et les configurer
		for (int i = 0; i < NbBandes; i++)
		{
			var bande = pool.GetOrCreate(factory); // prend une bande du pool ou en crée une nouvelle
			bande.Visible = true; // rend la bande visible

			int frameAssigned = indices[i]; // quelle "tranche" de la texture cette bande va afficher
			int sourceId = 0; // ici, une seule source d'image

			Vector2 pos = new Vector2(i * LargeurBande + i * Marge, 0); // position de départ

			// configure la bande (texture, frame, shader, position, taille)
			bande.Configure(sourceId, BandeTexture, NbBandes, frameAssigned, ShadowMaterial, pos, LargeurBande, HauteurBande);

			// ajouter à l'arbre de scène si ce n'est pas déjà fait
			if (bande.GetParent() != this)
				AddChild(bande);
		}
	}

	public override void _Process(double delta)
	{
		// Debug : appuyer sur "hasard" pour réinitialiser et mélanger les bandes
		if (Input.IsActionJustPressed("hasard"))
		{
			ResetAndShuffle();
		}
	}

	/// <summary>
	/// Remet les bandes au pool et les réinstancie avec un nouvel ordre
	/// </summary>
	private void ResetAndShuffle()
	{
		// Retourne toutes les bandes existantes au pool
		var toReturn = new List<BandeNode>();
		foreach (Node child in GetChildren())
		{
			if (child is BandeNode b)
				toReturn.Add(b);
		}
		foreach (var b in toReturn)
		{
			pool.Return(b); // met la bande dans le pool et la rend invisible
			RemoveChild(b); // on la retire de l'arbre de scène
		}

		// Refaire le shuffle comme dans _Ready()
		int NbBandes = NbPages * NbDechirures;

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

		// Réinstancier les bandes depuis le pool
		for (int i = 0; i < NbBandes; i++)
		{
			var bande = pool.GetOrCreate(() =>
			{
				return BandeScenePacked?.Instantiate() as BandeNode ?? new BandeNode();
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



/*

using Godot;
using System;
using System.Collections.Generic;

public partial class Chapitre : Node2D
{
	[Export] public int PoolSize = 50; // nombre de bandes dans le pool via l'inspecteur
	[Export] public string TexturePath = "res://assets/sprites/chap1_total.png";
	public Texture2D BandeTexture;

	public int NbPages = 22;
	[Export] public int NbDechirures = 8;
	public int LargeurBande;
	public int HauteurBande;
	[Export] public int Marge = 40;
	[Export] public ShaderMaterial ShadowMaterial;
	[Export] public PackedScene BandeScenePacked;
	private BandePool pool;

	// FlatList horizontale
	private Dictionary<int, BandeNode> activeBandes = new();
	private int totalBandes;
	private float scrollX = 0f;
	[Export] public float ScrollSpeed = 600f;
	private List<int> indices; // indices mélangés pour garder le désordre

	public override void _Ready()
	{
		BandeTexture = ResourceLoader.Load<Texture2D>(TexturePath);
		if (BandeTexture == null)
		{
			GD.PrintErr("Impossible de charger la texture : " + TexturePath);
			return;
		}

		totalBandes = NbPages * NbDechirures;
		LargeurBande = BandeTexture.GetWidth() / totalBandes;
		HauteurBande = BandeTexture.GetHeight();

		// Créer le pool de bandes
		pool = new BandePool();
		pool.BandeScene = BandeScenePacked;
		AddChild(pool);

		// Factory pour le pool
		Func<BandeNode> factory = () =>
		{
			if (BandeScenePacked != null)
			{
				var inst = BandeScenePacked.Instantiate();
				if (inst is BandeNode bn) return bn;
				if (inst is Node node)
				{
					var bnChild = node.GetNodeOrNull<BandeNode>("BandeNode");
					if (bnChild != null) return bnChild;
				}
			}
			return new BandeNode();
		};

		pool.Prewarm(PoolSize, factory);

		// Shuffle initial
		indices = new List<int>();
		for (int i = 0; i < totalBandes; i++) indices.Add(i);
		Shuffle(indices);

		// Instancier seulement les bandes visibles
		UpdateVisibleBandes(factory);
	}

	public override void _Process(double delta)
	{
		float d = (float)delta;

		// Scroll horizontal
		if (Input.IsActionPressed("ui_right")) scrollX += ScrollSpeed * d;
		if (Input.IsActionPressed("ui_left")) scrollX -= ScrollSpeed * d;

		// Mettre à jour les bandes visibles
		UpdateVisibleBandes(() =>
		{
			return pool.GetOrCreate(() =>
			{
				if (BandeScenePacked != null)
				{
					var inst = BandeScenePacked.Instantiate();
					if (inst is BandeNode bn) return bn;
					if (inst is Node node)
					{
						var bnChild = node.GetNodeOrNull<BandeNode>("BandeNode");
						if (bnChild != null) return bnChild;
					}
				}
				return new BandeNode();
			});
		});

		if (Input.IsActionJustPressed("hasard"))
		{
			ResetAndShuffle();
		}
	}

	private void UpdateVisibleBandes(Func<BandeNode> factory)
	{
		if (BandeTexture == null) return;

		float margin = 200f; // marge pour précharger/décharger les bandes
		float screenLeft = scrollX - margin;
		float screenRight = scrollX + GetViewportRect().Size.X + margin;

		// Retirer les bandes hors écran
		var keys = new List<int>(activeBandes.Keys);
		foreach (var idx in keys)
		{
			float bandeLeft = idx * (LargeurBande + Marge);
			float bandeRight = bandeLeft + LargeurBande;
			if (bandeRight < screenLeft || bandeLeft > screenRight)
			{
				RemoveChild(activeBandes[idx]);
				pool.Return(activeBandes[idx]);
				activeBandes.Remove(idx);
			}
		}

		// Ajouter les bandes visibles manquantes
		for (int i = 0; i < totalBandes; i++)
		{
			float bandeLeft = i * (LargeurBande + Marge);
			float bandeRight = bandeLeft + LargeurBande;

			if (bandeRight >= screenLeft && bandeLeft <= screenRight)
			{
				if (!activeBandes.ContainsKey(i))
				{
					var bande = factory();
					bande.Visible = true;

					Vector2 pos = new Vector2(bandeLeft, 0);
					bande.Configure(0, BandeTexture, totalBandes, indices[i], ShadowMaterial, pos, LargeurBande, HauteurBande);

					AddChild(bande);
					activeBandes[i] = bande;
				}
			}
		}
	}

	private void ResetAndShuffle()
	{
		// Retourne toutes les bandes existantes au pool
		var toReturn = new List<BandeNode>();
		foreach (Node child in GetChildren())
		{
			if (child is BandeNode b)
				toReturn.Add(b);
		}
		foreach (var b in toReturn)
		{
			pool.Return(b);
			RemoveChild(b);
		}

		// Shuffle
		Shuffle(indices);

		// Mise à jour des bandes visibles
		UpdateVisibleBandes(() =>
		{
			return pool.GetOrCreate(() =>
			{
				return BandeScenePacked?.Instantiate() as BandeNode ?? new BandeNode();
			});
		});
	}

	private void Shuffle(List<int> list)
	{
		var rand = new Random();
		for (int i = list.Count - 1; i > 0; i--)
		{
			int j = rand.Next(i + 1);
			int tmp = list[i];
			list[i] = list[j];
			list[j] = tmp;
		}
	}
}

*/
