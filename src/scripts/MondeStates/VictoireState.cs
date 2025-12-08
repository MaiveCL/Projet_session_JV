using Godot;

public partial class VictoireState : MondeState
{
	[Export] public PackedScene VictoireScene;
	private Node instance;

	public override void Enter()
	{
		var container = GetTree().CurrentScene.GetNode("SceneContainer");
		instance = VictoireScene.Instantiate();
		container.AddChild(instance);

		var scene = instance as VictoireScene;
		if (scene != null)
		{
			var machine = GetNode<MondeStateMachine>("/root/Main/MondeStateMachine");
			scene.Inject(machine);
		}
	}

	public override void Exit()
	{
		instance?.QueueFree();
	}
}
