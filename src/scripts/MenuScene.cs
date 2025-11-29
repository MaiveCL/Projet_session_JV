using Godot;
using System;

public partial class MenuScene : Control
{
	public override void _Ready()
	{
		GetNode<Button>("VBoxContainer/Button").Pressed += OnStartPressed;
	}

	private void OnStartPressed()
	{
		GetNode<MondeStateMachine>("/root/Main/MondeStateMachine")
			.ChangeState("GameState");
	}
}
