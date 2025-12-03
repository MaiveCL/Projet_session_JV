using Godot;
using System.Collections.Generic;

public partial class MondeStateMachine : Node
{
	[Export] public MondeState InitialState;
	private MondeState currentState;
	private Dictionary<string, MondeState> states = new();

	public override void _Ready()
	{
		foreach (Node child in GetChildren())
		{
			if (child is MondeState st)
			{
				states[st.Name] = st;
				st.Transitioned += OnChildTransition;
			}
		}
		
		if (InitialState != null)
		{
			currentState = InitialState;
			currentState.Enter();
			return;
		}
	}

	public override void _Process(double delta)
	{
		currentState?.Update(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		currentState?.PhysicsUpdate(delta);
	}

	private void OnChildTransition(MondeState state, string newStateName)
	{
		if (state != currentState)
			return;

		if (states.TryGetValue(newStateName, out var newState))
		{
			currentState?.Exit();
			currentState = newState;
			currentState.Enter();
		}
	}

	public void ChangeState(string stateName)
	{
		if (states.TryGetValue(stateName, out var newState))
		{
			GD.Print($"[StateMachine] Changing state from {currentState?.Name ?? "null"} to {newState.Name}");
			currentState?.Exit();
			currentState = newState;
			currentState.Enter();
		}
	}
}
