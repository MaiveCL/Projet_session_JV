using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Chapitre : Node2D
{
	private const float MARGE_HAUTE = 20f; // marge fixe en haut
	public int PoolSize = 20; // remplacer par pool.bullet_pool.Count
	[Export] public string TexturePath = "res://assets/sprites/chap1_total.png";
	
	public Texture2D BandeTexture;

	public int NbPages = 22; 
	[Export] public int NbTranches = 8;
	
	public int LargeurBande;
	public int HauteurBande;
	
	[Export] public int Marge = 40; // espace visuel entre bandes
	[Export] public ShaderMaterial ShadowMaterial;

	[Export] public PackedScene BandeScenePacked; // Scene de bande à instancier
	
	private BandePool pool; // notre pool de bandes
	private List<BandeNode> bandesPool;
	
	private List<int> ordreBandes;
	
	public override void _Ready()
	{
		// Récupérer la caméra
		var camera = GetNode<Camera2D>("/root/Monde/Auteur/Camera2D");

		// Calculer le bord gauche de l'écran
		float viewportWidth = GetViewport().GetVisibleRect().Size.X;
		float bordGauche = camera.GlobalPosition.X - (viewportWidth / 2f);

		// Positionner le Chapitre au bord gauche
		GlobalPosition = new Vector2(bordGauche, GlobalPosition.Y);

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
		
		// Préparation de l’ordre logique
		ordreBandes = Enumerable.Range(0, NbBandes).ToList();
		ShuffleOrdre(ordreBandes);
		
		// --------- NOUVELLE LOGIQUE DE GÉNÉRATION ---------

		float bordDroit = camera.GlobalPosition.X + (viewportWidth / 2f);
		bool depasse = false;
		
		int index = 0;
		
		// On va créer bande par bande, tant qu’on n’a pas dépassé le bord
		while (!depasse)
		{
			// S’assurer que le pool contient index+1 objets
			pool.Prewarm(1);

			float xLocal = index * (LargeurBande + Marge);
			float xGlobal = GlobalPosition.X + xLocal;

			if (xGlobal > bordDroit)
			{
				depasse = true;
			}
			else
			{
				index++;
			}
		}
		
		// Ajouter les 6 bandes supplémentaires
		int extraCount = 6;
		pool.Prewarm(extraCount);
		bandesPool = pool.GetChildren().OfType<BandeNode>().ToList();

		PoolSize = bandesPool.Count;

		// Maintenant que tout est créé, on configure les bandes
		for (int i = 0; i < PoolSize; i++)
		{
			var bande = bandesPool[i];
			int frame = ordreBandes[i % NbBandes];
			Vector2 pos = new Vector2(i * (LargeurBande + Marge), MARGE_HAUTE);

			bande.Configure(
				0,
				BandeTexture,
				NbBandes,
				frame,
				ShadowMaterial,
				pos,
				LargeurBande,
				HauteurBande
			);
			bande.Visible = true;
		}
		CenterChapitre();
		var joueur = GetNode<Node2D>("/root/Monde/Auteur");
		joueur.GlobalPosition = pool.GetCenterPosition();
		// ---------------------------------------------------
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("hasard"))
		{
			ShuffleOrdre(ordreBandes);
			ReassignerPool(bandesPool, ordreBandes);
		}
	}

	private void ShuffleOrdre(List<int> ordre)
	{
		var rand = new Random();
		for (int i = ordre.Count - 1; i > 0; i--)
		{
			int j = rand.Next(i + 1);
			(ordre[i], ordre[j]) = (ordre[j], ordre[i]);
		}
	}
	
	private void ReassignerPool(List<BandeNode> pool, List<int> ordre, int? targetIndex = null)
	{
		int NbBandes = NbPages * NbTranches;

		if (targetIndex.HasValue)
		{
			int i = targetIndex.Value % pool.Count;
			var bande = pool[i];
			int frame = ordre[i % NbBandes];
			Vector2 pos = new Vector2(i * LargeurBande + i * Marge, MARGE_HAUTE);

			bande.Configure(0, BandeTexture, NbBandes, frame, ShadowMaterial, pos, LargeurBande, HauteurBande);
		}
		else
		{
			for (int i = 0; i < pool.Count; i++)
			{
				var bande = pool[i];
				int frame = ordre[i % NbBandes];
				Vector2 pos = new Vector2(i * LargeurBande + i * Marge, MARGE_HAUTE);

				bande.Configure(0, BandeTexture, NbBandes, frame, ShadowMaterial, pos, LargeurBande, HauteurBande);
			}
		}
	}
	
	public void CenterChapitre()
	{
		if (bandesPool == null || bandesPool.Count == 0) return;

		// Positions locales au lieu de globales
		float minX = bandesPool.Min(b => b.Position.X);
		float maxX = bandesPool.Max(b => b.Position.X);
		float minY = bandesPool.Min(b => b.Position.Y);
		float maxY = bandesPool.Max(b => b.Position.Y);

		Vector2 boundingCenter = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
		Vector2 viewportCenter = GetViewport().GetVisibleRect().Size / 2;

		// Décaler Chapitre pour centrer les bandes
		Position += viewportCenter - boundingCenter;
	}

}
