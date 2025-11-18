using Godot;

public partial class IdleState : State
{
	public override void Enter()
	{
		var b = GetBandeNode();
		// idle: s'assurer visible et ombre désactivée
		if (b != null)
		{
			b.Visible = true;
			b.ShowShadow(false);
		}
	}

	public override void Update(double delta)
	{
		// comportement idle minimal (ou animations légères)
	}
}
