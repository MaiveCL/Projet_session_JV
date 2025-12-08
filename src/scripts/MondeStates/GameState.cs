using Godot;

public partial class GameState : MondeState
{
	[Export] public PackedScene GameScene;
	private Node instance;
	private Node container;

	public override void Enter()
	{
		container = GetTree().CurrentScene.GetNode("SceneContainer");
		instance = GameScene.Instantiate();
		container.AddChild(instance);
		
		// Connexion victoire
		var chapitre = GetTree().CurrentScene.GetNode<Chapitre>("SceneContainer/Monde/TableJeu/Chapitre");
		if (chapitre != null)
		{
			chapitre.VictoireDetectee += () => EmitTransition("VictoireState");
		}
		else
		{
			GD.PrintErr("Chapitre introuvable dans la scène instanciée !");
		}

		// Connexion défaite
		var gameScene = instance.GetNode<GameScene>("GameScene");
		if (gameScene != null)
		{
			gameScene.GameOverDetecte += () => {
				GD.Print("Transition vers DefaiteState demandée");
				EmitTransition("DefaiteState");
			};
		}
		else
		{
			GD.PrintErr("GameScene introuvable dans la scène instanciée !");
		}

	}
	
	public override void Update(double delta)
	{
		if (Input.IsActionJustPressed("pause"))
		{
			var settings = GetNode<Settings>("/root/Main");
			settings.IsPaused = !settings.IsPaused;
			GD.Print("Pause toggled: ", settings.IsPaused);
		}
	}

	public override void Exit()
	{
		instance?.QueueFree();
		
		GD.Print("Children of SceneContainer:", container.GetChildren().Count);
		foreach(var child in container.GetChildren())
		GD.Print(" - ", child.Name);
	}
}
