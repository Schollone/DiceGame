using UnityEngine;
using System.Collections;

public interface IState {

	//IAction GetActionStrategy ();

	//void SetActionStrategy (IAction action);

	void OnEnter ();

	void Execute (IAction action = null);

	void OnExit ();

	void StartGame ();

	void NextPlayer ();

	void EnterEvaluationPhase ();

	void EnterBidding ();

	void LeaveGame ();
}