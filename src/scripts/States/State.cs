using Godot;
using System;

public partial class State : Node
{
	public event Action<State, string> Transitioned;

	public virtual void Enter() {}
	public virtual void Exit() {}
	public virtual void Update(double delta) {}
	public virtual void PhysicsUpdate(double delta) {}

	protected void EmitTransition(string newStateName)
	{
		Transitioned?.Invoke(this, newStateName);
	}

	// helper to get the BandeNode owner, works with structure:
	// BandeNode
	//  └─ tranche (Sprite2D)
	//     └─ StateMachine
	//        └─ this State
	protected BandeNode GetBandeNode()
	{
		var machine = GetParent() as Node;
		if (machine == null) return null;

		var tranche = machine.GetParent() as Node;
		if (tranche == null) return null;

		return tranche.GetParent() as BandeNode;
	}
}
