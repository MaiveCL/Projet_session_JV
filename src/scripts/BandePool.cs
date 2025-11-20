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

/*

---

Au lieu de tirer des balles à travers un écran avec un canon :
Un auteur, invisible et toujours caméras centré, 
navigue vers la droite ou la gauche,
ça "téléporte" dynamiquement les tranches de pages de livre fixées sur une table qui se retrouvent loin / hors de l'écran (recyclage/pooling d'affichage).
On doit "associer" le joueur à la bande la plus proche de lui (point de référence).

BandesVisibles = (x bandes en dehors de l'écran, réparties chaque côté). Quand un côté a plus de bandes que l'autre, la plus éloignée se retrouve "téléportée" de l'autre côté avec son index mise à jour pour afficher la bande correcte selon le tableau de suivit de l'ordre des tranches. MAIS : serait-ce plus simple de créer une zone de collision qui une fois atteinte téléporte la bande ? Est-ce plus simple de créer une, deux ou 3 listes (tampon gauche, bandes visibles, tampon droite) et/ou de conserver des bandes "limites" en mémoire ? Quelle sorte de liste utiliser ?

Mes bandes ont des états : 
- Idle quand elles sont affichées (tout le pool au complet), mais pas sélectionnées par le joueur. (on garde "la trace" de la bande la plus proche, celle qui serait sélectionnée si le joueur appuis sur une touche)
- Moving quand une bande a été sélectionnée par le joueur. A ce stade, bouger vers la gauche ou la droite va déplacer la bande pour la repositionner entre les autres bandes en attente. Le joueur appuira a nouveau sur la même touche pour "relacher" la bande là ou il l'a déplacé (toggle?). Ainsi la bande revient en état Idle
- Je ne suis pas certaine d'en avoir besoin, mais j'ai préparé un état état Pooled, pour les bandes qui ne sont pas affichées.
- Puis l'état Collection. Pour entrer dans cet état il faut avoir sélectionné une bande et l'avoir envoyé vers le haut ou vers le bas ET que cette bande soit marquée comme pouvant faire partie de la collection "contratEdition".

J'ai déjà des nodes de states machines et du code, mais le code fait pitié. Allons-y étape par étape. Par exemple on commence par intégrer la logique pour suivre l'index le plus proche du joueur.



*/
