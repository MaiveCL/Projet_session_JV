using Godot;
using System.Collections.Generic;

public partial class MovingState : State
{
	[Export] public float Speed = 200f;
	[Export] public float TargetScale = 1.25f;
	[Export] public float GrowSpeed = 7f;

	private float positionLibreX; // position libre à remplir
	private bool positionLibreInit = false; // pour mémoriser une seule fois

	public override void Enter()
	{
		var b = GetBandeNode();
		if (b != null)
		{
			b.ShowShadow(true);
			// On ne mémorise pas ici, on le fera dans le premier Update
		}
	}

	public override void Update(double delta)
	{
		var b = GetBandeNode();
		if (b == null) return;

		var chapitre = GetNode<Chapitre>("/root/Monde/TableJeu/Chapitre");
		var joueur = chapitre?.GetNode<Node2D>("/root/Monde/Auteur");
		if (chapitre == null || joueur == null) return;

		// --- Mémoriser la position initiale de la bande une seule fois ---
		if (!positionLibreInit)
		{
			positionLibreX = b.Position.X;
			positionLibreInit = true;
		}

		// --- Animation du grossissement ---
		b.Scale = b.Scale.Lerp(Vector2.One * TargetScale, (float)delta * GrowSpeed);

		// --- Suivi horizontal du joueur ---
		b.Position = new Vector2(
			Mathf.Lerp(b.Position.X, joueur.Position.X, (float)delta * Speed),
			b.Position.Y
		);

		// --- Gestion des collisions horizontales ---
		foreach (var autre in chapitre.BandesPool)
		{
			if (autre == b) continue;

			if (Mathf.Abs(autre.Position.X - b.Position.X) < (chapitre.LargeurBande + chapitre.Marge) / 2f)
			{
				// mémoriser l'ancienne position X de la bande collisionnée
				float ancienneX = autre.Position.X;

				// placer la bande collisionnée à la position libre
				autre.Position = new Vector2(positionLibreX, autre.Position.Y);

				// mettre à jour la position libre pour la prochaine collision
				positionLibreX = ancienneX;
			}
		}
	}
}
