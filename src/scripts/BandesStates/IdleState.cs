using Godot;

public partial class IdleState : BandeState
{
	private const float OFFSET_SELECTION = 40f;
	private float originalY;

	public override void Enter()
	{
		var b = GetBandeNode();
		//DebugTool.NodeOnce("BandeNode (Enter)", b);

		if (b != null)
		{
			originalY = b.Position.Y;

			Vector2 pos = b.Position;
			pos.Y = Chapitre.MARGE_HAUTE - OFFSET_SELECTION;
			b.Position = pos;

			b.ShowShadow(true);
		}
	}

	public override void Update(double delta)
	{
		var settings = GetNode<Settings>("/root/Main");
		if (settings.IsPaused)
			return;
		//DebugTool.LogOnce("Bande PATH = " + GetBandeNode()?.GetPath());

		var bande = GetBandeNode();
		//DebugTool.NodeOnce("BandeNode (Update)", bande);
		if (bande == null) return;

		var chapitre = bande.Chapitre;
		//DebugTool.NodeOnce("Chapitre", chapitre);
		if (chapitre == null) return;

		// chemin dégueu, mais on le trace
		Node2D joueur = null;

		var parent1 = chapitre.GetParent<Node2D>();
		//DebugTool.NodeOnce("Parent Chapitre", parent1);

		var parent2 = parent1?.GetParent<Node2D>();
		//DebugTool.NodeOnce("Parent Parent", parent2);

		joueur = parent2?.GetNode<Node2D>("Auteur");
		//DebugTool.NodeOnce("Auteur (joueur)", joueur);

		if (joueur == null) return;

		// Mettre à jour la bande la plus proche
		chapitre.MettreAJourBandeProche();

		var bandeProche = chapitre.BandeProche;
		//DebugTool.NodeOnce("Bande Proche", bandeProche);

		var machine = bande.GetStateMachine();
		//DebugTool.NodeOnce("StateMachine Courante", machine);

		// Passer en MovingState si on appuie sur "up"
		if (Input.IsActionJustPressed("up"))
		{
			DebugTool.LogOnce("Input UP détecté, passage en MovingState");
			machine?.ChangeState("MovingState");
			return;
		}

		// Si ce n'est PLUS la bande proche → on dégage vers NullState
		if (bandeProche != bande)
		{
			//DebugTool.LogOnce("Bande plus proche → swap Idle ⇆ Null");
			machine?.ChangeState("NullState");
			bandeProche?.GetStateMachine()?.ChangeState("IdleState");
			return;
		}
	}

	public override void Exit()
	{
		var b = GetBandeNode();
		//DebugTool.NodeOnce("BandeNode (Exit)", b);

		if (b == null) return;

		Vector2 pos = b.Position;
		pos.Y = originalY;
		b.Position = pos;

		b.ShowShadow(false);
	}
}
