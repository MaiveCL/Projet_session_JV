using Godot;
using System.Collections.Generic;

public partial class BandesStateMachine : Node
{
	[Export] public BandeState InitialState;
	private BandeState currentState;
	private Dictionary<string, BandeState> states = new();

	public override void _Ready()
	{
		foreach (Node child in GetChildren())
		{
			if (child is BandeState st)
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

	private void OnChildTransition(BandeState state, string newStateName)
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
			currentState?.Exit();
			currentState = newState;
			currentState.Enter();
		}
	}
}
