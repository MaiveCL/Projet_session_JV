using Godot;
using System.Linq;
using System.Collections.Generic;

public partial class MovingState : State
{
	[Export] public float Speed = 200f;
	[Export] public float TargetScale = 1.25f;
	[Export] public float GrowSpeed = 7f;

	private float positionLibreX;
	private bool positionLibreInit = false;

	public override void Enter()
	{
		var b = GetBandeNode();
		if (b != null)
		{
			b.ShowShadow(true);
			b.ZIndex = 1000; // très devant
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

				// --- Mettre à jour l'ordre global OrdreBandes ---
				EchangerOrdreGlobale(chapitre, b, autre);

				// mettre à jour la position libre pour la prochaine collision
				positionLibreX = ancienneX;
			}

			// --- Lâcher de la bande ---
			if (Input.IsActionJustReleased("down"))
			{
				FinaliserDepot(b, chapitre);
				return;
			}
		}
	}

	private void FinaliserDepot(BandeNode b, Chapitre chapitre)
	{
		// Placer la bande à sa position finale (positionLibreX mémorisée)
		b.Position = new Vector2(positionLibreX, b.Position.Y);

		// Réinitialiser la taille
		b.Scale = Vector2.One;

		// Changer d'état pour revenir en Idle
		b.GetStateMachine()?.ChangeState("IdleState");

		// Reset du flag pour la prochaine manipulation
		positionLibreInit = false;
	}

	// --- Fonction pour échanger l'ordre global ---
	private void EchangerOrdreGlobale(Chapitre chapitre, BandeNode bandeA, BandeNode bandeB)
	{
		if (bandeA == null || bandeB == null || chapitre == null) 
			return;

		List<int> ordre = chapitre.OrdreBandes;

		int indexA = ordre.IndexOf(bandeA.FrameIndex);
		int indexB = ordre.IndexOf(bandeB.FrameIndex);

		if (indexA == -1 || indexB == -1) return; // sécurité

		// Échanger les valeurs dans OrdreBandes
		(ordre[indexA], ordre[indexB]) = (ordre[indexB], ordre[indexA]);
	}
	
	public override void Exit()
	{
		var b = GetBandeNode();
		if (b != null)
		{
			b.ZIndex = 0; // valeur standard (ou celle que tu veux)
			b.Scale = Vector2.One; // au cas où on quitte Moving abruptement
		}
	}

}
