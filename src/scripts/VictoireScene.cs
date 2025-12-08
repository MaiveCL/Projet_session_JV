using Godot;
using System;

public partial class VictoireScene : Control
{
	[Export] private Button boutonMenu;
	[Export] private Button boutonQuitter;
	[Export] private SpinBox fanDepartInput;
	[Export] private SpinBox tranchesInput;
	[Export] private SpinBox perteFanInput;
	[Export] private Button startButton;


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

		if (startButton != null)
			startButton.Pressed += OnStartNextLevel;
	}

	private void OnRetourMenu()
	{
		stateMachine?.ChangeState("MenuState");
	}

	private void OnQuitter()
	{
		GetTree().Quit();
	}
	
	private void OnStartNextLevel()
	{
		var settings = GetNode<Settings>("/root/Main");
		settings.FansDepart = (int)fanDepartInput.Value;
		settings.Tranches = (int)tranchesInput.Value;
		settings.PerteParSeconde = (float)perteFanInput.Value;

		// Demande à la machine de basculer vers GameState
		stateMachine?.ChangeState("GameState");
	}
}
