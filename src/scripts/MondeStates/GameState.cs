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
		
		// On récupère le Chapitre dans la scène instanciée
		var chapitre = GetTree().CurrentScene.GetNode<Chapitre>("SceneContainer/Monde/TableJeu/Chapitre");
		if (chapitre != null)
		{
			 chapitre.VictoireDetectee += () => EmitTransition("MenuState"); // %%%
			//chapitre.InitMachine(this); // si GameState hérite de MondeState, ça marche
		}
		else
		{
			GD.PrintErr("Chapitre introuvable dans la scène instanciée !");
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
