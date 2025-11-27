using Godot;

public partial class IdleState : State
{
	public override void Enter()
	{
		var b = GetBandeNode();
		if (b != null)
		{
			b.Visible = true;
			b.ShowShadow(false);
		}
	}

	public override void Update(double delta)
	{
		var chapitre = GetNode<Chapitre>("/root/Monde/TableJeu/Chapitre");
		var joueur = chapitre?.GetNode<Auteur>("/root/Monde/Auteur");

		if (chapitre != null && joueur.IsMoving)
		{
			chapitre.MettreAJourBandeProche();
		}
		
		if (Input.IsActionJustPressed("up") && chapitre.BandeProche != null)
		{
			var machine = chapitre.BandeProche.GetStateMachine();
			if (machine != null)
				machine.ChangeState("MovingState");
		}
	}

}
