using Godot;
using System.Linq;
using System.Collections.Generic;

public partial class MovingState : BandeState
{
	[Export] public float Speed = 200f;
	[Export] public float TargetScale = 1.25f;
	[Export] public float GrowSpeed = 7f;

	private float positionLibreX;
	private bool positionLibreInit = false;
	
	public override void Enter()
	{
		var b = GetBandeNode();
		DebugTool.NodeOnce("BandeNode (Enter)", b);
		if (b != null)
		{
			b.ShowShadow(true);
			b.ZIndex = 1000; // très devant
		}
	}

	public override void Update(double delta)
	{
		var b = GetBandeNode();
		DebugTool.NodeOnce("BandeNode (Update)", b);
		if (b == null) return;

		var chapitre = b.Chapitre;
		DebugTool.NodeOnce("Chapitre", chapitre);
		if (chapitre == null) return;

		var joueur = b.Chapitre.GetParent<Node2D>().GetParent<Node2D>().GetNode<Node2D>("Auteur");
	DebugTool.NodeOnce("Joueur", joueur);
	if (joueur == null) return;

	if (!positionLibreInit)
	{
		positionLibreX = b.Position.X;
		positionLibreInit = true;
		DebugTool.LogOnce($"Position initiale mémorisée = {positionLibreX}");
	}

	b.Scale = b.Scale.Lerp(Vector2.One * TargetScale, (float)delta * GrowSpeed);
	b.Position = new Vector2(
		Mathf.Lerp(b.Position.X, joueur.Position.X, (float)delta * Speed),
		b.Position.Y
	);

	foreach (var autre in chapitre.BandesPool)
	{
		if (autre == b) continue;

		if (Mathf.Abs(autre.Position.X - b.Position.X) < (chapitre.LargeurBande + chapitre.Marge) / 2f)
		{
			float ancienneX = autre.Position.X;
			autre.Position = new Vector2(positionLibreX, autre.Position.Y);
			DebugTool.LogOnce($"Collision : {b.Name} ↔ {autre.Name}, ancienneX={ancienneX}, nouvelleX={positionLibreX}");

			EchangerOrdreGlobale(chapitre, b, autre);
			positionLibreX = ancienneX;
		}

		if (Input.IsActionJustReleased("down"))
		{
			DebugTool.LogOnce($"Lâché de bande : {b.Name}");
			FinaliserDepot(b, chapitre);
			return;
		}
	}
}

	private void FinaliserDepot(BandeNode b, Chapitre chapitre)
	{
		b.Position = new Vector2(positionLibreX, b.Position.Y);
		b.Scale = Vector2.One;
		b.GetStateMachine()?.ChangeState("IdleState");
		positionLibreInit = false;
		DebugTool.LogOnce($"Depot finalisé : {b.Name}, positionX={positionLibreX}");
	}

	private void EchangerOrdreGlobale(Chapitre chapitre, BandeNode bandeA, BandeNode bandeB)
	{
		if (bandeA == null || bandeB == null || chapitre == null) 
			return;

		List<int> ordre = chapitre.OrdreBandes;

		int indexA = ordre.IndexOf(bandeA.FrameIndex);
		int indexB = ordre.IndexOf(bandeB.FrameIndex);

		if (indexA == -1 || indexB == -1) return;

		(ordre[indexA], ordre[indexB]) = (ordre[indexB], ordre[indexA]);
		DebugTool.LogOnce($"Ordre échangé : {bandeA.Name}[{indexA}] ↔ {bandeB.Name}[{indexB}]");
	}

	public override void Exit()
	{
		var b = GetBandeNode();
		DebugTool.NodeOnce("BandeNode (Exit)", b);
		if (b != null)
		{
			b.ZIndex = 0;
			b.Scale = Vector2.One;
		}
	}
}


/*using Godot;
using System.Linq;
using System.Collections.Generic;

public partial class MovingState : BandeState
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

		var chapitre = b.Chapitre;
		if (chapitre == null) return;

		var joueur = b.Joueur; // <-- Utiliser la propriété Joueur de la bande
		if (joueur == null) return;

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
*/
