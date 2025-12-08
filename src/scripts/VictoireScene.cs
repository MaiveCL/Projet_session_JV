using Godot;
using System;

public partial class VictoireScene : Control
{
	[Export] private Button boutonMenu;
	[Export] private Button boutonQuitter;

	private MondeStateMachine stateMachine;

	// Injection depuis l’état VictoireState
	public void Inject(MondeStateMachine sm)
	{
		stateMachine = sm;
	}

	public override void _Ready()
	{
		if (boutonMenu != null)
			boutonMenu.Pressed += OnRetourMenu;

		if (boutonQuitter != null)
			boutonQuitter.Pressed += OnQuitter;
	}

	private void OnRetourMenu()
	{
		stateMachine?.ChangeState("MenuState");
	}

	private void OnQuitter()
	{
		GetTree().Quit();
	}
}
