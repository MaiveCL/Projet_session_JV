using Godot;

public partial class MovingState : State
{
	[Export] public float Speed = 200f; // px/s, override via inspector si nécessaire

	public override void Enter()
	{
		var b = GetBandeNode();
		if (b != null)
			b.ShowShadow(true);
	}

	public override void Update(double delta)
	{
		var b = GetBandeNode();
		if (b == null) return;

		// déplacement vers la gauche
		float dx = (float)(-Speed * delta);
		b.Position = new Vector2(b.Position.X + dx, b.Position.Y);

		// Si hors écran (exemple), transition vers Pooled
		if (b.Position.X + b.GetWidth() < -200) // threshold arbitraire
		{
			EmitTransition("Pooled");
		}
	}

	public override void Exit()
	{
		var b = GetBandeNode();
		if (b != null)
			b.ShowShadow(false);
	}
}
