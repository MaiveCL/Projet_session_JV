using Godot;

public partial class MenuState : MondeState
{
	[Export] public PackedScene MenuScene;
	private Node instance;

	public override void Enter()
	{
		var container = GetTree().CurrentScene.GetNode("SceneContainer");
		instance = MenuScene.Instantiate();

		var menu = instance as MenuScene;
		if (menu != null)
		{
			menu.Inject(
				GetNode<Settings>("/root/Main"),
				GetNode<MondeStateMachine>("/root/Main/MondeStateMachine"),
				GetNode<AudioStreamPlayer>("/root/Main/Musique"),
				GetNode<AudioStreamPlayer>("/root/Main/Action"),
				GetNode<AudioStreamPlayer>("/root/Main/Facile")
			);
		}

		container.AddChild(instance);
	}

	public override void Exit()
	{
		instance?.QueueFree();
	}
}
