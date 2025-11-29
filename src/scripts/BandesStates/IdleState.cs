using Godot;

public partial class IdleState : BandeState
{
	private const float OFFSET_SELECTION = 40f;
	private float originalY;

	public override void Enter()
	{
		var b = GetBandeNode();
		if (b != null)
		{
			originalY = b.Position.Y;

			// Surélévation initiale
			Vector2 pos = b.Position;
			pos.Y = Chapitre.MARGE_HAUTE - OFFSET_SELECTION;
			b.Position = pos;

			b.ShowShadow(true);
		}
	}

	public override void Update(double delta)
	{
		var chapitre = GetNode<Chapitre>("/root/Monde/TableJeu/Chapitre");
		if (chapitre == null) return;

		var joueur = chapitre.GetNode<Auteur>("/root/Monde/Auteur");
		if (joueur == null) return;

		// Mettre à jour la bande la plus proche
		chapitre.MettreAJourBandeProche();

		var bandeCourante = GetBandeNode();
		var bandeProche = chapitre.BandeProche;

		// Passer en MovingState si on appuie sur "up"
		if (Input.IsActionJustPressed("up"))
		{
			var machine = bandeCourante?.GetStateMachine();
			machine?.ChangeState("MovingState");
			return;
		}
		
		if (bandeProche != bandeCourante)
		{
			// Quitter l'ancienne bande
			var machine = bandeCourante?.GetStateMachine();
			machine?.ChangeState("NullState");

			// Entrer sur la nouvelle bande
			bandeProche?.GetStateMachine()?.ChangeState("IdleState");
			return;
		}
	}


	public override void Exit()
	{
		var b = GetBandeNode();
		if (b == null) return;

		Vector2 pos = b.Position;
		pos.Y = originalY;
		b.Position = pos;

		b.ShowShadow(false);
	}
}
