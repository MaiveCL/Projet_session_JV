using Godot;

public partial class PooledState : State
{
	public override void Enter()
	{
		var b = GetBandeNode();
		if (b == null) return;

		// cacher et neutraliser la bande : le pool la récupérera
		b.Visible = false;
		// Optionnel : remettre frame à -1 ou 0
	}
}
