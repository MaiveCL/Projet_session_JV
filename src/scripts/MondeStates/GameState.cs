using Godot;

public partial class GameState : MondeState
{
	[Export] public PackedScene GameScene;
	private Node instance;

	public override void Enter()
	{
		var container = GetTree().CurrentScene.GetNode("SceneContainer");
		instance = GameScene.Instantiate();
		container.AddChild(instance);
	}

	public override void Exit()
	{
		instance?.QueueFree();
	}
}
