using Godot;
using System.Linq;
using System.Collections.Generic;

public partial class MovingState : BandeState
{
	private Settings settings; // référence vers Settings
	private AudioStreamPlayer sonFacile;
	private AudioStreamPlayer sonAction;

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

		sonAction = GetNode<AudioStreamPlayer>("/root/Main/Action");
		sonFacile = GetNode<AudioStreamPlayer>("/root/Main/Facile");

		 var main = GetNode<Node>("/root/Main");
		settings = main as Settings;

		// Jouer le son Action si activé
		if (sonAction != null && settings != null && settings.ActionEnabled)
			sonAction.Play();
	}

	public override void Update(double delta)
	{
		var b = GetBandeNode();
		if (b == null) return;

		var chapitre = b.Chapitre;
		if (chapitre == null) return;

		var joueur = b.Chapitre.GetParent<Node2D>().GetParent<Node2D>().GetNode<Node2D>("Auteur");
		if (joueur == null) return;

		if (!positionLibreInit)
		{
			positionLibreX = b.Position.X;
			positionLibreInit = true;
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

				EchangerOrdreGlobale(chapitre, b, autre);
				positionLibreX = ancienneX;
			}

			if (Input.IsActionJustReleased("down"))
			{
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

		// Jouer son Action si activé
		if (sonAction != null && settings != null && settings.ActionEnabled)
			sonAction.Play();
	}

	private void EchangerOrdreGlobale(Chapitre chapitre, BandeNode bandeDeplacee, BandeNode bandeCollision)
	{
		if (bandeDeplacee == null || bandeCollision == null || chapitre == null)
			return;

		chapitre.RecycleBandes(); // si nécessaire

		List<int> ordre = chapitre.OrdreBandes;

		int indexDeplacee = ordre.IndexOf(bandeDeplacee.FrameIndex);
		int indexCollision = ordre.IndexOf(bandeCollision.FrameIndex);

		if (indexDeplacee == -1 || indexCollision == -1) return;

		(ordre[indexDeplacee], ordre[indexCollision]) = (ordre[indexCollision], ordre[indexDeplacee]);

		// --- Détection du “voisin correct” pour la bande déplacée ---
		int total = ordre.Count;
		bool voisinCorrect = false;

		if (indexDeplacee > 0 && ordre[indexDeplacee] == ordre[indexDeplacee - 1] + 1)
			voisinCorrect = true;
		if (indexDeplacee < total - 1 && ordre[indexDeplacee] == ordre[indexDeplacee + 1] - 1)
			voisinCorrect = true;

				// Jouer sonFacile si EasyMode activé
		if (voisinCorrect && settings != null && settings.EasyModeEnabled && sonFacile != null)
			sonFacile.Play();
	}

	public override void Exit()
	{
		var b = GetBandeNode();
		if (b != null)
		{
			b.ZIndex = 0;
			b.Scale = Vector2.One;
		}
	}
}
