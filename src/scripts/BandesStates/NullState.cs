using Godot;

public partial class NullState : BandeState
{
	public override void Enter()
	{
		var b = GetBandeNode();
		if (b != null)
		{
			b.ShowShadow(false);
		}
	}

	public override void Update(double delta)
	{
	}
}
