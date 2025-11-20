/*
	
	Algo de bande pooling
	utiliser le state poolled state ?
	utiliser le state Moving state pour calculer les 2 limites ?
	calculer les 2 limites en début de partie selon la taille de l'écran ?
*/
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

		// Calculer la largeur et hauteur d'une bande
		int NbBandes = NbPages * NbDechirures;
		LargeurBande = BandeTexture.GetWidth() / NbBandes;
		HauteurBande = BandeTexture.GetHeight();

		// Créer le pool de bandes
		pool = new BandePool();
		pool.BandeScene = BandeScenePacked;
		AddChild(pool);

		// Factory : fonction pour créer une bande si le pool est vide
		Func<BandeNode> factory = () =>
		{
			return BandeScenePacked.Instantiate<BandeNode>();
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

			int frameAssigned = indices[i]; // quelle tranche afficher
			int sourceId = 0; // ici, une seule source d'image

			Vector2 pos = new Vector2(i * LargeurBande + i * Marge, 0); // position de départ

			// configure la bande (texture, frame, shader, position, taille)
			bande.Configure(sourceId, BandeTexture, NbBandes, frameAssigned, ShadowMaterial, pos, LargeurBande, HauteurBande);
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
