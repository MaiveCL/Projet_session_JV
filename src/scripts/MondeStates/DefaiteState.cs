using Godot;

public partial class DefaiteState : MondeState
{
	[Export] public PackedScene DefaiteScene; // assigner res://scenes/Defaite.tscn dans l’inspecteur
	private Node instance;

	public override void Enter()
	{
		var container = GetTree().CurrentScene.GetNode("SceneContainer");
		instance = DefaiteScene.Instantiate();
		container.AddChild(instance);

		var scene = instance as DefaiteScene;
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
