using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Chapitre : Node2D
{
	private const float MARGE_HAUTE = 20f; // marge fixe en haut
	[Export] public int PoolSize = 20; // nombre de bandes dans le pool via l'inspecteur
	[Export] public string TexturePath = "res://assets/sprites/chap1_total.png";
	
	public Texture2D BandeTexture;

	public int NbPages = 22; // faire une fonction dans le futur pour déterminer le nombre de pages
	[Export] public int NbTranches = 8;
	
	public int LargeurBande;
	public int HauteurBande;
	
	[Export] public int Marge = 80;          // espace visuel entre bandes
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

	pool.Prewarm(PoolSize);
	bandesPool = pool.GetChildren().OfType<BandeNode>().ToList();

	// Créer et mémoriser l’ordre logique
	ordreBandes = Enumerable.Range(0, NbBandes).ToList();
	ShuffleOrdre(ordreBandes);

	// Configurer les PoolSize bandes visibles
	for (int i = 0; i < bandesPool.Count; i++)
	{
		var bande = bandesPool[i];
		int frame = ordreBandes[i % NbBandes];

		// Position locale depuis x = 0
		Vector2 pos = new Vector2(i * (LargeurBande + Marge),MARGE_HAUTE);

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
	}
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
}
