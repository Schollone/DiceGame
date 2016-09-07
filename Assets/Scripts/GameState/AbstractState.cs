using UnityEngine;
using System.Collections;

public class AbstractState : IState {

	protected IAction action = null;

	#region IState implementation

	public virtual IAction GetActionStrategy () {
		return this.action;
	}

	public virtual void SetActionStrategy (IAction action) {
		this.action = action;
	}

	public virtual void Execute () {
		
	}

	public virtual void StartGame () {
		
	}

	public virtual void NextPlayer () {
		
	}

	public virtual void EnterEvaluationPhase () {
		
	}

	public virtual void EnterBidding () {
		
	}

	public virtual void LeaveGame () {
		
	}

	#endregion

}

