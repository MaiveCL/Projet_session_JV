using Godot;
using System;

public partial class MenuScene : Control
{
	// Références exportées pour assigner dans l'inspecteur
	[Export] private Button startButton;
	[Export] private Button aide;
	[Export] private AcceptDialog dialog;

	public override void _Ready()
	{
		// Start button
		startButton.Pressed += OnStartPressed;

		// Comment Jouer button
		aide.Pressed += OnCommentJouerPressed;
	}

	private void OnStartPressed()
	{
		GetNode<MondeStateMachine>("/root/Main/MondeStateMachine")
			.ChangeState("GameState");
	}

	private void OnCommentJouerPressed()
	{
		// Affiche le popup
		dialog.Popup();
	}

	public override void _Input(InputEvent @event)
	{
		// Fermer le popup avec n'importe quelle touche
		if (dialog.Visible && @event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			dialog.Hide();
		}
	}
}
