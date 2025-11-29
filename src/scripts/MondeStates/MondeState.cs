using Godot;
using System;

public partial class MondeState : Node
{
	public event Action<MondeState, string> Transitioned; // observateur

	public virtual void Enter() {}
	public virtual void Exit() {}
	public virtual void Update(double delta) {}
	public virtual void PhysicsUpdate(double delta) {}

	protected void EmitTransition(string newStateName)
	{
		Transitioned?.Invoke(this, newStateName);
	}

}
