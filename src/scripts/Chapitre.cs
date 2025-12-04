using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Chapitre : Node2D
{
	// %%% Ajout de l'événement pour notifier la victoire
	public event Action VictoireDetectee;
	
	private BandeNode bandeProche;
	public BandeNode BandeProche => bandeProche;
	
	private MondeState machine;

	public void InitMachine(MondeState m)
	{
		machine = m;
		GD.Print("Machine initialisée dans Chapitre !");
	}

	public const float MARGE_HAUTE = 20f; // marge fixe en haut
	public int PoolSize = 20;
	[Export] public string TexturePath = "res://assets/sprites/chap1_total.png";
	
	public Texture2D BandeTexture;

	public int NbPages = 22; 
	[Export(PropertyHint.Range, "1,9,1")]
	public int NbTranches { get; set; } = 8;
	
	public int LargeurBande;
	public int HauteurBande;
	
	[Export] public int Marge = 40; // entre bandes
	[Export] public ShaderMaterial ShadowMaterial;

	[Export] public PackedScene BandeScenePacked; // Scene de bande à instancier
	
	private BandePool pool;

	private List<BandeNode> bandesPool;
	public List<BandeNode> BandesPool
	{
		get => bandesPool;
		set => bandesPool = value;
	}
	
	private List<int> ordreBandes;
	public List<int> OrdreBandes
	{
		get => ordreBandes;
		set => ordreBandes = value; // permet d’assigner une nouvelle liste
	}
	
	public override void _Ready()
	{
		// --- récupérer l'overlay sans caster Monde ---
		var overlay = GetNode<Lecteurs>("/root/Main/SceneContainer/Monde/GameInfos/Lecteurs");

		DebugTool.LogOnce("LOCAL PATH = " + GetPath());

		var camera = GetNode<Camera2D>("../../Auteur/Camera2D");

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

		// Maintenant que tout est créé, on configure
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
		var joueur = GetNode<Node2D>("../../Auteur");
		joueur.GlobalPosition = pool.GetCenterPosition();
		bandeProche = bandesPool
		.OrderBy(b => Mathf.Abs(b.GlobalPosition.X - joueur.GlobalPosition.X))
		.First();
		AppliquerEtatInitiale(bandeProche);
	}
	
		private void AppliquerEtatInitiale(BandeNode bandeIdle = null)
		{
			foreach (var bande in bandesPool)
			{
				var machine = bande.GetStateMachine();

				if (machine != null)
				{
					if (bande == bandeIdle)
						machine.ChangeState("IdleState");
					else
						machine.ChangeState("NullState");
				}
			}
		}
	
	public void MettreAJourBandeProche()
	{
		var joueur = GetNode<Node2D>("../../Auteur");
		
		BandeNode nouvelle = bandesPool
		.OrderBy(b => Mathf.Abs(b.GlobalPosition.X - joueur.GlobalPosition.X))
		.First();

		if (nouvelle == bandeProche)
			return; // rien n’a changé → on sort

		// Affecter la nouvelle
		bandeProche = nouvelle;
		
		// %%% Déclencher le rééquilibrage du pool seulement si la bande proche a changé
		RecycleBandes();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("hasard"))
		{
			ShuffleOrdre(ordreBandes);
			ReassignerPool(bandesPool, ordreBandes);
		}
		if (Input.IsActionJustPressed("triche"))
		{
			ActiverTriche();
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
	
	public void RecycleBandes()
	{
		var joueur = GetNode<Node2D>("../../Auteur");
		Vector2 posJoueur = joueur.GlobalPosition;

		var bandesGauche = bandesPool.Count(b => b.GlobalPosition.X < posJoueur.X);
		var bandesDroite = bandesPool.Count(b => b.GlobalPosition.X >= posJoueur.X);

		// On ne fait rien si équilibré
		if (Math.Abs(bandesGauche - bandesDroite) < 2)
			return;

		if (bandesDroite > bandesGauche)
		{
			// Déplacer la bande la plus à droite vers la gauche
			var droite = bandesPool.OrderByDescending(b => b.GlobalPosition.X).First();
			var gauche = bandesPool.OrderBy(b => b.GlobalPosition.X).First();

			droite.GlobalPosition = new Vector2(
				gauche.GlobalPosition.X - LargeurBande - Marge,
				droite.GlobalPosition.Y
			);

			// réassigner l'index dans frame
			int newIndex = (ordreBandes.IndexOf(droite.FrameIndex) - bandesPool.Count + ordreBandes.Count) % ordreBandes.Count;
			droite.SetFrame(ordreBandes[newIndex]);
		}
		else
		{
			// Déplacer la bande la plus à gauche vers la droite
			var gauche = bandesPool.OrderBy(b => b.GlobalPosition.X).First();
			var droite = bandesPool.OrderByDescending(b => b.GlobalPosition.X).First();

			gauche.GlobalPosition = new Vector2(
				droite.GlobalPosition.X + LargeurBande + Marge,
				gauche.GlobalPosition.Y
			);

			int newIndex = (ordreBandes.IndexOf(gauche.FrameIndex) + bandesPool.Count) % ordreBandes.Count;
			gauche.SetFrame(ordreBandes[newIndex]);
		}
		
		PrintOrdre();
		CheckVictoire();
		// Après tout changement de position, on vérifie la victoire
	}
	
	private void CheckVictoire()
	{
		if (ordreBandes == null || ordreBandes.Count == 0)
			return;

		int total = ordreBandes.Count;
		
		// La séquence peut commencer n'importe où
		int start = ordreBandes[0];
	
		for (int i = 0; i < total; i++)
		{
			int expected = (start + i) % total;

			if (ordreBandes[i] != expected)
			{
				GD.Print("L'ordre n'est pas encore correct...");
				return;
			}
		}
		GD.Print("Victoire détectée !");
		VictoireDetectee?.Invoke();
	}
	
	private void PrintOrdre()
	{
		GD.Print("ORDRE LOGIQUE COMPLET : ", string.Join(", ", ordreBandes));
	}
	
	public void ActiverTriche()
	{
		int total = ordreBandes.Count;

		// Remettre les indices dans l’ordre croissant
		for (int i = 0; i < total; i++)
		{
			ordreBandes[i] = i;
		}

		// Réassigner les frames et positions pour que l'affichage corresponde
		for (int i = 0; i < bandesPool.Count; i++)
		{
			bandesPool[i].SetFrame(ordreBandes[i]);
			bandesPool[i].GlobalPosition = new Vector2(i * (LargeurBande + Marge), MARGE_HAUTE);
		}

		// Mettre à jour la bande la plus proche
		var joueur = GetNode<Node2D>("../../Auteur");
		bandeProche = bandesPool
			.OrderBy(b => Mathf.Abs(b.GlobalPosition.X - joueur.GlobalPosition.X))
			.First();

		GD.Print("Triche activée : ordreBandes réinitialisé et bandes positionnées !");
	}
}
