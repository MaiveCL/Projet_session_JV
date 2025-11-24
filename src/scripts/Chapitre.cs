using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Chapitre : Node2D
{
	[Export] public int PoolSize = 10; // nombre de bandes dans le pool via l'inspecteur
	// Chemin vers l'image totale du chapitre
	[Export] public string TexturePath = "res://assets/sprites/chap1_total.png";
	public Texture2D BandeTexture;

	public int NbPages = 22; // faire une fonction dans le futur pour déterminer le nombre de pages
	[Export] public int NbTranches = 8;
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

		// Calculer la largeur et hauteur d'une bande
		int NbBandes = NbPages * NbTranches;
		LargeurBande = BandeTexture.GetWidth() / NbBandes;
		HauteurBande = BandeTexture.GetHeight();

		// Créer le pool de bandes
		pool = new BandePool();
		pool.BandeScene = BandeScenePacked;
		AddChild(pool);

		pool.Prewarm(PoolSize);

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
		var bandes = pool.GetChildren().OfType<BandeNode>().ToList(); // ou IEnumerable, peu importe

		for (int i = 0; i < bandes.Count; i++)
		{
			var bande = bandes[i];

			// On recycle les tranches : modulo NbBandes pour ne jamais dépasser
			int frameAssigned = indices[i % NbBandes];

			Vector2 pos = new Vector2(i * LargeurBande + i * Marge, 0);

			bande.Configure(0, BandeTexture, NbBandes, frameAssigned, ShadowMaterial, pos, LargeurBande, HauteurBande);
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
		// 1. Récolter les bandes existantes
		var bandes = new List<BandeNode>();
		foreach (Node child in GetChildren())
			if (child is BandeNode b)
				bandes.Add(b);

		int NbBandes = bandes.Count;

		// 2. Générer les nouveaux index mélangés
		var indices = Enumerable.Range(0, NbBandes).ToList();
		var rand = new Random();
		for (int i = indices.Count - 1; i > 0; i--)
		{
			int j = rand.Next(i + 1);
			(indices[i], indices[j]) = (indices[j], indices[i]);
		}

		// 3. Réassigner les bandes existantes
		for (int i = 0; i < NbBandes; i++)
		{
			var bande = bandes[i];

			int frameAssigned = indices[i];
			Vector2 pos = new Vector2(i * LargeurBande + i * Marge, 0);

			bande.Configure(
				0,
				BandeTexture,
				NbBandes,
				frameAssigned,
				ShadowMaterial,
				pos,
				LargeurBande,
				HauteurBande
			);
		}
	}
}
