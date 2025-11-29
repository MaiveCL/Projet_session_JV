using Godot;

	public partial class InitialState : MondeState
	{
		public override void Enter()
		{
			EmitTransition("MenuState");
		}
	}
